using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
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
                    Logger.LogInfo("Similar results build started");

                    _updatedPagesCount = -1;
                    UpdateInProgress = true;
                    _cancellationTokenSource = cancellationTokenSource ?? new CancellationTokenSource();

                    var tfIdfInfos = GetTfIdfInfos();

                    _updatedPagesCount = 0;

                    GetKNearest(tfIdfInfos, resultsCount);

                    UpdateInProgress = false;

                    Logger.LogInfo("Similar results build finished");
                }
                catch (Exception exception)
                {
                    UpdateInProgress = false;
                    Logger.LogError(String.Format("SimilarResultsBuilder.FindNearestResults failed"), exception);                    
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

        private TfIdfInfo[] GetTfIdfInfos()
        {
            List<SitePageTfIdf> pages;
            
            using (var cacheAdapter = new SimilarResultsAdapter())
            {
                pages = cacheAdapter.GetWordsTfIdf();
            }

            var resultsList = new List<TfIdfInfo>();

            for (int i = 0; i < pages.Count; ++i)
            {
                if (String.IsNullOrEmpty(pages[i].WordsTfIdf))
                {
                    continue;
                }

                var result = new TfIdfInfo
                {
                    Id = pages[i].Id,
                    WordsTfIdf = new Dictionary<string, double>()
                };                

                var tfIdfParts = pages[i].WordsTfIdf.Split(new char[]{' '}, StringSplitOptions.RemoveEmptyEntries);

                for (int j = 0; j < tfIdfParts.Length; ++j)
                {
                    var tfIdfWordParts = tfIdfParts[j].Split(',');

                    var word = tfIdfWordParts[0];
                    double tfidf = Double.Parse(tfIdfWordParts[1], CultureInfo.InvariantCulture);

                    result.WordsTfIdf.Add(word, tfidf);
                }

                resultsList.Add(result);
            }

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
                    var dists = new OrderedBag<Tuple<double, int>>();
                    double maxDist = double.MaxValue;
                    int distsSize = 0;

                    for (int j = 0; j < pages.Length; ++j)
                    {
                        if (i == j)
                        {
                            continue;
                        }

                        var dist = FindDistance(pages[i].WordsTfIdf, pages[j].WordsTfIdf, maxDist);
                        if (dist > maxDist)
                        {
                            continue;
                        }

                        dists.Add(new Tuple<double, int>(dist, pages[j].Id));

                        if (distsSize == k)
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

                    try
                    {
                        cacheAdapter.UpdateSimilarResults(
                            pages[i].Id, 
                            dists.Select(item => item.Item2).ToList(), 
                            dists.Select(item => (int)item.Item1).ToList());
                    }
                    catch (Exception exception)
                    {
                        Logger.LogError(String.Format("SimilarResultsBuilder.GetKNearest save failed"), exception);
                    }

                    Interlocked.Increment(ref _updatedPagesCount);
                }             
            });         
        }

        private double FindDistance(Dictionary<string, double> first, Dictionary<string, double> second, double maxAllowedDist)
        {
            double dist = 0;

            foreach (var key in first.Keys)
            {
                double firstVal = first[key];
                double secondVal = 0;

                if (second.ContainsKey(key))
                {
                    secondVal = second[key];
                }

                dist += (firstVal - secondVal)*(firstVal - secondVal);

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
                    dist += secondVal*secondVal;
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
