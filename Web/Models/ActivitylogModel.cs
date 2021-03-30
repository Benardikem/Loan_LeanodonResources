using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ActivitylogModel
    {
        public List<ActivitylogItem> ActivitylogItems { get; set; }
    }
    public class ActivitylogItem
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public string Details { get; set; }
        public string UserName { get; set; }
        public string ActivityType { get; set; }
        public DateTime ActivityDate { get; set; }
    }
}