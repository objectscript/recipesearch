using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipesSearch.BusinessServices.SqlRepositories;
using RecipesSearch.CacheService.Services;
using RecipesSearch.WebApplication.ViewModels;

namespace RecipesSearch.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly SearchService _searchProvider = new SearchService();
        private readonly SearchSettingsRepository _searchSettingsRepository = new SearchSettingsRepository();

        public ActionResult Index(string query, int pageNumber = 0)
        {
            var searchSettings = _searchSettingsRepository.GetSearchSettings();

            ViewBag.SearchQuery = query ?? String.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                var searchResult = _searchProvider
                    .SearchByQuery(query, pageNumber, searchSettings.ResultsOnPage)
                    .Select(result => new SearchResultViewModel(result))
                    .ToList();               
                return View(searchResult);
            } 

            return View();
        }
    }
}