using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class GotovimDoma : BaseParser
    {
        public override string Id
        {
            get { return "GotovimDoma"; }
        }

        public override string ParseContent(CrawledPage crawledPage, ref string recipeName)
        {
            var csQueryDocument = crawledPage.CsQueryDocument;

            var recipeWrapper = csQueryDocument.Find("#wrapper > #content_wrapper > #content > .hrecipe");
            if (!recipeWrapper.Any())
            {
                return null;
            }

            recipeName = GetTextBySelector(recipeWrapper, ".rcptitle.fn");

            var recipe = new StringBuilder();
            
            recipe.Append(GetDelimitedTextBySelector(recipeWrapper, ".rcptitle.fn"));
            recipe.Append(GetDelimitedTextBySelector(recipeWrapper, ".rcpstru"));
            recipe.Append(GetDelimitedTextBySelector(recipeWrapper, ".instructions"));

            return recipe.ToString();
        }                
    }
}
