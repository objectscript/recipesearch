using System;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using InterSystems.AspNet.Identity.Cache;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.WebApplication.ViewModels;
using Microsoft.AspNet.Identity.Owin;

namespace RecipesSearch.WebApplication.Controllers
{
    public class AccountController : Controller
    {
        public ApplicationUserManager UserManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return HttpContext.GetOwinContext().GetUserManager<ApplicationSignInManager>();
            }
        }

        [HttpGet]
        public ViewResult Login(bool resetPassword = false)
        {
            ViewBag.ResetPassword = resetPassword;
            return View(new LogonViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LogonViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("Error");
                }

                await EnsureTestUserExists();

                var result = await SignInManager.PasswordSignInAsync(model.Login, model.Password, model.RememberMe, shouldLockout: false);
                switch (result)
                {
                    case SignInStatus.Success:
                        return String.IsNullOrEmpty(model.RedirectUrl) ? 
                            (ActionResult) RedirectToAction("Index", "Home") :
                            Redirect(model.RedirectUrl);

                    case SignInStatus.Failure:
                    default:
                        ViewBag.InvalidAttempt = true;
                        return View("Login", new LogonViewModel());
                }             
            }
            catch (Exception ex)
            {
                Logger.LogError("Login error.", ex);
                return View("Error");
            }
        }

        //TODO: Remove when/if we get registratration
        //For now just one admin user hardcoded
        private async Task EnsureTestUserExists()
        {
            var user = await UserManager.FindByNameAsync("admin");
            if (user == null)
            {
                await UserManager.CreateAsync(new IdentityUser { UserName = "admin" }, "admin");
            }
        }

        [Authorize]
        [HttpGet]
        public RedirectToRouteResult LogOff()
        {
            try
            {
                HttpContext.GetOwinContext().Authentication.SignOut();
            }
            catch (Exception ex)
            {
                Logger.LogError("Login error.", ex);
            }
            return RedirectToAction("Index", "Home");
        }
	}
}