using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ReturnDetailsModel
    {
        public List<RecievingAgentItem> Agents { get; set; }
    }
    public class RecievingAgentItem
    {
        public Int64 Id { get; set; }
        public string UserId { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
        public string DisplayName { get; set; }
    }
}