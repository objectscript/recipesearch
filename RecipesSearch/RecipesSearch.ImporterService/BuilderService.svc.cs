using System.ServiceModel;
using RecipesSearch.Data.Views;
using RecipesSearch.SearchEngine.SimilarResults;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders;

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
        private readonly ClustersBuilder _clustersBuilder = ClustersBuilder.GetInstance();

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

        public void BuildSimilarResults(int resultsCount)
        {
            _similarResultsBuilder.FindNearestResults(resultsCount);
        }

        public void StopSimilarResultsBuild()
        {
            _similarResultsBuilder.StopUpdating();
        }

        public void BuildClusters(int threshold)
        {
            _clustersBuilder.FindClusters(threshold);
        }

        public void StopClustersBuild()
        {
            _clustersBuilder.StopUpdating();
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
                ClustersBuildInProgress = _clustersBuilder.UpdateInProgress,
                ClustersBuildFailed = _clustersBuilder.PreviousBuildFailed
            };
        }
    }
}
