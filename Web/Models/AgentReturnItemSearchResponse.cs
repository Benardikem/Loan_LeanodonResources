using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class AgentReturnItemSearchResponse : SearchResponse<AgentItemreturns>
    {
    }
    public class AgentItemreturns : SearchDetail
    {
        public long ID { get; set; }
        public String UsersID { get; set; }
        public String Address { get; set; }
        public String Phone { get; set; }
        public string Active { get; set; }
        public string UploadedBy { get; set; }
        public DateTime UploadedDate { get; set; }
        public Boolean Approved { get; set; }
        public string DisplayName { get; set; }
        public string Number { get; set; }
        public string UserID { get; set; }
        public string EmailAddress { get; set; }
        public string UserCategory { get; set; }
        public List<string> Roles { get; set; }

        public Boolean Modified { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public bool Deleted { get; set; }
        public String DeletedBy { get; set; }
        public DateTime? DeletedDate { get; set; }
        //public ICollection<UserItemReturns> UserReturns { get; set; }
    }
   
   
}