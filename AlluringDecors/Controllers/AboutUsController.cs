using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace AlluringDecors.Controllers
{
    public class AboutUsController : Controller
    {
        // GET: AboutUs
        [AllowAnonymous]
        public ActionResult Index()
        {
            Dictionary<string, string> dict = Misc.LoadJson(@"~\..\Text_Files\AboutUs.json");
            ViewBag.text = dict;
            return View();
        }
        [HttpPost]
        public void Edit()
        {
            Misc.UpdateJson(@"~\..\Text_Files\AboutUs.json", Request.Form["Article"], Request.Form["Text"]);
        }
    }
}