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
        
        public List<SitePage> SearchByQuery(string query, int pageNumber, int pageSize, out int totalCount)
        {
            try
            {
                return _searchService.SearchByQuery(query, pageNumber, pageSize, out totalCount);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SearchProvider.SearchByQuery failed"), exception);
                totalCount = 0;
                return new List<SitePage>();
            }     
        }
    }
}
