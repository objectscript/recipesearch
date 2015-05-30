using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using CsQuery;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SitePagesImporter.Pipeline.Base
{
    class RecipeSchemaParser : BaseParser
    {
        protected string ImageSelector = "[itemprop=image]";
        protected string RecipeInstructionsSelector = "[itemprop=recipeInstructions]";
        protected string RecipeNameSelector = "[itemprop=name]";
        protected string DescriptionSelector = "[itemprop=description]";
        protected string IngredientsSelector = "[itemprop=ingredients]";
        protected string SummarySelector = "[itemprop=summary]";
        protected string RecipeCategorySelector = "[itemprop=recipeCategory]";
        protected string RatingSelector = null;
        protected string CommentsCountSelector = null;

        public override string Id
        {
            get { return "RecipeSchema"; }
        }

        public override void ParseContent(CrawledPage crawledPage, SitePage sitePage)
        {
            var csQueryDocument = crawledPage.CsQueryDocument;

            if (!CheckForRecipeSchema(csQueryDocument))
            {
                return;
            }

            sitePage.RecipeName = GetPlainTextBySelector(csQueryDocument, RecipeNameSelector);
            sitePage.Description = GetTextBySelector(csQueryDocument, DescriptionSelector);
            sitePage.Ingredients = GetTextBySelector(csQueryDocument, IngredientsSelector, ",");
            sitePage.RecipeInstructions = GetTextBySelector(csQueryDocument, RecipeInstructionsSelector);
            sitePage.AdditionalData = GetTextBySelector(csQueryDocument, SummarySelector);
            sitePage.Category = GetPlainTextBySelector(csQueryDocument, RecipeCategorySelector);
           
            sitePage.Rating = ParseIntValue(csQueryDocument, RatingSelector);
            sitePage.CommentsCount = ParseIntValue(csQueryDocument, CommentsCountSelector);

            sitePage.ImageUrl = GetImageUrl(crawledPage, csQueryDocument, ImageSelector);
        }

        protected virtual bool CheckForRecipeSchema(CQ queryDocument)
        {
            return queryDocument.Has("[itemtype=\"http://schema.org/Recipe\"]").Any();
        }
    }
}
