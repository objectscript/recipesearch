using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class KulinarParser : RecipeSchemaParser
    {       
        public override string Id
        {
            get { return "Kulinar"; }
        }

        public KulinarParser()
        {
            RecipeInstructionsSelector = "[itemprop=recipeInstructions] p";
            DescriptionSelector = null;
            CommentsCountSelector = ".recipe_info_stat .article-info_stat-i:nth-child(2) > span";
            RatingSelector = ".recipe_info_stat .article-info_stat-i:nth-child(4) > span";
            RecipeCategorySelector = ".prl-panel.prl-breadcrumb nav span";
        }
    }
}
