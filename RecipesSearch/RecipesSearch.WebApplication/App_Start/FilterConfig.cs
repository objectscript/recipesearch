using System.Web.Mvc;
using RecipesSearch.WebApplication.Infrastructure;

namespace RecipesSearch.WebApplication
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ExtendedHandleErrorAttribute());
        }
    }
}
