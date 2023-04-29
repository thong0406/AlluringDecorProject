using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class FAQController : Controller
    {
        // GET: FAQ
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public void Edit()
        {
            Misc.UpdateJson(@"~\..\Text_Files\FAQ.json", Request.Form["Article"], Request.Form["Text"]);
        }
        [HttpPost]
        public void Delete()
        {
            Misc.DeleteFromJson(@"~\..\Text_Files\FAQ.json", Request.Form["Article"]);
        }
        [HttpPost]
        public void Add()
        {
            Misc.AddToJson(@"~\..\Text_Files\FAQ.json", Request.Form["Article"], Request.Form["Text"]);
        }
        [HttpGet]
        public ActionResult LoadPartial()
        {
            Dictionary<string, string> dict = Misc.LoadJson(@"~\..\Text_Files\FAQ.json");
            ViewBag.Questions = dict;
            return PartialView("_FAQPartial");
        }
    }
}