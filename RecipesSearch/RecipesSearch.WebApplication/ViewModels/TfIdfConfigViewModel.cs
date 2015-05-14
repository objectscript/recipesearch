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

        public List<string> AvailableTfBuilders { get; set; }

        public List<string> AvailableIdfBuilders { get; set; } 

        public static TfIdfConfig GetEntity(TfIdfConfigViewModel viewModel)
        {
            return new TfIdfConfig
            {
                TfBuilderName = viewModel.TfBuilderName,
                IdfBuilderName = viewModel.IdfBuilderName,
                LastUsedIdfBuilder = viewModel.LastUsedIdfBuilder,
                LastUsedTfBuilder = viewModel.LastUsedIdfBuilder
            };
        }

        public static TfIdfConfigViewModel GetViewModel(TfIdfConfig entity, List<string> availableTfBuilders, List<string> availableIdfBuilders)
        {
            return new TfIdfConfigViewModel
            {
                TfBuilderName = entity.TfBuilderName,
                IdfBuilderName = entity.IdfBuilderName,
                AvailableTfBuilders = availableTfBuilders,
                AvailableIdfBuilders = availableIdfBuilders
            };
        }
    }
}
