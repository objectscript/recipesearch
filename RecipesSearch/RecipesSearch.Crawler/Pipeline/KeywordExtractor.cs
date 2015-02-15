using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using HtmlAgilityPack;
using RecipesSearch.CacheService.Services;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline
{
    class KeywordExtractor : IPageProcessor
    {
        private readonly bool _enhancedKeywordProcessingEnabled;

        public KeywordExtractor(bool enhancedKeywordProcessingEnabled)
        {
            _enhancedKeywordProcessingEnabled = enhancedKeywordProcessingEnabled;
        }

        public void ProcessContent(SitePage sitePage, CrawledPage crawledPage)
        {
            var keywordsTag = crawledPage.HtmlDocument.DocumentNode.SelectNodes("//meta[@name='keywords']").FirstOrDefault();
            
            if (keywordsTag == null)
            {
                return;
            }

            var keywords = keywordsTag.Attributes["content"].Value;

            if (_enhancedKeywordProcessingEnabled && !String.IsNullOrEmpty(keywords))
            {
                var keywordsProcessingService = new KeywordsProcessingService();
                keywords = keywordsProcessingService.ProcessKeywords(keywords);
            }

            sitePage.Keywords = keywords;
        }
    }
}
