using Web.Models;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Web.Application;
using API.Utilities;
using Web.Interfaces;
using Web.Codes;
using API.Models;
using System.Linq;
using Web.Extensions;
using Web.Attributes;

namespace Web.Controllers
{
    public class ActivitylogController : Controller
    {
        [CustomAuthorizeAttribute(OperationKey = "/ActivityLog")]
        public ActionResult Index()
        {
            ActivitylogModel model = new ActivitylogModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {                   
                        model.ActivitylogItems = _db.ActivityLogs//.Where(a => !a.Deleted)
                                           .Select(a => new ActivitylogItem
                                           {
                                               Id = a.Id,
                                                ActivityDate = a.ActivityDate,
                                                ActivityType = a.ActivityType,
                                                Details = a.Details,
                                                Title = a.Title,
                                                UserName = a.UserName,
                                           })
                                           .OrderBy(a => a.Id)
                                           .ToList();

                    }  
            }
            catch (Exception ex)
            {
                General.LogExceptions("", ex);
                ViewBag.MessageText = ex.Message.ToString();
                ViewBag.MessageType = MessageLabelControl.MessageType.danger;
            }
            return View(model);
        }
    }
}