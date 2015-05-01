using System;
using System.Collections;
using System.Collections.Generic;
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

        public string Description { get; set; }

        public string Ingredients { get; set; }

        public string RecipeInstructions { get; set; }

        public string AdditionalData { get; set; }

        [NotMapped]
        public DateTime CreatedDate { get; set; }

        [NotMapped]
        public DateTime ModifiedDate { get; set; }

        [NotMapped]
        public List<SitePage> SimilarResults { get; set; }
    }
}
