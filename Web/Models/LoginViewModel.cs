using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "CustomerID")]
        public string CustomerID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }

    public class ResetPasswordModel
    {
        [Required]
        [Display(Name = "UserID")]
        public string UserID { get; set; }
    }
}