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
using Web.Codes.Constants;

namespace Web.Controllers
{
    public class SMSController : Controller
    {
        [CustomAuthorizeAttribute(OperationKey = "/SMS")]
        public ActionResult Index()
        {
            SMSModel model = new SMSModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.SMSItems = _db.SMSLogs//.Where(a => !a.Deleted)
                                       .Select(a => new SMSItem
                                       {
                                           Id = a.Id,
                                           ActivityDate = a.ActivityDate,
                                           Message = a.Message,
                                           Recipient = a.Recipient,
                                           Status = a.Status,
                                           UnitsUsed = a.UnitsUsed,
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

        [CustomAuthorizeAttribute(OperationKey = "/SMS/SendSMS")]
        public ActionResult SendSMS()
        {
            SMSModel model = new SMSModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.SMSCategoryItems = _db.SMSContactCategorys//.Where(a => !a.Deleted)
                                       .Select(a => new SMSCategoryItem
                                       {
                                           Id = a.Id,
                                            CategoryName = a.CategoryName,
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