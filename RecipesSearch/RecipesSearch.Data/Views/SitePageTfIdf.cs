using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.Data.Views
{
    public class SitePageTfIdf
    {
        public int Id { get; set; }

        public int RecipeId { get; set; }

        public string Word { get; set; }

        public double TFIDF { get; set; }
    }
}
