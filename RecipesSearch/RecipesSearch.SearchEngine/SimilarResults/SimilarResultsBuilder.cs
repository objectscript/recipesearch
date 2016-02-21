using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.Cache.Adapters;
using RecipesSearch.Data.Views;
using Wintellect.PowerCollections;

namespace RecipesSearch.SearchEngine.SimilarResults
{
    public class SimilarResultsBuilder
    {
        public int UpdatedPagesCount
        {
            get { return _updatedPagesCount; }
        }

        public bool UpdateInProgress { get; private set; }

        public decimal Percentage { get; private set; }

        public bool PreviousBuildFailed { get; private set; }

        private static SimilarResultsBuilder _instance;

        private int _updatedPagesCount;

        private CancellationTokenSource _cancellationTokenSource;

        private SimilarResultsBuilder()
        {           
        }

        public static SimilarResultsBuilder GetInstance()
        {
            if (_instance == null)
            {
                _instance = new SimilarResultsBuilder();
            }

            return _instance;
        }

        public Task FindNearestResults(int resultsCount, CancellationTokenSource cancellationTokenSource = null)
        {
            return Task.Factory.StartNew(() =>
            {
                try
                {
                    UpdateInProgress = true;
                    _updatedPagesCount = -1;
                    PreviousBuildFailed = false;
                    Percentage = 0;

                    LoggerWrapper.LogInfo("Similar results build started");
                 
                    _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

                    LoggerWrapper.LogInfo("Similar results build: GetInfo started");
                    var tfIdfInfos = GetTfIdfInfos();
                    LoggerWrapper.LogInfo("Similar results build: GetInfo finished");

                    _updatedPagesCount = 0;

                    GetKNearest(tfIdfInfos, resultsCount);

                    UpdateInProgress = false;

                    LoggerWrapper.LogInfo("Similar results build finished");
                }
                catch (Exception exception)
                {
                    UpdateInProgress = false;
                    PreviousBuildFailed = true;
                    LoggerWrapper.LogError(String.Format("SimilarResultsBuilder.FindNearestResults failed"), exception);                    
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

        private TfIdfInfo[] GetTfIdfInfos()
        {
            List<SitePageTfIdf> tfidfs;

            using (var cacheAdapter = new SimilarResultsAdapter())
            {
                tfidfs = cacheAdapter.GetWordsTfIdf();
            }

            var resultsList = new List<TfIdfInfo>();

            tfidfs
                .GroupBy(tfidf => tfidf.RecipeId)
                .ToList()
                .ForEach(group =>
                {
                    var tfIdfInfo = new TfIdfInfo {Id = group.Key};

                    tfIdfInfo.WordsTfIdf = group.ToDictionary(
                        sitePageTfIdf => sitePageTfIdf.Word,
                        sitePageTfIdf => sitePageTfIdf.TFIDF);

                    resultsList.Add(tfIdfInfo);
                });

            return resultsList.ToArray();
        }

        private void GetKNearest(TfIdfInfo[] pages, int k)
        {           
            var parallelOptions = new ParallelOptions
            {
                CancellationToken = _cancellationTokenSource.Token
            };

            Parallel.For(0, pages.Length, parallelOptions, i =>
            {
                using (var cacheAdapter = new SimilarResultsAdapter())
                {                  
                    try
                    {
                        FindNearest(pages, i, k, cacheAdapter.UpdateSimilarResults);
                    }
                    catch (Exception exception)
                    {
                        LoggerWrapper.LogError(String.Format("SimilarResultsBuilder.GetKNearest for id = {0} failed", pages[i].Id), exception);
                    }

                    Interlocked.Increment(ref _updatedPagesCount);
                    Percentage = _updatedPagesCount * 1m /pages.Length * 100m;
                }             
            });         
        }

        public static void FindNearest(TfIdfInfo[] pages, int idx, int countToFind, Action<int, List<int>, List<int>> callback)
        {
            var dists = new OrderedBag<Tuple<double, int>>();
            double maxDist = double.MaxValue;
            int distsSize = 0;
            int wordsCount = 0;
            int wordsN = 0;
            for (int j = 0; j < pages.Length; ++j)
            {
                if (idx == j)
                {
                    continue;
                }

                int unique;
                var dist = FindDistance(pages[idx].WordsTfIdf, pages[j].WordsTfIdf, maxDist, out unique);
                if (dist > maxDist)
                {
                    continue;
                }

                wordsCount += unique;
                wordsN++;
                dists.Add(new Tuple<double, int>(dist, pages[j].Id));

                if (distsSize == countToFind)
                {
                    dists.RemoveLast();
                    var lastDist = dists.GetLast().Item1;
                    maxDist = lastDist;
                }
                else
                {
                    distsSize++;
                }
            }

            double avgWordsCount = wordsCount * 1.0 / wordsN;

            callback(pages[idx].Id,
                dists.Select(item => item.Item2).ToList(),
                dists.Select(item => (int) item.Item1).ToList());
        }

        public static double FindDistance(Dictionary<string, double> first, Dictionary<string, double> second, double maxAllowedDist, out int unique)
        {
            double dist = 0;
            unique = 0;
            foreach (var key in first.Keys)
            {
                double firstVal = first[key];
                double secondVal = 0;

                if (second.ContainsKey(key))
                {
                    secondVal = second[key];
                }
                unique++;
                dist += (firstVal - secondVal) * (firstVal - secondVal);

                if (dist > maxAllowedDist)
                {
                    return dist;
                }
            }

            foreach (var key in second.Keys)
            {
                double secondVal = second[key];

                if (!first.ContainsKey(key))
                {
                    unique++;
                    dist += secondVal * secondVal;
                }

                if (dist > maxAllowedDist)
                {
                    return dist;
                }
            }

            return dist;
        }
    }
}
