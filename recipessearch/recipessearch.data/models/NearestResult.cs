namespace RecipesSearch.Data.Models
{
    public class NearestResult
    {
        public int RecipeId { get; set; }

        public int SimilarRecipeId { get; set; }       

        public int Order { get; set; }

        public double Weight { get; set; }
    }
}
