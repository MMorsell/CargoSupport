using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using CargoSupport.Interfaces;

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

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
            analyzeModels = await _qh.AddNamesToData(analyzeModels);
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

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
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

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);

            var res = _dataConversionHelper.ConvertDataToCarStatisticsModel(analyzeModels);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public async Task<ActionResult> GetSimplifiedRecordsForDriver(string fromDate, string toDate, int driverId)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
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

            List<DataModel> analyzeModels = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
            analyzeModels = analyzeModels.Where(data => data.Driver.Id == driverId).ToList();
            var res = _dataConversionHelper.ConvertTodaysDataToGraphModelsAsParalell(analyzeModels, true);
            return Ok(res);
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public async Task<ActionResult> GetUnderBoss(int? sectionId, int? staffCatId, string fromDate, string toDate)
        {
            if (DatesAreNotValid(fromDate, toDate, out string errorMessage, out DateTime from, out DateTime to))
            {
                return BadRequest(errorMessage);
            }

            if (sectionId != null)
            {
                var matchingRecordsInDatabase = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
                var recordsWithDriverNames = await _qh.AddNamesToData(matchingRecordsInDatabase);

                var matchingRecordsBySectionId = recordsWithDriverNames.Where(d => d.Driver.ExtendedInformationModel != null && d.Driver.ExtendedInformationModel.Section == sectionId).ToList();

                if (staffCatId == 28899)
                {
                    var res = _dataConversionHelper.ConvertDataModelsToMultipleDriverTableData(matchingRecordsBySectionId);
                    return Ok(res);
                }
                else
                {
                    /*
                     * Since all external drivers are grouped by same section id, we need extra grouping to correctly seperate by external company
                     */
                    matchingRecordsBySectionId = matchingRecordsBySectionId.Where(d => d.Driver.ExtendedInformationModel.StaffCat == staffCatId).ToList();
                    var res = _dataConversionHelper.ConvertDataModelsToMultipleDriverTableData(matchingRecordsBySectionId);
                    return Ok(res);
                }
            }
            else
            {
                var matchingRecordsInDatabase = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
                var recordsWithDriverNames = await _qh.AddNamesToData(matchingRecordsInDatabase);

                var res = _dataConversionHelper.ConvertDatRowsToBossGroup(recordsWithDriverNames.Where(d => d.Driver.ExtendedInformationModel != null).ToList());
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

            var matchingRecordsInDatabase = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);
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