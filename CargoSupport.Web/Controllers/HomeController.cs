using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CargoSupport.Web.Models;
using CargoSupport.Models;
using CargoSupport.Helpers;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Quartz;

namespace CargoSupport.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbHelper _dbHelper;
        private readonly IScheduler _scheduler;

        public HomeController(ILogger<HomeController> logger/*, IScheduler factory*/)
        {
            //var th = new CargoSupport.Web.Helpers.TaskHelper(_scheduler);
            //th.CheckAvailability().Wait();

            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            List<PinRouteModel> allRoutes = await _dbHelper.GetAllRecords<PinRouteModel>(Constants.MongoDb.OutputScreenTableName);
            if (allRoutes.Count == 0)
            {
                var ph = new PinHelper();
                ph.RetrieveRoutesForToday();
            }
            ViewBag.DataTable = JsonSerializer.Serialize(allRoutes);
            return View(allRoutes);
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