using AlluringDecors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class FeedbackController : Controller
    {
        // GET: Feedback
        [AllowAnonymous]
        public ActionResult Index()
        {
            using (var db = new AlluringDecorEntities())
            {
                if (User.Identity.IsAuthenticated) ViewBag.UserId = db.users.Where(m => m.Username == User.Identity.Name).FirstOrDefault().Id;
                else ViewBag.UserId = -1;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Create()
        {
            using (var db = new AlluringDecorEntities())
            {
                feedback fdb = new feedback();
                int? maxId = db.feedbacks.Max(m => (int?)m.Id);
                fdb.Id = (maxId == null ? 0 : (int)maxId) + 1;
                fdb.Comment = Request.Form["Comment"];
                fdb.DateSent = DateTime.Now;
                if (Request.Form["UserId"] != "-1") fdb.UserId = Int32.Parse(Request.Form["UserId"]);
                db.feedbacks.Add(fdb);
                db.SaveChanges();
                TempData["success"] = "Feedback Sent!";
                return RedirectToAction("Index");
            }
            TempData["error"] = "Something went wrong!";
            return RedirectToAction("Index");
        }
    }
}