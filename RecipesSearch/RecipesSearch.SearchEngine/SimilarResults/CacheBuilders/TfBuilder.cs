using System;
using System.Collections.Generic;
using System.Threading;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders.Base;

namespace RecipesSearch.SearchEngine.SimilarResults.CacheBuilders
{
    public class TfBuilder : BaseCacheBuilder
    {
        public decimal Progress { get; private set; }

        private static readonly TfBuilder Instance = new TfBuilder();

        private CancellationTokenSource _cancellationTokenSource;

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
            _cancellationTokenSource = new CancellationTokenSource();
            Progress = 0;
            var tfIdfConfig = new TfIdfConfigRepository().GetConfig();

            using (var cacheAdapter = new TfIdfAdapter())
            {
                cacheAdapter.UpdateTf(tfIdfConfig.TfBuilderName, _cancellationTokenSource.Token, progress => Progress = progress);
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
                LoggerWrapper.LogError(String.Format("TfBuilder.GetTfBuilders failed"), exception);
                return new List<string>();
            }
        }

        public void StopUpdating()
        {
            if (UpdateInProgress)
            {
                _cancellationTokenSource.Cancel();
                UpdateInProgress = false;
                Progress = 0;
            }
        }
    }
}
