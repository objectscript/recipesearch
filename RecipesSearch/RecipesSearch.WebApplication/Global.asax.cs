using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

            Logger.LogInfo("Application start.");
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Logger.LogError("Unhandler error.", Server.GetLastError());
        }

        protected void Application_End()
        {
            Logger.LogInfo("Application end.");
        }
    }
}
