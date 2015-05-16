using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    [CachePackage(Constants.DefaultCachePackage)]
    public class TfIdfConfig : Entity
    {
        public string TfBuilderName { get; set; }

        public string IdfBuilderName { get; set; }

        public string LastUsedTfBuilder { get; set; }

        public string LastUsedIdfBuilder { get; set; }

        public int SimilarResultsCount { get; set; }
    }
}
