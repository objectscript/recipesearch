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
    public class SearchSettingsViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of search results on page")]
        public int ResultsOnPage { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of suggestions")]
        public int SuggestionsCount { get; set; }

        [Display(Name = "Spellcheck search query")]
        public bool EnableSpellchecking { get; set; }

        [Display(Name = "Spellcheck autosuggest query")]
        public bool EnableSpellcheckingForSuggest { get; set; }

        public static SearchSettings GetEntity(SearchSettingsViewModel viewModel)
        {
            return new SearchSettings
            {
                Id = viewModel.Id,
                ResultsOnPage = viewModel.ResultsOnPage,
                SuggestionsCount = viewModel.SuggestionsCount,
                EnableSpellchecking = viewModel.EnableSpellchecking,
                EnableSpellcheckingForSuggest = viewModel.EnableSpellcheckingForSuggest
            };
        }

        public static SearchSettingsViewModel GetViewModel(SearchSettings entity)
        {
            return new SearchSettingsViewModel
            {
                Id = entity.Id,
                ResultsOnPage = entity.ResultsOnPage,
                SuggestionsCount = entity.SuggestionsCount,
                EnableSpellchecking = entity.EnableSpellchecking,
                EnableSpellcheckingForSuggest = entity.EnableSpellcheckingForSuggest
            };
        }
    }
}
