using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RecipesSearch.BusinessServices.Logging;
using RecipesSearch.WebApplication.ViewModels;

namespace RecipesSearch.WebApplication.Controllers
{
    public class AccountController : Controller
    {
        [HttpGet]
        public ViewResult Login(bool resetPassword = false)
        {
            ViewBag.ResetPassword = resetPassword;
            return View(new LogonViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LogonViewModel model)
        {
            try
            {
                EnsureTestUserExists();
                if (ModelState.IsValid)
                {
                    if (Membership.ValidateUser(model.Login, model.Password))
                    {
                        FormsAuthentication.SetAuthCookie(model.Login, model.RememberMe);
                        return Redirect(model.RedirectUrl);
                    }
                    ViewBag.InvalidAttempt = true;
                    return View("Login", new LogonViewModel());
                }
                return View("Error");
            }
            catch (Exception ex)
            {
                Logger.LogError("Login error.", ex);
                return View("Error");
            }
        }

        //TODO: Remove when/if we get registrastration
        //For now just one admin user hardcoded
        private void EnsureTestUserExists()
        {
            if (!Membership.ValidateUser("admin", "admin"))
            {
                Membership.CreateUser("admin", "admin");
            }
        }

        [Authorize]
        [HttpGet]
        public RedirectToRouteResult LogOff()
        {
            try
            {
                FormsAuthentication.SignOut();
            }
            catch (Exception ex)
            {
                Logger.LogError("Login error.", ex);
            }
            return RedirectToAction("Index", "Home");
        }
	}
}