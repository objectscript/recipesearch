using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.CacheService.Services;
using RecipesSearch.Data.Models;

namespace RecipesSearch.SearchEngine.Suggestion
{
    public class SuggestionProvider
    {
        private readonly SuggestionService _suggestionService = new SuggestionService();
        
        public List<string> SuggestByQuery(string query, int count)
        {
            try
            {
                return _suggestionService.SuggestByQuery(query, count);
            }
            catch (Exception exception)
            {
                Logger.LogError(String.Format("SearchProvider.SuggestByQuery failed"), exception);
                return new List<string>();
            }     
        }
    }
}
