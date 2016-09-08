using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class AllrecipesParser: RecipeSchemaParser
    {
        public override string Id
        {
            get { return "Allrecipes"; }
        }

        public AllrecipesParser()
        {
            CommentsCountSelector = ".recipe-reviews__header--count";
            RecipeCategorySelector = ".breadcrumbs li:not(:first-child,:last-child,:nth-child(2)) a span";
        }
    }
}
