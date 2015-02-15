using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.CacheService.Services;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SearchEngine.Search
{
    public class SearchProvider
    {
        private readonly SearchService _searchService = new SearchService();
        
        public List<SitePage> SearchByQuery(string query)
        {
            try
            {
                return _searchService.SearchByQuery(query);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SearchProvider.SearchByQuery failed"), exception);
                return new List<SitePage>();
            }     
        }
    }
}
