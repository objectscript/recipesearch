using System;
using System.Collections.Generic;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders.Base;

namespace RecipesSearch.SearchEngine.SimilarResults.CacheBuilders
{
    public class IdfBuilder : BaseCacheBuilder
    {
        private static readonly IdfBuilder Instance = new IdfBuilder();

        private IdfBuilder()
        {
            BuilderName = "IdfBuilder";
        }

        public static IdfBuilder GetInstance()
        {
            return Instance;
        }

        protected override void BuildAction()
        {
            var tfIdfConfig = new TfIdfConfigRepository().GetConfig();

            using (var cacheAdapter = new TfIdfAdapter())
            {
                cacheAdapter.UpdateIdf(tfIdfConfig.IdfBuilderName);
            }
        }

        public List<String> GetIdfBuilders()
        {
            try
            {
                using (var cacheAdapter = new TfIdfAdapter())
                {
                    return cacheAdapter.GetIdfBuilders();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("IdfBuilder.GetIdfBuilders failed"), exception);
                return new List<string>();
            }
        }
    }
}
