using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipesSearch.DAL.Cache.Adapters.Base;
using RecipesSearch.Data.Models;
using RecipesSearch.DAL.Cache.Adapters;

namespace ExportTool
{
    class Program
    {
        private const string RecipesExportFileName = "recipes.txt";
        private const string EdgesExportFileName = "edges.txt";

        static void Main(string[] args)
        {
            if(String.Equals(args[0], "Export", StringComparison.InvariantCultureIgnoreCase))
            {
                Cleanup();
                ExportData();
            }
            else
            {
                Console.WriteLine("Unknown command.");
            }
        }

        private static void Cleanup()
        {
            DeleteFileIfExists(RecipesExportFileName);
            DeleteFileIfExists(EdgesExportFileName);
        }

        private static void DeleteFileIfExists(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private static void ExportData()
        {
            ExportRecipesData();
            ExportNearestResultsData();
        }

        private static void ExportRecipesData()
        {
            Console.WriteLine("Start exporting recipes data.");

            using (CacheAdapter adapter = new CacheAdapter())
            {
                List<SitePage> recipes = adapter.GetEntities<SitePage>();

                string fileContent = String.Empty;
                string fileHeader = "Id,SiteID,URL,Keywords,RecipeName,Description,Ingredients,RecipeInstructions,AdditionalData,ImageUrl,Category,Rating,CommentsCount\r\n";

                fileContent += fileHeader;

                foreach (SitePage recipe in recipes)
                {
                    string fileRow = String.Format(
                        "\"{0}\",\"{1}\",\"{2}\",\"{3}\",\"{4}\",\"{5}\",\"{6}\",\"{7}\",\"{8}\",\"{9}\",\"{10}\",\"{11}\"\r\n",
                        StringToCSVCell(recipe.Id.ToString()),
                        StringToCSVCell(recipe.SiteID.ToString()),
                        StringToCSVCell(recipe.URL),
                        StringToCSVCell(recipe.Keywords),
                        StringToCSVCell(recipe.RecipeName),
                        StringToCSVCell(recipe.Description),
                        StringToCSVCell(recipe.Ingredients),
                        StringToCSVCell(recipe.RecipeInstructions),
                        StringToCSVCell(recipe.AdditionalData),
                        StringToCSVCell(recipe.ImageUrl),
                        StringToCSVCell(recipe.Category),
                        StringToCSVCell(recipe.Rating.ToString()),
                        StringToCSVCell(recipe.CommentsCount.ToString()));

                    fileContent += fileRow;
                }

                File.WriteAllText(RecipesExportFileName, fileContent);
            }

            Console.WriteLine("End exporting recipes data.");
        }

        private static void ExportNearestResultsData()
        {
            Console.WriteLine("Start exporting edges data.");

            using (SimilarResultsAdapter adapter = new SimilarResultsAdapter())
            {
                List<NearestResult> edges = adapter.GetNearestResults();

                string fileContent = String.Empty;
                string fileHeader = "RecipeId,SimilarRecipeId,Weight\r\n";

                fileContent += fileHeader;

                foreach (NearestResult edge in edges)
                {
                    string fileRow = String.Format(
                        "\"{0}\",\"{1}\",\"{2}\"\r\n",
                        StringToCSVCell(edge.RecipeId.ToString()),
                        StringToCSVCell(edge.SimilarRecipeId.ToString()),
                        StringToCSVCell(edge.Weight.ToString()));

                    fileContent += fileRow;
                }

                File.WriteAllText(EdgesExportFileName, fileContent);
            }

            Console.WriteLine("End exporting edges data.");
        }

        public static string StringToCSVCell(string str)
        {
            return str.Replace("\n", " ").Replace("\r", " ").Replace("\"", "\\\"");
        }
    }
}
