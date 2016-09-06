using System.Collections.Generic;
using RecipesSearch.WebApplication.Enums;

namespace RecipesSearch.WebApplication.ViewModels
{
    public class SearchViewModel
    {
        public List<SearchResultItemViewModel> ResultItems { get; set; }

        public int TotalCount { get; set; }

        public int ResultsOnPage { get; set; }

        public int CurrentPage { get; set; }

        public int ResultsOnGraphView { get; set; }

        public string CurrentQuery { get; set; }

        public bool SpellcheckingEnabled { get; set; }

        public string SpellcheckedQuery { get; set; }

        public bool ExactMatch { get; set; }

        public bool UseClustering { get; set; }

        public ResultsViews DefaultResultView { get; set; }
    }
}
