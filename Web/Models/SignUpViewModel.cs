using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Web.Models
{
    public class SignUpViewModel
    {
        [Required]
        [Display(Name = "Display Name")]
        public string DisplayName { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [Display(Name = "Phone Number")]
        //[Required(ErrorMessage = "Phone Number Required!"), MinLength(9)]
        //[StringLength(9, ErrorMessage = "The {0} must be at least {2} and at max {1} character long", MinimumLength = 9)]
        public string PhoneNumber { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "EmailAddress")]
        public string EmailAddress { get; set; }


        [Display(Name = "Province")]
        public string Province { get; set; }

        [Required(ErrorMessage = "Provide us with your Residential Address!")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Residential/Permananent Address")]
        public string ResidentialAddress { get; set; }

        //[Required]
        [DataType(DataType.PostalCode)]
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Provide a Password you can remember!")]
        [DataType(DataType.Password)]
        //[StringLength(20, ErrorMessage = "The {0} must be at least {2} and at max {1} character long", MinimumLength = 6)]
        //[RegularExpression(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[*$-+?_&amp;=!%{}/@#^]).*$", ErrorMessage = "New password must meet the following criteria; 1. Must be at least 8 characters. 2. Must contain at least one lower case letter, one upper case letter, one digit and one special character. 3. Valid special characters are -@#$%^&+=")]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirm password do not match")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        //[Required]
        //[Display(Name = "Country")]
        //public string Country { get; set; }
        //public List<CountriesList> Countrieslist { get; set; }
               
        [Display(Name = "Security Question")]
        public string SecurityQ { get; set; }
               
        [Display(Name = "Security Answer")]
        public string SecurityA { get; set; }
        public string Message { get; set; }
    }
}