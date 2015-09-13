using System.Web.Mvc;
using RecipesSearch.BusinessServices.Logging;

namespace RecipesSearch.WebApplication.Infrastructure
{
    class ExtendedHandleErrorAttribute : HandleErrorAttribute
    {
        public override void OnException(ExceptionContext filterContext)
        {
            LoggerWrapper.LogError("Unhandled MVC exception", filterContext.Exception);
            base.OnException(filterContext);
        }
    }
}
