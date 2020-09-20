using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CargoSupport.Models.Auth.ManageViewModels
{
    public class DisplayRecoveryCodesViewModel
    {
        [Required]
        public IEnumerable<string> Codes { get; set; }

    }
}
