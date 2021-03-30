using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class InvoiceModel
    {
         public List<InvoiceItem> InvoiceItems { get; set; }
    }

    public class InvoiceItem
    {
        public Int64 Id { get; set; }
        public string InvoiceURL { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InvoiceDate { get; set; }
    }

    public class InvoiceViewModel
    {
        public Int64 Id { get; set; }
        public string InvoiceURL { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}