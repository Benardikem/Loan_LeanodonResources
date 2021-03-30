using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class SMSModel
    {
        public List<SMSItem> SMSItems { get; set; }
        public List<SMSCategoryItem> SMSCategoryItems { get; set; }
        public List<SMSContactItem> SMSContactItems { get; set; }
    }

    public class SMSItem
    {
        public Int64 Id { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string UnitsUsed { get; set; }
        public DateTime ActivityDate { get; set; }
    }

    public class SMSCategoryItem
    {
        public Int64 Id { get; set; }
        public string CategoryName { get; set; }
    }

    public class SMSContactItem
    {
        public Int64 Id { get; set; }
        public Int64 CategoryID { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
    }

    public class SMSrequests
    {
        public Int64 Id { get; set; }
        public string Recipient { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public string UnitsUsed { get; set; }
        public DateTime ActivityDate { get; set; }
        public string CategoryName { get; set; }
        public Int64 CategoryID { get; set; }
        public string PhoneNumber { get; set; }
        public string Name { get; set; }
    }
}