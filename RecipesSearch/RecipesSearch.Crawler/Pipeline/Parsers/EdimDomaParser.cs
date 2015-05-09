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
    class EdimDomaParser : RecipeSchemaParser
    {       
        public override string Id
        {
            get { return "EdimDoma"; }
        }

        public EdimDomaParser()
        {
            RecipeInstructionsSelector = "[itemprop=recipeInstructions] .b-page_block__content";
        }
    }
}
