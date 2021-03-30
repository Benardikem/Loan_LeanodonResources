using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Roles"), Serializable]
    public class WRole
    {
        [Key]
        public Int64 ID { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }
        public int UserCategoryId { get; set; }
        public Boolean Active { get; set; }
        /// <summary>
        /// Date Added
        /// </summary>
        public DateTime AddedDate { get; set; }
        /// <summary>
        /// Added By
        /// </summary>
        public String AddedBy { get; set; }

        /// <summary>
        /// Approved
        /// </summary>
        public Boolean Approved { get; set; }

        /// <summary>
        /// Approved By
        /// </summary>
        public String ApprovedBy { get; set; }

        /// <summary>
        /// Date Approved
        /// </summary>
        public DateTime? ApprovedDate { get; set; }


        /// <summary>
        /// Modified By
        /// </summary>
        public String ModifiedBy { get; set; }

        /// <summary>
        /// Date Modified
        /// </summary>
        public DateTime? ModifiedDate { get; set; }

        /// <summary>
        /// Deleted
        /// </summary>
        public Boolean Deleted { get; set; }

        /// <summary>
        /// Deleted By
        /// </summary>
        public String DeletedBy { get; set; }

        /// <summary>
        /// Date Deleted
        /// </summary>
        public DateTime? DeletedDate { get; set; }
        //[ForeignKey("UserCategoryId")]
        //public virtual UserCategory UserCategory { get; set; }
    }
}
