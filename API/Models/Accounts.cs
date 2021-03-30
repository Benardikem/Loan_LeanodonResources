using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Accounts"), Serializable]
    public class Account
    {
        [Key]
        public Int64 ID { get; set; }
        public String AccountNo { get; set; }
        public String AccountName { get; set; }
        public Int64 BankId { get; set; }
        public long? OfferID { get; set; }
        public String UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
        public Boolean Modified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public bool Deleted { get; set; }
        public String DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        public Boolean Active { get; set; }
        //[ForeignKey("OfferID")]
        //public virtual Offer AccountOffer { get; set; }
        //[ForeignKey("BankId")]
        //public virtual Bank AccountBank { get; set; }
    }
}
