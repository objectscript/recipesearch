using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using RecipesSearch.BusinessServices.Logging;

namespace RecipesSearch.WebApplication
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            LoggerWrapper.LogInfo("Application start.");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            LoggerWrapper.LogError("Unhandled error.", Server.GetLastError());
        }

        protected void Application_End()
        {
            LoggerWrapper.LogInfo("Application end.");
        }
    }
}
