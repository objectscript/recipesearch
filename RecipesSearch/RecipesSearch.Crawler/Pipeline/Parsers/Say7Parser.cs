using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class Say7Parser : RecipeSchemaParser
    {       
        public override string Id
        {
            get { return "Say7"; }
        }

        public Say7Parser()
        {
            CommentsCountSelector = "[itemprop=interactionCount]";
            RecipeCategorySelector = "#sitepos.bc li > a";
        }
    }
}
