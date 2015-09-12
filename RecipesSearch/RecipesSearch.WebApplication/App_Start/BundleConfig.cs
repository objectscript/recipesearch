using System.Web;
using System.Web.Optimization;

namespace RecipesSearch.WebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/libs")
                .IncludeDirectory("~/Scripts/libs", "*.js"));

            bundles.Add(new ScriptBundle("~/bundles/site")
                .IncludeDirectory("~/Scripts/views", "*.js")
                .Include("~/Scripts/site.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/simplePagination.css",
                      "~/Content/vis.css",
                      "~/Content/site.css"));

            BundleTable.EnableOptimizations = false;
        }
    }
}
