using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.Data.Models;
using RecipesSearch.SitePagesImporter.Pipeline.Base;

namespace RecipesSearch.WebApplication.ViewModels
{
    public class TfIdfConfigViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Tf builder to use")]
        public string TfBuilderName { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [Display(Name = "Idf builder to use")]
        public string IdfBuilderName { get; set; }

        [Display(Name = "Last used Tf builder")]
        public string LastUsedTfBuilder { get; set; }

        [Display(Name = "Last used Idf builder")]
        public string LastUsedIdfBuilder { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Count of nearest results to find")]
        public int SimilarResultsCount { get; set; }

        public List<string> AvailableTfBuilders { get; set; }

        public List<string> AvailableIdfBuilders { get; set; } 

        public static TfIdfConfig GetEntity(TfIdfConfigViewModel viewModel)
        {
            return new TfIdfConfig
            {
                Id = viewModel.Id,
                TfBuilderName = viewModel.TfBuilderName,
                IdfBuilderName = viewModel.IdfBuilderName,
                LastUsedIdfBuilder = viewModel.LastUsedIdfBuilder,
                LastUsedTfBuilder = viewModel.LastUsedTfBuilder,
                SimilarResultsCount = viewModel.SimilarResultsCount
            };
        }

        public static TfIdfConfigViewModel GetViewModel(TfIdfConfig entity, List<string> availableTfBuilders, List<string> availableIdfBuilders)
        {
            return new TfIdfConfigViewModel
            {
                Id = entity.Id,
                TfBuilderName = entity.TfBuilderName,
                IdfBuilderName = entity.IdfBuilderName,
                LastUsedIdfBuilder = entity.LastUsedIdfBuilder,
                LastUsedTfBuilder = entity.LastUsedTfBuilder,
                AvailableTfBuilders = availableTfBuilders,
                AvailableIdfBuilders = availableIdfBuilders,
                SimilarResultsCount = entity.SimilarResultsCount
            };
        }
    }
}
