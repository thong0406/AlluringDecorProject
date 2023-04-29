using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class ContactUsController : Controller
    {
        // GET: AboutUs
        [AllowAnonymous]
        public ActionResult Index()
        {
            Dictionary<string, string> dict = Misc.LoadJson(@"~\..\Text_Files\ContactUs.json");
            ViewBag.text = dict;
            return View();
        }
        [HttpPost]
        public void Edit()
        {
            Misc.UpdateJson(@"~\..\Text_Files\ContactUs.json", Request.Form["Article"], Request.Form["Text"]);
        }
    }
}