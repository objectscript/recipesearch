using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders.Base;

namespace RecipesSearch.SearchEngine.SimilarResults.CacheBuilders
{
    public class TfBuilder : BaseCacheBuilder
    {
        private static readonly TfBuilder Instance = new TfBuilder();

        private TfBuilder()
        {
            BuilderName = "TfBuilder";
        }

        public static TfBuilder GetInstance()
        {
            return Instance;
        }

        protected override void BuildAction()
        {
            var tfIdfConfig = new TfIdfConfigRepository().GetConfig();

            using (var cacheAdapter = new TfIdfAdapter())
            {
                cacheAdapter.UpdateTf(tfIdfConfig.TfBuilderName);
            }
        }

        public List<String> GetTfBuilders()
        {
            try
            {
                using (var cacheAdapter = new TfIdfAdapter())
                {
                    return cacheAdapter.GetTfBuilders();
                }
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("TfBuilder.GetTfBuilders failed"), exception);
                return new List<string>();
            }
        }
    }
}
