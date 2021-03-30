using API.Models;
using API.Utilities;
using Web.Application;
using Web.Codes.Constants;
using Web.Extensions;
using Web.Interfaces;
using Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Globalization;
using System.Net.Mail;
using System.Threading;
using API;
using System.Web;

namespace Web.Codes
{
    public class AppManager : IAppManager
    {
        public ActionResponse IsLogin(string strUser, string strPwd)
        {
            ActionResponse result = new ActionResponse { ResponseCode = "96", ResponseMsg = "Invalid Login Details" };
            using (MyDbContext _db = new MyDbContext())
            {
                string pwd = Crypto.Encrypt(strPwd);
                User user = _db.Users.FirstOrDefault(a => a.UserID == strUser && a.Approved);
                if (user != null)
                {
                    string _hashed = Crypto.Encrypt(strPwd);
                    if (user.Password == _hashed)
                    {
                        if (user.Active)
                        {
                            result.ResponseCode = "00";
                            result.ResponseMsg = "Successful";
                        }
                        else
                        {
                            result.ResponseMsg = String.Format("Your account has not been activated. Please contact the Administrator on {0}", API.Settings.Site.Phone);
                        }
                    }
                    else
                    {
                        result.ResponseMsg = "Invalid Username or Password";
                    }
                }
            }
            return result;
        }
        public async Task<ActionResponse> ChangePasswordAsync(string username, string oldPassword, string newPassword)
        {
            ActionResponse responseModel = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (var _db = new MyDbContext())
            {
                var existingUser = _db.Users.FirstOrDefault(u => u.UserID == username);
                if (existingUser == null)
                {
                    responseModel.ResponseMsg = "User does not exist";
                    return responseModel;
                }
                string _hashed = Crypto.Encrypt(oldPassword);
                if (!existingUser.Password.Equals(_hashed))
                {
                    responseModel.ResponseMsg = "Invalid old password.";
                    return responseModel;
                }
                //Hash password
                _hashed = Crypto.Encrypt(newPassword);
                existingUser.Password = _hashed;
                existingUser.ModifiedBy = username;
                existingUser.DateModified = Date.GetDateTimeByTimeZone(DateTime.Now);

                //Update Activity
                ActivityLog _log = _db.ActivityLogs.Create();
                _log.Title = "Password Changed";
                _log.Details = $"You successfully changed your password";
                _log.UserName = LoggedInUser.UserName;
                _log.ActivityType = ApprovalStatus.APPROVED;
                _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                _db.ActivityLogs.Add(_log);
                await _db.SaveChangesAsync();

                responseModel.ResponseCode = "00";
                responseModel.ResponseMsg = "Successful";
            }
            return responseModel;
        }

        //public List<AccountItem> GetAllAccounts(long BankId)
        //{
        //    List<AccountItem> accts = new List<AccountItem>();
        //    using (var _db = new MyDbContext())
        //    {
        //        DateTime dte = DateTime.Today;
        //        try
        //        {
        //            accts = _db.Accounts.Where(a => a.Active && a.BankId == BankId)
        //                .Select(a => new AccountItem { ID = a.ID, AccountName = a.AccountName, AccountNo = a.AccountNo }).ToList();
        //        }
        //        catch (Exception ex)
        //        {
        //            General.LOGGER.Error(ex.Source, ex);
        //        }
        //    }
        //    return accts;
        //}

        public IEnumerable<UserItem> FetchUsersWithRight(string _OperationKey)
        {
            List<UserItem> users = new List<UserItem>();
            using (MyDbContext _db = new MyDbContext())
            {
                string _link = _OperationKey;
                if (!_link.StartsWith("/"))
                    _link = "/" + _link;
                //                select u.*from[SubMenus] a
                //join SubMenu_In_Role b on a.ID = b.SubMenuID
                //join User_In_Role c on c.RoleID = b.RoleID
                //join users u on u.UserID = c.UserID
                //where a.Link = '/returns/AuthorizationLog'
                users = (from _s in _db.SubMenus
                         join _sr in _db.SubmenuRoles on _s.ID equals _sr.SubMenuId
                         join _ur in _db.UserRoles on _sr.RoleId equals _ur.RoleId
                         where _s.Link == _link && _ur.User.Active
                         select new UserItem
                         {
                             DisplayName = _ur.User.DisplayName,
                             EmailAddress = _ur.User.Email
                         }).ToList();
            }
            return users;
        }

