using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers
{
    public class AnalyzeController : Controller
    {
        private readonly ILogger<AnalyzeController> _logger;
        private readonly MongoDbHelper _dbHelper;
        private readonly QuinyxHelper _qh;

        public AnalyzeController(ILogger<AnalyzeController> logger)
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _qh = new QuinyxHelper();
            _logger = logger;
        }

        public async Task<ActionResult> Index()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            List<DataModel> allRoutes = await _dbHelper.GetAllRecords<DataModel>(Constants.MongoDb.OutputScreenTableName);
            var analyzeModels = CargoSupport.Helpers.DataConversionHelper.ConvertPinRouteModelToAnalyzeModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(analyzeModels);
            return View();
        }

        public async Task<IActionResult> AllData()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            List<DataModel> allRoutes = await _dbHelper.GetAllRecords<DataModel>(Constants.MongoDb.OutputScreenTableName);
            var analyzeModels = CargoSupport.Helpers.DataConversionHelper.ConvertPinRouteModelToAnalyzeModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(allRoutes);
            return View(allRoutes);
        }

        public async Task<ActionResult> DriversUnderMe()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return View();
        }
    }
}