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

            var recipeWrapper = csQueryDocument.Find("[itemtype=\"http://schema.org/Recipe\"]");

            sitePage.RecipeName = GetPlainTextBySelector(recipeWrapper, RecipeNameSelector);
            sitePage.Description = GetTextBySelector(recipeWrapper, DescriptionSelector);
            sitePage.Ingredients = GetTextBySelector(recipeWrapper, IngredientsSelector, ",");
            sitePage.RecipeInstructions = GetTextBySelector(recipeWrapper, RecipeInstructionsSelector);
            sitePage.AdditionalData = GetTextBySelector(recipeWrapper, SummarySelector);
            sitePage.Category = GetPlainTextBySelector(recipeWrapper, RecipeCategorySelector);

            sitePage.Rating = ParseIntValue(recipeWrapper, RatingSelector);
            sitePage.CommentsCount = ParseIntValue(recipeWrapper, CommentsCountSelector);

            sitePage.ImageUrl = GetImageUrl(crawledPage, recipeWrapper, ImageSelector);
        }

        protected virtual bool CheckForRecipeSchema(CQ queryDocument)
        {
            return queryDocument.Has("[itemtype=\"http://schema.org/Recipe\"]").Any();
        }
    }
}
