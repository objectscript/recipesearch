using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
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

        public void BuildTf()
        {
            _tfBuilder.Build();
        }

        public void BuildTfIdf()
        {
            _tfIdfBuilder.Build();
        }

        public void BuildIdf()
        {
            _idfBuilder.Build();
        }

        public void BuildAllTasks()
        {
            _allTasksBuilder.RunAllTasks();
        }

        public void StopAllTasksUpdating()
        {
            _allTasksBuilder.StopUpdating();
        }

        public bool IsTfBuildInProgress()
        {
            return _tfBuilder.UpdateInProgress;
        }

        public bool IsIdfBuildInProgress()
        {
            return _idfBuilder.UpdateInProgress;
        }

        public bool IsTfIdfBuildInProgress()
        {
            return _tfIdfBuilder.UpdateInProgress;
        }

        public bool AllTasksBuildInProgress()
        {
            return _allTasksBuilder.UpdateInProgress;
        }
    }
}
