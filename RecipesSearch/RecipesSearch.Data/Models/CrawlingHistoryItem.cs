using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    public class CrawlingHistoryItem : Entity
    {
        [Required]
        public int SiteId { get; set; }

        [Required]
        public DateTime StardDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int CrawledPagesCount { get; set; }
        
        [Required]
        public bool IsStopped { get; set; }

        public DateTime LocalStartDate
        {
            get
            {
                return StardDate.Add(CurrentTimeZone.GetUtcOffset(StardDate));
            }
        }

        public DateTime LocalEndDate
        {
            get
            {
                return EndDate.Add(CurrentTimeZone.GetUtcOffset(EndDate));
            }
        }

        [ForeignKey("SiteId")]
        public virtual SiteToCrawl SiteToCrawl { get; set; }
    }
}
