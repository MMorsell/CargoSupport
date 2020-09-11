﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers
{
    public class AnalyzeController : Controller
    {
        private readonly ILogger _logger;
        private readonly MongoDbHelper _dbHelper;
        private readonly QuinyxHelper _qh;
        private readonly DataConversionHelper _dh;

        public AnalyzeController(ILoggerFactory logger)
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _qh = new QuinyxHelper(logger);
            _dh = new DataConversionHelper(logger);
            _logger = logger.CreateLogger("AnalyzeController");
        }

        public async Task<ActionResult> Index()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            return View();
        }

        [Route("[controller]/AllData/{id:int}")]
        public async Task<IActionResult> AllData(int id)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            if (id <= 0)
            {
                return BadRequest();
            }

            List<DataModel> allRoutes = await _dbHelper.GetAllRecordsByDriverId(Constants.MongoDb.OutputScreenTableName, id);
            var analyzeModels = await _dh.ConvertDataModelsToFullViewModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(analyzeModels);
            return View(allRoutes);
        }

        [Route("[controller]/DriverDiscreteData/{id:int}")]
        public async Task<IActionResult> DriverDiscreteData(int id)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            if (id <= 0)
            {
                return BadRequest();
            }

            ViewBag.DriverId = JsonSerializer.Serialize(id);
            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetSlim")]
        public async Task<ActionResult> GetSlim(string fromDate, string toDate)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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
            var res = await _dh.ConvertDataModelsToSlimViewModels(analyzeModels);
            return Ok(res);
        }

        public async Task<ActionResult> TodayGraphs()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetTodayGraphs")]
        public async Task<ActionResult> GetTodayGraphs(string fromDate, string toDate, bool splitData)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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
            var res = _dh.ConvertTodaysDataToGraphModelsAsParalell(analyzeModels, splitData);
            return Ok(res);
        }

        [HttpGet]
        [Route("api/[controller]/GetTodayGraphsForDriver")]
        public async Task<ActionResult> GetTodayGraphsForDriver(string fromDate, string toDate, int driverId)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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
            analyzeModels = analyzeModels.Where(data => data.Driver.Id == driverId).ToList();
            var res = _dh.ConvertTodaysDataToGraphModelsAsParalell(analyzeModels, true);
            return Ok(res);
        }

        [HttpGet]
        [Route("api/[controller]/GetSimplifiedRecordsForDriver")]
        public async Task<ActionResult> GetSimplifiedRecordsForDriver(string fromDate, string toDate, int driverId)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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
            analyzeModels = analyzeModels.Where(data => data.Driver.Id == driverId).ToList();
            var res = _dh.ConvertDataToSimplifiedRecordsAsParalell(analyzeModels);
            return Ok(res);
        }

        public async Task<ActionResult> CarStats()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetCarStats")]
        public async Task<ActionResult> GetCarStats(string fromDate, string toDate)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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

            var res = _dh.ConvertDataToCarStatisticsModel(analyzeModels);
            return Ok(res);
        }

        public async Task<ActionResult> DataByGroup()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            return View();
        }

        [HttpGet]
        [Route("api/[controller]/GetUnderBoss")]
        public async Task<ActionResult> GetUnderBoss(int? sectionId, int? staffCatId, string fromDate, string toDate)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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

            if (sectionId != null)
            {
                var matchingRecordsInDatabase = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
                var recordsWithDriverNames = await _qh.AddNamesToData(matchingRecordsInDatabase);

                var matchingRecordsBySectionId = recordsWithDriverNames.Where(d => d.Driver.ExtendedInformationModel != null && d.Driver.ExtendedInformationModel.Section == sectionId).ToList();

                if (staffCatId == 28899)
                {
                    var res = _dh.ConvertDataModelsToMultipleDriverTableData(matchingRecordsBySectionId);
                    return Ok(res);
                }
                else
                {
                    /*
                     * Since all external drivers are grouped by same section id, we need extra grouping to correctly seperate by external company
                     */
                    matchingRecordsBySectionId = matchingRecordsBySectionId.Where(d => d.Driver.ExtendedInformationModel.StaffCat == staffCatId).ToList();
                    var res = _dh.ConvertDataModelsToMultipleDriverTableData(matchingRecordsBySectionId);
                    return Ok(res);
                }
            }
            else
            {
                var matchingRecordsInDatabase = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
                var recordsWithDriverNames = await _qh.AddNamesToData(matchingRecordsInDatabase);

                var res = _dh.ConvertDatRowsToBossGroup(recordsWithDriverNames.Where(d => d.Driver.ExtendedInformationModel != null).ToList());
                return Ok(res);
            }
        }

        [HttpGet]
        [Route("api/[controller]/GetSingleDriverUnderBoss")]
        public async Task<ActionResult> GetSingleDriverUnderBoss(int driverId, string fromDate, string toDate)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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

            var matchingRecordsInDatabase = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
            matchingRecordsInDatabase = matchingRecordsInDatabase.Where(rec => rec.Driver.Id.Equals(driverId)).ToList();
            var recordsWithDriverNames = await _qh.AddNamesToData(matchingRecordsInDatabase);

            var res = _dh.ConvertDataModelsToMultipleDriverTableData(recordsWithDriverNames);
            return Ok(res);
        }
    }
}