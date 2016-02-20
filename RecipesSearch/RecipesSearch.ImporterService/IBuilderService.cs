using System.ServiceModel;
using RecipesSearch.Data.Views;
using RecipesSearch.SearchEngine.Clusters.Base;

namespace RecipesSearch.ImporterService
{
    [ServiceContract]
    public interface IBuilderService
    {
        [OperationContract]
        void BuildTf();

        [OperationContract]
        void StopTfBuild();

        [OperationContract]
        void BuildTfIdf();

        [OperationContract]
        void StopTfIdfBuild();

        [OperationContract]
        void BuildIdf();

        [OperationContract]
        void BuildSimilarResults(int resultsCount);

        [OperationContract]
        void StopSimilarResultsBuild();

        [OperationContract]
        void BuildClusters(ClusterBuilders builder);

        [OperationContract]
        void StopClustersBuild();

        [OperationContract]
        void BuildAllTasks();

        [OperationContract]
        void StopAllTasksUpdating();

        [OperationContract]
        BuildersState GetBuildersState();
    }
}
