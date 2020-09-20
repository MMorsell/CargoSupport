using AspNetCore.Identity.MongoDbCore.Models;
using CargoSupport.Models.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class RolesAndSuperUser
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<MongoIdentityRole> _roleManager;
        private readonly ILogger _logger;

        public RolesAndSuperUser(
            UserManager<ApplicationUser> userManager,
            RoleManager<MongoIdentityRole> roleManager,
            ILoggerFactory loggerFactory)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = loggerFactory.CreateLogger<RolesAndSuperUser>();
        }
    }
}