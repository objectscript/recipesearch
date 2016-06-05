using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using CsQuery;

namespace RecipesSearch.SitePagesImporter.Crawler
{
    class Configuration : CrawlConfiguration
    {
        public bool LoggingEnabled { get; set; }
        public Configuration(Data.Models.Config config)
        {
            MaxCrawlDepth = config.MaxCrawlDepth;
            CrawlTimeoutSeconds = config.CrawlTimeoutSeconds;
            MaxPagesToCrawl = config.MaxPagesToCrawl;

            MaxConcurrentThreads = config.MaxConcurrentThreads;         
            UserAgentString = "abot v1.0 http://code.google.com/p/abot";
            IsExternalPageCrawlingEnabled = false;
            IsExternalPageLinksCrawlingEnabled = false;
            IsHttpRequestAutoRedirectsEnabled = true;
            IsRespectMetaRobotsNoFollowEnabled = false;
            IsRespectAnchorRelNoFollowEnabled = false;
            IsRespectRobotsDotTextEnabled = false;
            IsSendingCookiesEnabled = true;            
            IsUriRecrawlingEnabled = false;

            LoggingEnabled = config.LoggingEnabled;
        }
    }
}
