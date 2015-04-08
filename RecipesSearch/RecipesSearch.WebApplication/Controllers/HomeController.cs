using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.SearchEngine.Search;
using RecipesSearch.SearchEngine.Suggestion;
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

            ViewBag.SearchQuery = query ?? String.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                int totalCount;
                string spellcheckedQuery;

                var searchResult = _searchProvider
                    .SearchByQuery(query, pageNumber, searchSettings.ResultsOnPage, searchSettings.EnableSpellchecking, exactMatch, out totalCount, out spellcheckedQuery)
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
                    ExactMatch = exactMatch
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
    }
}