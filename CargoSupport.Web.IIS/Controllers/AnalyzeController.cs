using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Interfaces;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CargoSupport.Web.IIS.Controllers
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class AnalyzeController : Controller
    {
        private readonly IQuinyxHelper _qh;
        private readonly IMongoDbService _dbService;
        private readonly IDataConversionHelper _dataConversionHelper;

        public AnalyzeController(IDataConversionHelper dataConversionHelper, IQuinyxHelper quinyxHelper, IMongoDbService dbService)
        {
            _qh = quinyxHelper;
            _dbService = dbService;
            _dataConversionHelper = dataConversionHelper;
        }

        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public ActionResult Index()
        {
            return View();
        }

        [Route("[controller]/AllData/{id:int}")]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<IActionResult> AllData(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            List<DataModel> allRoutes = await _dbService.GetAllRecordsByDriverId(Constants.MongoDb.OutputScreenCollectionName, id);
            var analyzeModels = await _dataConversionHelper.ConvertDataModelsToFullViewModel(allRoutes);
            ViewBag.DataTable = JsonSerializer.Serialize(analyzeModels);
            return View(allRoutes);
        }

        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public ActionResult TodayGraphs()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public ActionResult CarStats()
        {
            return View();
        }

        [Route("[controller]/DriverDiscreteData/{id:int}")]
        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public IActionResult DriverDiscreteData(int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            ViewBag.DriverId = JsonSerializer.Serialize(id);
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.GruppChefAndUp)]
        public ActionResult DataByGroup()
        {
            return View();
        }
    }
}