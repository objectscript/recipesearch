using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using RecipesSearch.BusinessServices.PageStorage;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.SitePagesImporter.Pipeline;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Importer
{
    class PageSaver : IDisposable
    {
        private readonly int _siteId;
        private readonly bool _keywordsProcessingEnabled;

        private readonly List<IPageProcessor> _pageProcessors = new List<IPageProcessor>();
        private readonly CachePageStorage _pageStorage = new CachePageStorage();

        public PageSaver(int siteId, bool keywordsProcessingEnabled)
        {
            _siteId = siteId;
            _keywordsProcessingEnabled = keywordsProcessingEnabled;

            _pageProcessors.AddRange(new IPageProcessor[]
            {
                new KeywordExtractor(), 
                new Preprocessor(),            
                new Parser()
            });
        }

        public void SavePage(CrawledPage crawledPage)
        {
            var sitePage = new SitePage
            {
                SiteID = _siteId,
                URL = crawledPage.Uri.ToString(),
                Content = crawledPage.Content.Text,
                Keywords = String.Empty
            };

            foreach (var pageProcessor in _pageProcessors)
            {
                pageProcessor.ProcessContent(sitePage, crawledPage);
            }

            _pageStorage.SaveSitePage(sitePage, _keywordsProcessingEnabled);
        }

        public void Dispose()
        {
            _pageStorage.Dispose();
        }
    }
}
