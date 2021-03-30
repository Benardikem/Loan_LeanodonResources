using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Codes.Constants
{
    public class Constants
    {
        public const string TestMail = "admin@jdictstores.com.ng";
        public const string TestMailPassword = "Jdadmin10$$$$$";
    }

    public class ApprovalStatus
    {
        public const string APPROVED = "APPROVED";
        public const string PENDING = "PENDING";
        public const string UNDERREVIEW = "UNDER-REVIEW";
        public const string PROCESSING = "PROCESSING";
        public const string REJECTED = "REJECTED";
        
    }
    public class LoanTitles
    {
        public const string NewCustomer = "NEW CUSTOMER CREATED";
        public const string LoanRepay = "LOAN REPAY";
        public const string LoanApply = "APPLY LOAN";
        public const string AdminTakeAction = "ACTION BY ADMIN";

    }
    public class LoanLogStatus
    {
        public const string CreatednewCustomer = "Created a New Customer";
        public const string ApplyLoan = "Loan Aplication";
        public const string MadePayment = "Repayment Made";
        public const string CompletedPayment = "Payment Completed";
        public const string LoanApproved = "LoanApproved";
    }
    public class LoanLogDetails
    {
        public const string NewCustDetails = "Details of New customer Created";
        public const string ApplyLoan = "Applied for a loan";
        public const string MadePayment = "Made a Payment";
        public const string CompletedPayment = "Completed payment of a loan";

    }

    public class RecordType
    {
        public const string Repayment = "Loan Repayment";
        public const string TakeAction = "Take Action";
    }

    public class InsertInvoiceHtmltoLocalFolder
    {
        //System.IO.File.Create(@"C:\Users\HP\source\repos\Benardikem\LoanApp\Web\Invoices\"+ APPno+".html");
        //System.IO.File.WriteAllText(@"C:\Users\PC88\source\repos\Benardikem\LoanApp\Web\Invoices\" + APPno + ".html", content);
        public const string HtmlFilePath_Local = @"C:\Users\user\source\repos\Benardikem\LoanApp\Web\Invoices\";
        public const string HtmlFilePath_Online = @"h:\root\home\benardikem-001\www\jd-ict-stores\invoices\";
    }

    //public class AddInvoiceAttachmentAsEmail
    //{
    //    public const string FilePath = @"~/emailtemp/invoice.pdf";
    //}

    public class EmailDownloadString
    {
        public const string OnlineFilePath = "http://jdictstores.com.ng/emailtemp/EmptyMail.html";
        public const string LocalFilePath = "C:/Users/user/source/repos/Benardikem/LoanApp/Web/emailtemp/EmptyMail.html";
    }

    public class InvoiceDownloadString
    {
        public const string OnlineFilePath = "http://jdictstores.com.ng/emailtemp/Invoice.html";
        public const string LocalFilePath = "C:/Users/user/source/repos/Benardikem/LoanApp/Web/emailtemp/Invoice.html";
    }

    public class ReparLoanInvoiceDownloadString
    {
        public const string OnlineFilePath = "http://jdictstores.com.ng/emailtemp/Invoice_RL.html";
        public const string LocalFilePath = "C:/Users/user/source/repos/Benardikem/LoanApp/Web/emailtemp/Invoice_RL.html";
    }


    public class Defaultpassword
    {
        public const string Cpassword = "customer123";
        public const string Apassword = "admin123";
    }

}