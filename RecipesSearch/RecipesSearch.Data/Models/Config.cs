using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    [CachePackage(Constants.DefaultCachePackage)]
    public class Config : Entity
    {
        public bool LoggingEnabled { get; set; }

        public bool EnhancedKeywordProcessing { get; set; }

        public bool EnableSpellcheckDictionaryUpdate { get; set; }

        public int MaxPagesToCrawl { get; set; }

        public int MaxCrawlDepth { get; set; }

        public int CrawlTimeoutSeconds { get; set; }

        [NotMapped]
        public List<SiteToCrawl> SitesToCrawl { get; set; }
    }
}
