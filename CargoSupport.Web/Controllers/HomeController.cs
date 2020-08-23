using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CargoSupport.Web.Models;
using CargoSupport.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Quartz;
using CargoSupport.Web.Models.DatabaseModels;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace CargoSupport.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbHelper _dbHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        //private readonly IScheduler _scheduler;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor/*, IScheduler factory*/)
        {
            //var th = new CargoSupport.Web.Helpers.TaskHelper(_scheduler);
            //th.CheckAvailability().Wait();
            _httpContextAccessor = httpContextAccessor;

            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _logger = logger;
        }

        public IActionResult Index()
        {
            var a = HttpContext.User;
            var a2 = Environment.UserName;
            string email = a.FindFirstValue(ClaimTypes.Email); // Always null. Not returning logged in user's Email
            string userName = a.FindFirstValue(ClaimTypes.Name); // Returning logged in user's UserName
            string userId = a.FindFirstValue(ClaimTypes.NameIdentifier);

            //string UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return View();
        }

        public IActionResult Index2()
        {
            //var a = _httpContextAccessor.HttpContext.User;
            //string email = a.FindFirstValue(ClaimTypes.Email); // Always null. Not returning logged in user's Email
            //string userName = a.FindFirstValue(ClaimTypes.Name); // Returning logged in user's UserName
            //string userId = a.FindFirstValue(ClaimTypes.NameIdentifier);

            //string UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
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