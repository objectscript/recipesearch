using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.CacheService.Services;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Views;
using RecipesSearch.SearchEngine.SimilarResults.Builders.Base;
using Wintellect.PowerCollections;

namespace RecipesSearch.SearchEngine.SimilarResults.Builders
{
    public class TfIdfBuilder : BaseCacheBuilder
    {
        private static readonly TfIdfBuilder Instance = new TfIdfBuilder();

        private TfIdfBuilder()
        {
            BuilderName = "TfIdfBuilder";
        }

        public static TfIdfBuilder GetInstance()
        {
            return Instance;
        }

        protected override void BuildAction()
        {
            using (var cacheAdapter = new TfIdfAdapter())
            {
                cacheAdapter.UpdateTfIdf();
            }
        }
    }
}
