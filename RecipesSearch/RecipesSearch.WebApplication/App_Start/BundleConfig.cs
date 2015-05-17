using System.Web;
using System.Web.Optimization;

namespace RecipesSearch.WebApplication
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/libs").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/typeahead.bundle.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/jquery.simplePagination.js",
                        "~/Scripts/vis.js"));

            bundles.Add(new ScriptBundle("~/bundles/site").Include(
                      "~/Scripts/site.js",
                      "~/Scripts/graphView.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/simplePagination.css",
                      "~/Content/vis.css",
                      "~/Content/site.css"));
        }
    }
}
