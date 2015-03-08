using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using CsQuery;
using HtmlAgilityPack;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline.Base
{
    abstract class BaseParser
    {
        protected const string DelimiterFormat = "{0}; ";

        public abstract string Id { get; }

        public abstract string ParseContent(CrawledPage crawledPage, ref string recipeName);

        protected virtual string GetTextBySelector(CQ queryObject, string selector)
        {
            return queryObject.Find(selector).Text();        
        }

        protected virtual string GetDelimitedTextBySelector(CQ queryObject, string selector)
        {
            return String.Format(DelimiterFormat, GetTextBySelector(queryObject, selector));
        }

    }
}
