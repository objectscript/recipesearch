using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class EdimDomaParser : RecipeSchemaParser
    {       
        public override string Id
        {
            get { return "EdimDoma"; }
        }

        public EdimDomaParser()
        {
            IngredientsSelector = ".rec-ingredients .b-page_block__outside > p, .rec-ingredients .b-page_block__outside [itemprop=ingredients]";
            RecipeInstructionsSelector = "[itemprop=recipeInstructions] .b-page_block__content .b-list__clause__text";
            RecipeCategorySelector = ".l-centercol.utk-recipe .b-breadcrumbs p a";
            RatingSelector = "[itemprop=ratingValue]";
        }
    }
}
