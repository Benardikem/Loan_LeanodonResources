using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace API.Models
{
    [Serializable]
    public class User
    {
        /// <summary>
        /// User ID
        /// </summary>
        [Key]
        public String UserID { get; set; }

        /// <summary>
        /// Display Name
        /// </summary>
        public String DisplayName { get; set; }

        /// <summary>
        /// Number
        /// </summary>
        public String R_Address { get; set; }

        /// <summary>
        /// Number
        /// </summary>
        public String C_Address { get; set; }

        /// <summary>
        /// Number
        /// </summary>
        public String Phone { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public String Password { get; set; }

        /// <summary>
        /// Password
        /// </summary>
        public Boolean ChangePassword { get; set; }

        /// <summary>
        /// Email
        /// </summary>
        public String Email { get; set; }

        /// <summary>
        /// Date Added
        /// </summary>
        public DateTime DateAdded { get; set; }
        /// <summary>
        /// Added By
        /// </summary>
        public String AddedBy { get; set; }

        /// <summary>
        /// Active
        /// </summary>
        public Boolean Active { get; set; }

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
        public DateTime? DateApproved { get; set; }

        /// <summary>
        /// Modified
        /// </summary>
        public Boolean Modified { get; set; }

        /// <summary>
        /// Modified By
        /// </summary>
        public String ModifiedBy { get; set; }

        /// <summary>
        /// Date Modified
        /// </summary>
        public DateTime? DateModified { get; set; }

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

        /// <summary>
        /// file URL for Passports
        /// </summary>
        public string ImageURL { get; set; }
               
        /// <summary>
        /// UserCategory
        /// </summary>
        public String UserCategory { get; set; }       
        public virtual ICollection<UserRole> UserRoles { get; set; }

        /// <summary>
        /// Customer's BVN Number
        /// </summary>
        public String BVN_Number { get; set; }

        /// <summary>
        /// Guarantor's Full Name
        /// </summary>
        public String Guarantor { get; set; }

        /// <summary>
        /// Guarantor's Phone
        /// </summary>
        public String GuarantorPhone { get; set; }

        /// <summary>
        /// Guarantor's Address 
        /// </summary>
        public String G_Address { get; set; }

        //[ForeignKey("UserCategoryId")]
        //public UserCategory UserCategory { get; set; }

        /// <summary>
        /// Priviledges for users
        /// </summary>
        public List<Menu> GetPrivileges()
        {
            List<Menu> menus = new List<Menu>();
            using (MyDbContext _db = new MyDbContext())
            {
                var roleIds = this.UserRoles.Select(a => a.RoleId).ToList();
                var query = _db.SubmenuRoles.Where(a => roleIds.Contains(a.RoleId) && a.Role.Active && a.Role.Approved && a.Submenu.Active)
                                            .Select(sr => new
                                            {
                                                MenuName = sr.Submenu.Menu.Name,
                                                MenuId = sr.Submenu.Menu.ID,
                                                MenuClass = sr.Submenu.Menu.Class,
                                                MenuOrder = sr.Submenu.Menu.Order,
                                                sr.Submenu.ID,
                                                sr.Submenu.Active,
                                                sr.Submenu.Class,
                                                sr.Submenu.Link,
                                                sr.Submenu.Name,
                                                sr.Submenu.Order,
                                                sr.Submenu.Parent
                                            })
                                            .ToList();
                var distinctMenus = query.Select(a => new { a.MenuName, a.MenuId, a.MenuOrder, a.MenuClass })
                                            .Distinct()
                                            .OrderBy(a => a.MenuOrder);
                foreach (var m in distinctMenus)
                {
                    Menu mnu = new Menu
                    {
                        ID = m.MenuId,
                        Name = m.MenuName,
                        Class = m.MenuClass,
                        Order = m.MenuOrder
                    };
                    var subs = query.Where(s => s.MenuId == m.MenuId && s.Active)
                                    .Select(a => new SubMenu
                                    {
                                        Active = a.Active,
                                        Class = a.Class,
                                        ID = a.ID,
                                        Link = a.Link,
                                        Name = a.Name,
                                        Parent = a.Parent
                                    })
                                    .OrderBy(a => a.Order)
                                    .ToList();
                    mnu.SubMenus = new List<SubMenu>();
                    mnu.SubMenus.AddRange(subs);
                    foreach (var s in subs)
                    {
                        var children = _db.SubMenus.Where(a => a.Parent == s.ID).ToList();
                        if (children != null)
                        {
                            mnu.SubMenus.AddRange(children);
                        }
                    }
                    menus.Add(mnu);
                }
            }
            return menus;
        }
    }
}