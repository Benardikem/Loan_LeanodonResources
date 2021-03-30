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
using System.Web;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.IO;
using System.Collections.Generic;

namespace Web.Controllers
{
    [Authorize]
    public class LoanManagerController : Controller
    {
        [CustomAuthorizeAttribute(OperationKey = "loanmanager/index")]
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

        [CustomAuthorizeAttribute(OperationKey = "loanmanager/history")]
        public ActionResult History()
        {
            LoanModel model = new LoanModel();
            //try
            //{
                using (MyDbContext _db = new MyDbContext())
                {
                    var UserCat = LoggedInUser.UserCategory;

                    //ADMIN
                    if (UserCat == UserCategory.ADMIN)
                    {
                        model.LoanApplicationItems = _db.LoanApplications
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
                                                                         //AppStatus = a.AppStatus.Status,
                                                                         DisplayName = a.User.DisplayName,
                                                                         PhoneNumber = a.User.Phone,
                                                                         MaturityDate = a.MaturityDate,
                                                                         LoanSpread = a.LoanSpread,
                                                                         InvoiceURL = a.InvoiceURL,
                                                                     })
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
                                                .ToList();
                }

                    //CUSTOMER
                    if (UserCat == UserCategory.USER)
                    {
                        model.LoanApplicationItems = _db.LoanApplications.Where(a => a.UserID == LoggedInUser.UserId)
                                                                       .Select(a => new LoanApplicationItem
                                                                       {
                                                                           //ID = a.ID,
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
                                                                           AppStatus = a.AppStatus.Status,
                                                                       })
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
                                                    .ToList();
                    }
                }
        //}
        //    catch (System.Exception ex)
        //    {
        //        General.LogExceptions("", ex);
        //        ViewBag.MessageText = ex.Message.ToString();
        //        ViewBag.MessageType = MessageLabelControl.MessageType.danger;
        //    }
            return View(model);
        }

        [CustomAuthorizeAttribute(OperationKey = "loanmanager/repay")]
        public ActionResult Repay()
        {
            return View();
        }

        [CustomAuthorizeAttribute(OperationKey = "loanmanager/repaymenthistory")]
        public ActionResult RepaymentHistory()
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
                                                        Comment = a.Comment,
                                                        RepaymentStatus = a.RepaymentStatus,
                                                        LoanAmount = a.LoanAmount,
                                                        DisplayName = a.User.DisplayName,
                                                        InvoiceURL = a.RepaymentInvoiceURL,
                                                    })
                                                    .OrderBy(a => a.ID)
                                                    .ToList();
                    }

                    //CUSTOMER
                    if (UserCat == UserCategory.USER)
                    {
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
                                                        Comment = a.Comment,
                                                        RepaymentStatus = a.RepaymentStatus,
                                                        LoanAmount = a.LoanAmount
                                                    })
                                                    .OrderBy(a => a.ID)
                                                    .ToList();
                    }
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

        #region API
        //Download Invoices
        public FileResult Download(string ImageName)
        {
            var FileVirtualPath = "~/Invoices/" + ImageName;
            return File(FileVirtualPath, "application/force-download", Path.GetFileName(FileVirtualPath));
        }



        #endregion

    }

    public class LoanRequests
    {
        public Int64 ID { get; set; }
        public string UserID { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal LoanBalance { get; set; }
        public string LoanStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string RejectionReason { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string Comment { get; set; }
        public string LoanApplicationNumber { get; set; }
    }
}