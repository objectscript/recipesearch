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
    public class SearchViewModel
    {
        public List<SearchResultItemViewModel> ResultItems { get; set; }

        public int TotalCount { get; set; }

        public int ResultsOnPage { get; set; }

        public int CurrentPage { get; set; }

        public string CurrentQuery { get; set; }

        public bool SpellcheckingEnabled { get; set; }

        public string SpellcheckedQuery { get; set; }

        public bool ExactMatch { get; set; }
    }
}
