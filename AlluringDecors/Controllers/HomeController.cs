using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [AllowAnonymous]
        public ActionResult Index()
        {
            Dictionary<string, string> dict = Misc.LoadJson(@"~\..\Text_Files\Home.json");
            ViewBag.text = dict;
            return View();
        }
        [HttpPost]
        public void Edit()
        {
            Misc.UpdateJson(@"~\..\Text_Files\Home.json", Request.Form["Article"], Request.Form["Text"]);
        }
    }
}