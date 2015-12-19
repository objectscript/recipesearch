using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;
using RecipesSearch.WebApplication.Enums;

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
        
        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of results for graph view")]
        public int ResultsForGraphView { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Default results view")]
        public string DefaultResultsView { get; set; }

        [Display(Name = "Spellcheck search query")]
        public bool EnableSpellchecking { get; set; }

        [Display(Name = "Spellcheck autosuggest query")]
        public bool EnableSpellcheckingForSuggest { get; set; }

        [Display(Name = "Online TF/IDF calculation is enabled")]
        public bool OnlineTfIdfEnabled { get; set; }

        [Display(Name = "Show clusters instead of similar results")]
        public bool UseClusters { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of recipes to compute TF/IDF")]
        public int MaxOnlineIdfRecipesCount { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of similar results to find during online TF/IDF")]
        public int OnlineTfIdfSimilarResultsCount { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Online TF/IDF builder to use")]
        public string OnlineTfIdfBuilderName { get; set; }

        public List<string> AvailableOnlineTfIdfBuilders { get; set; }

        public static SearchSettings GetEntity(SearchSettingsViewModel viewModel)
        {
            return new SearchSettings
            {
                Id = viewModel.Id,
                ResultsOnPage = viewModel.ResultsOnPage,
                SuggestionsCount = viewModel.SuggestionsCount,
                EnableSpellchecking = viewModel.EnableSpellchecking,
                EnableSpellcheckingForSuggest = viewModel.EnableSpellcheckingForSuggest,
                ResultsForGraphView = viewModel.ResultsForGraphView,
                DefaultResultsView = (int)Enum.Parse(typeof(ResultsViews), viewModel.DefaultResultsView),
                OnlineTfIdfEnabled = viewModel.OnlineTfIdfEnabled,
                MaxOnlineIdfRecipesCount = viewModel.MaxOnlineIdfRecipesCount,
                OnlineTfIdfBuilderName = viewModel.OnlineTfIdfBuilderName,
                OnlineTfIdfSimilarResultsCount = viewModel.OnlineTfIdfSimilarResultsCount,
                UseClusters = viewModel.UseClusters
            };
        }

        public static SearchSettingsViewModel GetViewModel(SearchSettings entity, List<string> availableTfIdfBuilders)
        {
            return new SearchSettingsViewModel
            {
                Id = entity.Id,
                ResultsOnPage = entity.ResultsOnPage,
                SuggestionsCount = entity.SuggestionsCount,
                EnableSpellchecking = entity.EnableSpellchecking,
                EnableSpellcheckingForSuggest = entity.EnableSpellcheckingForSuggest,
                ResultsForGraphView = entity.ResultsForGraphView,
                DefaultResultsView = ((ResultsViews)entity.DefaultResultsView).ToString(),
                OnlineTfIdfEnabled = entity.OnlineTfIdfEnabled,
                AvailableOnlineTfIdfBuilders = availableTfIdfBuilders,
                MaxOnlineIdfRecipesCount = entity.MaxOnlineIdfRecipesCount,
                OnlineTfIdfBuilderName = entity.OnlineTfIdfBuilderName,
                OnlineTfIdfSimilarResultsCount = entity.OnlineTfIdfSimilarResultsCount,
                UseClusters = entity.UseClusters
            };
        }
    }
}
