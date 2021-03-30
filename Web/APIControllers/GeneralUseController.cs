using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Models;
using Web.Models;
using System.Data;
//using Web.Application;
using System.Web;
using System.IO;
using Web.Application;

namespace PaperHybrid.Web.APIControllers
{
    public class BankController : ApiController
    {
        [HttpGet, Route("api/gen/fetchEmail")]
        public HttpResponseMessage fetchEmail(int Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.EmailLogs
                             where l.Id == Id
                             select new
                             {
                                 l.Body,
                                 DisplayName = l.User.DisplayName,
                                 MailID = l.Id
                             }).SingleOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }


        [HttpGet, Route("api/gen/ResendMail")]
        public HttpResponseMessage ResendMail(int Id)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            if (Id != 0)
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    try
                    {
                        EmailLog _item;
                        {
                            _item = _db.EmailLogs.SingleOrDefault(a => a.Id == Id);
                            _item.Sent = false;
                            _db.SaveChanges();
                            resp.ResponseCode = "00";
                            resp.ResponseMsg = "Email has been scheduled to be re-sent";
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ResponseMsg = ex.Message;
                    }
                }
            }
            else
            {
                resp.ResponseMsg = String.Format("Unknown request");
            }
            return Request.CreateResponse(HttpStatusCode.OK);
        }


        [HttpGet, Route("api/gen/UserImageURLAndOtherDetails")]
        public HttpResponseMessage UserImageURLAndOtherDetails(string Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.Users
                             where l.UserID == Id
                             select new
                             {
                                 l.UserID,
                                 l.R_Address,
                                 l.Approved,
                                 l.DisplayName,
                                 l.Phone,
                                 l.Email,
                                 l.ImageURL,
                             }).SingleOrDefault();

                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpGet, Route("api/gen/LoggedinImageURL")]
        public HttpResponseMessage LoggedinImageURL()
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.Users
                             where l.UserID == LoggedInUser.UserId
                             select new
                             {
                                 l.UserID,
                                 l.R_Address,
                                 l.Approved,
                                 l.DisplayName,
                                 l.Phone,
                                 l.Email,
                                 l.ImageURL,
                             }).SingleOrDefault();

                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpPost, Route("api/gen/upload")]
        public HttpResponseMessage UploadFile(string Name)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            string strExcelFilename = HttpContext.Current.Server.MapPath("~/UploadedFiles/");
            try
            {
                if (!String.IsNullOrEmpty(Name))
                {
                    {
                        var httpPostedFile = HttpContext.Current.Request.Files["fileUpload"];
                        if (httpPostedFile != null)
                        {
                            strExcelFilename = Path.Combine(strExcelFilename, httpPostedFile.FileName);
                            if (File.Exists(strExcelFilename))
                            {
                                resp.ResponseMsg = "This file has already been uploaded. Please choose another file or rename it.";
                                return Request.CreateResponse(HttpStatusCode.OK, resp);
                            }
                            httpPostedFile.SaveAs(strExcelFilename);
                            FileInfo fileInfo = new FileInfo(strExcelFilename);
                            string GetFileName = httpPostedFile.FileName;
                            string ImageURL = ("/UploadedFiles/" + GetFileName);
                            var readResp = FilesTransaction.readPaymentFile(fileInfo, GetFileName, strExcelFilename, ImageURL, Name);
                            if (readResp.ResponseCode == "00")
                            {
                                resp.ResponseCode = "00";
                                resp.ResponseMsg = "Uploaded Successfully";
                            }
                            else
                            {
                                fileInfo.Delete();
                                resp.ResponseCode = "96";
                                resp.ResponseMsg = readResp.ResponseMsg;
                            }
                        }
                        else
                        {
                            resp.ResponseCode = "96";
                            resp.ResponseMsg = "Please upload a file";
                        }
                    }
                }
                else
                {
                    resp.ResponseCode = "96";
                    resp.ResponseMsg = "Session expired, Logout and Login again!";
                }
            }
            catch (System.Exception ex)

            {

                resp.ResponseMsg = ex.Message;

            }

            return Request.CreateResponse(HttpStatusCode.OK, resp);

        }
 
    }

    public class FilesTransaction
    {
        public String FileName { get; set; }
        public String Picture { get; set; }
        public String ImageURL { get; set; }


        //public static ActionResponse readPaymentFile(FileInfo fileInfo, string GetFileName, string strExcelFilename, string ImageURL, string UserName)       
            public static ActionResponse readPaymentFile(FileInfo fileInfo, string GetFileName, string strExcelFilename, string ImageURL, string UserName)
            {
                ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            try
            {
                if (fileInfo.Length > 0)
                {
                    FilesTransaction trans = new FilesTransaction();
                    trans.FileName = GetFileName;
                    trans.Picture = strExcelFilename;
                    trans.ImageURL = ImageURL;
                    FilesTransaction.InsertBatch(trans, UserName);

                    resp.ResponseCode = "00";
                    resp.ResponseMsg = "Uploaded Successfully";
                }
                else
                {
                    resp.ResponseCode = "96";
                    resp.ResponseMsg = "This file is empty. Please choose another file";
                }
            }
            catch (Exception ex)
            {
                resp.ResponseMsg = ex.Message;
            }
            return resp;
        }

        //private static void InsertBatch(List<FilesTransaction> trans
        private static void InsertBatch(FilesTransaction trans, string UserID)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                var tran = _db.Database.BeginTransaction();
                try
                {
                    //foreach (FilesTransaction item in trans)
                    //{
                    User _item;
                    {
                        //var mCheck = _db.Users.FirstOrDefault(a => a.Picture.Trim() == trans.ImageURL.Trim()); // && !a.Deleted);
                        //if (mCheck == null)
                        //{
                            _item = _db.Users.FirstOrDefault(a => a.UserID == UserID);
                            _item.ImageURL = trans.ImageURL;
                            //_item.Picture = trans.Picture;
                            //_item.filename = trans.FileName;
                        //}
                        //else
                        //{
                        //    resp.ResponseMsg = String.Format("Duplicate File Name");
                        //}
                    }
                    //}

                    _db.SaveChanges();
                    tran.Commit();
                    resp.ResponseCode = "00";
                }
                catch (System.Exception ex)
                {
                    resp.ResponseCode = "96";
                    resp.ResponseMsg = ex.Message;
                    tran.Rollback();
                }
            }

            //throw new NotImplementedException();
        }
    }
}
