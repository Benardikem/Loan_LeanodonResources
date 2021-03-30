using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class RoleModel
    {
        public List<RoleItem> RoleItems { get; set; }
        public List<SubMenuItem> SubMenuItems { get; set; }
    }
    public class RoleItem
    {
        public string Active { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string Approved { get; set; }
        public string Description { get; set; }
        public long Id { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Name { get; set; }
        public string UserCategory { get; set; }
    }

    //public class UserCategoryItem
    //{
    //    public string CategoryName { get; set; }
    //    public int Id { get; set; }
    //}

    public class SubMenuItem
    {
        public long Id { get; set; }
        public string MenuName { get; set; }
        public int MenuOrder { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
    }
}