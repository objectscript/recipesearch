using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Views;
using RecipesSearch.SearchEngine.SimilarResults.CacheBuilders;
using Wintellect.PowerCollections;

namespace RecipesSearch.SearchEngine.SimilarResults
{
    public class AllTasksBuilder
    {
        public bool UpdateInProgress { get; private set; }

        private static AllTasksBuilder _instance;

        private CancellationTokenSource _cancellationTokenSource;

        private AllTasksBuilder()
        {           
        }

        public static AllTasksBuilder GetInstance()
        {
            if (_instance == null)
            {
                _instance = new AllTasksBuilder();
            }

            return _instance;
        }

        public void RunAllTasks()
        {
            var tfBuilder = TfBuilder.GetInstance();
            var idfBuilder = IdfBuilder.GetInstance();
            var tfIdfBuilder = TfIdfBuilder.GetInstance();
            var similarResultsBuilder = SimilarResultsBuilder.GetInstance();
            var tfIdfConfig = new TfIdfConfigRepository().GetConfig();

            if (tfBuilder.UpdateInProgress || idfBuilder.UpdateInProgress || tfIdfBuilder.UpdateInProgress ||
                similarResultsBuilder.UpdateInProgress)
            {
                return;
            }

            _cancellationTokenSource = new CancellationTokenSource();

            Task.Factory.StartNew(async () =>
            {
                try
                {
                    UpdateInProgress = true;
                    
                    await tfBuilder.Build();

                    if(tfBuilder.PreviousBuildFailed || _cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await idfBuilder.Build();
                    if (idfBuilder.PreviousBuildFailed || _cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await tfIdfBuilder.Build();
                    if (tfIdfBuilder.PreviousBuildFailed || _cancellationTokenSource.Token.IsCancellationRequested)
                    {
                        return;
                    }

                    await similarResultsBuilder.FindNearestResults(tfIdfConfig.SimilarResultsCount, _cancellationTokenSource);

                    UpdateInProgress = false;
                }
                catch (Exception exception)
                {
                    Logger.LogError(String.Format("AllTasksBuilder.RunAllTasks failed"), exception);
                    UpdateInProgress = false;
                }               
            }, TaskCreationOptions.AttachedToParent);
        }

        public void StopUpdating()
        {
            if (UpdateInProgress)
            {
                _cancellationTokenSource.Cancel();
                UpdateInProgress = false;
            }
        }     
    }
}
