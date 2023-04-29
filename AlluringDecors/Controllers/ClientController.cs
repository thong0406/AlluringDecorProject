using AlluringDecors.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AlluringDecors.Controllers
{
    public class ClientController : Controller
    {
        // GET: Client
        public ActionResult Index()
        {
            return View();
        }
        #region bills
        [Authorize]
        public ActionResult Bills()
        {
            return View();
        }
        [Authorize]
        [HttpGet]
        public ActionResult LoadBillTable()
        {
            List<ServiceRequestModel> model = new List<ServiceRequestModel>();
            using (var db = new AlluringDecorEntities())
            {
                user usr = db.users.Where(m => m.Username == User.Identity.Name).FirstOrDefault();
                model = (from bil in db.bills
                         join srvrqst in db.service_request on bil.ServiceRequestId equals srvrqst.Id
                         join srvdmn in db.service_domain on srvrqst.ServiceDomainId equals srvdmn.Id
                         join srv in db.services on srvdmn.ServiceId equals srv.Id
                         join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                         where srvrqst.UserId == usr.Id
                         select new ServiceRequestModel {
                             Id = srvrqst.Id,
                             Receiver = srvrqst.Receiver,
                             Address = srvrqst.Address,
                             Contact = srvrqst.Contact,
                             Email = srvrqst.Email,
                             Status = srvrqst.Status,
                             User = new UserModel() { Id = usr.Id, Username = usr.Username },
                             Bill = new BillModel()
                             {
                                 Id = bil.Id,
                                 ServiceRequestId = bil.ServiceRequestId,
                                 Price = bil.Price,
                                 Note = bil.Note,
                                 DateSent = bil.DateSent
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
            return PartialView("_ClientBillTable", model);
        }
        #endregion
        #region requests
        [Authorize]
        public ActionResult Requests()
        {
            UserModel userData = new UserModel();
            Dictionary<string, List<string>> dmnsrv = new Dictionary<string, List<string>>();
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
                List<service> srvlst = db.services.ToList();
                List<domain> dmnlst = db.domains.ToList();
                foreach (domain dmn in dmnlst)
                {
                    List<int> srvIds = db.service_domain.Where(m => m.DomainId == dmn.Id).Select(m => m.ServiceId).ToList();
                    List<string> srvNames = srvlst.Where(m => !srvIds.Contains(m.Id)).Select(m => "srv_" + m.Id).ToList();
                    dmnsrv.Add("dmn_" + dmn.Id, srvNames);
                }
            }
            ViewBag.User = userData;
            ViewBag.DomainServices = Newtonsoft.Json.JsonConvert.SerializeObject(dmnsrv, Newtonsoft.Json.Formatting.Indented);
            return View();
        }
        [Authorize]
        public ActionResult CancelRequest(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                service_request srvrqst = db.service_request.Where(m => m.Id == id).FirstOrDefault();
                if (srvrqst != null)
                {
                    db.service_request.Remove(srvrqst);
                    db.SaveChanges();
                    TempData["success"] = "Cancelled successfully!";
                }
                else TempData["error"] = "There was a problem, please try again.";
            }
            return RedirectToAction("Requests");
        }
        [Authorize]
        [HttpPost]
        public ActionResult LoadRequestTable()
        {
            List<ServiceRequestModel> model = new List<ServiceRequestModel>();
            using (var db = new AlluringDecorEntities())
            {
                user userData = db.users.Where(m => m.Username == User.Identity.Name).FirstOrDefault();
                model = (from srvrqst in db.service_request
                         join srvdmn in db.service_domain on srvrqst.ServiceDomainId equals srvdmn.Id
                         join srv in db.services on srvdmn.ServiceId equals srv.Id
                         join dmn in db.domains on srvdmn.DomainId equals dmn.Id
                         join bil in db.bills on srvrqst.Id equals bil.ServiceRequestId
                         into dtld_srvrqst
                         from _bill in dtld_srvrqst.DefaultIfEmpty()
                         where srvrqst.UserId == userData.Id
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
                             DateSent = srvrqst.dateSent,
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
            return PartialView("_ClientRequestTable", model);
        }
        [Authorize]
        [HttpPost]
        public ActionResult LoadRequestModal(int id)
        {
            ServiceRequestModel model = new ServiceRequestModel();
            List<ServiceModel> srvlst = new List<ServiceModel>();
            List<DomainModel> dmnlst = new List<DomainModel>();
            Dictionary<string, List<string>> dmnsrv = new Dictionary<string, List<string>>();
            using (var db = new AlluringDecorEntities())
            {
                model = db.service_request.Where(m => m.Id == id).Select(m => new ServiceRequestModel()
                    {
                        Id = m.Id,
                        Receiver = m.Receiver,
                        Address = m.Address,
                        Contact = m.Contact,
                        Email = m.Email
                    })
                    .FirstOrDefault();
                srvlst = db.services.Select(m => new ServiceModel { Id = m.Id, Name = m.Name }).ToList();
                dmnlst = db.domains.Select(m => new DomainModel { Id = m.Id, Name = m.Name }).ToList();
                foreach (DomainModel dmn in dmnlst)
                {
                    List<int> srvIds = db.service_domain.Where(m => m.DomainId == dmn.Id).Select(m => m.ServiceId).ToList();
                    List<string> srvNames = srvlst.Where(m => !srvIds.Contains(m.Id)).Select(m => "srv_" + m.Id).ToList();
                    dmnsrv.Add("dmn_" + dmn.Id, srvNames);
                }
            }
            ViewBag.Services = srvlst;
            ViewBag.Domains = dmnlst;
            return PartialView("_ClientRequestModal", model);
        }
        [Authorize]
        [HttpPost]
        public void UpdateRequest(int id)
        {
            using (var db = new AlluringDecorEntities())
            {
                service_request srvrqst = db.service_request.Where(m => m.Id == id).FirstOrDefault();

                string domainIdStr = Request.Form["DomainId"];
                int domainId = Int32.Parse(domainIdStr.Substring(domainIdStr.IndexOf("_") + 1));
                string serviceIdStr = Request.Form["ServiceId"];
                int serviceId = Int32.Parse(serviceIdStr.Substring(serviceIdStr.IndexOf("_") + 1));
                service_domain srvdmn = db.service_domain.Where(m => m.DomainId == domainId && m.ServiceId == serviceId).FirstOrDefault();

                srvrqst.ServiceDomainId = srvdmn.Id;
                srvrqst.Receiver = Request.Form["Receiver"];
                srvrqst.Email = Request.Form["Email"];
                srvrqst.Contact = Request.Form["Contact"];
                srvrqst.Address = Request.Form["Address"];

                db.SaveChanges();
            }
        }
        #endregion
    }
}