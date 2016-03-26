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
            bool intersectCluster = GetSetting<bool>(config, "allowIntersect") ?? false;
            bool useFullGraph = GetSetting<bool>(config, "fullGraph") ?? true;
            var threshold = GetSetting<int>(config, "threshold") ?? 0;
            var k = GetSetting<double>(config, "k") ?? 0.5;

            var graphInfo = BuildGraphInfo(results);
            int[] edgesCount = new int[graphInfo.RecipesCount];
            for (int i = 0; i < graphInfo.Edges.Length; ++i)
            {
                int from = graphInfo.Edges[i].FromSurrogateId;
                int to = graphInfo.Edges[i].ToSurrogateId;
                double weight = graphInfo.Edges[i].Weight;
                graphInfo.Graph[from].Add(new Tuple<int, double>(to, weight));
                graphInfo.Graph[to].Add(new Tuple<int, double>(from, weight));
                edgesCount[from]++;
                edgesCount[to]++;
            }

            double[] ranks = ComputePageRank(graphInfo.Graph, edgesCount, graphInfo.RecipesCount, 0.001);

            if (!useFullGraph)
            {
                graphInfo.Graph = new List<Tuple<int, double>>[graphInfo.RecipesCount];
                for (int i = 0; i < graphInfo.RecipesCount; ++i)
                {
                    graphInfo.Graph[i] = new List<Tuple<int, double>>();
                }

                Array.Sort(graphInfo.Edges, (IComparer)null);

                _parents = new int[graphInfo.RecipesCount];
                _ranks = new int[graphInfo.RecipesCount];
                for (int i = 0; i < graphInfo.RecipesCount; ++i)
                {
                    MakeSet(i);
                }

                List<Edge> minTreeEdges = new List<Edge>();

                for (int i = 0; i < graphInfo.Edges.Length; ++i)
                {
                    int a = graphInfo.Edges[i].FromSurrogateId;
                    int b = graphInfo.Edges[i].ToSurrogateId;

                    if (FindSet(a) != FindSet(b))
                    {
                        minTreeEdges.Add(graphInfo.Edges[i]);
                        UnionSets(a, b);
                    }
                }

                for (int i = 0; i < minTreeEdges.Count; ++i)
                {
                    int from = minTreeEdges[i].FromSurrogateId;
                    int to = minTreeEdges[i].ToSurrogateId;
                    double weight = minTreeEdges[i].Weight;
                    graphInfo.Graph[from].Add(new Tuple<int, double>(to, weight));
                    graphInfo.Graph[to].Add(new Tuple<int, double>(from, weight));
                }
            }

            List<Tuple<double, int>> recipeEdges = ranks
                .Select((c, idx) => new Tuple<double, int>(c, idx))
                .OrderByDescending(x => x.Item1)
                .ToList();

            int clusterId = 1;
            for (int i = 0; i < recipeEdges.Count; ++i)
            {
                int recipeId = recipeEdges[i].Item2;

                Dfs(recipeId, graphInfo.Graph, graphInfo.Clusters, clusterId++, threshold, k, intersectCluster);
            }

            SaveResults(graphInfo);
        } 

        private double[] ComputePageRank(List<Tuple<int, double>>[] graph, int[] edgesCount, int n, double eps)
        {
            double[] rank = new double[n];
            for(int i = 0; i < n; ++i)
            {
                rank[i] = 0.25;
            }
                       
            double rankChange = Double.MaxValue;
            while (rankChange > eps)
            {
                rankChange = 0;
                double[] newRank = new double[n];

                for(int i = 0; i < n; ++i)
                {
                    newRank[i] = 0;

                    for (int j = 0; j < graph[i].Count; ++j)
                    {
                        int to = graph[i][j].Item1;
                        if(i == to)
                        {
                            continue;
                        }

                        newRank[i] += rank[to] / edgesCount[to];
                    }

                    rankChange += Math.Abs(newRank[i] - rank[i]);
                }

                rank = newRank;
            }

            return rank;
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

        private void Dfs(int v, List<Tuple<int, double>>[] graph, List<int>[] clusters, int clusterId, double threshold, double k, bool intersectCluster)
        {
            if (clusters[v].Any() && !intersectCluster)
            {
                return;
            }

            if(clusters[v].Any(id => id == clusterId))
            {
                return;
            }

            clusters[v].Add(clusterId);

            for (int i = 0; i < graph[v].Count; ++i)
            {
                double weight = graph[v][i].Item2;
                int to = graph[v][i].Item1;
                if(weight < threshold)
                {
                    Dfs(to, graph, clusters, clusterId, threshold * k, k, intersectCluster);
                }
            }
        }
    }
}
