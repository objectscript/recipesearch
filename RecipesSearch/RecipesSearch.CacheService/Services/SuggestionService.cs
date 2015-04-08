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
    public class SuggestionService : CacheServiceBase
    {
        private const string Endpoint = "/iknow/suggest";

        public List<string> SuggestByQuery(string query, int count, bool spellcheck)
        {
            var url = ServiceBase + Endpoint;
            var parameters = new Dictionary<string, string>
            {
                {"query", Uri.EscapeDataString(query)},
                {"count", count.ToString()},
                {"spellcheck", (spellcheck ? 1 : 0).ToString()}
            };

            var respose = RestHelper.MakeRequest<SuggestionReponse>(url, RestHelper.HttpVerb.GET, parameters, null);

            return respose.Children.Select(item => item.RecipeName).ToList();
        }
    }
}
