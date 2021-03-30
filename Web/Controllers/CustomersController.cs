using Web.Attributes;
using Web.Extensions;
using Web.Models;
using System.Linq;
using System.Web.Mvc;
using API.Models;
using API.Utilities;
using Web.Codes.Constants;
using System.Web;
using System;
using Web.Application;
using System.IO;

namespace Web.Controllers
{
    [Authorize]
    public class CustomersController : Controller
    {
        #region Customers
        [CustomAuthorize]
        public ActionResult Index()
        {
            UserModel model = new UserModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.UserItems = _db.Users.Where(a => !a.Deleted && a.UserCategory == UserCategory.USER)
                                                .ToList()
                                                .Select(a => new UserItem
                                                {
                                                    UserID = a.UserID,
                                                    DisplayName = a.DisplayName,
                                                    Number = a.Phone,
                                                    AddedBy = a.AddedBy,
                                                    EmailAddress = a.Email,
                                                    UserCategory = a.UserCategory,
                                                    AddedDate = a.DateAdded,
                                                    Active = a.Active ? "Yes" : "No",
                                                    ModifiedDate = a.DateModified,
                                                    Approved = a.Approved ? "Yes" : "No",
                                                    Roles = a.UserRoles.Select(r => r.Role.Name).ToList()
                                                })
                                                .OrderBy(a => a.DisplayName)
                                                .ToList();

                    model.RoleItems = (from s in _db.Roles
                                       where s.Approved && s.Active
                                       select new RoleItem
                                       {
                                           Id = s.ID,
                                           Name = s.Name
                                       }).OrderBy(a => a.Name)
                                        .ToList();
                }
            }
            catch (System.Exception ex)
            {
                General.LogExceptions("", ex);
                ViewBag.MessageText = ex.Message.ToString();
                ViewBag.MessageType = MessageLabelControl.MessageType.danger;
            }
            return View(model);
        }


        [CustomAuthorize]
        public ActionResult Approval()
        {
            UserModel model = new UserModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.UserItems = _db.Users.Where(a => !a.Deleted && !a.Approved && a.UserCategory == UserCategory.USER)
                                                .ToList()
                                                .Select(a => new UserItem
                                                {
                                                    UserID = a.UserID,
                                                    DisplayName = a.DisplayName,
                                                    UserCategory = a.UserCategory,
                                                    EmailAddress = a.Email,
                                                    Number = a.Phone,
                                                    AddedBy = a.AddedBy,
                                                    AddedDate = a.DateAdded,
                                                    Active = a.Active ? "Yes" : "No",
                                                    ModifiedDate = a.DateModified,
                                                    Approved = a.Approved ? "Yes" : "No"
                                                })
                                                .OrderBy(a => a.DisplayName)
                                                .ToList();

                    //model.RoleItems = (from s in _db.Roles
                    //                   where s.Approved && s.Active
                    //                   select new RoleItem
                    //                   {
                    //                       Id = s.ID,
                    //                       Name = s.Name
                    //                   }).OrderBy(a => a.Name)
                    //                    .ToList();
                }
            }
            catch (System.Exception ex)
            {
                General.LogExceptions("", ex);
                ViewBag.MessageText = ex.Message.ToString();
                ViewBag.MessageType = MessageLabelControl.MessageType.danger;
            }
            return View(model);
        }
        #endregion

        //public ActionResult CreatePP(string Name, HttpPostedFileBase studImg)
        //{
        //    ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
        //    string strExcelFilename = HttpContext.Current.Server.MapPath("~/UploadedFiles/");
        //    try
        //    {
        //        if (!String.IsNullOrEmpty(LoggedInUser.UserName))
        //        {
        //            {
        //                var httpPostedFile = HttpContext.Current.Request.Files["excelfile"];
        //                if (httpPostedFile != null)
        //                {
        //                    strExcelFilename = Path.Combine(strExcelFilename, httpPostedFile.FileName);
        //                    if (File.Exists(strExcelFilename))
        //                    {
        //                        resp.ResponseMsg = "This file has already been uploaded. Please choose another file or rename it.";
        //                        return Request.CreateResponse(HttpStatusCode.OK, resp);
        //                    }
        //                    httpPostedFile.SaveAs(strExcelFilename);
        //                    FileInfo fileInfo = new FileInfo(strExcelFilename);
        //                    string GetFileName = httpPostedFile.FileName;
        //                    string ImageURL = ("/UploadedFiles/" + GetFileName);
        //                    var readResp = FilesTransaction.readPaymentFile(fileInfo, GetFileName, strExcelFilename, ImageURL, httpPostedFile.ContentType);
        //                    if (readResp.ResponseCode == "00")
        //                    {
        //                        resp.ResponseCode = "00";
        //                        resp.ResponseMsg = "Uploaded Successfully";
        //                    }
        //                    else
        //                    {
        //                        fileInfo.Delete();
        //                        resp.ResponseCode = "96";
        //                        resp.ResponseMsg = readResp.ResponseMsg;
        //                    }
        //                }
        //                else
        //                {
        //                    resp.ResponseCode = "96";
        //                    resp.ResponseMsg = "Please upload a file";
        //                }
        //            }
        //        }
        //        else
        //        {
        //            resp.ResponseCode = "96";
        //            resp.ResponseMsg = "Session expired, Logout and Login again!";
        //        }
        //    }
        //    catch (System.Exception ex)

        //    {

        //        resp.ResponseMsg = ex.Message;

        //    }
        //}
    }
}