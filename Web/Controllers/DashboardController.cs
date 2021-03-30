using Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Web.Application;
using API.Models;
using API.Utilities;
using API;
using Web.Attributes;
using Web.Extensions;
using Web.Codes.Constants;
using System.Collections.Generic;
using Web.Interfaces;
using Web.Codes;

namespace Web.Controllers
{
    public class DashboardController : Controller
    {
        private IAppManager _appMgr;
        public DashboardController()
        {
            _appMgr = new AppManager();
        }
        [Authorize]
        public ActionResult Index()
        {
            LoanModel model = new LoanModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
            {
                var UserCat = LoggedInUser.UserCategory;

                //ADMIN
                if (UserCat == UserCategory.ADMIN)
                {
                    model.LoanApplicationItems = _db.LoanApplications//.Where(a => a.UserID == LoggedInUser.UserId)
                                                                  .Select(a => new LoanApplicationItem
                                                                  {
                                                                      ID = a.ID,
                                                                      CustomerID = a.UserID,
                                                                      CustomerName = a.User.DisplayName,
                                                                      ApprovedBy = a.ApprovedBy,
                                                                      ApprovedDate = a.ApprovedDate,
                                                                      CreatedDate = a.CreatedDate,
                                                                      LoanAmount = a.LoanAmount,
                                                                      LoanApplicationNumber = a.LoanApplicationNumber,
                                                                      CreatedBy = a.CreatedBy,
                                                                      LoanStatus = a.LoanStatus,
                                                                      Repaid = a.Repaid ? "Yes" : "No",
                                                                      BalanceRecieved = a.BalanceRecieved,
                                                                      BalanceRemaining = a.BalanceRemaining,
                                                                      Comment = a.Comment,
                                                                      MonthYear = a.MonthYear,
                                                                      NextRepayMonth = a.LastLoanMonth.LastMonthPaidFor,
                                                                  })
                                                                  .OrderBy(a => a.ID)
                                                                  .ToList();


                    model.LoanRepaymentItems = _db.LoanRepayments//.Where(a => a.UserID == LoggedInUser.UserId)
                                                .Select(a => new LoanRepaymentItem
                                                {
                                                    ID = a.ID,
                                                    CustomerID = a.UserID,
                                                    CustomerName = a.User.DisplayName,
                                                    AmountPaid = a.AmountPaid,
                                                    ApprovedBy = a.ApprovedBy,
                                                    ApprovedDate = a.ApprovedDate,
                                                    LoanApplicationNumber = a.LoanApplicationNumber,
                                                    CreatedDate = a.CreatedDate,
                                                    BalanceAfterP = a.BalanceAfterP,
                                                    CreatedBy = a.CreatedBy,
                                                    BalanceBeforeP = a.BalanceBeforeP,
                                                    MonthYear = a.MonthYear,
                                                    Comment = a.Comment
                                                })
                                                .OrderBy(a => a.ID)
                                                .ToList();

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
                                                    BVN_Number = a.BVN_Number,
                                                    Roles = a.UserRoles.Select(r => r.Role.Name).ToList()
                                                })
                                                .OrderBy(a => a.DisplayName)
                                                .ToList();
                }

