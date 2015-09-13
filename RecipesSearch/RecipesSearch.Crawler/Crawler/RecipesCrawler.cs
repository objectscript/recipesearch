using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Abot.Core;
using Abot.Crawler;
using Abot.Poco;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Importer;

namespace RecipesSearch.SitePagesImporter.Crawler
{
    class RecipesCrawler
    {
        private readonly PoliteWebCrawler _crawler;
        private readonly Configuration _configuration;
        private readonly PageSaver _pageSaver;

        private readonly string _url;

        public bool CrawlingStarted { get; private set; }
        public int CrawledPages { get; private set; }

        public RecipesCrawler(string url, PageSaver pageSaver, Configuration configuration)
        {
            _url = url;
            _configuration = configuration;
            _crawler = new PoliteWebCrawler(configuration);
            _pageSaver = pageSaver;

            _crawler.PageCrawlCompleted += ProcessPageCrawlCompleted;

            _crawler.PageCrawlStartingAsync += ProcessPageCrawlStarting;
            _crawler.PageCrawlDisallowedAsync += PageCrawlDisallowed;
            _crawler.PageLinksCrawlDisallowedAsync += PageLinksCrawlDisallowed;
        }

        public void Crawl(CancellationTokenSource cancellationTokenSource)
        {
            CrawlingStarted = true;
            _crawler.Crawl(new Uri(_url), cancellationTokenSource);
        }

        private void ProcessPageCrawlCompleted(object sender, PageCrawlCompletedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;

            if (crawledPage.WebException != null || crawledPage.HttpWebResponse.StatusCode != HttpStatusCode.OK)
            {
                Log(String.Format("Crawl of page failed {0}", crawledPage.Uri.AbsoluteUri));
            }
            else
            {
                _pageSaver.SavePage(crawledPage);
                CrawledPages++;
                Log(String.Format("Crawl of page succeeded {0}", crawledPage.Uri.AbsoluteUri));
            }

            if (string.IsNullOrEmpty(crawledPage.Content.Text))
            {
                Log(String.Format("Page had no content {0}", crawledPage.Uri.AbsoluteUri));
            }
        }

        private void ProcessPageCrawlStarting(object sender, PageCrawlStartingArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Log(String.Format("About to crawl link {0} which was found on page {1}", pageToCrawl.Uri.AbsoluteUri, pageToCrawl.ParentUri.AbsoluteUri));
        }

        private void PageLinksCrawlDisallowed(object sender, PageLinksCrawlDisallowedArgs e)
        {
            CrawledPage crawledPage = e.CrawledPage;
            Log(String.Format("Did not crawl the links on page {0} due to {1}", crawledPage.Uri.AbsoluteUri, e.DisallowedReason));
        }

        private void PageCrawlDisallowed(object sender, PageCrawlDisallowedArgs e)
        {
            PageToCrawl pageToCrawl = e.PageToCrawl;
            Log(String.Format("Did not crawl page {0} due to {1}", pageToCrawl.Uri.AbsoluteUri, e.DisallowedReason));
        }

        private void Log(string message)
        {
            if (_configuration.LoggingEnabled)
            {
                LoggerWrapper.LogCrawlerInfo(message);
            }

        }
    }
}
