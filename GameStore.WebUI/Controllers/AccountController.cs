using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GameStore.WebUI.Models;
using GameStore.WebUI.Infrastructure.Abstract;

namespace GameStore.WebUI.Controllers
{
    public class AccountController : Controller
    {
        IAuthProvider authProvider = null;

        public AccountController(IAuthProvider authProvider) {
            this.authProvider = authProvider;
        }
        // GET: Account
        public ViewResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel loginViewModel, string returnUrl) {
            if (ModelState.IsValid) {
                bool access = authProvider.Authenticate(loginViewModel.UserName, loginViewModel.Password);
                if (access) return Redirect(returnUrl ?? Url.Action("Index", "Admin"));
                else {
                    ModelState.AddModelError("", "Неверный логин или пароль");
                    return View();
                }
            } else return View();
        }
    }
}