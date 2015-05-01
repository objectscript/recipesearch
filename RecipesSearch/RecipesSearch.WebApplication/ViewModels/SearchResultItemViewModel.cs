using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;

namespace RecipesSearch.WebApplication.ViewModels
{
    public class SearchResultItemViewModel
    {
        public string Name { get; set; }

        public string URL { get; set; }

        public string Description { get; set; }

        public string Ingredients { get; set; }

        public string RecipeInstructions { get; set; }

        public string AdditionalData { get; set; }

        public List<SearchResultItemViewModel> SimilarResults { get; set; } 

        public SearchResultItemViewModel(SitePage enity)
        {
            URL = enity.URL;
            Description = enity.Description;
            Ingredients = enity.Ingredients;
            RecipeInstructions = enity.RecipeInstructions;
            AdditionalData = enity.AdditionalData;
            Name = String.IsNullOrEmpty(enity.RecipeName) ? enity.URL : enity.RecipeName;

            if (enity.SimilarResults != null)
            {
                SimilarResults = enity.SimilarResults.Select(sitePage => new SearchResultItemViewModel(sitePage)).ToList();
            }           
        }
    }
}
