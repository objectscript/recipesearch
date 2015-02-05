using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    public class SiteToCrawl : Entity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string URL { get; set; }

        [Required]
        public int ConfigId { get; set; }

        [ForeignKey("ConfigId")]
        public Config ContentGroup { get; set; }

        public virtual List<CrawlingHistoryItem> CrawlingHistory { get; set; }
    }
}
