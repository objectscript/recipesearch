using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class RecipeSchemaParser : BaseParser
    {       
        public override string Id
        {
            get { return "RecipeSchema"; }
        }

        public override string ParseContent(CrawledPage crawledPage, ref string recipeName)
        {
            var csQueryDocument = crawledPage.CsQueryDocument;

            if (!csQueryDocument.Has("[itemtype=\"http://schema.org/Recipe\"]").Any())
            {
                return null;
            }

            recipeName = GetTextBySelector(csQueryDocument, "[itemprop=name]");

            var recipe = new StringBuilder();

            recipe.Append(GetDelimitedTextBySelector(csQueryDocument, "[itemprop=name]"));
            recipe.Append(GetDelimitedTextBySelector(csQueryDocument, "[itemprop=summary]"));
            recipe.Append(GetDelimitedTextBySelector(csQueryDocument, "[itemprop=description]"));
            recipe.Append(GetDelimitedTextBySelector(csQueryDocument, "[itemprop=ingredients]"));
            recipe.Append(GetDelimitedTextBySelector(csQueryDocument, "[itemprop=recipeInstructions]"));

            return recipe.ToString();
        }
    }
}
