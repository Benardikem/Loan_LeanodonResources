using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class ChangePasswordViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        //[StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} character long", MinimumLength = 6)]
        [RegularExpression(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[*$-+?_&amp;=!%{}/@#^]).*$", ErrorMessage = "New password must meet the following criteria; 1. Must be at least 8 characters. 2. Must contain at least one lower case letter, one upper case letter, one digit and one special character. 3. Valid special characters are -@#$%^&+=")]
        public string NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "The new password and confirm password do not match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Username { get; set; }
        public string Message { get; set; }
    }
}