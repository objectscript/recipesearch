using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.CacheService.Models;
using RecipesSearch.CacheService.Utils;
using RecipesSearch.Data.Models;

namespace RecipesSearch.CacheService.Services
{
    public class SearchService : CacheServiceBase
    {
        private const string Endpoint = "/iknow/search";

        public List<SitePage> SearchByQuery(string query, int pageNumber, int pageSize)
        {
            var url = ServiceBase + Endpoint;
            var parameters = new Dictionary<string, string>
            {
                {"query", Uri.EscapeDataString(query)},
                {"startIndex", (pageNumber * pageSize).ToString()},
                {"pageSize", pageSize.ToString()},
            };

            var respose = RestHelper.MakeRequest<SearchResponse>(url, RestHelper.HttpVerb.GET, parameters, null);

            return respose.Children.ToList();
        }
    }
}
