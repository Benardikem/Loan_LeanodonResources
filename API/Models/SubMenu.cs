using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("SubMenus"), Serializable]
    public class SubMenu
    {
        [Key]
        public Int64 ID { get; set; }
        public String Name { get; set; }
        public String Class { get; set; }
        public int Order { get; set; }
        public Int64 MenuId { get; set; }
        public String Link { get; set; }
        public Boolean Active { get; set; }
        public Int64 Parent { get; set; }
        [ForeignKey("MenuId")]
        public Menu Menu { get; set; }

    }
}
