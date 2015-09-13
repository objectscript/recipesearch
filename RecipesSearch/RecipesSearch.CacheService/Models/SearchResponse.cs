using System.Collections.Generic;
using RecipesSearch.Data.Models;
using RecipesSearch.Data.Views;

namespace RecipesSearch.CacheService.Models
{
    class SearchResponse
    {
        public IEnumerable<SitePageResult> Items;

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }

        public string SpellcheckedQuery { get; set; }

        internal class SitePageResult
        {
            public SitePage Result { get; set; }

            public List<SitePage> SimilarResults { get; set; }

            public List<SitePageTfIdf> WordsTfIdf { get; set; } 
        }
    }
}
