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
    public class SearchSettingsViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of search results on page")]
        public int ResultsOnPage { get; set; }

        public static SearchSettings GetEntity(SearchSettingsViewModel viewModel)
        {
            return new SearchSettings
            {
                Id = viewModel.Id,
                ResultsOnPage = viewModel.ResultsOnPage
            };
        }

        public static SearchSettingsViewModel GetViewModel(SearchSettings enity)
        {
            return new SearchSettingsViewModel
            {
                Id = enity.Id,
                ResultsOnPage = enity.ResultsOnPage
            };
        }
    }
}
