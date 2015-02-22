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

            sitePage.Keywords = keywords;
        }
    }
}
