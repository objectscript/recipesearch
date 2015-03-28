using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using HtmlAgilityPack;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline
{
    class Parser : IPageProcessor
    {
        public void ProcessContent(SitePage sitePage, CrawledPage crawledPage, SiteToCrawl site)
        {
            var parser = ParsersResolver.GetParserById(site.ParserId);

            if (parser == null)
            {
                return;
            }
            
            parser.ParseContent(crawledPage, sitePage);
        }
    }
}
