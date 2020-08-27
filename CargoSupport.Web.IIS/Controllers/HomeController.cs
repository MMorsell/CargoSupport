using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CargoSupport.Models;
using CargoSupport.Helpers;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using static CargoSupport.Helpers.AuthorizeHelper;
using CargoSupport.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Linq;

namespace CargoSupport.Web.IIS.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbHelper _dbHelper;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly IScheduler _scheduler;

        public HomeController(ILogger<HomeController> logger /*, IHttpContextAccessor httpContextAccessor*//*, IScheduler factory*/)
        {
            //var th = new CargoSupport.Web.Helpers.TaskHelper(_scheduler);
            //th.CheckAvailability().Wait();
            //_httpContextAccessor = httpContextAccessor;

            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _logger = logger;
        }

        public async Task<IActionResult> Transport()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
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

        public async Task<IActionResult> Plock()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
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
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            return View();
        }

        public IActionResult Privacy()
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