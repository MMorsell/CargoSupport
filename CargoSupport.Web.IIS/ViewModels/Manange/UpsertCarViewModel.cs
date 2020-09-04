using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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