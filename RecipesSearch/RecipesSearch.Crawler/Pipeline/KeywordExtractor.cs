using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecipesSearch.SitePagesImporter.Pipeline.Base;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline
{
    class KeywordExtractor : IPageProcessor
    {
        public HtmlDocument ProcessContent(SitePage sitePage, HtmlDocument htmlDocument)
        {
            return htmlDocument;
        }
    }
}
