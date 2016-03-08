using System;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.CacheService.Services;
using RecipesSearch.Data.Models;
using RecipesSearch.SearchEngine.SimilarResults;

namespace RecipesSearch.SearchEngine.Search
{
    public class SearchProvider
    {
        private readonly SearchService _searchService = new SearchService();

        public List<SitePage> SearchByQuery(
            string query, 
            int pageNumber,
            int pageSize,
            bool exactMatch, 
            SearchSettings searchSettings,
            out int totalCount, 
            out string spellcheckQuery)
        {
            try
            {
                var searchResults = _searchService.SearchByQuery(
                    query, 
                    pageNumber,
                    pageSize, 
                    searchSettings.EnableSpellchecking,
                    exactMatch, 
                    searchSettings.OnlineTfIdfEnabled,
                    searchSettings.UseClusters,
                    searchSettings.OnlySearchResultsWhenUsingClusters,
                    searchSettings.SkipIrrelevantResults,
                    searchSettings.FilterSearchQuery,
                    searchSettings.MaxOnlineIdfRecipesCount,
                    searchSettings.OnlineTfIdfBuilderName,
                    out totalCount, 
                    out spellcheckQuery);

                int startIndex = (pageNumber - 1)*pageSize;

                if (searchSettings.OnlineTfIdfEnabled)
                {
                    var tfIdfInfo = searchResults.Select(searchResult => searchResult.TfIdfInfo).ToArray();
                    var resultsToUpdate = searchResults.Skip(startIndex).Take(pageSize).ToList();

                    for (var i = 0; i < resultsToUpdate.Count; ++i)
                    {                      
                        SimilarResultsBuilder.FindNearest(tfIdfInfo, i, searchSettings.OnlineTfIdfSimilarResultsCount, (id, nearestResults, weights) =>
                        {
                            resultsToUpdate[i].SimilarResults = new List<SitePage>();

                            for(var j = 0; j < nearestResults.Count; ++j)
                            {
                                var nearestPage = FindAndCopyPage(searchResults, nearestResults[j]);
                                nearestPage.SimilarRecipeWeight = weights[j];
                                resultsToUpdate[i].SimilarResults.Add(nearestPage);
                            }

                            resultsToUpdate[i].SimilarResults.Reverse();
                        });
                    }

                    searchResults = resultsToUpdate;
                }

                return searchResults;
            }
            catch (Exception exception)
            {
                LoggerWrapper.LogError(String.Format("SearchProvider.SearchByQuery failed"), exception);
                totalCount = 0;
                spellcheckQuery = String.Empty;
                return new List<SitePage>();
            }     
        }

        private SitePage FindAndCopyPage(IEnumerable<SitePage> pages, int id)
        {
            var page = pages.First(p => p.Id == id);
            return new SitePage
            {
                Id = page.Id,
                SiteID = page.Id,
                AdditionalData = page.AdditionalData,
                Category = page.Category,
                CommentsCount = page.CommentsCount,
                CreatedDate = page.CreatedDate,
                ModifiedDate = page.ModifiedDate,
                Description = page.Description,
                IsActive = page.IsActive,
                ImageUrl = page.ImageUrl,
                Ingredients = page.Ingredients,
                Keywords = page.Keywords,
                Rating = page.Rating,
                RecipeInstructions = page.RecipeInstructions,
                RecipeName = page.RecipeName,
                URL = page.URL
            };
        }
    }
}
