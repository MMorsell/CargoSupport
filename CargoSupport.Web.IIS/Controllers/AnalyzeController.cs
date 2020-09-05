using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
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
                return BadRequest();
            }

            List<DataModel> allRoutes = await _dbHelper.GetAllRecordsByDriverId(Constants.MongoDb.OutputScreenTableName, id);
            var analyzeModels = await CargoSupport.Helpers.DataConversionHelper.ConvertDataModelsToFullViewModel(allRoutes);
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
                return BadRequest($"fromDate is not valid, expecting 2020-01-01, recieved: '{fromDate}'");
            }

            DateTime.TryParse(toDate, out DateTime to);

            if (to.ToString(@"yyyy-MM-dd") != toDate)
            {
                return BadRequest($"toDate is not valid, expecting 2020-01-01, recieved: '{toDate}'");
            }

            List<DataModel> analyzeModels = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
            var res = await CargoSupport.Helpers.DataConversionHelper.ConvertDataModelsToSlimViewModels(analyzeModels);
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
        public async Task<ActionResult> GetTodayGraphs(string fromDate, string toDate, bool splitData)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(fromDate, out DateTime from);

            if (from.ToString(@"yyyy-MM-dd") != fromDate)
            {
                return BadRequest($"fromDate is not valid, expecting 2020-01-01, recieved: '{fromDate}'");
            }

            DateTime.TryParse(toDate, out DateTime to);

            if (to.ToString(@"yyyy-MM-dd") != toDate)
            {
                return BadRequest($"toDate is not valid, expecting 2020-01-01, recieved: '{toDate}'");
            }

            List<DataModel> analyzeModels = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);

            var res = CargoSupport.Helpers.DataConversionHelper.ConvertTodaysDataToGraphModels(analyzeModels, splitData);
            return Ok(res);
        }

        public async Task<ActionResult> CarStats()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetCarStats")]
        public async Task<ActionResult> GetCarStats(string fromDate, string toDate)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(fromDate, out DateTime from);

            if (from.ToString(@"yyyy-MM-dd") != fromDate)
            {
                return BadRequest($"fromDate is not valid, expecting 2020-01-01, recieved: '{fromDate}'");
            }

            DateTime.TryParse(toDate, out DateTime to);

            if (to.ToString(@"yyyy-MM-dd") != toDate)
            {
                return BadRequest($"toDate is not valid, expecting 2020-01-01, recieved: '{toDate}'");
            }

            List<DataModel> analyzeModels = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);

            var res = CargoSupport.Helpers.DataConversionHelper.ConvertDataToCarStatisticsModel(analyzeModels);
            return Ok(res);
        }

        public async Task<ActionResult> AllBosses()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetUnderBoss")]
        public async Task<ActionResult> GetUnderBoss(string reportingTo, string fromDate, string toDate)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(fromDate, out DateTime from);

            if (from.ToString(@"yyyy-MM-dd") != fromDate)
            {
                return BadRequest($"fromDate is not valid, expecting 2020-01-01, recieved: '{fromDate}'");
            }

            DateTime.TryParse(toDate, out DateTime to);

            if (to.ToString(@"yyyy-MM-dd") != toDate)
            {
                return BadRequest($"toDate is not valid, expecting 2020-01-01, recieved: '{toDate}'");
            }

            //1104 - oliwer
            //900001 - Firas

            //9999999992 - Konsulter

            //9006 - Kari

            //9001 - Carl

            //9007 - Christian

            var allDriversUnderBoss = _qh.GetAllDriversWithReportingTo(reportingTo);

            var matchingRecordsInDatabase = _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
            await Task.WhenAll(allDriversUnderBoss, matchingRecordsInDatabase);

            await _qh.AddNamesToData(matchingRecordsInDatabase.Result);

            var ressadas = matchingRecordsInDatabase.Result.Where(d => d.Driver.ExtendedInformationModel != null && d.Driver.ExtendedInformationModel.ReportingTo == reportingTo).ToList();

            var res = CargoSupport.Helpers.DataConversionHelper.ConvertDataModelsToMultipleDriverTableData(ressadas);

            return Ok(res);
        }
    }
}