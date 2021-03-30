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
    public class EmaillogController : Controller
    {
        [CustomAuthorizeAttribute(OperationKey = "/emaillog/index")]
        public ActionResult Index()
        {
            MailModel model = new MailModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    User _usr;
                    _usr = _db.Users.FirstOrDefault(m => m.UserID == LoggedInUser.UserId);
                    if (_usr.UserCategory == "ADMIN")
                    {
                        model.MailItems = _db.EmailLogs//.Where(a => !a.Deleted)
                                           .Select(a => new MailItem
                                           {
                                               Id = a.Id,
                                               From = a.From,
                                               To = a.To,
                                               Subject = a.Subject,
                                               Body = a.Body,
                                               CreatedDate = a.CreatedDate,
                                               Sent = a.Sent ? "Yes" : "No",
                                               SentDate = a.SentDate,
                                               Module = a.Module,
                                               Cc = a.Cc,
                                               Bcc = a.Bcc,
                                               Attachments = a.Attachments,
                                           })
                                           .OrderBy(a => a.CreatedDate)
                                           .ToList();

                    }
                    if (_usr.UserCategory == "USER")
                    {
                        model.MailItems = _db.EmailLogs.Where(a => a.To == LoggedInUser.Email || a.From == LoggedInUser.Email)
                                            .ToList()
                                           .Select(a => new MailItem
                                           {
                                               Id = a.Id,
                                               From = a.From,
                                               To = a.To,
                                               Subject = a.Subject,
                                               Body = a.Body,
                                               CreatedDate = a.CreatedDate,
                                               Sent = a.Sent ? "Yes" : "No",
                                               SentDate = a.SentDate,
                                               Module = a.Module,
                                               Cc = a.Cc,
                                               Bcc = a.Bcc,
                                               Attachments = a.Attachments,
                                           })
                                           .OrderBy(a => a.CreatedDate)
                                           .ToList();
                    }
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