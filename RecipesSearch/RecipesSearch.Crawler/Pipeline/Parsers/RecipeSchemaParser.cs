﻿using System;
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
    class RecipeSchemaParser : BaseParser
    {       
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

            sitePage.RecipeName = GetTextBySelector(csQueryDocument, "[itemprop=name]");
            sitePage.Description = GetTextBySelector(csQueryDocument, "[itemprop=description]");
            sitePage.Ingredients = GetTextBySelector(csQueryDocument, "[itemprop=ingredients]");
            sitePage.RecipeInstructions = GetTextBySelector(csQueryDocument, "[itemprop=recipeInstructions]");
            sitePage.AdditionalData = GetTextBySelector(csQueryDocument, "[itemprop=summary]");
        }

        protected virtual bool CheckForRecipeSchema(CQ queryDocument)
        {
            return queryDocument.Has("[itemtype=\"http://schema.org/Recipe\"]").Any();
        }
    }
}
