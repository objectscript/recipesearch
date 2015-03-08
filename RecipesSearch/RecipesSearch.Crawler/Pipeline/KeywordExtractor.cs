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
        public void ProcessContent(SitePage sitePage, CrawledPage crawledPage, SiteToCrawl site)
        {
            if (crawledPage.HtmlDocument == null || crawledPage.HtmlDocument.DocumentNode == null)
            {
                return;
            }

            var keywords = GetMetaTagContent(crawledPage, "keywords");
            var description = GetMetaTagContent(crawledPage, "description");
            sitePage.Keywords = String.Format("{0}; {1}", keywords, description);
        }

        private string GetMetaTagContent(CrawledPage crawledPage, string tagName)
        {
            var tags = crawledPage.HtmlDocument.DocumentNode.SelectNodes(String.Format("//meta[@name='{0}']", tagName));

            if (tags == null)
            {
                return String.Empty;
            }

            var tag = tags.FirstOrDefault();

            if (tag == null)
            {
                return String.Empty;
            }

            var tagContent = tag.Attributes["content"].Value;

            return tagContent;
        }
    }
}
