using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RecipesSearch.SearchEngine.Clusters.Base
{
    public abstract class BaseClustersBuilder
    {
        public bool UpdateInProgress { get; private set; }

        public decimal Percentage { get; private set; }

        public bool PreviousBuildFailed { get; private set; }

        protected CancellationTokenSource _cancellationTokenSource;

        public Task FindClusters(CancellationTokenSource cancellationTokenSource = null)
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

                    var tfIdfConfigRepository = new TfIdfConfigRepository();
                    TfIdfConfig config = tfIdfConfigRepository.GetConfig();
                    
                    LoggerWrapper.LogInfo("Clusters build: GetInfo started");
                    var nearestResults = GetNearestResult();
                    LoggerWrapper.LogInfo("Clusters build: GetInfo finished");

                    ComputeClusters(nearestResults, config);

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

        protected abstract void ComputeClusters(List<NearestResult> results, TfIdfConfig config);

        protected List<NearestResult> GetNearestResult()
        {
            List<NearestResult> nearestResults;

            using (var cacheAdapter = new SimilarResultsAdapter())
            {
                nearestResults = cacheAdapter.GetNearestResults();
            }

            return nearestResults;
        }

        protected Edge[] GetEdges(List<NearestResult> results)
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

        protected T? GetSetting<T>(TfIdfConfig config, string name) where T : struct
        {
            string[] lines = config.ClusteringParameters.Split('\n');

            foreach(var line in lines)
            {
                string[] keyValue = line.Split(':');
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();

                if (String.Equals(key, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (T)Convert.ChangeType(value, typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return null;
        }

        protected GraphInfo BuildGraphInfo(List<NearestResult> results)
        {
            var graphInfo = new GraphInfo();

            var edges = GetEdges(results);

            graphInfo.Edges = edges;
            graphInfo.IdToSurrogateIdMap = new Dictionary<int, int>();
            graphInfo.SurrogateIdToIdMap = new Dictionary<int, int>();
            graphInfo.RecipesCount = 0;
            for (int i = 0; i < edges.Length; i++)
            {
                if (!graphInfo.IdToSurrogateIdMap.ContainsKey(edges[i].FromId))
                {
                    int surrogateId = graphInfo.RecipesCount++;
                    edges[i].FromSurrogateId = surrogateId;
                    graphInfo.IdToSurrogateIdMap[edges[i].FromId] = surrogateId;
                    graphInfo.SurrogateIdToIdMap[surrogateId] = edges[i].FromId;
                }
                else
                {
                    edges[i].FromSurrogateId = graphInfo.IdToSurrogateIdMap[edges[i].FromId];
                }

                if (!graphInfo.IdToSurrogateIdMap.ContainsKey(edges[i].ToId))
                {
                    int surrogateId = graphInfo.RecipesCount++;
                    edges[i].ToSurrogateId = surrogateId;
                    graphInfo.IdToSurrogateIdMap[edges[i].ToId] = surrogateId;
                    graphInfo.SurrogateIdToIdMap[surrogateId] = edges[i].ToId;
                }
                else
                {
                    edges[i].ToSurrogateId = graphInfo.IdToSurrogateIdMap[edges[i].ToId];
                }
            }

            graphInfo.Graph = new List<Tuple<int, double>>[graphInfo.RecipesCount];
            graphInfo.Clusters = new List<int>[graphInfo.RecipesCount];
            for (int i = 0; i < graphInfo.RecipesCount; ++i)
            {
                graphInfo.Graph[i] = new List<Tuple<int, double>>();
                graphInfo.Clusters[i] = new List<int>();
            }

            return graphInfo;
        }

        protected void SaveResults(GraphInfo graphInfo)
        {
            using (var similarResults = new SimilarResultsAdapter())
            {
                for (int i = 0; i < graphInfo.RecipesCount; ++i)
                {
                    similarResults.UpdateClusterId(graphInfo.SurrogateIdToIdMap[i], graphInfo.Clusters[i]);
                }
            }
        }

        protected class GraphInfo
        {
            public Dictionary<int, int> IdToSurrogateIdMap { get; set; }

            public Dictionary<int, int> SurrogateIdToIdMap { get; set; }

            public List<Tuple<int, double>>[] Graph { get; set; }

            public List<int>[] Clusters { get; set; }

            public int RecipesCount { get; set; }

            public Edge[] Edges { get; set; }
        }

        protected struct Edge : IComparable
        {
            public int FromId;

            public int ToId;

            public double Weight;

            public int FromSurrogateId;

            public int ToSurrogateId;

            public int CompareTo(object obj)
            {
                return Math.Sign(Weight - ((Edge)obj).Weight);
            }
        }
    }
}
