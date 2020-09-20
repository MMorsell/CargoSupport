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
        //private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly IScheduler _scheduler;

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
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            //var a2 = Environment.UserName;
            //string email = a.FindFirstValue(ClaimTypes.Email); // Always null. Not returning logged in user's Email
            //string userName = a.FindFirstValue(ClaimTypes.Name); // Returning logged in user's UserName
            //string userId = a.FindFirstValue(ClaimTypes.NameIdentifier);

            //string UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Plock()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var isInRole = await _userManager.IsInRoleAsync(user, "Admin");
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            //var a = _httpContextAccessor.HttpContext.User;
            //string email = a.FindFirstValue(ClaimTypes.Email); // Always null. Not returning logged in user's Email
            //string userName = a.FindFirstValue(ClaimTypes.Name); // Returning logged in user's UserName
            //string userId = a.FindFirstValue(ClaimTypes.NameIdentifier);

            //string UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        public async Task<IActionResult> Medarbetare()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}