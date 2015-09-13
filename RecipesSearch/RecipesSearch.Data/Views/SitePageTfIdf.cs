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
