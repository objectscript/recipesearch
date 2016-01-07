using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.Data.Models;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Views;
using RecipesSearch.DAL.Cache.Adapters.Base;
using Wintellect.PowerCollections;

namespace RecipesSearch.SearchEngine.SimilarResults
{
    public class ClustersBuilder
    {
        public bool UpdateInProgress { get; private set; }

        public decimal Percentage { get; private set; }

        public bool PreviousBuildFailed { get; private set; }

        private static ClustersBuilder _instance;

        private CancellationTokenSource _cancellationTokenSource;

        private int[] _parents;

        private int[] _ranks;

        private ClustersBuilder()
        {
        }

        public static ClustersBuilder GetInstance()
        {
            if (_instance == null)
            {
                _instance = new ClustersBuilder();
            }

            return _instance;
        }

        public Task FindClusters(int threshold, CancellationTokenSource cancellationTokenSource = null)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdateInProgress = true;
                    PreviousBuildFailed = false;
                    Percentage = 0;

                    LoggerWrapper.LogInfo("Clusters build started");

                    _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

                    LoggerWrapper.LogInfo("Clusters build: GetInfo started");
                    var nearestResults = GetNearestResult();
                    LoggerWrapper.LogInfo("Clusters build: GetInfo finished");

                    ComputeClusters(nearestResults, threshold);

                    UpdateInProgress = false;

                    LoggerWrapper.LogInfo("Clusters build finished");
                }
                catch (Exception exception)
                {
                    UpdateInProgress = false;
                    PreviousBuildFailed = true;
                    LoggerWrapper.LogError(String.Format("Clusters build failed"), exception);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void StopUpdating()
        {
            if (UpdateInProgress)
            {
                _cancellationTokenSource.Cancel();
                UpdateInProgress = false;
            }
        }

        private void ComputeClusters(List<NearestResult> results, int threshold)
        {
            var edges = GetEdges(results);

            Dictionary<int, int> idToSurrogateIdMap = new Dictionary<int, int>();
            Dictionary<int, int> surrogateIdToIdMap = new Dictionary<int, int>();
            int recipesCount = 0;
            for (int i = 0; i < edges.Length; i++)
            {
                if (!idToSurrogateIdMap.ContainsKey(edges[i].FromId))
                {
                    int surrogateId = recipesCount++;
                    edges[i].FromSurrogateId = surrogateId;
                    idToSurrogateIdMap[edges[i].FromId] = surrogateId;
                    surrogateIdToIdMap[surrogateId] = edges[i].FromId;
                }
                else
                {
                    edges[i].FromSurrogateId = idToSurrogateIdMap[edges[i].FromId];
                }

                if (!idToSurrogateIdMap.ContainsKey(edges[i].ToId))
                {
                    int surrogateId = recipesCount++;
                    edges[i].ToSurrogateId = surrogateId;
                    idToSurrogateIdMap[edges[i].ToId] = surrogateId;
                    surrogateIdToIdMap[surrogateId] = edges[i].ToId;
                }
                else
                {
                    edges[i].ToSurrogateId = idToSurrogateIdMap[edges[i].ToId];
                }
            }

            Array.Sort(edges, (IComparer)null);

            _parents = new int[recipesCount];
            _ranks = new int[recipesCount];
            for (int i = 0; i < recipesCount; ++i)
            {
                MakeSet(i);
            }

            for (int i = 0; i < edges.Length; ++i)
            {
                int a = edges[i].FromSurrogateId;
                int b = edges[i].ToSurrogateId;

                if (FindSet(a) != FindSet(b))
                {
                    UnionSets(a, b);
                }
            }

            using (var similarResults = new SimilarResultsAdapter())
            {
                for (int i = 0; i < recipesCount; ++i)
                {
                    similarResults.UpdateClusterId(surrogateIdToIdMap[i], FindSet(i));
                }
            }
        } 

        private List<NearestResult> GetNearestResult()
        {
            List<NearestResult> nearestResults;

            using (var cacheAdapter = new SimilarResultsAdapter())
            {
                nearestResults = cacheAdapter.GetNearestResults();
            }
          
            return nearestResults;
        }

        private Edge[] GetEdges(List<NearestResult> results)
        {
            var edges = new Edge[results.Count];        

            for (int i = 0; i < results.Count; i++)
            {
                edges[i] = new Edge
                {
                    FromId = results[i].RecipeId,
                    ToId = results[i].SimilarRecipeId,
                    Weight = results[i].Weight
                };
            }

            return edges;
        }

        private void MakeSet(int v)
        {
            _parents[v] = v;
            _ranks[v] = 0;
        }

        private int FindSet(int v)
        {
            if (v == _parents[v])
                return v;
            return _parents[v] = FindSet(_parents[v]);
        }

        private void UnionSets(int a, int b)
        {
            a = FindSet(a);
            b = FindSet(b);

            if (a != b)
            {
                if (_ranks[a] < _ranks[b])
                {
                    a = a ^ b;
                    b = a ^ b;
                    a = a ^ b;
                }
                _parents[b] = a;

                if (_ranks[a] == _ranks[b])
                {
                    ++_ranks[a];
                }
            }
        }

        private struct Edge : IComparable
        {
            public int FromId;

            public int ToId;

            public double Weight;

            public int FromSurrogateId;

            public int ToSurrogateId;

            public int CompareTo(object obj)
            {
                return Math.Sign(Weight - ((Edge) obj).Weight);
            }
        }
    }
}
