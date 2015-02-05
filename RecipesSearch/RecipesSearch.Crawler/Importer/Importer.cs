using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Crawler;

namespace RecipesSearch.SitePagesImporter.Importer
{
    public class Importer
    {
        private readonly SiteToCrawlRepository _siteToCrawlRepository = new SiteToCrawlRepository();
        private readonly CrawlingHistoryRepository _crawlingHistoryRepository = new CrawlingHistoryRepository();

        private CancellationTokenSource _importerCancellationTokenSource;
        private CancellationTokenSource _crawlerCancellationTokenSource;
        private RecipesCrawler _currentCrawler;
        private CrawlingHistoryItem _currentCrawlingHistoryItem;
        private int _crawledPages;

        private readonly object _lock = new object();

        private List<SiteToCrawl> _sitesQueue;

        public List<SiteToCrawl> SitesQueue
        {
            get { return (_sitesQueue ?? new List<SiteToCrawl>()).ToList(); }
        }

        private static Importer _instance;

        public bool IsImportingInProgress { get; set; }

        public int CrawledPages
        {
            get
            {
                return _crawledPages + (_currentCrawler != null ? _currentCrawler.CrawledPages : 0);
            }
        }

        public static Importer GetImporter()
        {
            if (_instance == null)
            {
                _instance = new Importer();
            }

            return _instance;
        }

        private Importer()
        {
        }

        public void ImportData()
        {
            var sitesToCrawl = _siteToCrawlRepository.GetSitesToCrawl();
            if (!sitesToCrawl.Any())
            {
                return;
            }

            ImportData(sitesToCrawl);
        }

        public void ImportData(IEnumerable<SiteToCrawl> sitesToCrawl)
        {
            if (!IsImportingInProgress)
            {
                _sitesQueue = new List<SiteToCrawl>(sitesToCrawl);
                _importerCancellationTokenSource = new CancellationTokenSource();

                Task.Run((Action)CrawlSites, _importerCancellationTokenSource.Token);

                IsImportingInProgress = true;
            }
            else
            {
                foreach (var siteToCrawl in sitesToCrawl)
                {
                    if (_sitesQueue.All(site => siteToCrawl.Id != site.Id))
                    {
                        _sitesQueue.Add(siteToCrawl);
                    }
                }
            }
        }

        public bool RemoveFromQueue(int siteId)
        {
            if (!IsImportingInProgress)
            {
                return false;
            }

            lock (_lock)
            {
                _sitesQueue.Remove(_sitesQueue.FirstOrDefault(site => site.Id == siteId));
            }

            return true;
        }

        private void CrawlSites()
        {
            var crawlerConfig = new ConfigRepository().GetConfig();

            while (_sitesQueue.Count != 0)
            {
                if (_importerCancellationTokenSource.IsCancellationRequested)
                {
                    return;
                }

                var siteToCrawl = _sitesQueue.First();
                try
                {
                    var pageSaver = new PageSaver(siteToCrawl.Id);
                    _currentCrawlingHistoryItem = new CrawlingHistoryItem
                    {
                        SiteId = siteToCrawl.Id,
                        StardDate = DateTime.Now.ToUniversalTime()
                    };

                    _crawlerCancellationTokenSource = new CancellationTokenSource();
                    _currentCrawler = new RecipesCrawler(siteToCrawl.URL, pageSaver, new Configuration(crawlerConfig));
                    _currentCrawler.Crawl(_crawlerCancellationTokenSource);

                    _currentCrawlingHistoryItem.EndDate = DateTime.Now.ToUniversalTime();
                    _currentCrawlingHistoryItem.IsStopped = _crawlerCancellationTokenSource.IsCancellationRequested;
                    _currentCrawlingHistoryItem.CrawledPagesCount = _currentCrawler.CrawledPages;
                    _crawlingHistoryRepository.SaveCrawlingHistoryItem(_currentCrawlingHistoryItem);

                    if (_importerCancellationTokenSource.IsCancellationRequested)
                    {
                        return;
                    }

                    _crawledPages += _currentCrawler.CrawledPages;
                }
                catch (Exception exception)
                {
                    Logger.LogError(
                        String.Format("SitePagesImporter.Importer.CrawlSites(). Failed to crawl site {0}", siteToCrawl.URL), exception);
                }
                finally
                {
                    lock (_lock)
                    {
                        if (_sitesQueue != null)
                        {
                            _sitesQueue.RemoveAt(0);
                        }
                    }
                }
            }

            ResetParameters();
        }

        public void StopImporting()
        {
            if (!IsImportingInProgress)
            {
                return;
            }

            _importerCancellationTokenSource.Cancel();
            _crawlerCancellationTokenSource.Cancel();
            
            ResetParameters();
        }

        public void StopCurrentSiteImporting()
        {
            if (IsImportingInProgress)
            {
                _crawlerCancellationTokenSource.Cancel();

                //TODO: Revisit
                // Wait for worker thread to remove item from queue
                Thread.Sleep(100);
            }
        }

        private void ResetParameters()
        {
            _crawledPages = 0;
            _currentCrawler = null;
            IsImportingInProgress = false;
            _currentCrawlingHistoryItem = null;
            _sitesQueue = null;
            _crawlerCancellationTokenSource = null;
        }
    }
}
