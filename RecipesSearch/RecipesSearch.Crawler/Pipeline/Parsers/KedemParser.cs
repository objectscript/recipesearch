﻿using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class KedemParser : RecipeSchemaParser
    {       
        public override string Id
        {
            get { return "Kedem"; }
        }

        public KedemParser()
        {
            RecipeCategorySelector = ".page_path a:nth-last-child(2)";
            RatingSelector = "[itemprop=ratingValue]";
        }
    }
}
