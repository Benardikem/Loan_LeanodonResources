using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("SubMenu_In_Role"), Serializable]
    public class SubMenuRole
    {
        [Key]
        public Int64 ID { get; set; }
        public Int64 SubMenuId { get; set; }
        public Int64 RoleId { get; set; }
        [ForeignKey("SubMenuId")]
        public SubMenu Submenu { get; set; }
        [ForeignKey("RoleId")]
        public WRole Role { get; set; }
    }
}
