using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class EmailLog
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public string Module { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
        public DateTime CreatedDate { get; set; }
        public Boolean Sent { get; set; }
        public DateTime? SentDate { get; set; }

        public override string ToString()
        {
            return Subject;
        }
        [ForeignKey("To")]
        public User User { get; set; }
    }
}