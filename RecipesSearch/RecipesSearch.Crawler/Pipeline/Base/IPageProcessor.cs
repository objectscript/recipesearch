using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline.Base
{
    interface IPageProcessor
    {
        HtmlDocument ProcessContent(SitePage sitePage, HtmlDocument htmlDocument);
    }
}
