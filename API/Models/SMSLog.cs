using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class SMSLog
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string UnitsUsed { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}