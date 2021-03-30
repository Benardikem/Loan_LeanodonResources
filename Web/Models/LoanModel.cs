using System;
using System.Collections.Generic;


namespace Web.Models
{
    public class LoanModel
    {
        public List<LoanApplicationItem> LoanApplicationItems { get; set; }
        public List<LoanRepaymentItem> LoanRepaymentItems { get; set; }
        public List<UserItem> UserItems { get; set; }
    }

    public class LoanApplicationItem
    {
        public Int64 ID { get; set; }
        public string UserID { get; set; }
        public decimal LoanAmount { get; set; }
        public string LoanApplicationNumber { get; set; }
        public decimal BalanceRecieved { get; set; }
        public decimal? BalanceRemaining { get; set; }
        public string Repaid { get; set; }
        public string LoanStatus { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Comment { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string Url { get; set; }
        public bool CanApplyMore { get; set; }
        public string Action { get; set; }
        public decimal AmountPaid { get; set; }
        public int Month { get; set; }
        public string Year { get; set; }
        public string MonthYear { get; set; }
        public string NextRepayMonth { get; set; }
        public string Customer1_admin { get; set; }
        public bool AppStatus { get; set; }
        public string LoanGUID { get; set; }
        public string LoanSpread { get; set; }
        public string DisplayName { get; set; }
        public string PhoneNumber { get; set; }
        public string MatDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        public string InvoiceURL { get; set; }
    }

    public class LoanRepaymentItem
    {
        public Int64 ID { get; set; }
        public string UserID { get; set; }
        public string LoanApplicationNumber { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal BalanceBeforeP { get; set; }
        public decimal BalanceAfterP { get; set; }
        public string MonthYear { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Comment { get; set; }
        public string CustomerID { get; set; }
        public string CustomerName { get; set; }
        public string RepaymentStatus { get; set; }
        public string FullyRepaid { get; set; }
        public decimal LoanAmount { get; set; }
        public decimal? BalanceRemaining { get; set; }
        public decimal? BalanceRecieved { get; set; }
        public string DisplayName { get; set; }
        public string InvoiceURL { get; set; }
    }
}


