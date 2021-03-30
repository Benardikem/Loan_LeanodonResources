using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class SMSContactCategory
    {
        /// <summary>
        /// ID
        /// </summary>
        [Key]
        public Int64 Id { get; set; }
        public string CategoryName { get; set; }
    }
}