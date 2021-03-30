using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class UserModel
    {
        public List<RoleItem> RoleItems { get; set; }
        public List<UserItem> UserItems { get; set; }
    }
    public class UserItem
    {
        public string Active { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string Approved { get; set; }
        public string DisplayName { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string Number { get; set; }
        public string UserID { get; set; }
        public string EmailAddress { get; set; }
        public string UserCategory { get; set; }
        public List<string> Roles { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public DateTime DateAdded { get; set; }
        public string Phone { get; set; }
        public string ImageURL { get; set; }
        public string BVN_Number { get; set; }
    }
}