using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.BusinessServices.SqlRepositories.Base;
using RecipesSearch.Data.Models;
using RecipesSearch.SearchEngine.Search;
using RecipesSearch.SearchEngine.Suggestion;
using RecipesSearch.WebApplication.Controllers.Filters;
using RecipesSearch.WebApplication.Enums;
using RecipesSearch.WebApplication.ViewModels;

namespace RecipesSearch.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly SearchProvider _searchProvider = new SearchProvider();
        private readonly SuggestionProvider _suggestionProvider = new SuggestionProvider();
        private readonly SearchSettingsRepository _searchSettingsRepository = new SearchSettingsRepository();

        public ActionResult Index(string query, int pageNumber = 1, bool exactMatch = false)
        {
            var searchSettings = _searchSettingsRepository.GetSearchSettings();

            query = query ?? String.Empty;
            query = query.Trim();

            ViewBag.SearchQuery = query;

            if (!string.IsNullOrEmpty(query))
            {
                int totalCount;
                string spellcheckedQuery;

                var searchResult = _searchProvider
                    .SearchByQuery(query, pageNumber, searchSettings.ResultsOnPage, exactMatch, searchSettings, out totalCount, out spellcheckedQuery)
                    .Select(result => new SearchResultItemViewModel(result))
                    .ToList();

                var searchViewModel = new SearchViewModel
                {
                    ResultItems = searchResult,
                    TotalCount = totalCount,
                    ResultsOnPage = searchSettings.ResultsOnPage,
                    CurrentPage = pageNumber,
                    CurrentQuery = query,
                    SpellcheckingEnabled = searchSettings.EnableSpellchecking,
                    SpellcheckedQuery = spellcheckedQuery,
                    ExactMatch = exactMatch,
                    DefaultResultView = (ResultsViews)searchSettings.DefaultResultsView,
                    UseClustering = searchSettings.UseClusters
                };

                return View(searchViewModel);
            } 

            return View();
        }

        public JsonResult SuggestRecipe(string query)
        {
            var searchSettings = _searchSettingsRepository.GetSearchSettings();
            var items = _suggestionProvider.SuggestByQuery(query, searchSettings.SuggestionsCount, searchSettings.EnableSpellcheckingForSuggest);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        [Compress]
        public ActionResult GetGraphData(string query, bool exactMatch)
        {
            var searchSettings = _searchSettingsRepository.GetSearchSettings();

            query = query ?? String.Empty;
            query = query.Trim();

            ViewBag.SearchQuery = query;

            var results = new List<SearchResultItemViewModel>();

            if (!string.IsNullOrEmpty(query))
            {
                int totalCount;
                string spellcheckedQuery;

                results = _searchProvider
                    .SearchByQuery(query, 1, searchSettings.ResultsForGraphView, exactMatch, searchSettings, out totalCount, out spellcheckedQuery)
                    .Select(result => new SearchResultItemViewModel(result))
                    .ToList();               
            }

            return Json(new { Recipes = results, UseClusters = searchSettings.UseClusters }, JsonRequestBehavior.AllowGet);
        }

        [Compress]
        public ActionResult GetRecipe(int recipeId)
        {
            var repository = new SqlRepositoryBase();
            var recipe = repository.GetEntityById<SitePage>(recipeId);

            return Json(recipe, JsonRequestBehavior.AllowGet);
        }
    }
}