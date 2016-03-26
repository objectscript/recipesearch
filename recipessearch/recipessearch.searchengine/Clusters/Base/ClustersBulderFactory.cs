using System.Collections.Generic;

namespace RecipesSearch.SearchEngine.Clusters.Base
{
    public class ClustersBulderFactory
    {
        public static BaseClustersBuilder GetBuilder(ClusterBuilders builder)
        {
            if(builder == ClusterBuilders.SpanningTree2)
            {
                return SpanningTreeBuilder2.GetInstance();
            }

            if(builder == ClusterBuilders.SpanningTree)
            {
                return SpanningTreeBuilder.GetInstance();
            }

            if (builder == ClusterBuilders.PageRank)
            {
                return PageRankBuilder.GetInstance();
            }

            return null;
        }

        public static List<ClusterBuilders> GetClustersBuilders()
        {
            return new List<ClusterBuilders>
            {
                ClusterBuilders.SpanningTree,
                ClusterBuilders.PageRank,
                ClusterBuilders.SpanningTree2
            };
        }
    }
}
