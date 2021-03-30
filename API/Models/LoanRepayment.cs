using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class LoanRepayment
    {
          [Key]
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
        public string RecordType { get; set; }
        public string RepaymentStatus { get; set; }
        public decimal LoanAmount { get; set; }
       public string RepaymentInvoiceURL { get; set; }

        [ForeignKey("UserID")]
        public virtual User User { get; set; }
        
        //[ForeignKey("LoanApplicationNumber")]
        //public virtual LoanApplication LoanApplicationKeyTable { get; set; }

        [ForeignKey("UserID")]
        public virtual CheckAppStatus AppStatus { get; set; }


    }
}