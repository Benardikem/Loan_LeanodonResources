using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace API
{
    public class EmailItem
    {
        public Int64 Id { get; set; }
        public string Title { get; set; }
        public List<string> To { get; set; }
        public List<string> Cc { get; set; }
        public List<string> Bcc { get; set; }
        public List<string> Attachments { get; set; }
        public string Body { get; set; }
    }
}