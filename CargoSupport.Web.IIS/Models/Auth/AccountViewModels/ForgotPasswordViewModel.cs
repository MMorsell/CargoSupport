using System.ComponentModel.DataAnnotations;

namespace CargoSupport.Models.Auth.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}