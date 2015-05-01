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
    public class TasksViewModel
    {
        public bool NearestsResultsUpdatingInProgress { get; set; }

        public int NearestsResultsUpdatedCount { get; set; }
    }
}
