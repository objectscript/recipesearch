using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RecipesSearch.Data.Models
{
    public class SitePage
    {
        [Key]
        public int SiteID { get; set; }

        [Key]
        public string URL { get; set; }

        public string Keywords { get; set; }

        public string RecipeName { get; set; }

        public string Content { get; set; }

        [NotMapped]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public DateTime ModifiedDate { get; set; }
    }
}
