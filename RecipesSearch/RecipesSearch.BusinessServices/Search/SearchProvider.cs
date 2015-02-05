using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.DAL.Cache;
using RecipesSearch.Data.Models;

namespace RecipesSearch.BusinessServices.Search
{
    public class SearchProvider
    {
        private readonly CacheAdapter _cacheAdapter = new CacheAdapter();
        
        public List<SitePage> SearchByQuery(string query)
        {
            try
            {
                return _cacheAdapter.SearchByQuery(query);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SearchProvider.SearchByQuery failed"), exception);
                return new List<SitePage>();
            }     
        }
    }
}
