using System.ComponentModel.DataAnnotations;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    [CachePackage(Constants.DefaultCachePackage)]
    public class SearchSettings : Entity
    {
        public int ResultsOnPage { get; set; }

        public int SuggestionsCount { get; set; }
    }
}
