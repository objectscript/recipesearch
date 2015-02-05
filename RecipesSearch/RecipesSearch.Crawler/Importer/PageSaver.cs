using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using RecipesSearch.BusinessServices.PageStorage;
using RecipesSearch.SitePagesImporter.Pipeline;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Importer
{
    class PageSaver
    {
        private readonly int _siteId;

        private readonly List<IPageProcessor> _pageProcessors = new List<IPageProcessor>();
        private readonly CachePageStorage _pageStorage = new CachePageStorage();

        public PageSaver(int siteId)
        {
            _siteId = siteId;

            _pageProcessors.AddRange(new IPageProcessor[]
            {
                new KeywordExtractor(), 
                new Preprocessor(),            
                new Parser()
            });
        }

        public void SavePage(CrawledPage crawledPage)
        {
            var htmlDocument = crawledPage.HtmlDocument;
            var sitePage = new SitePage
            {
                SiteID = _siteId,
                URL = crawledPage.Uri.ToString(),
                Content = crawledPage.Content.Text,
                Keyword = String.Empty
            };

            foreach (var pageProcessor in _pageProcessors)
            {
                htmlDocument = pageProcessor.ProcessContent(sitePage, htmlDocument);
            }

            _pageStorage.SaveSitePage(sitePage);
        }
    }
}