                //CUSTOMER
                if (UserCat == UserCategory.USER)
                {
                    model.LoanApplicationItems = _db.LoanApplications.Where(a => a.UserID == LoggedInUser.UserId)
                                                                   .Select(a => new LoanApplicationItem
                                                                   {
                                                                       ID = a.ID,
                                                                       CustomerID = a.UserID,
                                                                       CustomerName = a.User.DisplayName,
                                                                       ApprovedBy = a.ApprovedBy,
                                                                       ApprovedDate = a.ApprovedDate,
                                                                       CreatedDate = a.CreatedDate,
                                                                       LoanAmount = a.LoanAmount,
                                                                       LoanApplicationNumber = a.LoanApplicationNumber,
                                                                       CreatedBy = a.CreatedBy,
                                                                       LoanStatus = a.LoanStatus,
                                                                       Repaid = a.Repaid ? "Yes" : "No",
                                                                       BalanceRecieved = a.BalanceRecieved,
                                                                       BalanceRemaining = a.BalanceRemaining,
                                                                       Comment = a.Comment,
                                                                       MonthYear = a.MonthYear,
                                                                       NextRepayMonth = a.LastLoanMonth.LastMonthPaidFor,
                                                                   })
                                                                   .OrderBy(a => a.ID)
                                                                   .ToList();


                    model.LoanRepaymentItems = _db.LoanRepayments.Where(a => a.UserID == LoggedInUser.UserId)
                                                .Select(a => new LoanRepaymentItem
                                                {
                                                    ID = a.ID,
                                                    CustomerID = a.UserID,
                                                    CustomerName = a.User.DisplayName,
                                                    AmountPaid = a.AmountPaid,
                                                    ApprovedBy = a.ApprovedBy,
                                                    LoanApplicationNumber = a.LoanApplicationNumber,
                                                    ApprovedDate = a.ApprovedDate,
                                                    CreatedDate = a.CreatedDate,
                                                    BalanceAfterP = a.BalanceAfterP,
                                                    CreatedBy = a.CreatedBy,
                                                    BalanceBeforeP = a.BalanceBeforeP,
                                                    MonthYear = a.MonthYear,
                                                    Comment = a.Comment
                                                })
                                                .OrderBy(a => a.ID)
                                                .ToList();
                }
            }
            }
            catch (System.Exception ex)
            {
                General.LOGGER.Error(ex.Source, ex);
                ViewBag.MessageText = ex.Message.ToString();
            }
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult TestEmail()
        {
            EmailItem _mail = new EmailItem
            {
                Title = "Test Email",
                Body = "We are testing",
                To = new List<string> { "wintopeo@windraysystems.com", "dwintope@yahoo.com" },
            };
            Messaging.LogMail(_mail);
            return View();
        }

        [HttpGet]
        public ActionResult ResetDemo()
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {

                    string query2 = "Truncate table LoanApplications";
                    _db.Database.ExecuteSqlCommand(query2, "");

                    string query1 = "Truncate table CheckAppStatus";
                    _db.Database.ExecuteSqlCommand(query1, "");

                    string query3 = "Truncate table LoanRepayments";
                    _db.Database.ExecuteSqlCommand(query3, "");

                    //User _user;
                    //_user = _db.Users.FirstOrDefault(m => m.UserID == "AD_admin@mfi.com");
                    //_user.Password = Crypto.Encrypt("PAT10$$$$$");
                    //_db.SaveChanges();

                    //string query = "CREATE TABLE [dbo].[MMLoanHistory]([ID] [bigint] IDENTITY(1,1) NOT NULL,[UserID] [varchar](1000) NULL,[LastName] [varchar](1000) NULL,[FirstName] [varchar](1000) NULL,[OtherNames] [varchar](1000) NULL,[Address] [varchar](1000) NULL,	[City] [varchar](1000) NULL,[LoanApplicationNumber] [varchar](50) NULL,[Postcode] [varchar](1000) NULL,[Email] [varchar](1000) NULL,[DOb] [varchar](1000) NULL,[NationalInsuranceNnumber] [varchar](1000) NULL,[PhoneNumber] [varchar](1000) NULL,[EmploymentStatus] [varchar](1000) NULL,[EmployerName] [varchar](1000) NULL,[EmploymentNature] [varchar](1000) NULL,[EmployerAddress] [varchar](1000) NULL,[EmployerPhone] [varchar](1000) NULL,[CanYouBeContactedHere] [varchar](1000) NULL,[YearsWithCurrentEmployer] [varchar](1000) NULL,[DateEmployed] [varchar](1000) NULL,[DetailsOfPreviousEmployment] [varchar](1000) NULL,[Housing] [varchar](1000) NULL,[HousingAddressinPast3years] [varchar](1000) NULL,[PurposeOfLoan] [varchar](1000) NULL,[AmountRequested] [varchar](1000) NULL,[Salary] [varchar](1000) NULL,[RepaymentPeriod] [varchar](1000) NULL,[NameOfBank] [varchar](1000) NULL,[AccountName] [varchar](1000) NULL,[AccountNumber] [varchar](50) NULL,[SortCode] [varchar](1000) NULL,[BankAddress] [varchar](1000) NULL,[LoanStatus] [varchar](50) NULL) ON [PRIMARY]";
                    //_db.Database.ExecuteSqlCommand(query, "");

                    //string query = "INSERT [dbo].[SubMenus] ([Name], [MenuID], [Order], [Class], [Link], [Active], [Parent]) VALUES (N'Withdrawal Requests', 7, 8, NULL, N'/Admin/WithdrawalRequests', 1, 0)";
                    //_db.Database.ExecuteSqlCommand(query, "");

                    //IEnumerable<MMLoanApplication> Usr = _db.MMLoanApplications.Where(x => x.UserID == "MM_mygolden201@gmail.com").ToList();
                    //_db.MMLoanApplications.RemoveRange(Usr);
                    //_db.SaveChanges();

                    resp.ResponseCode = "00";
                    resp.ResponseMsg = "Demo Database was RESET successfully";

                }
                catch (Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Json(resp, JsonRequestBehavior.AllowGet);
        }


        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel { Username = LoggedInUser.UserName };
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            model.Message = "";
            if (ModelState.IsValid)
            {
                var response = await _appMgr.ChangePasswordAsync(model.Username, model.OldPassword, model.NewPassword);
                if (response.ResponseCode == "00")
                {
                    model.Message = "Password Change Successful";
                }
                else
                {
                    ModelState.AddModelError("", response.ResponseMsg);
                }
            }
            return View(model);
        }


        [AllowAnonymous]
        public ActionResult ManageProfile()
        {
            UserModel model = new UserModel();
            try
            {
                using (MyDbContext _db = new MyDbContext())
                {
                    model.UserItems = _db.Users.Where(a => a.UserID == LoggedInUser.Email)
                                       .Select(a => new UserItem
                                       {
                                           UserID = a.UserID,
                                           DisplayName = a.DisplayName,
                                           Address = a.R_Address,
                                           Phone = a.Phone,
                                           Email = a.Email,
                                           DateAdded = a.DateAdded,
                                           UserCategory = a.UserCategory,
                                           ImageURL = a.ImageURL,
                                       })
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