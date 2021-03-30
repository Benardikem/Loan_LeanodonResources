using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class InvestorValidationResponse
    {
        public int Row { get; set; }
        public List<ValidationResult> ValidationResults { get; set; }
    }
}