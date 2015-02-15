using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    public class Config : Entity
    {
        public bool LoggingEnabled { get; set; }

        public bool EnhancedKeywordProcessing { get; set; }

        public int MaxPagesToCrawl { get; set; }

        public int MaxCrawlDepth { get; set; }

        public int CrawlTimeoutSeconds { get; set; }

        public virtual List<SiteToCrawl> SitesToCrawl { get; set; }
    }
}
