using AlluringDecors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class ServicesController : Controller
    {
        // GET: Services
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Details(string service, string domain)
        {
            ServiceModel model = new ServiceModel();
            List<DomainModel> dmns = new List<DomainModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.services.Where(m => m.Name == service).Select(m => new ServiceModel { Id = m.Id, Name = m.Name, Description = m.Description }).FirstOrDefault();
                ViewBag.DomainId = (domain == null) ? -1 : db.domains.Where(m => m.Name == domain).FirstOrDefault().Id;
                dmns = (from dmn in db.domains
                        join srvdmn in db.service_domain on dmn.Id equals srvdmn.DomainId
                        where srvdmn.ServiceId == model.Id
                        group dmn by dmn.Id into _dmn
                        select new DomainModel { Id = _dmn.FirstOrDefault().Id, Name = _dmn.FirstOrDefault().Name }).ToList();
            }
            ViewBag.Domains = dmns;
            return View(model);
        }
        [HttpPost]
        public void UpdateExample(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                service_example srvex = db.service_example.Find(id);
                srvex.Name = Request.Form["Description"];
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void DeleteExample(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                service_example srvex = db.service_example.Find(id);
                db.service_example.Remove(srvex);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void UpdateService(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                service srv = db.services.Find(id);
                srv.Description = Request.Form["Description"];
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void CreateExample(int serviceId)
        {
            using (var db = new AlluringDecorEntities())
            {
                service_example srvex = new service_example();
                int? maxId = db.service_example.Max(m => (int?)m.Id);
                srvex.Id = (maxId == null ? 0 : (int)maxId) + 1;
                srvex.ServiceId = serviceId;
                HttpPostedFileBase image = Request.Files[0];
                string testfiles = image.FileName.Split(new char[] { '\\' }).Last();
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string fname = Misc.RandomString(8) + testfiles.Substring(testfiles.LastIndexOf("."));
                srvex.ImagePath = fname;
                fname = System.IO.Path.Combine(Server.MapPath("~/Images/"), fname);
                image.SaveAs(fname);
                srvex.Name = Request.Form["Description"];
                db.service_example.Add(srvex);
                db.SaveChanges();
            }
        }
        [HttpGet]
        public ActionResult LoadExamples(int id)
        {
            List<ServiceExampleModel> model = new List<ServiceExampleModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.service_example.Where(m => m.ServiceId == id).Select(m => new ServiceExampleModel { Id = m.Id, Name = m.Name, ImagePath = m.ImagePath }).ToList();
            }
            return PartialView("_ServiceExampleCarousel", model);
        }
        [HttpGet]
        public ActionResult LoadServiceDomainTable()
        {
            List<DomainModel> model = new List<DomainModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.domains.Select(m => new DomainModel
                {
                    Id = m.Id,
                    Name = m.Name
                }).ToList();
                foreach (DomainModel dmn in model)
                {
                    List<ServiceDomainModel> srvdmns = db.service_domain.Where(m => m.DomainId == dmn.Id).Select(m => new ServiceDomainModel
                    {
                        Id = m.Id,
                        Service = new ServiceModel() { Id = m.service.Id, Name = m.service.Name }
                    }).ToList();
                    dmn.ServiceDomains = srvdmns;
                }
            }
            return PartialView("_ClientServiceDomainTable", model);
        }
        [HttpGet]
        public ActionResult LoadServiceForm()
        {
            UserModel model = new UserModel();
            using (var db = new AlluringDecorEntities())
            {
                model = db.users.Where(m => m.Username == User.Identity.Name).Select(m => new UserModel
                {
                    Name = m.Name,
                    Address = m.Address,
                    Email = m.Email,
                    Phonenumber = m.Phonenumber
                }).FirstOrDefault() ?? new UserModel();
            }
            return PartialView("_ClientServiceModal", model);
        }
        public ActionResult OrderService(UserModel model)
        {
            bool isValid =
                !String.IsNullOrWhiteSpace(Request.Form["Name"]) &&
                !String.IsNullOrWhiteSpace(Request.Form["Address"]) &&
                !String.IsNullOrWhiteSpace(Request.Form["Phonenumber"]) &&
                !String.IsNullOrWhiteSpace(Request.Form["Email"]);
            if (isValid)
            {
                using (var db = new AlluringDecorEntities())
                {
                    int id = (db.service_request.Max(m => (int?)m.Id) ?? 0) + 1;
                    int userId = db.users.Where(m => m.Username == User.Identity.Name).FirstOrDefault().Id;
                    int serviceDomainId = -1;
                    if (Request.Form["serviceDomainId"] == "-1")
                    {
                        int serviceId = Int32.Parse(Request.Form["serviceId"]);
                        int domainId = Int32.Parse(Request.Form["domainId"]);
                        serviceDomainId = db.service_domain.Where(m =>
                             m.ServiceId == serviceId &&
                             m.DomainId == domainId
                        ).FirstOrDefault().Id;
                    }
                    else
                    {
                        serviceDomainId = Int32.Parse(Request.Form["serviceDomainId"]);
                    }
                    string receiver = model.Name;
                    string address = model.Address;
                    string contact = model.Phonenumber;
                    string email = model.Email;
                    db.service_request.Add(new service_request() {
                        Id = id,
                        UserId = userId,
                        ServiceDomainId = serviceDomainId,
                        Receiver = receiver,
                        Address = address,
                        Contact = contact,
                        Email = email,
                        Status = "Request Received",
                        dateSent = DateTime.Now
                    });
                    db.SaveChanges();
                    TempData["success"] = "Ordered successfully, do you wish to view your orders? <a class='text-white' href='Client/Requests'>Click here.</a>";
                    return RedirectToAction("Index");
                }
            }
            TempData["error"] = "Something went wrong, please try again.";
            return RedirectToAction("Index");
        }
    }
}