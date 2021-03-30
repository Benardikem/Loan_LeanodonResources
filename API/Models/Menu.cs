using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Table("Menus"), Serializable]
    public class Menu
    {
        [Key]
        public Int64 ID { get; set; }
        public String Name { get; set; }
        public String Class { get; set; }
        public int Order { get; set; }
        public virtual List<SubMenu> SubMenus { get; set; }
    }
}
