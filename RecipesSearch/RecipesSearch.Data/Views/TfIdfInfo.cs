using System.Collections.Generic;

namespace RecipesSearch.Data.Views
{
    public class TfIdfInfo
    {
        public int Id { get; set; }

        public Dictionary<string, double> WordsTfIdf { get; set; }
    }
}
