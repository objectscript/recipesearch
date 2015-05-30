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
            RecipeInstructionsSelector = "[itemprop=recipeInstructions] .b-page_block__content .b-list__clause__text";
            RecipeCategorySelector = ".l-centercol.utk-recipe .b-breadcrumbs p a:last";
            RatingSelector = "[itemprop=ratingValue]";
        }
    }
}
