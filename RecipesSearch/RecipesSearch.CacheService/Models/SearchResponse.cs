using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;

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
        }
    }
}
