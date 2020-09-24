using System.Collections.Generic;

namespace CargoSupport.Web.IIS.ViewModels.Auth
{
    public class CreateViewModel
    {
        public CreateViewModel()
        {
            Roles = new List<string>();
        }

        public List<string> Roles { get; set; }
    }
}