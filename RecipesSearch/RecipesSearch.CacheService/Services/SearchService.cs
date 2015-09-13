using System;
using System.Collections.Generic;
using System.Linq;
using RecipesSearch.CacheService.Models;
using RecipesSearch.CacheService.Utils;
using RecipesSearch.Data.Models;

namespace RecipesSearch.CacheService.Services
{
    public class SearchService : CacheServiceBase
    {
        private const string Endpoint = "/recipes/doSearch";

        public List<SitePage> SearchByQuery(string query, int pageNumber, int pageSize, bool spellcheck, bool exactMatch, out int totalCount, out string spellcheckQuery)
        {
            var url = ServiceBase + Endpoint;
            var parameters = new Dictionary<string, string>
            {
                {"query", Uri.EscapeDataString(query)},
                {"pageNumber", pageNumber.ToString()},
                {"pageSize", pageSize.ToString()},
                {"spellcheck", (spellcheck ? 1 : 0).ToString()},
                {"exactMatch", (exactMatch ? 1 : 0).ToString()}
            };

            var respose = RestHelper.MakeRequest<SearchResponse>(url, RestHelper.HttpVerb.GET, parameters, null);

            totalCount = respose.TotalCount;
            spellcheckQuery = respose.SpellcheckedQuery;

            return respose.Items.Select(result =>
            {
                result.Result.SimilarResults = result.SimilarResults;
                return result.Result;
            })
            .ToList();
        }
    }
}
