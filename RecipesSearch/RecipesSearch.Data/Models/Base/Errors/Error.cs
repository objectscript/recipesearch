using System;

namespace RecipesSearch.Data.Models.Base.Errors
{
    public class Error
    {
        public int ErrorCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
