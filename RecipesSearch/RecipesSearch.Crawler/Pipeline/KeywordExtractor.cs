using System;
using System.Linq;
using Abot.Poco;
using CsQuery.ExtensionMethods.Internal;
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
            var capitalizedTagName = tagName[0].ToUpper() + tagName.Substring(1);

            var tags = crawledPage.HtmlDocument.DocumentNode.SelectNodes(
                String.Format("//meta[@name='{0}' or @name='{1}']", tagName, capitalizedTagName));

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