        public ActionResponse IsSignUp(string Displayname, string Email, string Phone, string ResidentialAddress, string ZipCode, string Pswd, string SecurityQ, string SecurityA)
        {
            ActionResponse result = new ActionResponse { ResponseCode = "96", ResponseMsg = "Unknown Error" };
            using (MyDbContext _db = new MyDbContext())
            {
                //Checking if Email already exists 
                var mCheck = _db.Users.SingleOrDefault(a => a.UserID == Email);
                if (mCheck == null)
                {
                    //checking the lenght of password and password
                    string str = Phone;
                    if (str.Length > 0 && str.Length >= 9)
                    {
                        string strpass = Pswd;
                        if (strpass.Length > 0 && strpass.Length >= 9)
                        {
                            //checking if password contains numbers
                            string Phonedigits = Phone;
                            bool isAllDigits = !Phonedigits.Any(ch => ch < '0' || ch > '9');
                            if (isAllDigits == true)
                            {
                                //store record to DB
                                User _item;
                                User user = _db.Users.FirstOrDefault(a => a.Email == Email);
                                if (user == null)
                                {

                                    User PhoneNumber = _db.Users.FirstOrDefault(a => a.Phone == Phone);
                                    if (PhoneNumber == null)
                                        using (var client = new WebClient())
                                        {

                                            string html = "";
                                            CultureInfo cultureInfo = Thread.CurrentThread.CurrentCulture;
                                            TextInfo textInfo = cultureInfo.TextInfo;
                                            string userdisplayname = textInfo.ToTitleCase(Displayname);
                                            var receiverEmail = new MailAddress(Email, userdisplayname);
                                            var senderEmail = new MailAddress(Constants.Constants.TestMail, "Loan App Support || Email Verification");

                                            html = client.DownloadString("C:/Users/user/source/repos/Benardikem/LoanApp/Web/emailtemp/EmptyMail.html");
                                            //html = client.DownloadString("https://www.hubcsr.com/emailtemps/Verifyemail.html");

                                            //Sending Verify Email
                                            var sub = $"welcome mail to { userdisplayname}";
                                            //string sFileName = HttpContext.Current.Server.MapPath(@"~/Attachments/Consumer-Loan-Application-Form.pdf");
                                            string mailbody = $"I want to use this medium to welcome you to our loan application portal,we shall be communicating with you through SMS and Emails." +
                                                $" so please make out time to look out for our messages. just incase you did not get a welcome SMS right now. it could be that you have DND activated " +
                                                $"on your Number. we need you to please deactivate DND by texting (as SMS) the word “STOP” to the short code 2442 and then text “ALLOW” to the" +
                                                $" same 2442.<br><br> Just so you know, We have been in business for 15 years now, we are here for persons who have an urgent need for cash to settle bills, " +
                                                $"take care of emergencies or grab" +
                                                $" an opportunity? Does payday seem so far and bills are piling up? Don’t worry! Our convenient," +
                                                $" secure and instant loans have got you covered. Apply for a loan now and get the funds in less than 3 hours. <br><br><br><br>Regards,<br>Managing Director, Loendon Resources";

                                            StringBuilder sb = new StringBuilder(html);
                                            string textBody =
                                            sb
                                               .Replace("Subject", sub)
                                                .Replace("RecieversName", userdisplayname)
                                                .Replace("Enter your message here", mailbody)
                                              .ToString();

                                            var body = textBody;
                                            {
                                                try
                                                {
                                                    EmailItem _mail = new EmailItem
                                                    {
                                                        Title = sub,
                                                        Body = body,
                                                        To = new List<string> { Email.ToString() },
                                                        //Attachments = new List<string> { sFileName.ToString() },
                                                    };
                                                    Messaging.LogMail(_mail);

                                                    _item = _db.Users.Create();
                                                    _item.DisplayName = Displayname;
                                                    _item.Email = Email;
                                                    _item.UserID = Email;
                                                    _item.Phone = Phone;
                                                    _item.UserCategory = UserCategory.USER;
                                                    _item.DateAdded = API.Utilities.Date.GetDateTimeByTimeZone(DateTime.Now);
                                                    _item.Password = Crypto.Encrypt(Pswd);
                                                    _item.Approved = true;
                                                    _item.Active = true;
                                                    _item.ChangePassword = true;
                                                    _item.UserCategory = UserCategory.USER;
                                                    _db.Users.Add(_item);
                                                    _db.SaveChanges();
                                                }
                                                catch (SmtpFailedRecipientException ex)
                                                {
                                                    result.ResponseMsg = ex.Message;
                                                }
                                            }

                                            result.ResponseCode = "00";
                                            result.ResponseMsg = $"Welcome {userdisplayname}, Your Registration was Successful.";
                                        }
                                    else
                                    {
                                        result.ResponseMsg = $"Phone number : {Phone} already exists in our database, try with another number.";
                                    }
                                }
                                else
                                {
                                    result.ResponseMsg = $"Email : {Email} already exists in our database, try with another email.";
                                }
                            }
                            else
                            {
                                result.ResponseMsg = $"Phone number : {Phone} has to be numbers only.";
                            }
                        }
                        else
                        {
                            result.ResponseMsg = $"Password too short; has to be minimum of 9 characters.";
                        }
                    }
                    else
                    {
                        result.ResponseMsg = "Phone number too short; has to be minimum of 9 characters.";
                    }
                }
                else
                {
                    result.ResponseMsg = $"Email: {Email} already exists in our database";
                }
            }
            return result;
        }

    }
}
