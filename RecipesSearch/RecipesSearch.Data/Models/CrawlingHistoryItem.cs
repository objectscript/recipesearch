using System;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    [CachePackage(Constants.DefaultCachePackage)]
    public class CrawlingHistoryItem : Entity
    {
        public int SiteId { get; set; }

        public DateTime StardDate { get; set; }

        public DateTime EndDate { get; set; }

        public int CrawledPagesCount { get; set; }
        
        public bool IsStopped { get; set; }

        [NotMapped]
        public DateTime LocalStartDate
        {
            get
            {
                return StardDate.Add(CurrentTimeZone.GetUtcOffset(StardDate));
            }
        }

        [NotMapped]
        public DateTime LocalEndDate
        {
            get
            {
                return EndDate.Add(CurrentTimeZone.GetUtcOffset(EndDate));
            }
        }

        [NotMapped]
        public SiteToCrawl SiteToCrawl { get; set; }
    }
}
