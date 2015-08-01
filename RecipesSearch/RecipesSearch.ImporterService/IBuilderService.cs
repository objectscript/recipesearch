using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace RecipesSearch.ImporterService
{
    [ServiceContract]
    public interface IBuilderService
    {
        [OperationContract]
        void BuildTf();

        [OperationContract]
        void BuildTfIdf();

        [OperationContract]
        void BuildIdf();

        [OperationContract]
        void BuildAllTasks();

        [OperationContract]
        void StopAllTasksUpdating();

        [OperationContract]
        bool IsTfBuildInProgress();

        [OperationContract]
        bool IsIdfBuildInProgress();

        [OperationContract]
        bool IsTfIdfBuildInProgress();

        [OperationContract]
        bool AllTasksBuildInProgress();
    }
}
