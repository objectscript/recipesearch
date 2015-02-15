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
            if (crawledPage.HtmlDocument == null || crawledPage.HtmlDocument.DocumentNode == null)
            {
                return;

            }
            var keywordsTags = crawledPage.HtmlDocument.DocumentNode.SelectNodes("//meta[@name='keywords']");

            if (keywordsTags == null)
            {
                return;
            }

            var keywordsTag = keywordsTags.FirstOrDefault();
            
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
