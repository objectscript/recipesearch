using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline
{
    class Preprocessor : IPageProcessor
    {
        public HtmlDocument ProcessContent(SitePage sitePage, HtmlDocument htmlDocument)
        {
            sitePage.Content = htmlDocument.DocumentNode.SelectNodes("//body")[0].OuterHtml;
            return htmlDocument;
        }
    }
}
