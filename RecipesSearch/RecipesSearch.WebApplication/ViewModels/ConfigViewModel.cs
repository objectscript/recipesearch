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
    public class ConfigViewModel
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Logging Enabled")]
        public bool LoggingEnabled { get; set; }

        [Display(Name = "Stem keywords")]
        public bool EnhancedKeywordProcessing { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Maximum count of pages to crawl per site")]
        public int MaxPagesToCrawl { get; set; }


        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([1-9][0-9]*)", ErrorMessage = "Must be a natural number")]
        [Display(Name = "Maximum crawling depth")]
        public int MaxCrawlDepth { get; set; }

        [Required(ErrorMessage = "Field is required")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Must be a non-negative number")]
        [Display(Name = "Max crawling time (0 to disable)")]
        public int CrawlTimeoutSeconds { get; set; }

        public List<SiteToCrawlViewModel> SitesToCrawl { get; set; }

        public static Config GetEntity(ConfigViewModel viewModel)
        {
            return new Config
            {
                Id = viewModel.Id,
                EnhancedKeywordProcessing = viewModel.EnhancedKeywordProcessing,
                LoggingEnabled = viewModel.LoggingEnabled,
                MaxCrawlDepth = viewModel.MaxCrawlDepth,
                MaxPagesToCrawl = viewModel.MaxPagesToCrawl,
                CrawlTimeoutSeconds = viewModel.CrawlTimeoutSeconds,
                SitesToCrawl = new List<SiteToCrawl>()
            };
        }

        public static ConfigViewModel GetViewModel(Config enity)
        {
            return new ConfigViewModel
            {
                Id = enity.Id,
                EnhancedKeywordProcessing = enity.EnhancedKeywordProcessing,
                LoggingEnabled = enity.LoggingEnabled,
                MaxCrawlDepth = enity.MaxCrawlDepth,
                MaxPagesToCrawl = enity.MaxPagesToCrawl,
                CrawlTimeoutSeconds = enity.CrawlTimeoutSeconds,
                SitesToCrawl = enity.SitesToCrawl.Select(SiteToCrawlViewModel.GetViewModel).ToList(),
            };
        }
    }
}
