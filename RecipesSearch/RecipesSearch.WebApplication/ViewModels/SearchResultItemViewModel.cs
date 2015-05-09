﻿using System;
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

        public string ImageUrl { get; set; }

        public List<SearchResultItemViewModel> SimilarResults { get; set; } 

        public SearchResultItemViewModel(SitePage entity)
        {
            URL = entity.URL;
            Description = entity.Description;
            Ingredients = entity.Ingredients;
            RecipeInstructions = entity.RecipeInstructions;
            AdditionalData = entity.AdditionalData;
            Name = String.IsNullOrEmpty(entity.RecipeName) ? entity.URL : entity.RecipeName;
            ImageUrl = entity.ImageUrl;

            if (entity.SimilarResults != null)
            {
                SimilarResults = entity.SimilarResults.Select(sitePage => new SearchResultItemViewModel(sitePage)).ToList();
            }           
        }
    }
}
