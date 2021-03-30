using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class InitiateReturnRequest
    {

        public decimal AmountPaid { get; set; }
        public int OfferId { get; set; }
        public long Shares { get; set; }
        public int Records { get; set; }
        public decimal TotalAmount { get; set; }
        public string FilePath { get; set; }
        public List<EvidenceRequest> Evidences { get; set; }
        public string OfferName { get; set; }
    }

    public class EvidenceRequest
    {
        public string Path { get; set; }
        public int AccountId { get; set; }
        public int BankId { get; set; }
        public decimal Amount { get; set; }
        public String PaymentDate { get; set; }
    }

    public class AuthRequest
    {
        public long Id { get; set; }
    }
    public class DeclineRequest
    {
        public long Id { get;  set; }
        public string Comment { get; set; }
    }
}