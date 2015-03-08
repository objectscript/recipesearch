using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using HtmlAgilityPack;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline.Base
{
    interface IPageProcessor
    {
        void ProcessContent(SitePage sitePage, CrawledPage crawledPage, SiteToCrawl site);
    }
}
