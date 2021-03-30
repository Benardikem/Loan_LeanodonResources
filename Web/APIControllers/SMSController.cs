using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Models;
using Web.Models;
using System.Data;
using Web.Application;
using API.Utilities;
using System.Text;
using API;
using System.Collections.Generic;
using System.Net.Mail;
using System.IO;
using System.Web;
using Web.Codes.Constants;
using System.Globalization;

namespace Web.APIControllers
{
    public class SMSController : ApiController
    {

        // function to get the full month name  
        static string getFullName(int month)
        {
            return CultureInfo.CurrentCulture.
                DateTimeFormat.GetMonthName
                (month);
        }

        //create
        [HttpPost, Route("api/SMS/create-category")]
        public HttpResponseMessage CreateCategory(SMSrequests request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    SMSContactCategory _item;

                    _item = _db.SMSContactCategorys.FirstOrDefault(m => m.CategoryName == request.CategoryName);
                    if (_item == null)
                    {
                        _item = _db.SMSContactCategorys.Create();
                        _item.CategoryName = request.CategoryName;
                        _db.SMSContactCategorys.Add(_item);
                        _db.SaveChanges();

                        //UPDATE ACTIVITY LOG
                        UpdateActivityLog("Created SMS Contact Category", "An Admin Created a new SMS Contact Category", $"{LoggedInUser.UserName} Created Category Name : {request.CategoryName} for SMS)");


                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"New Contact Category created";
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("Category Name already exists");
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }


        [HttpGet, Route("api/SMS/fetch")]
        public HttpResponseMessage FetchReciepients(int Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.SMSContactLists
                             where l.CategoryID == Id
                             select new
                             {
                                 l.CategoryID,
                                 Categoryname = l.ContactCategory.CategoryName,
                                 l.PhoneNumber,
                                 l.Name                                
                             }).SingleOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpPost, Route("api/SMS/addReciepient")]
        public HttpResponseMessage AddNewSMSReciepient(SMSrequests request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    SMSContactList _item;
                    //check if phone number already exists in same category
                    _item = _db.SMSContactLists.FirstOrDefault(m => m.PhoneNumber == request.PhoneNumber && m.CategoryID == request.CategoryID);
                    if (_item == null)
                    {
                        var APPno = "LRL20" + Alphanumeric.Generate(6);
                        //var InvoiceHtmlURL = $"{InsertInvoiceHtmltoLocalFolder.HtmlFilePath_Local}{APPno}.html";
                        _item = _db.SMSContactLists.Create();
                        _item.CategoryID = request.CategoryID;
                        _item.Name = request.Name;
                        _item.PhoneNumber = request.PhoneNumber;
                        _db.SMSContactLists.Add(_item);
                        _db.SaveChanges();   

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"SMS Contact Added";
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("phone number exists for this category");
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }


        public HttpResponseMessage UpdateActivityLog(string ActivityTitle, string ActivityType, string Details)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    //Update Activity
                    ActivityLog _log = _db.ActivityLogs.Create();
                    _log.Title = ActivityTitle;
                    _log.UserName = LoggedInUser.UserName;
                    _log.ActivityType = ActivityType;
                    _log.Details = Details;
                    _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                    _db.ActivityLogs.Add(_log);
                    _db.SaveChanges();
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

    }
}
