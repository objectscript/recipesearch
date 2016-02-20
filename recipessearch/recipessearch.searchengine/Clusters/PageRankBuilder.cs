using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.Data.Models;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.SearchEngine.Clusters.Base;

namespace RecipesSearch.SearchEngine.Clusters
{
    public class PageRankBuilder : BaseClustersBuilder
    {
        private int[] _parents;

        private int[] _ranks;

        protected static PageRankBuilder _instance;

        private PageRankBuilder()
        {
        }

        public static PageRankBuilder GetInstance()
        {
            if (_instance == null)
            {
                _instance = new PageRankBuilder();
            }

            return _instance;
        }

        protected override void ComputeClusters(List<NearestResult> results, TfIdfConfig config)
        {
            bool useFullGraph = true;
            var threshold = GetSetting<int>(config, "threshold") ?? 0;

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

            List<Tuple<int, double>>[] graph = new List<Tuple<int, double>>[recipesCount];
            for (int i = 0; i < recipesCount; ++i)
            {
                graph[i] = new List<Tuple<int, double>>();
            }

            int[] clusters = new int[recipesCount];
            int[] edgesCount = new int[recipesCount];

            if (!useFullGraph)
            {
                Array.Sort(edges, (IComparer)null);

                _parents = new int[recipesCount];
                _ranks = new int[recipesCount];
                for (int i = 0; i < recipesCount; ++i)
                {
                    MakeSet(i);
                }

                List<Edge> minTreeEdges = new List<Edge>();

                for (int i = 0; i < edges.Length; ++i)
                {
                    int a = edges[i].FromSurrogateId;
                    int b = edges[i].ToSurrogateId;

                    if (FindSet(a) != FindSet(b))
                    {
                        minTreeEdges.Add(edges[i]);
                        UnionSets(a, b);
                    }
                }

                Dictionary<int, int> values = new Dictionary<int, int>();
                int step = 5;
                foreach(var edge in minTreeEdges)
                {
                    int weight = ((int)edge.Weight) / step * step;
                    if (!values.ContainsKey(weight))
                    {
                        values.Add(weight, 0);
                    }
                    values[weight]++;
                }
                string hist = String.Join("\n", values.Select(x => String.Format("{0},{1}", x.Key, x.Value)));

                for (int i = 0; i < minTreeEdges.Count; ++i)
                {
                    int from = minTreeEdges[i].FromSurrogateId;
                    int to = minTreeEdges[i].ToSurrogateId;
                    double weight = minTreeEdges[i].Weight;
                    //if(minTreeEdges[i].Weight <= threshold)
                    //{
                    graph[from].Add(new Tuple<int, double>(to, weight));
                    graph[to].Add(new Tuple<int, double>(from, weight));
                    //}
                    edgesCount[from]++;
                    edgesCount[to]++;
                }
            }
            else
            {
                for (int i = 0; i < edges.Length; ++i)
                {
                    int from = edges[i].FromSurrogateId;
                    int to = edges[i].ToSurrogateId;
                    double weight = edges[i].Weight;
                    graph[from].Add(new Tuple<int, double>(to, weight));
                    graph[to].Add(new Tuple<int, double>(from, weight));
                    edgesCount[from]++;
                    edgesCount[to]++;
                }
            }

            List<Tuple<int, int>> recipeEdges = edgesCount
                .Select((c, idx) => new Tuple<int, int>(c, idx))
                .OrderByDescending(x => x.Item1)
                .ToList();

            int clusterId = 1;
            for (int i = 0; i < recipeEdges.Count; ++i)
            {
                int recipeId = recipeEdges[i].Item2;
                if (clusters[recipeId] != 0)
                {
                    continue;
                }

                Dfs(recipeId, graph, clusters, clusterId++, threshold, 0.5);
            }

            using (var similarResults = new SimilarResultsAdapter()) 
            {
                for (int i = 0; i < recipesCount; ++i)
                {
                    similarResults.UpdateClusterId(surrogateIdToIdMap[i], clusters[i]);
                }
            }
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

        private void Dfs(int v, List<Tuple<int, double>>[] graph, int[] clusters, int clusterId, double threshold, double k)
        {
            if (clusters[v] != 0)
            {
                return;
            }

            clusters[v] = clusterId;

            for (int i = 0; i < graph[v].Count; ++i)
            {
                double weight = graph[v][i].Item2;
                int to = graph[v][i].Item1;
                if(weight < threshold)
                {
                    Dfs(to, graph, clusters, clusterId, threshold * k, k);
                }
            }
        }
    }
}
