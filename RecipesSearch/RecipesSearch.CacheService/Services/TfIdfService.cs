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
    public class TfIdfService : CacheServiceBase
    {
        private const string Endpoint = "/iknow/updateTfIdf";

        public bool UpdateTfIdf()
        {
            var url = ServiceBase + Endpoint;
           
            RestHelper.MakeRequest<SuggestionReponse>(url, RestHelper.HttpVerb.POST, null, null);

            return true;
        }
    }
}
