using System.Threading;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders.Base;

namespace RecipesSearch.SearchEngine.SimilarResults.CacheBuilders
{
    public class TfIdfBuilder : BaseCacheBuilder
    {
        public decimal Progress { get; private set; }

        private static readonly TfIdfBuilder Instance = new TfIdfBuilder();

        private CancellationTokenSource _cancellationTokenSource;

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
            _cancellationTokenSource = new CancellationTokenSource();
            Progress = 0;

            using (var cacheAdapter = new TfIdfAdapter())
            {
                cacheAdapter.UpdateTfIdf(_cancellationTokenSource.Token, progress => Progress = progress);
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
