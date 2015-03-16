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
        public string Content { get; set; }

        public SearchResultItemViewModel(SitePage enity)
        {
            URL = enity.URL;
            Content = enity.Content;
            Name = String.IsNullOrEmpty(enity.RecipeName) ? enity.URL : enity.RecipeName;
        }
    }
}
