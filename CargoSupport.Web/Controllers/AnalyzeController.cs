using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Helpers;
using CargoSupport.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CargoSupport.Web.Controllers
{
    public class AnalyzeController : Controller
    {
        private readonly ILogger<AnalyzeController> _logger;
        private readonly MongoDbHelper _dbHelper;

        public AnalyzeController(ILogger<AnalyzeController> logger)
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _logger = logger;
        }

        public ActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> AllData()
        {
            List<PinRouteModel> allRoutes = await _dbHelper.GetAllRecords<PinRouteModel>(Constants.MongoDb.OutputScreenTableName);
            var analyzeModels = Helpers.DataConversionHelper.ConvertPinRouteModelToAnalyzeModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(allRoutes);
            return View(allRoutes);
        }

        public ActionResult DriversUnderMe()
        {
            return View();
        }
    }
}