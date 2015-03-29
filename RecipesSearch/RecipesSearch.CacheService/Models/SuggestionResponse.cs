using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;

namespace RecipesSearch.CacheService.Models
{
    class SuggestionReponse
    {
        public IEnumerable<SuggestionItemResponse> Children { get; set; } 
    }
    
    class SuggestionItemResponse
    {
        public int SiteId { get; set; }

        public string Keywords { get; set; }

        public string URL { get; set; }

        public string RecipeName { get; set; }
    }
}
