using System.ComponentModel.DataAnnotations;

namespace CargoSupport.Models.Auth.AccountViewModels
{
    public class ExternalLoginConfirmationViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}