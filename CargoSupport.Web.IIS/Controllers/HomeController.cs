using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CargoSupport.Models;
using CargoSupport.Helpers;
using Microsoft.AspNetCore.Http;
using static CargoSupport.Helpers.AuthorizeHelper;
using CargoSupport.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CargoSupport.Models.Auth;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;

namespace CargoSupport.Web.IIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbHelper _dbHelper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<MongoIdentityRole> _roleManager;

        public HomeController(
            ILogger<HomeController> logger,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<MongoIdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _logger = logger;
        }

        public async Task<IActionResult> Transport()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        public async Task<IActionResult> Plock()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.MedarbetareAndUp)]
        public async Task<IActionResult> Medarbetare()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}