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
        public IEnumerable<SitePage> Items;

        public int TotalCount { get; set; }

        public int PageNumber { get; set; }
    }
}
