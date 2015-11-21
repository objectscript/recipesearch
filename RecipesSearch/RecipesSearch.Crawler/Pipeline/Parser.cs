using Abot.Poco;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline
{
    class Parser : IPageProcessor
    {
        public void ProcessContent(SitePage sitePage, CrawledPage crawledPage, SiteToCrawl site)
        {
            var parser = ParsersResolver.GetParserById(site.ParserId);

            parser.ParseContent(crawledPage, sitePage);
        }
    }
}
