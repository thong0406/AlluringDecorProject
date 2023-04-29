using AlluringDecors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class ProjectsController : Controller
    {
        // GET: Projects
        [AllowAnonymous]
        public ActionResult Index()
        {
            List<ProjectModel> onGoing = new List<ProjectModel>();
            List<ProjectModel> upComing = new List<ProjectModel>();
            List<ProjectModel> acomplished = new List<ProjectModel>();
            using (var db = new AlluringDecorEntities())
            {
                onGoing = db.projects.Where(m => m.Status == "On-Going").Select(m => new ProjectModel { Id = m.Id, Name = m.Name, Description = m.Description, ImagePath = m.ImagePath, Status = m.Status }).ToList();
                upComing = db.projects.Where(m => m.Status == "Up-Coming").Select(m => new ProjectModel { Id = m.Id, Name = m.Name, Description = m.Description, ImagePath = m.ImagePath, Status = m.Status }).ToList();
                acomplished = db.projects.Where(m => m.Status == "Accomplished").Select(m => new ProjectModel { Id = m.Id, Name = m.Name, Description = m.Description, ImagePath = m.ImagePath, Status = m.Status }).ToList();
            }
            ViewBag.OnGoing = onGoing;
            ViewBag.UpComing = upComing;
            ViewBag.Acomplished = acomplished;
            return View();
        }
    }
}