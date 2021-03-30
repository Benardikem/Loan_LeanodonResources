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
    public class LoanController : ApiController
    {

        // function to get the full month name  
        static string getFullName(int month)
        {
            return CultureInfo.CurrentCulture.
                DateTimeFormat.GetMonthName
                (month);
        }

        //create
        [HttpPost, Route("api/loan/create")]
        public HttpResponseMessage Create(LoanApplicationItem request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    CheckAppStatus _ChckAppStatus;
                    LoanApplication _item;
                    Invoice _inv;
                    LastLoanMonthRepaid _MonthRepay;
                    int intNextMontToPay = request.Month + 1;
                    string NextMontToPay = getFullName(intNextMontToPay);
                    string FullMonthName = getFullName(request.Month);

                    DateTime dt = Date.GetDateTimeByTimeZone(DateTime.Now);
                    var Matdate = dt.AddMonths(Convert.ToInt32(request.MatDate));

                    //check if user can apply for more loans
                    _ChckAppStatus = _db.CheckAppStats.FirstOrDefault(m => m.UserID == LoggedInUser.UserId && m.Status == false);
                    if (_ChckAppStatus == null)
                    {
                        var APPno = "LRL20" + Alphanumeric.Generate(6);
                        //var InvoiceHtmlURL = $"{InsertInvoiceHtmltoLocalFolder.HtmlFilePath_Local}{APPno}.html";
                        _item = _db.LoanApplications.Create();
                        _item.UserID = request.UserID;
                        _item.LoanAmount = request.LoanAmount;
                        _item.MaturityDate = Matdate;
                        _item.LoanSpread = request.LoanSpread;
                        int LoanAmount = Convert.ToInt32(request.LoanAmount);
                        int TenPercentVal = 10 * LoanAmount / 100;
                        _item.BalanceRemaining = TenPercentVal + LoanAmount;
                        _item.MonthYear = FullMonthName + ' ' + request.Year;
                        _item.LoanStatus = ApprovalStatus.PENDING;
                        _item.InvoiceURL = $"{APPno}.html";
                        _item.Repaid = false;
                        _item.LoanApplicationNumber = APPno;
                        _item.CreatedBy = LoggedInUser.UserName;
                        _item.CreatedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.LoanApplications.Add(_item);
                        _db.SaveChanges();




                        //CREATING INVOICE ENTRY
                        var total = _db.Invoices.Count(x => x.CustomerName != null);
                        var invnumber = "";
                        if (total < 1000)
                        {
                            invnumber = "00000" + total;
                        }
                        if (total < 10000)
                        {
                            invnumber = "0000" + total;
                        }
                        _inv = _db.Invoices.Create();
                        _inv.InvoiceURL = $"{APPno}.html";
                        _inv.InvoiceNumber = invnumber;
                        _inv.CustomerName = request.CustomerName;
                        _inv.CustomerID = request.UserID;
                        _inv.CreatedBy = LoggedInUser.DisplayName;
                        _inv.InvoiceDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.Invoices.Add(_inv);
                        _db.SaveChanges();


                        //CREATING EMAIL CONTENT
                        string html = "";
                        var client = new WebClient();

                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.ApplyLoan)
                           .Replace("RecieversName", request.CustomerName)
                            .Replace("Enter your message here", $"Hi {request.CustomerName}, <br><br>We wish to inform you that your Loan Application was successful.<br><br>" +
                            $" <b>Loan Application Number :</b> {APPno} <br><br>Regards.<br><br><br><br><br>,We also wish to use this medium to inform you that you can make" +
                            $"  repayments of your loan at your convenience any time through any of our payment channels.<br><br><br> As we also urge you to login to your customer backoffice To preview the invoice for this transaction</a>")
                          .ToString();

                        var Emailbody = textBody;


                        //CREATING INVOICE CONTENT
                        string html1 = "";
                        html1 = client.DownloadString(InvoiceDownloadString.LocalFilePath);
                        var getcustomeraddress = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).R_Address;
                        var getcustomerphone = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).Phone;
                        var getcustomeremail = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).Email;
                        var formattedLoanAmt = @String.Format("{0:#,##0}", request.LoanAmount);
                        var formattedTenPercentVal = @String.Format("{0:#,##0}", TenPercentVal);
                        var formattednextpaymentAmount = @String.Format("{0:#,##0}", TenPercentVal + LoanAmount);

                        StringBuilder sb1 = new StringBuilder(html1);
                        string textBody1 =
                        sb1
                          .Replace("invoicenumber", invnumber)
                          .Replace("DateNow", @DateTime.Now.ToString("MM/dd/yyyy"))
                          .Replace("customername", request.CustomerName)
                          .Replace("InvoiceFor", $"Invoice generated for {request.CustomerName} on {@DateTime.Now.ToString("MM/dd/yyyy")}")
                          .Replace("customeraddress", getcustomeraddress)
                          .Replace("customerphone", getcustomerphone)
                          .Replace("customeremail", getcustomeremail)
                          .Replace("#ApplicationNumber", APPno)
                          .Replace("nextpaymentdue", $"{NextMontToPay} {request.Year}")
                          .Replace("customerID", request.UserID)
                          .Replace("loanamount", $"{formattedLoanAmt}")
                          .Replace("InterestonLoan", $"{formattedTenPercentVal}")
                          .Replace("nextpaymentAmount", $"{formattednextpaymentAmount}")
                          .ToString();

                        var Invoicebody = textBody1;
                        //inserting invoice to folder
                        System.IO.File.WriteAllText(InsertInvoiceHtmltoLocalFolder.HtmlFilePath_Local + APPno + ".html", Invoicebody);

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = LoanTitles.LoanApply;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = LoanLogStatus.ApplyLoan;
                        _log.Details = $"{LoanLogDetails.ApplyLoan} for {request.CustomerName} ({request.CustomerID}) with loan Application Number : {APPno}";
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();

                        //UPDATING CHECK APP STATUS TABLE
                        //CHECK IF RECORD EXISTS
                        _ChckAppStatus = _db.CheckAppStats.SingleOrDefault(a => a.UserID == request.UserID);
                        if (_ChckAppStatus == null)
                        {
                            //INSERT/CREATE
                            _ChckAppStatus = _db.CheckAppStats.Create();
                            _ChckAppStatus.UserID = request.UserID;
                            _ChckAppStatus.Status = false;
                            _db.CheckAppStats.Add(_ChckAppStatus);
                            _db.SaveChanges();
                        }
                        else
                        {
                            //UPDATE EXISTING RECORD
                            _ChckAppStatus = _db.CheckAppStats.SingleOrDefault(a => a.UserID == request.UserID);
                            _ChckAppStatus.Status = false;
                            _db.SaveChanges();
                        }


                        //CREATE LOAN DUE DATE
                        _MonthRepay = _db.LastLoanMonth.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        if (_MonthRepay == null)
                        {
                            //INSERT/CREATE
                            _MonthRepay = _db.LastLoanMonth.Create();
                            _MonthRepay.UserID = request.UserID;
                            _MonthRepay.LoanApplicationNumber = APPno;
                            _MonthRepay.LastMonthPaidFor = NextMontToPay + ' ' + request.Year;
                            _db.LastLoanMonth.Add(_MonthRepay);
                            _db.SaveChanges();
                        }
                        else
                        {
                            //UPDATE EXISTING RECORD
                            _MonthRepay = _db.LastLoanMonth.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                            _MonthRepay.LastMonthPaidFor = NextMontToPay + ' ' + request.Year;
                            _db.SaveChanges();
                        }

                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = LoanLogStatus.ApplyLoan,
                                Body = Emailbody,
                                To = new List<string> { getcustomeremail },
                                //Attachments = new List<string> { sFileName.ToString() },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }

                        //_db.SaveChanges();
                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Application created, an Invoice has been sent to {getcustomeremail}";
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("Sorry you cannot apply for a loan at this time, contact the admin");
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpPost, Route("api/loan/NewRepayLoan")]
        public HttpResponseMessage NewRepayLoan(LoanApplicationItem request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {

                    if (request.AmountPaid < request.BalanceRemaining + 1)
                    {
                        LoanApplication _item;
                        Invoice _inv;
                        LoanRepayment _itemRepay;
                        LastLoanMonthRepaid _MonthRepay;
                        int intNextMontToPay = request.Month + 1;
                        string NextMontToPay = getFullName(intNextMontToPay);

                        decimal BalanceRecieved = _db.LoanApplications.FirstOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber).BalanceRecieved;
                        decimal? BalanceRemaining = _db.LoanApplications.FirstOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber).BalanceRemaining;

                        //int LoanAmount = Convert.ToInt32(request.LoanAmount);
                        int AmountPaid = Convert.ToInt32(request.AmountPaid);

                        //old balance minus amount repaid
                        decimal? BalanceAfterPayment = BalanceRemaining - AmountPaid;

                        //Calculating the new balance Recieved
                        decimal NewbalanceRecieved = BalanceRecieved + AmountPaid;

                        //to get ballance reamining
                        decimal? TenPercentVal = 10 * BalanceAfterPayment / 100;

                        //add it to the balance after payment
                        decimal? NewBalanceafterpayment = BalanceAfterPayment + TenPercentVal;

                        //UPDATE LOAN APPLICATION
                        _item = _db.LoanApplications.FirstOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);                       
                        _item.BalanceRecieved = (decimal)NewbalanceRecieved;
                        _item.BalanceRemaining = NewBalanceafterpayment;
                        if (BalanceAfterPayment < 0)
                        {
                            _item.Repaid = false;
                        }
                        _item.ApprovedBy = LoggedInUser.UserName;
                        _item.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();

                        //CREATING INVOICE ENTRY
                        var total = _db.Invoices.Count(x => x.CustomerName != null);
                        var invnumber = "";
                        if (total < 1000)
                        {
                            invnumber = "00000" + total;
                        }
                        if (total < 10000)
                        {
                            invnumber = "0000" + total;
                        }
                        _inv = _db.Invoices.Create();
                        _inv.InvoiceURL = $"{request.LoanApplicationNumber}RL.html";
                        _inv.InvoiceNumber = invnumber+"RL";
                        _inv.CustomerName = request.CustomerName;
                        _inv.CustomerID = request.UserID;
                        _inv.CreatedBy = LoggedInUser.DisplayName;
                        _inv.InvoiceDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.Invoices.Add(_inv);
                        _db.SaveChanges();

                        //CREATING INVOICE CONTENT
                        string html1 = "";
                        var client = new WebClient();
                        html1 = client.DownloadString(ReparLoanInvoiceDownloadString.LocalFilePath);
                        var getcustomeraddress = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).R_Address;
                        var getcustomerphone = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).Phone;
                        var getcustomeremail = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).Email;
                        var formattedLoanAmt = @String.Format("{0:#,##0}", request.LoanAmount);
                        var formattedAmtPaid = @String.Format("{0:#,##0}", request.AmountPaid);
                        var formattedB_Before = @String.Format("{0:#,##0}", (decimal)BalanceRemaining);
                        var formattedB_After = @String.Format("{0:#,##0}", (decimal)NewBalanceafterpayment);

                        StringBuilder sb1 = new StringBuilder(html1);
                        string textBody1 =
                        sb1
                          .Replace("InvoiceFor", $"generated for {request.CustomerName} on {@DateTime.Now.ToString("MM/dd/yyyy")}")
                          .Replace("invoicenumber", invnumber)
                          .Replace("DateNow", @DateTime.Now.ToString("MM/dd/yyyy"))
                          .Replace("customername", request.CustomerName)
                          .Replace("customeraddress", getcustomeraddress)
                          .Replace("customerphone", getcustomerphone)
                          .Replace("customeremail", getcustomeremail)
                          .Replace("#ApplicationNumber", request.LoanApplicationNumber)
                          .Replace("nextpaymentdue", $"{NextMontToPay} {request.Year}")
                          .Replace("customerID", request.UserID)
                          .Replace("LoanAmt", $"{formattedLoanAmt}")
                          .Replace("AmtPaid", $"{formattedAmtPaid}")
                          .Replace("B.Before", $"{formattedB_Before}")
                          .Replace("B_After", $"{formattedB_After}")
                          .ToString();


                        //INSERTING INVOICE TO LOCAL FOLDER
                        var Invoicebody = textBody1;
                        System.IO.File.WriteAllText(InsertInvoiceHtmltoLocalFolder.HtmlFilePath_Local + request.LoanApplicationNumber + "RL.html", Invoicebody);


                        //CREATE REPAY LOAN  RECORD
                         _itemRepay = _db.LoanRepayments.Create();
                        _itemRepay.BalanceBeforeP = (decimal)BalanceRemaining;
                        _itemRepay.BalanceAfterP = (decimal)NewBalanceafterpayment;
                        _itemRepay.UserID = request.UserID;
                        _itemRepay.RepaymentInvoiceURL = $"{ request.LoanApplicationNumber}RL.html";
                        _itemRepay.ApprovedBy = LoggedInUser.UserName;
                        _itemRepay.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _itemRepay.LoanApplicationNumber = request.LoanApplicationNumber;
                        _itemRepay.MonthYear = request.Month + ' ' + request.Year;
                        _itemRepay.AmountPaid = request.AmountPaid;
                        _itemRepay.CreatedBy = LoggedInUser.UserName;
                        _itemRepay.CreatedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _itemRepay.RepaymentStatus = ApprovalStatus.APPROVED;
                        _itemRepay.RecordType = RecordType.Repayment;
                        _itemRepay.LoanAmount = request.LoanAmount;
                        _db.LoanRepayments.Add(_itemRepay);
                        _db.SaveChanges();

                        //CREATE LOAN DUE DATE
                        _MonthRepay = _db.LastLoanMonth.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        if (_MonthRepay == null)
                        {
                            //INSERT/CREATE
                            _MonthRepay = _db.LastLoanMonth.Create();
                            _MonthRepay.UserID = LoggedInUser.UserId;
                            _MonthRepay.LoanApplicationNumber = request.LoanApplicationNumber;
                            _MonthRepay.LastMonthPaidFor = NextMontToPay + ' ' + request.Year;
                            _db.LastLoanMonth.Add(_MonthRepay);
                            _db.SaveChanges();
                        }
                        else
                        {
                            //UPDATE EXISTING RECORD
                            _MonthRepay = _db.LastLoanMonth.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                            _MonthRepay.LastMonthPaidFor = NextMontToPay + ' ' + request.Year;
                            _db.SaveChanges();
                        }

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = LoanTitles.LoanRepay;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = LoanLogStatus.MadePayment;
                        _log.Details = $"{LoggedInUser.UserName} Repaid {request.AmountPaid} for Loan Application ({ request.LoanApplicationNumber})";
                        _log.Details = LoanLogDetails.MadePayment + " for" + request.LoanApplicationNumber + "";
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();


                        //CREATING EMAIL CONTENT
                        string html = "";
                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.ApplyLoan)
                           .Replace("Hello RecieversName", "")
                            .Replace("Enter your message here", $"Hi {request.CustomerName}, <br><br>We wish to inform you that the payment of {formattedAmtPaid} for your Loan Application was recieved.<br><br>The table below shows further details of your loan application and payment from our records." +
                            $"<br><br><table><tr><td style='background-color: yellow; text-align:center;'><h5><b>Loan Repayment </b></h5></td></tr><tr><td><b>Loan Amount: </b>{formattedLoanAmt}</td></tr><tr><td><b>Amount Paid: </b>{formattedAmtPaid}</td></tr><tr><td><b>Balance Before Repayment: </b>{formattedB_Before}</td></tr><tr><td><b>Balance after Repayment: </b> {formattedB_After}</td></tr></table>" +
                            $"<br><br>Regards.<br>Loan Officer,<br>Leondon Resources Ltd..<br><br><br><br><br><br><br><br>We also also urge you to login to your customer backoffice To preview the invoice for this transaction")
                          .ToString();

                        var Emailbody = textBody;
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan Repayment Update",
                                Body = Emailbody,
                                To = new List<string> { getcustomeremail },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }
                       

                        //_ChckAppStatus = _db.CheckAppStats.FirstOrDefault(a => a.UserID == request.UserID);
                        //if (BalanceAfterPayment < 0)
                        //{
                        //    _ChckAppStatus.Status = false;
                        //}
                        //_db.SaveChanges();

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Repayment has been logged, an email of this transaction has also been sent to {getcustomeremail}";
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("Amount repaid cannot be more than the Balance remaing");
                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }




        [HttpPost, Route("api/loan/OldRepayLoan")]
        public HttpResponseMessage OldRepayLoan(LoanApplicationItem request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    LoanRepayment _itemRepay;
                    Invoice _inv;
                    LastLoanMonthRepaid _MonthRepay;
                    int intNextMontToPay = request.Month + 1;
                    string NextMontToPay = getFullName(intNextMontToPay);

                    ////check if loan is approved
                    //if (request.LoanStatus == ApprovalStatus.APPROVED)
                    //{
                    //_itemRepay = _db.LoanRepayments.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber && a.RepaymentStatus == ApprovalStatus.PENDING);
                    //if (_itemRepay == null)
                    //{
                    if (request.AmountPaid < request.BalanceRemaining + 1)
                    {

                        //CREATING INVOICE ENTRY
                        var total = _db.Invoices.Count(x => x.CustomerName != null);
                        var invnumber = "";
                        if (total < 1000)
                        {
                            invnumber = "00000" + total;
                        }
                        if (total < 10000)
                        {
                            invnumber = "0000" + total;
                        }
                        _inv = _db.Invoices.Create();
                        _inv.InvoiceURL = $"{request.LoanApplicationNumber}_RP.html";
                        _inv.InvoiceNumber = invnumber;
                        _inv.CustomerName = request.CustomerName;
                        _inv.CustomerID = request.UserID;
                        _inv.CreatedBy = LoggedInUser.DisplayName;
                        _inv.InvoiceDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.Invoices.Add(_inv);
                        _db.SaveChanges();


                        //CREATING INVOICE CONTENT
                        string html1 = "";
                        var client = new WebClient();
                        html1 = client.DownloadString(ReparLoanInvoiceDownloadString.LocalFilePath);
                        var getcustomeraddress = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).R_Address;
                        var getcustomerphone = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).Phone;
                        var getcustomeremail = _db.Users.FirstOrDefault(f => f.UserID == request.UserID).Email;
                        var formattedLoanAmt = @String.Format("{0:#,##0}", request.LoanAmount);

                        StringBuilder sb1 = new StringBuilder(html1);
                        string textBody1 =
                        sb1
                          .Replace("invoicenumber", invnumber)
                          .Replace("DateNow", @DateTime.Now.ToString("MM/dd/yyyy"))
                          .Replace("customername", request.CustomerName)
                          .Replace("InvoiceFor", $"generated for {request.CustomerName} on {@DateTime.Now.ToString("MM/dd/yyyy")}")
                          .Replace("customeraddress", getcustomeraddress)
                          .Replace("customerphone", getcustomerphone)
                          .Replace("customeremail", getcustomeremail)
                          .Replace("#ApplicationNumber", request.LoanApplicationNumber)
                          .Replace("nextpaymentdue", $"{NextMontToPay} {request.Year}")
                          .Replace("customerID", request.UserID)
                          .Replace("loanamount", $"{formattedLoanAmt}")
                          .ToString();



                        //INSERTING INVOICE TO LOCAL FOLDER
                        var Invoicebody = textBody1;
                        System.IO.File.WriteAllText(InsertInvoiceHtmltoLocalFolder.HtmlFilePath_Local + request.LoanApplicationNumber + "_RL.html", Invoicebody);

                        _itemRepay = _db.LoanRepayments.Create();
                        _itemRepay.UserID = request.UserID;
                        _itemRepay.MonthYear = NextMontToPay + ' ' + request.Year;
                        _itemRepay.AmountPaid = request.AmountPaid;
                        _itemRepay.LoanApplicationNumber = request.LoanApplicationNumber;
                        _itemRepay.CreatedBy = LoggedInUser.UserName;
                        _itemRepay.CreatedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _itemRepay.RepaymentStatus = ApprovalStatus.PENDING;
                        _itemRepay.RecordType = RecordType.Repayment;
                        _itemRepay.LoanAmount = request.LoanAmount;
                        _db.LoanRepayments.Add(_itemRepay);
                        _db.SaveChanges();

                        //CREATE LOAN DUE DATE
                        _MonthRepay = _db.LastLoanMonth.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        if (_MonthRepay == null)
                        {
                            //INSERT/CREATE
                            _MonthRepay = _db.LastLoanMonth.Create();
                            _MonthRepay.UserID = LoggedInUser.UserId;
                            _MonthRepay.LoanApplicationNumber = request.LoanApplicationNumber;
                            _MonthRepay.LastMonthPaidFor = NextMontToPay + ' ' + request.Year;
                            _db.LastLoanMonth.Add(_MonthRepay);
                            _db.SaveChanges();
                        }
                        else
                        {
                            //UPDATE EXISTING RECORD
                            _MonthRepay = _db.LastLoanMonth.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                            _MonthRepay.LastMonthPaidFor = NextMontToPay + ' ' + request.Year;
                            _db.SaveChanges();
                        }

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = LoanTitles.LoanRepay;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = LoanLogStatus.MadePayment;
                        _log.Details = $"{LoggedInUser.UserName} Repaid {request.AmountPaid} for Loan Application ({ request.LoanApplicationNumber})";

                        _log.Details = LoanLogDetails.MadePayment + " for" + request.LoanApplicationNumber + "";
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();


                        //CREATING EMAIL CONTENT
                        string html = "";
                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.ApplyLoan)
                           .Replace("RecieversName", request.CustomerName)
                            .Replace("Enter your message here", $"Hi {request.CustomerName}, <br><br>We wish to inform you that your Repayment for Loan Application was successful.<br><br>" +
                            $" <b>Loan Application Number :</b> {request.LoanApplicationNumber} has been recived and logged. <br><br>Regards.<br><br><br><br><br>,We also wish to use this medium to inform you that you can make" +
                            $"  repayments of your loan at your convenience any time through any of our payment channels.<br><br><br> As we also urge you to login to your customer backoffice To preview the invoice for this transaction</a>")
                          .ToString();

                        var Emailbody = textBody;
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan Application Invoice",
                                Body = Emailbody,
                                To = new List<string> { getcustomeremail },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }


                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Your Loan Repayment was recieved, a mail has been sent to {getcustomeremail} for this transaction";
                    }
                    else
                    {
                        resp.ResponseMsg = String.Format("Amount repaid cannot be more than the Balance remaing");
                    }
                    //}
                    //else
                    //{
                    //    resp.ResponseMsg = String.Format("You have a pending loan repayment, please hold on a while for admin to verify and approve it");
                    //}

                    //}
                    //else
                    //{
                    //    resp.ResponseMsg = String.Format("Sorry, you can only repay an APPROVED Loan, contact the company");
                    //}
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }


        [HttpPost, Route("api/loan/takeaction")]
        public HttpResponseMessage TakeAction(LoanApplicationItem request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    CheckAppStatus _ChckAppStatus;
                    LoanApplication _item;

                    if (request.Action == ApprovalStatus.APPROVED)
                    {
                        _item = _db.LoanApplications.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        _item.LoanStatus = request.Action;
                        _item.Comment = request.Comment;
                        _item.ApprovedBy = LoggedInUser.UserName;
                        _item.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();

                        _ChckAppStatus = _db.CheckAppStats.SingleOrDefault(a => a.UserID == request.UserID);
                        _ChckAppStatus.Status = request.AppStatus;
                        _db.SaveChanges();

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = RecordType.TakeAction;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = $"{RecordType.TakeAction} on a Loan Application";
                        _log.Details = $"{LoggedInUser.UserName} Set Loan Application with Number :" + request.LoanApplicationNumber + " to " + request.Action;
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();

                        //CREATING EMAIL CONTENT
                        string html = "";
                        var client = new WebClient();

                        var getcustomername = _db.Users.FirstOrDefault(k => k.UserID == request.UserID).DisplayName;

                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.LoanApproved)
                           .Replace("RecieversName", getcustomername)
                            .Replace("Enter your message here", $"Hi {getcustomername}, <br><br>We wish to inform you that the status of your Loan Application with Number.<br><br>" +
                            $" with Number : { request.LoanApplicationNumber } has been updated to changed to {request.Action }.<br><br>Regards.<br>Loan Manager, Leon Resources Ltd.<br><br><br><br>,We also wish to use this medium to inform you that you can make" +
                            $"  repayments of your loan at your convenience any time through any of our payment channels.<br><br><br> As we also urge you to login to your customer backoffice To see activities on this transaction and others. you can request for your login credentials from your account officer.</a>")
                          .ToString();

                        var Emailbody = textBody;
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan application status updated",
                                Body = Emailbody,
                                To = new List<string> { request.CustomerEmail.ToString() },
                                //Attachments = new List<string> { sFileName.ToString() },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Application has been set to : {request.Action} successfully, an email has been sent to {request.CustomerEmail} in this regards";
                    }


                    if (request.Action == ApprovalStatus.UNDERREVIEW)
                    {
                        _item = _db.LoanApplications.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        _item.LoanStatus = request.Action;
                        _item.Comment = request.Comment;
                        _item.ApprovedBy = LoggedInUser.UserName;
                        _item.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();

                        _ChckAppStatus = _db.CheckAppStats.SingleOrDefault(a => a.UserID == request.UserID);
                        _ChckAppStatus.Status = request.AppStatus;
                        _db.SaveChanges();

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = RecordType.TakeAction;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = $"{RecordType.TakeAction} on a Loan Application";
                        _log.Details = $"{LoggedInUser.UserName} Set Loan Application with Number :" + request.LoanApplicationNumber + " to " + request.Action;
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();

                        //CREATING EMAIL CONTENT
                        string html = "";
                        var client = new WebClient();

                        var getcustomername = _db.Users.FirstOrDefault(k => k.UserID == request.UserID).DisplayName;

                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.LoanApproved)
                           .Replace("RecieversName", getcustomername)
                            .Replace("Enter your message here", $"Hi {getcustomername}, <br><br>We wish to inform you that the status of your Loan Application with Number : " +
                            $" { request.LoanApplicationNumber } has been to updated to {request.Action }.<br><br>Regards.<br>Loan Manager, Leon Resources Ltd.<br><br><br><br>,We also wish to use this medium to inform you that you can make" +
                            $"  repayments of your loan at your convenience any time through any of our payment channels.<br><br><br> As we also urge you to login to your customer backoffice To see activities on this transaction and others. you can request for your login credentials from your account officer.</a>")
                          .ToString();

                        var Emailbody = textBody;
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan Application Status Updated",
                                Body = Emailbody,
                                To = new List<string> { request.CustomerEmail.ToString() },
                                //Attachments = new List<string> { sFileName.ToString() },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Application has been set to : {request.Action} successfully, an email has been sent to {request.CustomerEmail} in this regards";

                    }

                    if (request.Action == ApprovalStatus.PROCESSING)
                    {
                        _item = _db.LoanApplications.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        _item.LoanStatus = request.Action;
                        _item.Comment = request.Comment;
                        _item.ApprovedBy = LoggedInUser.UserName;
                        _item.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();

                        _ChckAppStatus = _db.CheckAppStats.SingleOrDefault(a => a.UserID == request.UserID);
                        _ChckAppStatus.Status = request.AppStatus;
                        _db.SaveChanges();

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = RecordType.TakeAction;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = $"{RecordType.TakeAction} on a Loan Application";
                        _log.Details = $"{LoggedInUser.UserName} Set Loan Application with Number :" + request.LoanApplicationNumber + " to " + request.Action;
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();

                        //CREATING EMAIL CONTENT
                        string html = "";
                        var client = new WebClient();

                        var getcustomername = _db.Users.FirstOrDefault(k => k.UserID == request.UserID).DisplayName;
                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.LoanApproved)
                           .Replace("RecieversName", getcustomername)
                            .Replace("Enter your message here", $"Hi {getcustomername}, <br><br>We wish to inform you that the status of your Loan Application with Number : " +
                            $" { request.LoanApplicationNumber } has been to updated to {request.Action }.<br><br>Regards.<br>Loan Manager, Leon Resources Ltd.<br><br><br><br>,We also wish to use this medium to inform you that you can make" +
                            $"  repayments of your loan at your convenience any time through any of our payment channels.<br><br><br> As we also urge you to login to your customer backoffice To see activities on this transaction and others. you can request for your login credentials from your account officer.</a>")
                          .ToString();

                        var Emailbody = textBody;
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan Application Status Updated",
                                Body = Emailbody,
                                To = new List<string> { request.CustomerEmail.ToString() },
                                //Attachments = new List<string> { sFileName.ToString() },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Application has been set to : {request.Action} successfully, an email has been sent to {request.CustomerEmail} in this regards";

                    }


                    if (request.Action == ApprovalStatus.REJECTED)
                    {
                        _item = _db.LoanApplications.SingleOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        _item.LoanStatus = request.Action;
                        _item.Comment = request.Comment;
                        _item.ApprovedBy = LoggedInUser.UserName;
                        _item.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();

                        _ChckAppStatus = _db.CheckAppStats.SingleOrDefault(a => a.UserID == request.UserID);
                        _ChckAppStatus.Status = request.AppStatus;
                        _db.SaveChanges();

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = RecordType.TakeAction;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = $"{RecordType.TakeAction} on a Loan Application";
                        _log.Details = $"{LoggedInUser.UserName} Set Loan Application with Number :" + request.LoanApplicationNumber + " to " + request.Action;
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();

                        //CREATING EMAIL CONTENT
                        string html = "";
                        var client = new WebClient();

                        var getcustomername = _db.Users.FirstOrDefault(k => k.UserID == request.UserID).DisplayName;
                        html = client.DownloadString(EmailDownloadString.LocalFilePath);
                        StringBuilder sb = new StringBuilder(html);
                        string textBody =
                        sb
                          .Replace("Subject", LoanLogStatus.LoanApproved)
                           .Replace("RecieversName", getcustomername)
                            .Replace("Enter your message here", $"Hi {getcustomername}, <br><br>We wish to inform you that the status of your Loan Application with Number : " +
                            $" { request.LoanApplicationNumber } has been to updated to {request.Action }.<br><br>Regards.<br>Loan Manager, Leon Resources Ltd.<br><br><br><br>,We also wish to use this medium to inform you that you can make" +
                            $"  repayments of your loan at your convenience any time through any of our payment channels.<br><br><br> As we also urge you to login to your customer backoffice To see activities on this transaction and others. you can request for your login credentials from your account officer.</a>")
                          .ToString();

                        var Emailbody = textBody;
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan Application Status Updated",
                                Body = Emailbody,
                                To = new List<string> { request.CustomerEmail.ToString() },
                                //Attachments = new List<string> { sFileName.ToString() },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }

                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Application has been set to : {request.Action} successfully, an email has been sent to {request.CustomerEmail} in this regards";

                    }
                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }


        [HttpPost, Route("api/loan/fetchByUID")]
        public HttpResponseMessage FetchUserDetailByUID(LoanApplicationItem request)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var req = request.Url;
                string[] arrayValues = req.Split('?');
                string ActualUJD = arrayValues[1];

                var _item = (from l in _db.LoanApplications
                             where l.LoanApplicationNumber == ActualUJD
                             select new
                             {
                                 invoice = l.InvoiceURL,
                             }).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpGet, Route("api/loan/fetchLoanDet")]
        public HttpResponseMessage fetchAppForm(String Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.LoanApplications
                             where l.LoanApplicationNumber == Id
                             select new
                             {
                                 l.UserID,
                                 DisplayName = l.User.DisplayName,
                                 l.LoanApplicationNumber,
                                 l.LoanAmount,
                                 l.BalanceRecieved,
                                 l.BalanceRemaining,
                                 AppStatus = l.AppStatus.Status,
                                 l.Comment,
                                 l.LoanStatus,
                                 l.MonthYear,
                                 LoanNextPayment = l.LastLoanMonth.LastMonthPaidFor
                             }).SingleOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpGet, Route("api/loan/fetchRepaymentnfo")]
        public HttpResponseMessage fetchInfoForRepaymentHistory(String Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.LoanRepayments
                             where l.LoanApplicationNumber == Id && l.RepaymentStatus == ApprovalStatus.PENDING
                             select new
                             {
                                 l.ID,
                                 l.UserID,
                                 l.LoanApplicationNumber,
                                 l.AmountPaid,
                                 AppStatus = l.AppStatus.Status,
                                 l.Comment,
                                 l.LoanAmount
                             }).FirstOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpPost, Route("api/loan/takeRepayaction")]
        public HttpResponseMessage TakeRepaymentAction(LoanApplicationItem request)
        {
            ActionResponse resp = new ActionResponse { ResponseCode = "96", ResponseMsg = "System Malfunction" };
            using (MyDbContext _db = new MyDbContext())
            {
                try
                {
                    CheckAppStatus _ChckAppStatus;
                    LoanApplication _item;
                    //LoanRepayment _itemRepay;

                    if (request.Action == ApprovalStatus.APPROVED)
                    {
                        decimal BalanceRecieved = _db.LoanApplications.FirstOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber).BalanceRecieved;
                        decimal? BalanceRemaining = _db.LoanApplications.FirstOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber).BalanceRemaining;

                        //int LoanAmount = Convert.ToInt32(request.LoanAmount);
                        int AmountPaid = Convert.ToInt32(request.AmountPaid);

                        //old balance minus amount repaid
                        decimal? BalanceAfterPayment = BalanceRemaining - AmountPaid;

                        //Calculating the new balance Recieved
                        decimal NewbalanceRecieved = BalanceRecieved + AmountPaid;

                        //to get ballance reamining
                        decimal? TenPercentVal = 10 * BalanceAfterPayment / 100;

                        //add it to the balance after payment
                        decimal? NewBalanceafterpayment = BalanceAfterPayment + TenPercentVal;

                        //UPDATE REPAYLOAN RECORD

                        LoanRepayment _itemRepay = (from a in _db.LoanRepayments
                                                    where a.ID == request.ID && a.RepaymentStatus == ApprovalStatus.PENDING
                                                    select a).SingleOrDefault();
                        _itemRepay.BalanceBeforeP = (decimal)BalanceRemaining;
                        _itemRepay.BalanceAfterP = (decimal)NewBalanceafterpayment;
                        _itemRepay.UserID = request.UserID;
                        _itemRepay.ApprovedBy = LoggedInUser.UserName;
                        _itemRepay.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _itemRepay.Comment = request.Comment;
                        _itemRepay.RecordType = RecordType.TakeAction;
                        _itemRepay.RepaymentStatus = request.Action;
                        _itemRepay.LoanApplicationNumber = request.LoanApplicationNumber;
                        _db.SaveChanges();

                        //UPDATE LOAN APPLICATION
                        _item = _db.LoanApplications.FirstOrDefault(a => a.LoanApplicationNumber == request.LoanApplicationNumber);
                        _item.LoanStatus = request.Action;
                        _item.BalanceRecieved = (decimal)NewbalanceRecieved;
                        _item.BalanceRemaining = NewBalanceafterpayment;
                        if (BalanceAfterPayment < 0)
                        {
                            _item.Repaid = false;
                        }
                        _item.ApprovedBy = LoggedInUser.UserName;
                        _item.ApprovedDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.SaveChanges();

                        _ChckAppStatus = _db.CheckAppStats.FirstOrDefault(a => a.UserID == request.UserID);
                        if (BalanceAfterPayment < 0)
                        {
                            _ChckAppStatus.Status = false;
                        }
                        _db.SaveChanges();

                        //Update Activity
                        ActivityLog _log = _db.ActivityLogs.Create();
                        _log.Title = RecordType.TakeAction;
                        _log.UserName = LoggedInUser.UserName;
                        _log.ActivityType = LoanLogStatus.ApplyLoan;
                        _log.Details = $"Set Loan Application with Number :" + request.LoanApplicationNumber + " to" + request.Action;
                        _log.ActivityDate = Date.GetDateTimeByTimeZone(DateTime.Now);
                        _db.ActivityLogs.Add(_log);
                        _db.SaveChanges();

                        string sFileName = HttpContext.Current.Server.MapPath(@"~/emailtemp/invoice.pdf");
                        try
                        {
                            EmailItem _mail = new EmailItem
                            {
                                Title = $"Loan application status updated",
                                Body = $"Hi {request.CustomerEmail}, <br><br>We wish to inform you that the status of your Loan Repayment for loan application Number :" + request.LoanApplicationNumber + " has been to changed to " + request.Action + ";<br><br>Regards.<br><br><br><br><br>, We also wish to use this medium to inform you that you can make repayments at any time through any of our payment channels as shown in the invoice as attached",
                                To = new List<string> { request.CustomerEmail.ToString() },
                                Attachments = new List<string> { sFileName.ToString() },
                            };
                            Messaging.LogMail(_mail);
                        }
                        catch { }


                        resp.ResponseCode = "00";
                        resp.ResponseMsg = $"Loan Repayment has been set to : {request.Action} successfully, an email has been sent to {request.CustomerEmail} in this regards ";
                    }
                    if (request.Action != ApprovalStatus.APPROVED)
                    {
                        //INSERT ACTION TO DB
                        resp.ResponseCode = "96";
                        resp.ResponseMsg = $"Sorry, for demo purpose, kindly use APPROVED";
                    }

                }
                catch (System.Exception ex)
                {
                    resp.ResponseMsg = ex.Message;
                }
            }
            return Request.CreateResponse(HttpStatusCode.OK, resp);
        }

        [HttpGet, Route("api/loan/fetchUsersLoan")]
        public HttpResponseMessage fetchUsersLoan()
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.LoanApplications
                             where l.UserID == LoggedInUser.UserId && !l.Repaid
                             select new
                             {
                                 l.LoanApplicationNumber,
                                 l.LoanAmount,
                             }).ToList();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        [HttpGet, Route("api/loan/fetchLoanDetails")]
        public HttpResponseMessage fetchLoanDetails(String Id)
        {
            using (MyDbContext _db = new MyDbContext())
            {
                var _item = (from l in _db.LoanApplications
                             where l.LoanApplicationNumber == Id
                             select new
                             {
                                 l.UserID,
                                 l.LoanApplicationNumber,
                                 l.LoanAmount,
                                 l.BalanceRecieved,
                                 l.BalanceRemaining,
                                 l.LoanStatus,
                                 AppStatus = l.AppStatus.Status ? "Yes" : "No",
                                 l.MonthYear,
                                 LoanNextPayment = l.LastLoanMonth.LastMonthPaidFor,
                                 l.Comment,
                             }).SingleOrDefault();
                return Request.CreateResponse(HttpStatusCode.OK, _item);
            }
        }

        //public static void SendMailMessage(string toEmail, string subject, string body)
        //{
        //    string fromEmail = EmailAddress.TestMail;
        //    using (MailMessage mail = new MailMessage(fromEmail, toEmail))
        //    {
        //        mail.Subject = subject;
        //        mail.Body = body;

        //        string strFileFormat = System.Configuration.ConfigurationManager.AppSettings["FormateFilePath"].ToString();
        //        System.Net.Mail.Attachment attachment;
        //        attachment = new System.Net.Mail.Attachment(strFileFormat);
        //        mail.Attachments.Add(attachment);

        //        //Attachment attachFile = new Attachment(txtAttachmentPath.Text);
        //        //mail.Attachments.Add(attachFile);

        //        //string fileName = Path.GetFileName("C:/Users/HP/source/repos/Benardikem/LoanApp/Web/emailtemp/invoice.html");
        //        //mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));

        //        //if (fileUploader != null)
        //        //{
        //        //    string fileName = Path.GetFileName(fileUploader.FileName);
        //        //    mail.Attachments.Add(new Attachment(fileUploader.InputStream, fileName));
        //        //}


        //        mail.IsBodyHtml = true;
        //        SmtpClient smtp = new SmtpClient();
        //        smtp.Host = "mail.jdictstores.com.ng";
        //        smtp.EnableSsl = false;
        //        NetworkCredential networkCredential = new NetworkCredential(fromEmail, EmailAddress.TestMailPassword);
        //        smtp.UseDefaultCredentials = true;
        //        smtp.Credentials = networkCredential;
        //        smtp.Port = 8889;
        //        smtp.Send(mail);
        //    }
        //}
    }
}
