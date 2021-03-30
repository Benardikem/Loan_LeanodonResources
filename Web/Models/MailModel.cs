using System;
using System.Collections.Generic;

namespace Web.Models
{
    public class MailModel
    {
        public List<MailItem> MailItems { get; set; }
        //public List<MailTemplateItem> MailTemplateItems { get; set; }
        //public List<Contactusitem> Contactusitems { get; set; }
    }

    public class MailItem
    {
        public long Id { get; set; }
        public string Module { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Cc { get; set; }
        public string Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Attachments { get; set; }
        public DateTime CreatedDate { get; set; }
        public string EmailClient { get; set; }
        public string Sent { get; set; }
        public DateTime? SentDate { get; set; }
        public string DisplayName { get; set; }
        public long MailID { get; set; }
    }
}