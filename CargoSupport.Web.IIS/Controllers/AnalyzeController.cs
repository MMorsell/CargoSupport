using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Interfaces;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class AnalyzeController : Controller
    {
        private readonly ILogger _logger;
        private readonly IQuinyxHelper _qh;
        private readonly IMongoDbService _dbService;
        private readonly IDataConversionHelper _dataConversionHelper;

        public AnalyzeController(ILoggerFactory logger, IDataConversionHelper dataConversionHelper, IQuinyxHelper quinyxHelper, IMongoDbService dbService)
        {
            _logger = logger.CreateLogger("AnalyzeController");
            _qh = quinyxHelper;
            _dbService = dbService;
            _dataConversionHelper = dataConversionHelper;
        }

        public ActionResult Index()
        {
            return View();
        }

        [Route("[controller]/AllData/{id:int}")]
        public async Task<IActionResult> AllData(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            List<DataModel> allRoutes = await _dbService.GetAllRecordsByDriverId(Constants.MongoDb.OutputScreenTableName, id);
            var analyzeModels = await _dataConversionHelper.ConvertDataModelsToFullViewModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(analyzeModels);
            return View(allRoutes);
        }

        [Route("[controller]/DriverDiscreteData/{id:int}")]
        public IActionResult DriverDiscreteData(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            ViewBag.DriverId = JsonSerializer.Serialize(id);
            return View();
        }

        public ActionResult TodayGraphs()
        {
            return View();
        }

        public ActionResult CarStats()
        {
            return View();
        }

        public ActionResult DataByGroup()
        {
            return View();
        }
    }
}