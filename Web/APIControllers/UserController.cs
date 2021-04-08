using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using API.Models;
using Web.Models;
using Web.Application;
using API;
using Web.Codes;
using API.Utilities;
using System.Web;
using System.IO;
using System.Net.Mail;
using System.Globalization;
using System.Threading;
using Web.Codes.Constants;
using System.Text;

namespace Web.APIControllers
{
    public class UserController : ApiController
    {
        //[HttpGet, Route("api/user/fetch-from-ad")]
        //public HttpResponseMessage FetchUserFromAD(string username)
        //{
        //    UserItem _item = new UserItem();
        //    SendMailMessage("", "", "", "");
        //    using (ADManager _ad = new ADManager())
        //    {
        //        UserPrincipal aDUser = _ad.GetUser(username);
        //        if (aDUser != null)
        //        {
        //            _item = new UserItem
        //            {
        //                DisplayName = aDUser.DisplayName,
        //                UserID = aDUser.SamAccountName,
        //                EmailAddress = aDUser.EmailAddress,

        //            };
        //        }
        //        else
        //        {
        //            return Request.CreateResponse(HttpStatusCode.NotFound);
        //        }
        //    }
        //    return Request.CreateResponse(HttpStatusCode.OK, _item);
        //}

        [HttpGet, Route("api/user/is-exist")]
        public HttpResponseMessage IsUserExist(string username)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = _db.Users.SingleOrDefault(u => u.UserID == username && !u.Deleted);
                if (_item != null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, true);
                }
                else
                {
                    return Request.CreateResponse(HttpStatusCode.OK, false);
                }
            }
        }

        //[HttpGet, Route("api/user/search")]
        //public HttpResponseMessage Search(string q)
        //{
        //    using (MyDbContext _db = new MyDbContext())
        //    {
        //        var list = _db.Users.Where(a => a.UserCategoryId == API.Settings.Site.PermanentStaffCategory && a.DisplayName.ToLower().Contains(q.ToLower()))
        //                            .Select(a => new
        //                            {
        //                                a.DisplayName,
        //                                a.UserID
        //                            }).ToList();
        //        return Request.CreateResponse(HttpStatusCode.OK, list);
        //    }
        //}

        [HttpGet, Route("api/user/fetch")]
        public HttpResponseMessage FetchUserDetail(String Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {

                var _item = (from l in _db.Users
                             where l.UserID == Id
                             select new
                             {
                                 l.UserID,
                                 l.DisplayName,
                                 EmailAddress = l.Email,
                                 l.Active,
                                 l.Phone,
                                 l.UserCategory,
                                 //l.DividendPriviledge,
                                 Roles = _db.UserRoles.Where(a => a.UserId == Id).Select(a => new
                                 {
                                     a.ID,
                                     a.RoleId,
                                     a.UserId,
                                     a.Role.Name
                                 }).ToList()
                             }).SingleOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }
        [HttpPost, Route("api/user/add")]
        public HttpResponseMessage AddUser(UserRequest request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                var tran = _db.Database.BeginTransaction();
                try
                {
                    //Assigning ID to new Admin
                    string zeroBased = DateTime.Today.ToString("yy-MM-dd");
                    var AdminID = "LRA" + zeroBased + Alphanumeric.Generate(6);

                    User _item;
                    if (!String.IsNullOrEmpty(request.UserID) && request.Mode == "NEW")
                    {
                        var mCheck = _db.Users.SingleOrDefault(a => a.UserID == AdminID && !a.Deleted);
                        if (mCheck == null)
                        {
                            var ECheck = _db.Users.SingleOrDefault(a => a.Email.ToLower() == request.EmailAddress.Trim().ToLower() && !a.Deleted);
                            if (ECheck == null)
                            {
                                _item = _db.Users.Create();
                                _item.UserID = request.UserID;
                                _item.Email = request.EmailAddress;
                                _item.DisplayName = request.Name;
                                _item.Phone = request.Mobile;
                                _item.UserCategory = UserCategory.ADMIN;
                                _item.Active = request.Active;
                                _item.R_Address = request.R_Address;
                                _item.AddedBy = LoggedInUser.UserName; 
                                _item.Approved = false;
                                string pwd = Defaultpassword.Apassword;
                                _item.Password = API.Utilities.Crypto.Encrypt(pwd);
                                _item.DateAdded = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                                _db.Users.Add(_item);
                                _db.SaveChanges();

                                foreach (UserRoleRequest _roleReq in request.Roles)
                                {
                                    UserRole _role = _db.UserRoles.Create();
                                    _role.RoleId = _roleReq.RoleId;
                                    _role.UserId = _item.UserID;
                                    _db.UserRoles.Add(_role);
                                }
                                _db.SaveChanges();
                                tran.Commit();


                                using (var client = new WebClient())
                                {
                                    //Email BODY
                                    string html = "";
                                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                                    TextInfo textInfo = cultureInfo.TextInfo;
                                    string userdisplayname = textInfo.ToTitleCase(request.Name);
                                    var receiverEmail = new MailAddress(request.EmailAddress, userdisplayname);
                                    var senderEmail = new MailAddress(Codes.Constants.Constants.TestMail, "Loan App Support || Email Verification");

                                    html = client.DownloadString(EmailDownloadString.OnlineFilePath);

                                    //Sending reset password Email
                                    var sub = $"welcome mail to { userdisplayname}";
                                    //string sFileName = HttpContext.Current.Server.MapPath(@"~/Attachments/Consumer-Loan-Application-Form.pdf");
                                    string mailbody = $"<p>Your account has been created, please find below;</p>{UserCategory.ADMIN} ID: {request.UserID}<br/>Password: {pwd}" +
                                        $" <br><br><br><br>Secure and instant loans have got you covered. Apply for a loan now and get the funds in less than 3 hours." +
                                        $" <br><br><br><br>Best Regards,<br>Managing Director, Loendon Resources";

                                    StringBuilder sb = new StringBuilder(html);
                                    string textBody =
                                    sb
                                       .Replace("Subject", sub)
                                        .Replace("RecieversName", userdisplayname)
                                        .Replace("Enter your message here", mailbody)
                                      .ToString();

                                    var body = textBody;
                                    EmailItem _mail = new EmailItem
                                    {
                                        Title = "Admin Login Details",
                                        Body = body,
                                        To = new List<string> { request.EmailAddress },
                                    };
                                    Messaging.LogMail(_mail);
                                }
                                resp.ResponseCode = "00";
                                resp.ResponseMsg = "New Admin Successfully Created";
                            }
                            else
                            {
                                resp.ResponseMsg = String.Format("Duplicate Email Address");
                            }
                        }
                        else
                        {
                            resp.ResponseMsg = String.Format("Duplicate Admin ID");
                        }
                    }
                    else if (!String.IsNullOrEmpty(request.UserID) && request.Mode == "EDIT")
                    {
                        //var mCheck = _db.Users.SingleOrDefault(a => a.UserID != request.Id && !a.Deleted);
                        //if (mCheck == null)
                        //{
                        _item = _db.Users.SingleOrDefault(a => a.UserID == request.UserID);
                        _item.DisplayName = request.Name;
                        _item.Phone = request.Mobile;
                        _item.Active = request.Active;
                        _item.ModifiedBy = LoggedInUser.UserName;
                        _item.Email = request.EmailAddress;
                        _item.DateModified = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                        _item.Approved = false;

                        //Delete all Roles
                        var roles = _db.UserRoles.Where(s => s.UserId == _item.UserID);
                        if (roles != null && roles.Count() > 0)
                            _db.UserRoles.RemoveRange(roles);

                        //Insert new roles
                        foreach (UserRoleRequest _roleReq in request.Roles)
                        {
                            UserRole _role = _db.UserRoles.Create();
                            _role.RoleId = _roleReq.RoleId;
                            _role.UserId = _item.UserID;
                            _db.UserRoles.Add(_role);
                        }
                        _db.SaveChanges();
                        tran.Commit();

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = "Admin Details Successfully Updated";
                        //}
                        //else
                        //{
                        //    resp.ResponseMsg = String.Format("Duplicate Role Name");
                        //}
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("Unknown Request");
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                    tran.Rollback();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpPost, Route("api/user/addcustomer")]
        public HttpResponseMessage AddCustomer(UserRequest request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                var tran = _db.Database.BeginTransaction();
                try
                {
                    //Assigning ID to new Admin
                    DateTime date = DateTime.Now;
                    string zeroBased = string.Format("{0}", date.Millisecond); 
                    var CustomerID = "LRC" + zeroBased + Alphanumeric.Generate(3);

                    User _item;
                    if (!String.IsNullOrEmpty(CustomerID) && request.Mode == "NEW")
                    {
                        var mCheck = _db.Users.SingleOrDefault(a => a.UserID.ToLower() == CustomerID && !a.Deleted);
                        if (mCheck == null)
                        {
                            var ECheck = _db.Users.SingleOrDefault(a => a.Email.ToLower() == request.EmailAddress.Trim().ToLower() && !a.Deleted);
                            if (ECheck == null)
                            {
                                _item = _db.Users.Create();
                                _item.UserID = CustomerID;
                                _item.Email = request.EmailAddress;
                                _item.DisplayName = request.Name;
                                _item.Phone = request.Mobile;

                                _item.R_Address = request.R_Address;
                                _item.C_Address = request.C_Address;
                                _item.UserCategory = UserCategory.USER;
                                _item.Active = request.Active;
                                _item.AddedBy = LoggedInUser.UserName;
                                _item.Approved = false;
                                string pwd = Defaultpassword.Cpassword;
                                _item.Password = API.Utilities.Crypto.Encrypt(pwd);
                                _item.DateAdded = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                                _item.BVN_Number = request.BVN;
                                _item.Guarantor = request.Guarantor;
                                _item.GuarantorPhone = request.GuarantorPhone;
                                _item.G_Address = request.G_Address;
                                _db.Users.Add(_item);
                                _db.SaveChanges();

                                ActivityLog _log = _db.ActivityLogs.Create();
                                _log.Title = LoanTitles.NewCustomer;
                                _log.UserName = LoggedInUser.UserName;
                                _log.ActivityType = LoanLogStatus.CreatednewCustomer;
                                _log.Details = $"{LoanLogDetails.NewCustDetails} Customer ID: {CustomerID}";
                                _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                                _db.ActivityLogs.Add(_log);
                                _db.SaveChanges();

                                tran.Commit();

                                using (var client = new WebClient())
                                {
                                    //Email BODY
                                    string html = "";
                                    CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                                    TextInfo textInfo = cultureInfo.TextInfo;
                                    string userdisplayname = textInfo.ToTitleCase(request.Name);
                                    var receiverEmail = new MailAddress(request.EmailAddress, userdisplayname);
                                    var senderEmail = new MailAddress(Codes.Constants.Constants.TestMail, "Loan App Support || Welcome Email");

                                    html = client.DownloadString(EmailDownloadString.OnlineFilePath);

                                    string mailbody = $"<p>Your account has been created, please find below;</p>CUSTOMER ID: {request.UserID}<br/>Password: {pwd}" +
                                        $" <br><br><br><br>Secure and instant loans have got you covered. Apply for a loan now and get the funds in less than 3 hours." +
                                        $" <br><br><br><br>Best Regards,<br>Managing Director, Loendon Resources";

                                    StringBuilder sb = new StringBuilder(html);
                                    string textBody =
                                    sb
                                       .Replace("Subject", "")
                                        .Replace("RecieversName", userdisplayname)
                                        .Replace("Enter your message here", mailbody)
                                      .ToString();

                                    var body = textBody;
                                    EmailItem _mail = new EmailItem
                                    {
                                        Title = "New Customer Login Details",
                                        Body = body,
                                        To = new List<string> { request.EmailAddress },
                                    };
                                    Messaging.LogMail(_mail);
                                }
                                resp.ResponseCode = "00";
                                resp.ResponseMsg = "New Customer Successfully Created";
                            }
                            else
                            {
                                resp.ResponseMsg = String.Format("Duplicate Email Address");
                            }
                        }
                        else
                        {
                            resp.ResponseCode = "90";
                            resp.ResponseMsg = String.Format("The system is attempting to duplicate an existing CustomerID, kindly click ok to reset the system.");
                        }
                    }
                    else if (!String.IsNullOrEmpty(request.UserID) && request.Mode == "EDIT")
                    {
                        //var mCheck = _db.Users.SingleOrDefault(a => a.UserID != request.Id && !a.Deleted);
                        //if (mCheck == null)
                        //{
                        _item = _db.Users.SingleOrDefault(a => a.UserID == request.UserID);
                        _item.DisplayName = request.Name;
                        _item.Phone = request.Mobile;
                        _item.Active = request.Active;
                        _item.ModifiedBy = LoggedInUser.UserName;
                        _item.Email = request.EmailAddress;
                        _item.DateModified = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                        _item.Approved = false;

                        //Delete all Roles
                        var roles = _db.UserRoles.Where(s => s.UserId == _item.UserID);
                        if (roles != null && roles.Count() > 0)
                            _db.UserRoles.RemoveRange(roles);

                        //Insert new roles
                        foreach (UserRoleRequest _roleReq in request.Roles)
                        {
                            UserRole _role = _db.UserRoles.Create();
                            _role.RoleId = _roleReq.RoleId;
                            _role.UserId = _item.UserID;
                            _db.UserRoles.Add(_role);
                        }
                        _db.SaveChanges();
                        tran.Commit();

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = "User successfully updated";
                        //}
                        //else
                        //{
                        //    resp.ResponseMsg = String.Format("Duplicate Role Name");
                        //}
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("Unknown Request");
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                    tran.Rollback();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }


        [HttpPost, Route("api/user/resetpass")]
        public HttpResponseMessage ResetPassword(UserRequest request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    User _item;
                    if (!String.IsNullOrEmpty(request.UserID))
                    {
                        _item = _db.Users.SingleOrDefault(a => a.UserID == request.UserID);
                        var Useremail = _db.Users.FirstOrDefault(a => a.UserID == request.UserID).Email;
                        var UserDisplayName = _db.Users.FirstOrDefault(a => a.UserID == request.UserID).DisplayName;
                        var UserCategoryID = _db.Users.FirstOrDefault(a => a.UserID == request.UserID).UserCategory;
                        string pwd = Alphanumeric.Generate(8);
                        _item.Password = Crypto.Encrypt(pwd);
                        _db.SaveChanges();
                        using (var client = new WebClient())
                        {
                            //Email BODY
                            string html = "";
                            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                            TextInfo textInfo = cultureInfo.TextInfo;
                            string userdisplayname = textInfo.ToTitleCase(UserDisplayName);
                            var receiverEmail = new MailAddress(Useremail, userdisplayname);
                            var senderEmail = new MailAddress(Codes.Constants.Constants.TestMail, "Loan App Support || Email Verification");

                            html = client.DownloadString("C:/Users/user/source/repos/Benardikem/LoanApp/Web/emailtemp/EmptyMail.html");
                            //html = client.DownloadString("https://www.hubcsr.com/emailtemps/Verifyemail.html");

                            //Sending reset password Email
                            var sub = $"welcome mail to { userdisplayname}";
                            //string sFileName = HttpContext.Current.Server.MapPath(@"~/Attachments/Consumer-Loan-Application-Form.pdf");
                            string mailbody = $"<p>Your password has been reset, please find below;</p>{UserCategoryID} ID: {request.UserID}<br/>Password: {pwd}" +
                                $" secure and instant loans have got you covered. Apply for a loan now and get the funds in less than 3 hours." +
                                $" <br><br><br><br>Best Regards,<br>Managing Director, Loendon Resources";

                            StringBuilder sb = new StringBuilder(html);
                            string textBody =
                            sb
                               .Replace("Subject", sub)
                                .Replace("RecieversName", userdisplayname)
                                .Replace("Enter your message here", mailbody)
                              .ToString();

                            var body = textBody;


                            try
                            {
                                EmailItem _mail = new EmailItem
                                {
                                    Title = "Password was reset",
                                    Body = body,
                                    To = new List<string> { receiverEmail.ToString() },
                                };
                                Messaging.LogMail(_mail);
                            }
                            catch { }
                        }
                        resp.ResponseCode = "00";
                        resp.ResponseCode = $"Password reset was successful, an email has been sent to {Useremail}.";
                    }
                }
                catch (Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpPost, Route("api/user/forgotpass")]
        public HttpResponseMessage ForgotPassword(UserRequest request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };

            using (MyDbContext _db = new MyDbContext())
            {
                if (string.IsNullOrEmpty(LoggedInUser.UserName.ToLower()))
                {
                    try
                    {
                        var _item = _db.Users.SingleOrDefault(u => u.UserID == request.UserID && !u.Deleted);
                        if (_item != null)
                        {
                            _item = _db.Users.SingleOrDefault(a => a.UserID == request.UserID);
                            string pwd = Alphanumeric.Generate(8);
                            _item.Password = Crypto.Encrypt(pwd);
                            _db.SaveChanges();
                            try
                            {
                                EmailItem _mail = new EmailItem
                                {
                                    Title = "Your New Login Details",
                                    Body = $"<p>Please find below;</p>Username: {request.UserID}<br/>New Password: {pwd}<p>Best Regards.</p>",
                                    To = new List<string> { _item.Email },
                                };
                                Messaging.LogMail(_mail);
                            }
                            catch { }
                            resp.ResponseCode = "00";
                            resp.ResponseMsg = String.Format("An email has been sent to " + @_item.Email);
                        }
                        else
                        {
                            resp.ResponseMsg = String.Format("User ID Does not exist");
                        }
                    }
                    catch (Exception ex)
                    {
                        resp.ResponseMsg = ex.Message;
                    }
                }
                else
                {
                    resp.ResponseMsg = String.Format("Unknown Error!");
                }
            }

            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpGet, Route("api/user/approve")]
        public HttpResponseMessage ApproveUser(string Id)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    User _user = _db.Users.SingleOrDefault(a => a.UserID == Id);
                    if (_user != null)
                    {
                        _user.Approved = true;
                        _user.ApprovedBy = LoggedInUser.UserName;
                        _user.DateApproved = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();
                        resp.ResponseCode = "00";
                        resp.ResponseMsg = "Admin has been approved successfully";
                    }
                    else
                    {
                        resp.ResponseMsg = "User details not found";
                    }
                }

            }
            catch (System.Exception ex)
            {
                resp.ResponseMsg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }
        [HttpGet, Route("api/user/fetch-role")]
        public HttpResponseMessage FetchRoleDetail(int Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {

                var _item = (from l in _db.Roles
                             where l.ID == Id
                             select new
                             {
                                 l.ID,
                                 l.Name,
                                 l.Description,
                                 l.Active,
                                 l.UserCategoryId,
                                 //UserCategory = l.UserCategory.CategoryName,
                                 Submenus = _db.SubmenuRoles.Where(a => a.RoleId == Id).Select(a => new
                                 {
                                     a.ID,
                                     a.RoleId,
                                     a.SubMenuId
                                 }).ToList()
                             }).SingleOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }
        [HttpPost, Route("api/user/add-role")]
        public HttpResponseMessage AddRole(RoleRequest request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                var tran = _db.Database.BeginTransaction();
                try
                {
                    WRole _item;
                    if (request.Id == 0)
                    {
                        var mCheck = _db.Roles.SingleOrDefault(a => a.Name.ToLower() == request.Name.Trim().ToLower() && !a.Deleted);
                        if (mCheck == null)
                        {
                            _item = _db.Roles.Create();
                            _item.Name = request.Name;
                            _item.Description = request.Description;
                            _item.UserCategoryId = request.UserCategoryId;
                            _item.Active = request.Active;
                            _item.AddedBy = LoggedInUser.UserName;
                            _item.AddedDate = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                            _db.Roles.Add(_item);
                            _db.SaveChanges();
                            foreach (RoleSubmenuRequest _subReq in request.Submenus)
                            {
                                SubMenuRole _sub = _db.SubmenuRoles.Create();
                                _sub.RoleId = _item.ID;
                                _sub.SubMenuId = _subReq.SubMenuId;
                                _db.SubmenuRoles.Add(_sub);
                            }
                            _db.SaveChanges();
                            tran.Commit();
                            resp.ResponseCode = "00";
                            resp.ResponseMsg = "Role created successfuly";
                        }
                        else
                        {
                            resp.ResponseMsg = String.Format("Duplicate Role Name");
                        }
                    }
                    else
                    {
                        var mCheck = _db.Roles.SingleOrDefault(a => a.Name.ToLower() == request.Name.Trim().ToLower() && a.ID != request.Id && !a.Deleted);
                        if (mCheck == null)
                        {
                            _item = _db.Roles.SingleOrDefault(a => a.ID == request.Id);
                            _item.Name = request.Name;
                            _item.Description = request.Description;
                            _item.UserCategoryId = request.UserCategoryId;
                            _item.Active = request.Active;
                            _item.ModifiedBy = LoggedInUser.UserName;
                            _item.ModifiedDate = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                            _item.Approved = false;
                            //Delete all Privs
                            var privs = _db.SubmenuRoles.Where(s => s.RoleId == _item.ID);
                            if (privs != null && privs.Count() > 0)
                                _db.SubmenuRoles.RemoveRange(privs);
                            foreach (RoleSubmenuRequest _subReq in request.Submenus)
                            {
                                SubMenuRole _sub = _db.SubmenuRoles.Create();
                                _sub.RoleId = _item.ID;
                                _sub.SubMenuId = _subReq.SubMenuId;
                                _db.SubmenuRoles.Add(_sub);
                            }
                            _db.SaveChanges();
                            tran.Commit();
                            resp.ResponseCode = "00";
                            resp.ResponseMsg = "Role updated successfuly";
                        }
                        else
                        {
                            resp.ResponseMsg = String.Format("Duplicate Role Name");
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                    tran.Rollback();
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }
        [HttpGet, Route("api/user/approve-role")]
        public HttpResponseMessage ApproveRole(long Id)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    WRole _role = _db.Roles.SingleOrDefault(a => a.ID == Id);
                    if (_role != null)
                    {
                        _role.Approved = true;
                        _role.ApprovedBy = LoggedInUser.UserName;
                        _role.ApprovedDate = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();
                        resp.ResponseCode = "00";
                        resp.ResponseMsg = "Role Approved Successfully";
                    }
                    else
                    {
                        resp.ResponseMsg = "Role details not found";
                    }
                }

            }
            catch (System.Exception ex)
            {
                resp.ResponseMsg = ex.Message;
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

    }

    public class UserRoleRequest
    {
        public long RoleId { get; set; }
    }

    public class UserRequest
    {
        public bool Active { get; set; }
        //public string Id { get; set; }
        public String UserID { get; set; }
        public string Mobile { get; set; }
        public string Name { get; set; }
        public int UserCategoryId { get; set; }
        public IEnumerable<UserRoleRequest> Roles { get; set; }
        public string Mode { get; set; }
        public string EmailAddress { get; set; }
        public bool DividendPriviledge { get; set; }
        public String UserCategory { get; set; }
        public string BVN { get; set; }
        public string Guarantor { get; set; }
        public string GuarantorPhone { get; set; }
        public string G_Address { get; set; }
        public string R_Address { get; set; }
        public string C_Address { get; set; }
    }

    public class RoleSubmenuRequest
    {
        public long SubMenuId { get; set; }
    }

    public class RoleRequest
    {
        public bool Active { get; set; }
        public string Description { get; set; }
        public int Id { get; set; }
        public string Name { get; set; }
        public IEnumerable<RoleSubmenuRequest> Submenus { get; set; }
        public int UserCategoryId { get; set; }
    }
}
