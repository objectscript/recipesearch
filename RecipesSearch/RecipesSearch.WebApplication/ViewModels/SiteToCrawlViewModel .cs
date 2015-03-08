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
    public class SiteToCrawlViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Field is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Field is required")]
        public string URL { get; set; }

        [Required]
        public int ConfigId { get; set; }

        [Display(Name = "Parser")]
        public string ParserId { get; set; }

        public static SiteToCrawl GetEntity(SiteToCrawlViewModel toCrawlViewModel)
        {
            return new SiteToCrawl
            {
                Id = toCrawlViewModel.Id,
                Name = toCrawlViewModel.Name,
                URL = toCrawlViewModel.URL,
                ConfigId = toCrawlViewModel.ConfigId,
                ParserId = toCrawlViewModel.ParserId
            };
        }

        public static SiteToCrawlViewModel GetViewModel(SiteToCrawl enity)
        {
            return new SiteToCrawlViewModel
            {
                Id = enity.Id,
                Name = enity.Name,
                URL = enity.URL,
                ConfigId = enity.ConfigId,
                ParserId = enity.ParserId
            };
        }
    }
}
