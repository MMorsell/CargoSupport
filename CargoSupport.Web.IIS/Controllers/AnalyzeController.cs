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

            return View();
        }

        [Route("[controller]/AllData/{id:int}")]
        public async Task<IActionResult> AllData(int id)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            if (id <= 0)
            {
                return Unauthorized();
            }

            List<DataModel> allRoutes = await _dbHelper.GetAllRecordsByDriverId(Constants.MongoDb.OutputScreenTableName, id);
            var analyzeModels = CargoSupport.Helpers.DataConversionHelper.ConvertDataModelsToFullViewModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(analyzeModels);
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

        [HttpGet]
        [Route("api/[controller]/GetSlim")]
        public async Task<ActionResult> GetSlim(string fromDate, string toDate)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(fromDate, out DateTime from);

            if (from.ToString(@"yyyy-MM-dd") != fromDate)
            {
                //TODO: Implement return invalid date
            }

            DateTime.TryParse(toDate, out DateTime to);

            if (to.ToString(@"yyyy-MM-dd") != toDate)
            {
                //TODO: Implement return invalid date
            }

            List<DataModel> analyzeModels = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
            var res = CargoSupport.Helpers.DataConversionHelper.ConvertDataModelsToSlimViewModels(analyzeModels);
            return Ok(res);
        }

        public async Task<ActionResult> TodayGraphs()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetTodayGraphs")]
        public async Task<ActionResult> GetTodayGraphs(string fromDate, string toDate)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(fromDate, out DateTime from);

            if (from.ToString(@"yyyy-MM-dd") != fromDate)
            {
                //TODO: Implement return invalid date
            }

            DateTime.TryParse(toDate, out DateTime to);

            if (to.ToString(@"yyyy-MM-dd") != toDate)
            {
                //TODO: Implement return invalid date
            }

            List<DataModel> analyzeModels = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);

            var res = CargoSupport.Helpers.DataConversionHelper.ConvertTodaysDataToGraphModels(analyzeModels);
            return Ok(res);
        }
    }
}