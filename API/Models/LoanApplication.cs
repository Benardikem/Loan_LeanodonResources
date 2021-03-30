using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class LoanApplication
    {

        public Int64 ID { get; set; }
        public string UserID { get; set; }
        public decimal LoanAmount { get; set; }
        [Key]
        public string LoanApplicationNumber { get; set; }
        public decimal BalanceRecieved { get; set; }
        public decimal? BalanceRemaining { get; set; }
        public Boolean Repaid { get; set; }
        public string LoanStatus { get; set; }
        public string MonthYear { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? MaturityDate { get; set; }
        
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public string Comment { get; set; }
        public string InvoiceURL { get; set; }
        public string LoanSpread { get; set; }        

        [ForeignKey("UserID")]
        public virtual User User { get; set; }

        [ForeignKey("UserID")]
        public virtual CheckAppStatus AppStatus { get; set; }

        [ForeignKey("LoanApplicationNumber")]
        public virtual LastLoanMonthRepaid LastLoanMonth { get; set; }
    }

    [Serializable]
    public class CheckAppStatus
    {
        [Key]
        public String UserID { get; set; }
        public String Reason { get; set; }
        public Boolean Status { get; set; }
    }


    [Serializable]
    public class LastLoanMonthRepaid
    {
       
        public String UserID { get; set; }
        [Key] 
        public String LoanApplicationNumber { get; set; }
        public String LastMonthPaidFor { get; set; }
        public Boolean Status { get; set; }


    }
}