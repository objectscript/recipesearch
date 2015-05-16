using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.BusinessServices.PageStorage;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.SitePagesImporter.Pipeline;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Importer
{
    class PageSaver : IDisposable
    {
        private readonly SiteToCrawl _siteToCrawl;
        private readonly bool _keywordsProcessingEnabled;
        private readonly bool _updateSpellcheckDict;
        private readonly bool _buildTf;
        private readonly string _tfBuilderName;

        private readonly List<IPageProcessor> _pageProcessors = new List<IPageProcessor>();
        private readonly CachePageStorage _pageStorage = new CachePageStorage();

        public PageSaver(SiteToCrawl siteToCrawl, bool keywordsProcessingEnabled, bool updateSpellcheckDict, bool buildTf, string tfBuilderName)
        {
            _siteToCrawl = siteToCrawl;
            _keywordsProcessingEnabled = keywordsProcessingEnabled;
            _updateSpellcheckDict = updateSpellcheckDict;
            _tfBuilderName = tfBuilderName;
            _buildTf = buildTf;

            _pageProcessors.AddRange(new IPageProcessor[]
            {
                new KeywordExtractor(), 
                new Preprocessor(),            
                new Parser()
            });
        }

        public void SavePage(CrawledPage crawledPage)
        {
            try
            {
                var sitePage = new SitePage
                {
                    SiteID = _siteToCrawl.Id,
                    URL = crawledPage.Uri.ToString(),
                    Keywords = String.Empty
                };

                foreach (var pageProcessor in _pageProcessors)
                {
                    pageProcessor.ProcessContent(sitePage, crawledPage, _siteToCrawl);
                }

                // Do not save empty string; e.g. rejected by parser
                if (!String.IsNullOrEmpty(sitePage.Description) || !String.IsNullOrEmpty(sitePage.Ingredients) ||
                    !String.IsNullOrEmpty(sitePage.RecipeInstructions))
                {
                    _pageStorage.SaveSitePage(sitePage, _keywordsProcessingEnabled, _updateSpellcheckDict, _buildTf, _tfBuilderName);
                }

            }
            catch (Exception e)
            {
                Logger.LogError("Crawler: Error saving page.", e);
            }   
        }

        public void Dispose()
        {
            _pageStorage.Dispose();
        }
    }
}
