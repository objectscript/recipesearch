using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;

namespace RecipesSearch.WebApplication.ViewModels
{
    public class SearchResultItemViewModel
    {
        public string Name { get; set; }

        public int SiteId { get; set; }

        public string URL { get; set; }

        public string Description { get; set; }

        public string Ingredients { get; set; }

        public string RecipeInstructions { get; set; }

        public string AdditionalData { get; set; }

        public string ImageUrl { get; set; }

        public int? SimilarRecipeWeight { get; set; }

        public int Id { get; set; }

        public string Category { get; set; }

        public int? Rating { get; set; }

        public int? CommentsCount { get; set; } 

        public List<SearchResultItemViewModel> SimilarResults { get; set; } 

        public List<int> ClusterIds { get; set; }

        public SearchResultItemViewModel(SitePage entity)
        {
            Id = entity.Id;
            URL = entity.URL;
            Description = entity.Description;
            Ingredients = entity.Ingredients;
            RecipeInstructions = entity.RecipeInstructions;
            AdditionalData = entity.AdditionalData;
            Name = String.IsNullOrEmpty(entity.RecipeName) ? entity.URL : entity.RecipeName;
            ImageUrl = entity.ImageUrl;
            SiteId = entity.SiteID;
            SimilarRecipeWeight = entity.SimilarRecipeWeight;
            Category = entity.Category;
            Rating = entity.Rating;
            CommentsCount = entity.CommentsCount;

            if (entity.SimilarResults != null)
            {
                SimilarResults = entity.SimilarResults.Select(sitePage => new SearchResultItemViewModel(sitePage)).ToList();
            }

            ClusterIds = entity.ClusterIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(clusterId => Int32.Parse(clusterId)).ToList();
        }
    }
}
