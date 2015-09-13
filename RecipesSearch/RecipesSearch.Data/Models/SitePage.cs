using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Framework;
using RecipesSearch.Data.Models.Base;
using RecipesSearch.Data.Views;

namespace RecipesSearch.Data.Models
{
    [CachePackage(Constants.DefaultCachePackage)]
    public class SitePage : Entity
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

        public string ImageUrl { get; set; }

        public string Category { get; set; }

        public int? Rating { get; set; }

        public int? CommentsCount { get; set; }        

        [NotMapped]
        public int? SimilarRecipeWeight { get; set; }

        [NotMapped]
        public List<SitePage> SimilarResults { get; set; }

        public TfIdfInfo TfIdfInfo { get; set; }
    }
}
