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
using Wintellect.PowerCollections;

namespace RecipesSearch.SearchEngine.SimilarResults
{
    public class TfIdfBuilder
    {
        public bool UpdateInProgress { get; private set; }

        private static TfIdfBuilder _instance;

        private TfIdfBuilder()
        {           
        }

        public static TfIdfBuilder GetTfIdfBuilder()
        {
            if (_instance == null)
            {
                _instance = new TfIdfBuilder();
            }

            return _instance;
        }

        public void BuildTfIdf()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdateInProgress = true;

                    using (var cacheAdapter = new SimilarResultsAdapter())
                    {
                        cacheAdapter.UpdateTfIdf();
                    }

                    UpdateInProgress = false;
                }
                catch (Exception exception)
                {
                    Logger.LogError(String.Format("TfIdfBuilder.BuildTfIdf failed"), exception);
                    UpdateInProgress = false;
                }               
            }, TaskCreationOptions.AttachedToParent);
        }     
    }
}
