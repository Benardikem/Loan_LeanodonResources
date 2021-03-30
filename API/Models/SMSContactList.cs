using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class SMSContactList
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public Int64 CategoryID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }

        [ForeignKey("CategoryID")]
        public virtual SMSContactCategory ContactCategory { get; set; }
    }
}