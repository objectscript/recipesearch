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
    public class SearchResultViewModel
    {
        public string URL { get; set; }
        public string Content { get; set; }

        public SearchResultViewModel(SitePage enity)
        {
            URL = enity.URL;
            Content = enity.Content;
        }
    }
}
