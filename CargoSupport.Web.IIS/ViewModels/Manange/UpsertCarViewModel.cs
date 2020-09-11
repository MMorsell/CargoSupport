using CargoSupport.Models.DatabaseModels;
using System.Collections.Generic;

namespace CargoSupport.ViewModels.Manange
{
    public class UpsertUserViewModel
    {
        public UpsertUserViewModel()
        {
            ExistingUsers = new List<WhitelistModel>();
        }

        public WhitelistModel CurrentUser { get; set; }
        public List<WhitelistModel> ExistingUsers { get; set; }
    }
}