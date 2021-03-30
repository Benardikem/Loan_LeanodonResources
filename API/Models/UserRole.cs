using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("User_In_Role"), Serializable]
    public class UserRole
    {
        [Key]
        public Int64 ID { get; set; }
        public String UserId { get; set; }
        public Int64 RoleId { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        [ForeignKey("RoleId")]
        public virtual WRole Role { get; set; }
    }
}
