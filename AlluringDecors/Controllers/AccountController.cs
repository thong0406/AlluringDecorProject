using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AlluringDecors.Models;
using System.Web.Security;
using System.Web.Configuration;

namespace AlluringDecors.Controllers
{
    public class AccountController : Controller
    {
        // GET: Login
        public ActionResult Index(string account, string type)
        {
            if (type == "Register")
            {
                return View("Register");
            }
            else
            {
                if (account == null) ViewBag.Account = "";
                else ViewBag.Account = account;
                return View("Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserModel userData, string type)
        {
            using (var db = new AlluringDecorEntities())
            {
                bool userExists = db.users.Any(u => u.Username == userData.Username && u.Password == userData.Password);
                if (userExists)
                {
                    FormsAuthentication.SetAuthCookie(userData.Username, false);
                    TempData["success"] = "Login successful.";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["error"] = "Username or Password was invalid.";
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(UserModel userData)
        {
            using (var db = new AlluringDecorEntities())
            {
                if (ModelState.IsValid)
                {
                    user usr = new user();
                    int? userMaxId = db.users.Max(m => (int?)m.Id);
                    usr.Id = (userMaxId == null ? 0 : (int)userMaxId) + 1;
                    usr.Name = userData.Name;
                    usr.Address = userData.Address;
                    usr.Email = userData.Email;
                    usr.Phonenumber = userData.Phonenumber;
                    usr.Username = userData.Username;
                    usr.Password = userData.Password;
                    db.users.Add(usr);

                    user_role usrrol = new user_role();
                    usrrol.userId = usr.Id;
                    int clientRoleId = db.roles.Where(m => m.name == "Client").FirstOrDefault().id;
                    usrrol.roleId = clientRoleId;
                    db.user_role.Add(usrrol);

                    db.SaveChanges();

                    FormsAuthentication.SetAuthCookie(userData.Username, false);
                    TempData["success"] = "Login successful.";
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["error"] = "Username or Password was invalid.";
            return RedirectToAction("Index");
        }
        public ActionResult Logout(string returnUrl)
        {
            FormsAuthentication.SignOut();
            HttpCookie cookie1 = new HttpCookie(FormsAuthentication.FormsCookieName, "");
            cookie1.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie1);
            SessionStateSection sessionStateSection = (SessionStateSection)WebConfigurationManager.GetSection("system.web/sessionState");
            HttpCookie cookie2 = new HttpCookie(sessionStateSection.CookieName, "");
            cookie2.Expires = DateTime.Now.AddYears(-1);
            Response.Cookies.Add(cookie2);
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }
    }
}