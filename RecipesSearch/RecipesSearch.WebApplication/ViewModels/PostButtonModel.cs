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
    public class PostButtonModel
    {
        public string Controller { get; set; }
        public string Action { get; set; }
        public string ButtonClass { get; set; }
        public object RouteValues { get; set; }
        public string OnClick { get; set; }
        public string Text { get; set; }
    }
}
