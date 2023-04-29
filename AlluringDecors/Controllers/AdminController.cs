using AlluringDecors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;

namespace AlluringDecors.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        #region Feedback
        [Authorize]
        public ActionResult Feedback()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            List<FeedbackModel> model = new List<FeedbackModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.feedbacks.Select(m => new FeedbackModel {
                    Id = m.Id,
                    Comment = m.Comment,
                    DateSent = m.DateSent,
                    User = new UserModel() { Id = (m.user != null ? m.user.Id : -1), Username = (m.user != null ? m.user.Username : "") }
                }).ToList();
            }
            return View(model);
        }
        #endregion

        #region Users
        [Authorize]
        public ActionResult Users()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            List<RoleModel> model = new List<RoleModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.roles.Select(m => new RoleModel { Id = m.id, Name = m.name }).ToList();
            }
            return View(model);
        }
        [HttpGet]
        public ActionResult LoadUsersTable()
        {
            List<UserModel> model = new List<UserModel>();
            List<RoleModel> roles = new List<RoleModel>();
            using (var db = new AlluringDecorEntities())
            {
                roles = db.roles.Select(m => new RoleModel { Id = m.id, Name = m.name }).ToList();
                model = (from usr in db.users
                         join usrrol in db.user_role on usr.Id equals usrrol.userId
                         join rol in db.roles on usrrol.roleId equals rol.id
                         orderby usr.Id
                         select new UserModel
                         {
                             Id = usr.Id,
                             Username = usr.Username,
                             UserRole = new UserRoleModel()
                             {
                                 Role = new RoleModel()
                                 {
                                     Name = rol.name
                                 }
                             }
                         }).ToList();
            }
            ViewBag.Roles = roles;
            return PartialView("_AdminUserTable", model);
        }
        [HttpPost]
        public void DeleteUser(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                user usr = db.users.Find(id);
                db.users.Remove(usr);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void UpdateUserRole(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                user_role usrrol = db.user_role.Where(m => m.userId == id).FirstOrDefault();
                usrrol.roleId = Int32.Parse(Request.Form["RoleId"]);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void AddUser()
        {
            using (var db = new AlluringDecorEntities())
            {
                user usr = new user();
                int? userMaxId = db.users.Max(m => (int?)m.Id);
                usr.Id = (userMaxId == null ? 0 : (int)userMaxId) + 1;
                usr.Name = Request.Form["Name"];
                usr.Address = Request.Form["Address"];
                usr.Email = Request.Form["Email"];
                usr.Phonenumber = Request.Form["Phonenumber"];
                usr.Username = Request.Form["Username"];
                usr.Password = Request.Form["Password"];
                db.users.Add(usr);

                user_role usrrol = new user_role();
                usrrol.userId = usr.Id;
                usrrol.roleId = Int32.Parse(Request.Form["RoleId"]);
                db.user_role.Add(usrrol);

                db.SaveChanges();
            }
        }
        #endregion

        #region Projects
        [Authorize]
        public ActionResult Projects()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            return View();
        }
        [HttpGet]
        public ActionResult LoadProjectsTable()
        {
            List<ProjectModel> model = new List<ProjectModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.projects.Select(m => new ProjectModel { Id = m.Id, Name = m.Name, Description = m.Description, ImagePath = m.ImagePath, Status = m.Status }).ToList();
            }
            return PartialView("_AdminProjectTable", model);
        }
        [HttpPost]
        public void DeleteProject(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                project prj = db.projects.Find(id);
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                System.IO.File.Delete(dir + @"~\..\Images\" + prj.ImagePath);
                db.projects.Remove(prj);
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void UpdateProject(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                project prj = db.projects.Find(id);
                prj.Name = Request.Form["Name"];
                prj.Description = Request.Form["Description"];
                prj.Status = Request.Form["Status"];
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase image = Request.Files[0];
                    string testfiles = image.FileName.Split(new char[] { '\\' }).Last();
                    string dir = AppDomain.CurrentDomain.BaseDirectory;
                    string fname = Misc.RandomString(8) + testfiles.Substring(testfiles.LastIndexOf("."));
                    System.IO.File.Delete(dir + @"~\..\Images\" + prj.ImagePath);
                    prj.ImagePath = fname + testfiles.Substring(testfiles.LastIndexOf("."));
                    prj.ImagePath = fname;
                    fname = System.IO.Path.Combine(Server.MapPath("~/Images/"), fname);
                    image.SaveAs(fname);
                }
                db.SaveChanges();
            }
        }
        [HttpPost]
        public void AddProject()
        {
            using (var db = new AlluringDecorEntities())
            {
                project prj = new project();
                prj.Name = Request.Form["Name"];
                prj.Description = Request.Form["Description"];
                prj.Status = Request.Form["Status"];
                HttpPostedFileBase image = Request.Files[0];
                string testfiles = image.FileName.Split(new char[] { '\\' }).Last();
                string dir = AppDomain.CurrentDomain.BaseDirectory;
                string fname = Misc.RandomString(8) + testfiles.Substring(testfiles.LastIndexOf("."));
                prj.ImagePath = fname;
                fname = System.IO.Path.Combine(Server.MapPath("~/Images/"), fname);
                image.SaveAs(fname);
                db.projects.Add(prj);
                db.SaveChanges();
            }
        }
        #endregion

        #region Services
        [Authorize]
        public ActionResult Services()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            List<DomainModel> domains = new List<DomainModel>();
            List<ServiceModel> services = new List<ServiceModel>();
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            using (var db = new AlluringDecorEntities())
            {
                domains = db.domains.Select(m => new DomainModel { Id = m.Id, Name = m.Name }).ToList();
                services = db.services.Select(m => new ServiceModel { Id = m.Id, Name = m.Name }).ToList();
            }
            ViewBag.Domains = domains;
            ViewBag.Services = services;
            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadServiceTableAll()
        {
            List<ServiceDomainModel> srvdmns = new List<ServiceDomainModel>();
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            using (var db = new AlluringDecorEntities())
            {
                srvdmns = (from srvdmn in db.service_domain
                           join srv in db.services on srvdmn.ServiceId equals srv.Id
                           join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                           select new ServiceDomainModel {
                               Id = srvdmn.Id,
                               DomainId = dmn.Id,
                               Domain = new DomainModel() { Id = dmn.Id, Name = dmn.Name },
                               ServiceId = srv.Id,
                               Service = new ServiceModel() { Id = srv.Id, Name = srv.Name }
                           }
                          ).ToList();
            }
            return PartialView("_AdminServicesTableByService", srvdmns);
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadServiceTableByType()
        {
            List<ServiceModel> srvs = new List<ServiceModel>();
            using (var db = new AlluringDecorEntities())
            {
                srvs = db.services.Select(m => new ServiceModel { Id = m.Id, Name = m.Name, Description = m.Description }).ToList();
            }
            return PartialView("_AdminServicesTableByType", srvs);
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadServiceTableByDomain(int domainId)
        {
            List<ServiceDomainModel> srvdmns = new List<ServiceDomainModel>();
            using (var db = new AlluringDecorEntities())
            {
                if (domainId != -1)
                {
                    srvdmns = (from srvdmn in db.service_domain
                               join srv in db.services on srvdmn.ServiceId equals srv.Id
                               join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                               where dmn.Id == domainId
                               select new ServiceDomainModel
                               {
                                   Id = srvdmn.Id,
                                   DomainId = dmn.Id,
                                   Domain = new DomainModel() { Id = dmn.Id, Name = dmn.Name },
                                   ServiceId = srv.Id,
                                   Service = new ServiceModel() { Id = srv.Id, Name = srv.Name }
                               }
                              ).ToList();
                }
                else
                {
                    srvdmns = (from srvdmn in db.service_domain
                               join srv in db.services on srvdmn.ServiceId equals srv.Id
                               join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                               select new ServiceDomainModel
                               {
                                   Id = srvdmn.Id,
                                   DomainId = dmn.Id,
                                   Domain = new DomainModel() { Id = dmn.Id, Name = dmn.Name },
                                   ServiceId = srv.Id,
                                   Service = new ServiceModel() { Id = srv.Id, Name = srv.Name }
                               }
                              ).ToList();
                }
            }
            return PartialView("_AdminServicesTableByDomain", srvdmns);
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadServiceTableByService(int serviceId)
        {
            List<ServiceDomainModel> srvdmns = new List<ServiceDomainModel>();
            using (var db = new AlluringDecorEntities())
            {
                if (serviceId != -1)
                {
                    srvdmns = (from srvdmn in db.service_domain
                               join srv in db.services on srvdmn.ServiceId equals srv.Id
                               join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                               where srv.Id == serviceId
                               select new ServiceDomainModel
                               {
                                   Id = srvdmn.Id,
                                   DomainId = dmn.Id,
                                   Domain = new DomainModel() { Id = dmn.Id, Name = dmn.Name },
                                   ServiceId = srv.Id,
                                   Service = new ServiceModel() { Id = srv.Id, Name = srv.Name }
                               }
                              ).ToList();
                }
                else
                {
                    srvdmns = (from srvdmn in db.service_domain
                               join srv in db.services on srvdmn.ServiceId equals srv.Id
                               join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                               select new ServiceDomainModel
                               {
                                   Id = srvdmn.Id,
                                   DomainId = dmn.Id,
                                   Domain = new DomainModel() { Id = dmn.Id, Name = dmn.Name },
                                   ServiceId = srv.Id,
                                   Service = new ServiceModel() { Id = srv.Id, Name = srv.Name }
                               }
                              ).ToList();
                }
            }
            return PartialView("_AdminServicesTableByService", srvdmns);
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadServiceFormType()
        {
            return PartialView("_AdminServiceFormType");
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadServiceForm()
        {
            List<DomainModel> domains = new List<DomainModel>();
            List<ServiceModel> services = new List<ServiceModel>();
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            using (var db = new AlluringDecorEntities())
            {
                domains = db.domains.Select(m => new DomainModel { Id = m.Id, Name = m.Name }).ToList();
                services = db.services.Select(m => new ServiceModel { Id = m.Id, Name = m.Name }).ToList();
            }
            ViewBag.Domains = domains;
            ViewBag.Services = services;
            return PartialView("_AdminServiceForm");
        }
        [Authorize]
        [HttpPost]
        public void UpdateService(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                service srv = db.services.Find(id);
                srv.Name = Request.Form["Name"];
                srv.Description = Request.Form["Description"];
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void CreateService()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                int id = (db.services.Max(m => (int?)m.Id) ?? 0) + 1;
                string name = Request.Form["Name"];
                string description = Request.Form["Description"];
                db.services.Add(new Models.service() { Id = id, Name = name, Description = description });
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void DeleteService(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                db.services.Remove(db.services.Find(id));
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void CreateServiceDomain()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                int id = (db.service_domain.Max(m => (int?)m.Id) ?? 0) + 1;
                int serviceId = Int32.Parse(Request.Form["ServiceId"]);
                int domainId = Int32.Parse(Request.Form["DomainId"]);
                db.service_domain.Add(new Models.service_domain() { Id = id, ServiceId = serviceId, DomainId = domainId });
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void DeleteServiceDomain(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                db.service_domain.Remove(db.service_domain.Find(id));
                db.SaveChanges();
            }
        }
        #endregion

        #region Domains
        [Authorize]
        public ActionResult Domains()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult LoadDomainTable()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            List<DomainModel> model = new List<DomainModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.domains.Select(m => new DomainModel { Id = m.Id, Name = m.Name }).ToList();
            }
            return PartialView("_AdminDomainTable", model);
        }
        [Authorize]
        [HttpPost]
        public void UpdateDomain(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                domain dmn = db.domains.Find(id);
                dmn.Name = Request.Form["Name"];
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void CreateDomain()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                int id = (db.domains.Max(m => (int?)m.Id) ?? 0) + 1;
                string name = Request.Form["Name"];
                db.domains.Add(new Models.domain() { Id = id, Name = name });
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void DeleteDomain(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                db.domains.Remove(db.domains.Find(id));
                db.SaveChanges();
            }
        }
        #endregion

        #region Request/Bill
        [Authorize]
        public ActionResult Requests()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            UserModel userData = new UserModel();
            using (var db = new AlluringDecorEntities())
            {
                userData = db.users.Where(m => m.Username == User.Identity.Name).Select(m => new UserModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Address = m.Address,
                    Email = m.Email,
                    Phonenumber = m.Phonenumber
                }).FirstOrDefault() ?? new UserModel();
            }
            ViewBag.User = userData;
            return View();
        }
        [Authorize]
        [HttpPost]
        public ActionResult LoadRequestTable()
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin))
            {
                if (Request.UrlReferrer == null) return RedirectToAction("Index", "Account", new { account = "Admin" });
                else return Redirect(Request.UrlReferrer.ToString() + "&account=Admin");
            }
            List<ServiceRequestModel> model = new List<ServiceRequestModel>();
            using (var db = new AlluringDecorEntities())
            {
                model = (from srvrqst in db.service_request
                         join srvdmn in db.service_domain on srvrqst.ServiceDomainId equals srvdmn.Id
                         join srv in db.services on srvdmn.ServiceId equals srv.Id
                         join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                         join usr in db.users on srvrqst.UserId equals usr.Id
                         join bil in db.bills on srvrqst.Id equals bil.ServiceRequestId
                         into dtld_srvrqst
                         from _bill in dtld_srvrqst.DefaultIfEmpty()
                         select new ServiceRequestModel()
                         {
                             Id = srvrqst.Id,
                             UserId = srvrqst.UserId,
                             ServiceDomainId = srvrqst.ServiceDomainId,
                             Receiver = srvrqst.Receiver,
                             Address = srvrqst.Address,
                             Contact = srvrqst.Contact,
                             Email = srvrqst.Email,
                             Status = srvrqst.Status,
                             User = new UserModel() { Id = usr.Id, Username = usr.Username },
                             Bill = new BillModel()
                             {
                                 Id = _bill == null ? -1 : _bill.Id,
                                 ServiceRequestId = _bill == null ? -1 : _bill.ServiceRequestId,
                                 Price = _bill == null ? -1 : _bill.Price,
                                 Note = _bill == null ? "" : _bill.Note
                             },
                             ServiceDomain = new ServiceDomainModel()
                             {
                                 Domain = new DomainModel()
                                 {
                                     Id = dmn.Id,
                                     Name = dmn.Name
                                 },
                                 Service = new ServiceModel()
                                 {
                                     Id = srv.Id,
                                     Name = srv.Name
                                 }
                             }
                         }).ToList();
            }
            return PartialView("_AdminRequestTable", model);
        }
        [Authorize]
        [HttpPost]
        public void UpdateRequestStatus(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                service_request srvrqst = db.service_request.Where(m => m.Id == id).FirstOrDefault();
                srvrqst.Status = Request.Form["Status"];
                db.SaveChanges();
            }
        }
        [Authorize]
        [HttpPost]
        public void UpdateBill(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                bill bil = db.bills.Find(id);
                double price;
                if (double.TryParse(Request.Form["Price"], out price))
                {
                    bil.Note = Request.Form["Note"];
                    bil.Price = double.Parse(Request.Form["Price"]);
                    db.SaveChanges();
                }
                else return;
            }
        }
        [Authorize]
        [HttpPost]
        public void CreateBill(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                int billId = (db.bills.Max(m => (int?)m.Id) ?? 0) + 1;
                int serviceRequestId = id;
                string note = Request.Form["Note"];
                double price;
                if (double.TryParse(Request.Form["Price"], out price))
                {
                    db.bills.Add(new bill { Id = billId, ServiceRequestId = serviceRequestId, Price = price, Note = note });
                    db.SaveChanges();
                }
                else return;
            }
        }
        [Authorize]
        [HttpPost]
        public void DeleteBill(int id)
        {
            if (!Misc.UserIsRole(User.Identity.Name, Misc.UserRoles.Admin)) return;
            using (var db = new AlluringDecorEntities())
            {
                bill bil = db.bills.Find(id);
                db.bills.Remove(bil);
                db.SaveChanges();
            }
        }
        #endregion
    }
}