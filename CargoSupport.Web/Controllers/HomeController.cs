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

namespace CargoSupport.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MongoDbHelper _dbHelper;

        public HomeController(ILogger<HomeController> logger)
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var qh = new QuinyxHelper();
            var ph = new PinHelper();
            ph.RetrieveRoutesForToday();
            List<PinRouteModel> todaysRoutes = await _dbHelper.GetAllRecords<PinRouteModel>(Constants.MongoDb.OutputScreenTableName);
            return View(todaysRoutes);
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