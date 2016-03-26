using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.Data.Models;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.SearchEngine.Clusters.Base;

namespace RecipesSearch.SearchEngine.Clusters
{
    public class SpanningTreeBuilder : BaseClustersBuilder
    {
        private int[] _parents;

        private int[] _ranks;

        protected static SpanningTreeBuilder _instance;

        private SpanningTreeBuilder()
        {
        }

        public static SpanningTreeBuilder GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SpanningTreeBuilder();
            }

            return _instance;
        }

        protected override void ComputeClusters(List<NearestResult> results, TfIdfConfig config)
        {
            var threshold = GetSetting<int>(config, "threshold") ?? 0;

            var graphInfo = BuildGraphInfo(results);

            int[] edgesCount = new int[graphInfo.RecipesCount];

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
                if(minTreeEdges[i].Weight <= threshold)
                {
                    graphInfo.Graph[from].Add(new Tuple<int, double>(to, weight));
                    graphInfo.Graph[to].Add(new Tuple<int, double>(from, weight));
                }
            }

            int clusterId = 1;
            for (int i = 0; i < graphInfo.RecipesCount; ++i)
            {
                Dfs(i, graphInfo.Graph, graphInfo.Clusters, clusterId++);
            }

            SaveResults(graphInfo);
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

        private void Dfs(int v, List<Tuple<int, double>>[] graph, List<int>[] clusters, int clusterId)
        {
            if (clusters[v].Any())
            {
                return;
            }

            clusters[v].Add(clusterId);

            for (int i = 0; i < graph[v].Count; ++i)
            {
                int to = graph[v][i].Item1;
                Dfs(to, graph, clusters, clusterId);
            }
        }
    }
}
