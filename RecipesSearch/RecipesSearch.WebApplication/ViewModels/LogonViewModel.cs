using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipesSearch.WebApplication.ViewModels
{
    public class LogonViewModel
    {
        [Required(ErrorMessage = "Login is required")]
        [Display(Name = "Username")]
        public string Login { get; set; }

        [Display(Name = "Password")]
        [Required(ErrorMessage = "Password is required")]
        [PasswordPropertyText]
        public string Password { get; set; }

        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }

        public string RedirectUrl { get; set; }
    }
}
