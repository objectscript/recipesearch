using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace RecipesSearch.SearchEngine.Clusters.Base
{
    public abstract class BaseClustersBuilder
    {
        public bool UpdateInProgress { get; private set; }

        public decimal Percentage { get; private set; }

        public bool PreviousBuildFailed { get; private set; }

        protected CancellationTokenSource _cancellationTokenSource;

        public Task FindClusters(CancellationTokenSource cancellationTokenSource = null)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdateInProgress = true;
                    PreviousBuildFailed = false;
                    Percentage = 0;

                    LoggerWrapper.LogInfo("Clusters build started");

                    _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

                    var tfIdfConfigRepository = new TfIdfConfigRepository();
                    TfIdfConfig config = tfIdfConfigRepository.GetConfig();
                    
                    LoggerWrapper.LogInfo("Clusters build: GetInfo started");
                    var nearestResults = GetNearestResult();
                    LoggerWrapper.LogInfo("Clusters build: GetInfo finished");

                    ComputeClusters(nearestResults, config);

                    UpdateInProgress = false;

                    LoggerWrapper.LogInfo("Clusters build finished");
                }
                catch (Exception exception)
                {
                    UpdateInProgress = false;
                    PreviousBuildFailed = true;
                    LoggerWrapper.LogError(String.Format("Clusters build failed"), exception);
                }
            }, TaskCreationOptions.LongRunning);
        }

        public void StopUpdating()
        {
            if (UpdateInProgress)
            {
                _cancellationTokenSource.Cancel();
                UpdateInProgress = false;
            }
        }

        protected abstract void ComputeClusters(List<NearestResult> results, TfIdfConfig config);

        protected List<NearestResult> GetNearestResult()
        {
            List<NearestResult> nearestResults;

            using (var cacheAdapter = new SimilarResultsAdapter())
            {
                nearestResults = cacheAdapter.GetNearestResults();
            }

            return nearestResults;
        }

        protected Edge[] GetEdges(List<NearestResult> results)
        {
            var edges = new Edge[results.Count];

            for (int i = 0; i < results.Count; i++)
            {
                edges[i] = new Edge
                {
                    FromId = results[i].RecipeId,
                    ToId = results[i].SimilarRecipeId,
                    Weight = results[i].Weight
                };
            }

            return edges;
        }

        protected T? GetSetting<T>(TfIdfConfig config, string name) where T : struct
        {
            string[] lines = config.ClusteringParameters.Split('\n');

            foreach(var line in lines)
            {
                string[] keyValue = line.Split(':');
                string key = keyValue[0].Trim();
                string value = keyValue[1].Trim();

                if (String.Equals(key, name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return (T)Convert.ChangeType(value, typeof(T));
                }
            }

            return null;
        }

        protected struct Edge : IComparable
        {
            public int FromId;

            public int ToId;

            public double Weight;

            public int FromSurrogateId;

            public int ToSurrogateId;

            public int CompareTo(object obj)
            {
                return Math.Sign(Weight - ((Edge)obj).Weight);
            }
        }
    }
}
