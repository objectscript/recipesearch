using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.CacheService.Models;
using RecipesSearch.CacheService.Utils;

namespace RecipesSearch.CacheService.Services
{
    public class KeywordsProcessingService : CacheServiceBase
    {
        private const string Endpoint = "/iknow/processKeywords";

        public string ProcessKeywords(string keywords)
        {
            var url = ServiceBase + Endpoint;
            var parameters = new Dictionary<string, string>
            {
                {"keywords", Uri.EscapeDataString(keywords)}
            };

            var respose = RestHelper.MakeRequest<KeywordResponse>(url, RestHelper.HttpVerb.GET, parameters, null);

            return respose.Keywords;
        }
    }
}
