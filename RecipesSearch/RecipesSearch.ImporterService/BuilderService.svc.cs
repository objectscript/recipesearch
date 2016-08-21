using System.ServiceModel;
using RecipesSearch.Data.Views;
using RecipesSearch.SearchEngine.SimilarResults;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders;
using RecipesSearch.SearchEngine.Clusters.Base;

namespace RecipesSearch.ImporterService
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class BuilderService : IBuilderService
    {
        private readonly TfBuilder _tfBuilder = TfBuilder.GetInstance();
        private readonly IdfBuilder _idfBuilder = IdfBuilder.GetInstance();
        private readonly TfIdfBuilder _tfIdfBuilder = TfIdfBuilder.GetInstance();
        private readonly AllTasksBuilder _allTasksBuilder = AllTasksBuilder.GetInstance();
        private readonly SimilarResultsBuilder _similarResultsBuilder = SimilarResultsBuilder.GetInstance();

        private BaseClustersBuilder _builderInstance;

        public void BuildTf()
        {
            _tfBuilder.Build();
        }

        public void StopTfBuild()
        {
            _tfBuilder.StopUpdating();
        }

        public void BuildTfIdf()
        {
            _tfIdfBuilder.Build();
        }

        public void StopTfIdfBuild()
        {
            _tfIdfBuilder.StopUpdating();
        }

        public void BuildIdf()
        {
            _idfBuilder.Build();
        }

        public void BuildSimilarResults(int resultsCount, bool sameCategoryOnly)
        {
            _similarResultsBuilder.FindNearestResults(resultsCount, sameCategoryOnly);
        }

        public void StopSimilarResultsBuild()
        {
            _similarResultsBuilder.StopUpdating();
        }

        public void BuildClusters(ClusterBuilders builder)
        {
            if(_builderInstance != null)
            {
                _builderInstance.StopUpdating();
            }

            _builderInstance = ClustersBulderFactory.GetBuilder(builder);
            _builderInstance.FindClusters();
        }

        public void StopClustersBuild()
        {
            _builderInstance.StopUpdating();
        }

        public void BuildAllTasks()
        {
            _allTasksBuilder.RunAllTasks();
        }

        public void StopAllTasksUpdating()
        {
            _allTasksBuilder.StopUpdating();
        }

        public BuildersState GetBuildersState()
        {
            return new BuildersState
            {
                IdfBuildInProgress = _idfBuilder.UpdateInProgress,
                TfBuildInProgress = _tfBuilder.UpdateInProgress,
                TfBuildProgress = _tfBuilder.Progress,
                TfIdfBuildInProgress = _tfIdfBuilder.UpdateInProgress,
                TfIdfBuildProgress = _tfIdfBuilder.Progress,
                AllTasksBuildInProgress = _allTasksBuilder.UpdateInProgress,
                SimilarResultsUpdateInProgress = _similarResultsBuilder.UpdateInProgress,
                SimilarResultsUpdatedCount = _similarResultsBuilder.UpdatedPagesCount,
                TfBuildFailed = _tfBuilder.PreviousBuildFailed,
                IdfBuildFailed = _idfBuilder.PreviousBuildFailed,
                TfIdfBuildFailed = _tfIdfBuilder.PreviousBuildFailed,
                SimilarResultsBuildFailed = _similarResultsBuilder.PreviousBuildFailed,
                SimilarResultsPercentage = _similarResultsBuilder.Percentage,
                ClustersBuildInProgress = _builderInstance == null ? false : _builderInstance.UpdateInProgress,
                ClustersBuildFailed = _builderInstance == null ? false : _builderInstance.PreviousBuildFailed
            };
        }
    }
}
