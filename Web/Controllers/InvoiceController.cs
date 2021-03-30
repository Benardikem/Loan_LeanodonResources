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
using System.IO;

namespace Web.Controllers
{
    public class InvoiceController : Controller
    {
        [CustomAuthorizeAttribute(OperationKey = "/Invoice")]
        public ActionResult Index()
        {
            InvoiceModel model = new InvoiceModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {       model.InvoiceItems = _db.Invoices//.Where(a => !a.Deleted)
                                           .Select(a => new InvoiceItem
                                           {
                                               Id = a.Id,
                                                CreatedBy = a.CreatedBy,
                                                CustomerID = a.CustomerID,
                                                CustomerName = a.CustomerName,
                                                InvoiceDate = a.InvoiceDate,
                                                InvoiceNumber = a.InvoiceNumber,
                                                InvoiceURL = a.InvoiceURL,
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


        //Download Invoices
        public FileResult Download(string ImageName)
        {
            var FileVirtualPath = "~/Invoices/" + ImageName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }
    }
}