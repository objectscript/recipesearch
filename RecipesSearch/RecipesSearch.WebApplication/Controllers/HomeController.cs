using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RecipesSearch.BusinessServices.Search;
using RecipesSearch.WebApplication.ViewModels;

namespace RecipesSearch.WebApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly SearchProvider _searchProvider = new SearchProvider();

        public ActionResult Index(string query)
        {
            ViewBag.SearchQuery = query ?? String.Empty;

            if (!string.IsNullOrEmpty(query))
            {
                var searchResult = _searchProvider
                    .SearchByQuery(query)
                    .Select(result => new SearchResultViewModel(result))
                    .ToList();               
                return View(searchResult);
            } 

            return View();
        }
    }
}