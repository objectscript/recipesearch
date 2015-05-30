using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Abot.Poco;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.SitePagesImporter.Pipeline.Parsers
{
    class GotovimDoma : BaseParser
    {
        public override string Id
        {
            get { return "GotovimDoma"; }
        }

        public override void ParseContent(CrawledPage crawledPage, SitePage sitePage)
        {
            var csQueryDocument = crawledPage.CsQueryDocument;

            var recipeWrapper = csQueryDocument.Find("#wrapper > #content_wrapper > #content > .hrecipe");
            if (!recipeWrapper.Any())
            {
                return;
            }

            sitePage.RecipeName = GetPlainTextBySelector(recipeWrapper, ".rcptitle.fn");
            sitePage.Ingredients = GetTextBySelector(recipeWrapper, ".rcpstru");
            sitePage.RecipeInstructions = GetTextBySelector(recipeWrapper, ".instructions");
            sitePage.Description = String.Empty;
            sitePage.ImageUrl = GetImageUrl(crawledPage, recipeWrapper, ".photo");

            sitePage.Category = GetPlainTextBySelector(csQueryDocument, "#wrapper > #content_wrapper > #content > #nav1 > a:last");

            var commentsCountText = recipeWrapper.Find("#comments .hdr .count").Text();
            if (!String.IsNullOrEmpty(commentsCountText))
            {
                commentsCountText = commentsCountText.Trim().Replace("(", "").Replace(")", "");
                int commentsCount;

                if (Int32.TryParse(commentsCountText, out commentsCount))
                {
                    sitePage.CommentsCount = commentsCount;
                }
            }
        }                
    }
}
