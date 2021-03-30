using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Serializable, Table("ActivityLogs")]
    public class ActivityLog
    {
        [Key]
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string UserName { get; set; }
        public string ActivityType { get; set; }
        public DateTime ActivityDate { get; set; }
        [ForeignKey("UserName")]
        public virtual User User { get; set; }
    }
}