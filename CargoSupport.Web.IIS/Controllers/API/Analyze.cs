using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using CargoSupport.Interfaces;
using CargoSupport.Helpers;

namespace CargoSupport.Web.IIS.Controllers.API
{
    [Route("api/v1/Analyze/[action]")]
    [ApiController]
    [Authorize]
    public class Analyze : Controller
    {
        private readonly IQuinyxHelper _qh;
        private readonly IMongoDbService _dbService;
        private readonly IDataConversionHelper _dataConversionHelper;

        public Analyze(IDataConversionHelper dataConversionHelper, IQuinyxHelper quinyxHelper, IMongoDbService dbService)
        {
            _qh = quinyxHelper;
            _dbService = dbService;
            _dataConversionHelper = dataConversionHelper;
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> GetSlim(string fromDate, string toDate)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            var analyzeModels = await _qh.AddNamesToData(_dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to));
            var res = _dataConversionHelper.ConvertDataModelsToSlimViewModels(analyzeModels);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> GetTodayGraphs(string fromDate, string toDate, bool splitData)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to);
            var res = _dataConversionHelper.ConvertTodaysDataToGraphModelsAsParalell(analyzeModels, splitData);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> GetCarStats(string fromDate, string toDate)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to);

            var res = _dataConversionHelper.ConvertDataToCarStatisticsModel(analyzeModels);
            return Ok(new ReturnModel { data = res });
        }

        public class ReturnModel
        {
            public DataConversionHelper.CarStatisticsModel[] data { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public async Task<ActionResult> GetSimplifiedRecordsForDriver(string fromDate, string toDate, int driverId)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to);
            analyzeModels = analyzeModels.Where(data => data.Driver.Id == driverId).ToList();
            var res = _dataConversionHelper.ConvertDataToSimplifiedRecordsAsParalell(analyzeModels);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public async Task<ActionResult> GetTodayGraphsForDriver(string fromDate, string toDate, int driverId)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to);
            analyzeModels = analyzeModels.Where(data => data.Driver.Id == driverId).ToList();
            var res = _dataConversionHelper.ConvertTodaysDataToGraphModelsAsParalell(analyzeModels, true);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public async Task<ActionResult> GetUnderBoss(string reportingTo, int? staffCatId, int? sectionId, string fromDate, string toDate)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            var recordsWithDriverNames = await _qh.AddNamesToData(_dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to));

            if (staffCatId != null)
            {
                var matchingRecordsBySectionId = recordsWithDriverNames.Where(d => d.Driver.ExtendedInformationModel != null);

                if (staffCatId == 35858) //Role consultant - external groups
                {
                    /*
                     * Since all external drivers are grouped by same section id, we need extra grouping to correctly seperate by external company
                     */
                    matchingRecordsBySectionId = matchingRecordsBySectionId
                        .Where(d => d.Driver.ExtendedInformationModel.Section == sectionId && d.Driver.ExtendedInformationModel.StaffCat.Equals(staffCatId))
                        .ToList();

                    var res = _dataConversionHelper.ConvertDataModelsToMultipleDriverTableData(matchingRecordsBySectionId.ToList());
                    return Ok(res);
                }
                else
                {
                    //user has selected an Internal group - get by reporting to boss
                    var res = _dataConversionHelper.ConvertDataModelsToMultipleDriverTableData(
                        matchingRecordsBySectionId
                        .Where(d => d.Driver.ExtendedInformationModel.ReportingTo.Equals(reportingTo, StringComparison.CurrentCultureIgnoreCase) &&
                        d.Driver.ExtendedInformationModel.StaffCat.Equals(staffCatId))
                        .ToList());
                    return Ok(res);
                }
            }
            else
            {
                //no group has been chosen, display all groups
                var res = _dataConversionHelper.ConvertDataRowsToBossGroup(recordsWithDriverNames.Where(d => d.Driver.ExtendedInformationModel != null).ToList());
                return Ok(res);
            }
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public async Task<ActionResult> GetSingleDriverUnderBoss(int driverId, string fromDate, string toDate)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            var matchingRecordsInDatabase = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to);
            matchingRecordsInDatabase = matchingRecordsInDatabase.Where(rec => rec.Driver.Id.Equals(driverId)).ToList();
            var recordsWithDriverNames = await _qh.AddNamesToData(matchingRecordsInDatabase);

            var res = _dataConversionHelper.ConvertDataModelsToMultipleDriverTableData(recordsWithDriverNames);
            return Ok(res);
        }

        #region Helpers

        private bool DatesAreNotValid(string fromDate, string toDate, out string errorMessage, out DateTime from, out DateTime to)
        {
            DateTime.TryParse(fromDate, out DateTime fromParsed);

            if (fromParsed.ToString(@"yyyy-MM-dd") != fromDate)
            {
                errorMessage = $"fromDate is not valid, expecting 2020-01-01, recieved: '{fromDate}'";
                from = fromParsed;
                to = DateTime.Now;
                return true;
            }

            DateTime.TryParse(toDate, out DateTime toParsed);

            if (toParsed.ToString(@"yyyy-MM-dd") != toDate)
            {
                errorMessage = $"fromDate is not valid, expecting 2020-01-01, recieved: '{fromDate}'";
                from = fromParsed;
                to = toParsed;
                return true;
            }

            errorMessage = string.Empty;
            from = fromParsed;
            to = toParsed;
            return false;
        }

        #endregion Helpers
    }
}