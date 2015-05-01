using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.SearchEngine.SimilarResults
{
    internal class TfIdfInfo
    {
        public int Id { get; set; }

        public Dictionary<string, double> WordsTfIdf { get; set; }
    }
}
