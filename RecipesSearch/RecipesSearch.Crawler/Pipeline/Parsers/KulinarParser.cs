using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using CsQuery;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class KulinarParser : RecipeSchemaParser
    {       
        public override string Id
        {
            get { return "Kulinar"; }
        }

        public override void ParseContent(CrawledPage crawledPage, SitePage sitePage)
        {
            var csQueryDocument = crawledPage.CsQueryDocument;

            if (!CheckForRecipeSchema(csQueryDocument))
            {
                return;
            }

            sitePage.RecipeName = GetTextBySelector(csQueryDocument, "[itemprop=name]");
            sitePage.Description = String.Empty;
            sitePage.Ingredients = GetTextBySelector(csQueryDocument, "[itemprop=ingredients]");
            sitePage.RecipeInstructions = GetTextBySelector(csQueryDocument, "[itemprop=recipeInstructions] p");
            sitePage.AdditionalData = GetTextBySelector(csQueryDocument, "[itemprop=summary]");
            sitePage.ImageUrl = GetImageUrl(crawledPage, csQueryDocument, "[itemprop=image]");
        }
    }
}
