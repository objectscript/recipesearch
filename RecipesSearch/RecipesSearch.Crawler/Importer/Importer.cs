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
        private Task _importerTask;
        private SiteToCrawl _currentCrawledSite;
        private int _crawledPages;

        private readonly object _lock = new object();

        private List<SiteToCrawl> _sitesQueue;

        public List<SiteToCrawl> SitesQueue
        {
            get
            {
                if (_sitesQueue == null)
                {
                    return new List<SiteToCrawl>();
                }

                var queue = new List<SiteToCrawl>(_sitesQueue);
                
                if (_currentCrawledSite != null)
                {
                    queue.Insert(0, _currentCrawledSite);
                }               

                return queue;
            }
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

                _importerTask = Task.Run((Action)CrawlSites, _importerCancellationTokenSource.Token);

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

                try
                {
                    _currentCrawledSite = _sitesQueue.First();

                    lock (_lock)
                    {
                        _sitesQueue.RemoveAt(0);
                    }

                    using (var pageSaver = new PageSaver(_currentCrawledSite.Id, crawlerConfig.EnhancedKeywordProcessing))
                    {
                        _currentCrawlingHistoryItem = new CrawlingHistoryItem
                        {
                            SiteId = _currentCrawledSite.Id,
                            StardDate = DateTime.Now.ToUniversalTime()
                        };

                        _crawlerCancellationTokenSource = new CancellationTokenSource();
                        _currentCrawler = new RecipesCrawler(_currentCrawledSite.URL, pageSaver, new Configuration(crawlerConfig));
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
                }
                catch (Exception exception)
                {
                    Logger.LogError(
                        String.Format("SitePagesImporter.Importer.CrawlSites(). Failed to crawl site {0}", _currentCrawledSite.URL), exception);
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

            Task.WaitAll(_importerTask);
        }

        public void StopCurrentSiteImporting()
        {
            if (IsImportingInProgress)
            {
                _crawlerCancellationTokenSource.Cancel();
                _currentCrawledSite = null;

                if (_sitesQueue == null || _sitesQueue.Count == 0)
                {
                    Task.WaitAll(_importerTask);
                }
            }
        }

        private void ResetParameters()
        {
            _crawledPages = 0;
            _currentCrawler = null;
            IsImportingInProgress = false;
            _sitesQueue = null;
            _crawlerCancellationTokenSource = null;
            _currentCrawlingHistoryItem = null;
            _currentCrawledSite = null;
            _importerTask = null;
        }
    }
}
