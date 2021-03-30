using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    [Serializable, Table("Invoice")]
    public class Invoice
    {
        [Key]
        public Int64 Id { get; set; }
        public string InvoiceURL { get; set; }
        public string InvoiceNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerID { get; set; }
        public string CreatedBy { get; set; }
        public DateTime InvoiceDate { get; set; }
    }
}