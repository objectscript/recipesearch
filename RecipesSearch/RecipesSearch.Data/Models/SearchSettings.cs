using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RecipesSearch.Data.Models.Base;

namespace RecipesSearch.Data.Models
{
    public class SearchSettings : Entity
    {
        [Required]
        public int ResultsOnPage { get; set; }
    }
}
