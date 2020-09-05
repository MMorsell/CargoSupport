using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CargoSupport.ViewModels.Public;
using CargoSupport.Enums;
using static CargoSupport.Helpers.AuthorizeHelper;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using CargoSupport.Models.QuinyxModels;
using System.Diagnostics;
using System.Collections.Concurrent;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CargoSupport.Web.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly MongoDbHelper _dbHelper;
        private readonly QuinyxHelper _qnHelper;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public PublicController(/*IHttpContextAccessor httpContextAccessor*/)
        {
            //_httpContextAccessor = httpContextAccessor;
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _qnHelper = new QuinyxHelper();
        }

        [HttpGet]
        public async Task<ActionResult> GetTransport(string dateString)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var carOptionsTask = _dbHelper.GetAllRecords<CarModel>(Constants.MongoDb.CarTableName);
            var driversThatWorksOnThisDateTask = _qnHelper.GetAllDriversSortedToArray(date, false);
            var dataBaseResTask = ConvertToTransport(await _dbHelper.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date));

            await Task.WhenAll(carOptionsTask, driversThatWorksOnThisDateTask, dataBaseResTask);
            return Ok(new ReturnModel
            {
                data = dataBaseResTask.Result,
                selectValues = driversThatWorksOnThisDateTask.Result,
                carOptions = carOptionsTask.Result.Where(car => car.CanBeSelected).ToArray()
            });
        }

        public class ReturnModel
        {
            public TransportViewModel[] data { get; set; }
            public QuinyxModel[] selectValues { get; set; }
            public CarModel[] carOptions { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult> GetPublic(string dateString)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var res = ConvertToPublic(await _dbHelper.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date));
            return Ok(res.Result.ToArray());
        }

        [HttpGet]
        public async Task<ActionResult> GetStorage(string dateString)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var res = ConvertToStorage(await _dbHelper.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date));
            return Ok(res.Result.ToArray());
        }

        private async Task<List<StorageViewModel>> ConvertToStorage(List<DataModel> allRoutes)
        {
            allRoutes = await _qnHelper.AddNamesToData(allRoutes);
            var returnModels = new List<StorageViewModel>();
            for (int i = 0; i < allRoutes.Count; i++)
            {
                returnModels.Add(new StorageViewModel(
                    allRoutes[i]._Id,
                    allRoutes[i].PinRouteModel.ScheduledRouteStart.TimeOfDay,
                    allRoutes[i].PinRouteModel.ScheduledRouteEnd.TimeOfDay,
                    allRoutes[i].PinRouteModel.RouteName,
                    allRoutes[i].CarModel,
                    allRoutes[i].PortNumber,
                    allRoutes[i].LoadingLevel,
                    allRoutes[i].PinRouteModel.NumberOfCustomers,
                    allRoutes[i].ControlIsDone,
                    allRoutes[i].NumberOfColdBoxes,
                    allRoutes[i].RestPicking,
                    allRoutes[i].NumberOfFrozenBoxes,
                    allRoutes[i].NumberOfBreadBoxes
                    ));
            }
            return returnModels;
        }

        private async Task<TransportViewModel[]> ConvertToTransport(List<DataModel> allRoutes)
        {
            allRoutes = await _qnHelper.AddNamesToData(allRoutes);
            var returnModels = new ConcurrentBag<TransportViewModel>();
            Parallel.ForEach(allRoutes, route =>
            {
                returnModels.Add(new TransportViewModel(
                    route._Id,
                    route.PinRouteModel.Weight,
                    route.PinRouteModel.RouteName,
                    route.Driver.ConvertToDriverViewModel(),
                    route.CarModel,
                    route.PortNumber,
                    route.LoadingLevel,
                    route.PreRideAnnotation,
                    route.PostRideAnnotation,
                    route.PinRouteModel.NumberOfCustomers,
                    route.PinRouteModel.ScheduledRouteStart.TimeOfDay,
                    route.PinRouteModel.ScheduledRouteEnd.TimeOfDay,
                    route.NumberOfColdBoxes,
                    route.RestPicking,
                    route.NumberOfFrozenBoxes,
                    route.NumberOfBreadBoxes,
                    route.ControlIsDone
                    ));
            });

            return returnModels.ToArray();
        }

        private async Task<List<TransportViewModel>> ConvertToPublic(List<DataModel> allRoutes)
        {
            allRoutes = await _qnHelper.AddNamesToData(allRoutes);
            var returnModels = new List<TransportViewModel>();
            for (int i = 0; i < allRoutes.Count; i++)
            {
                returnModels.Add(new TransportViewModel(
                    allRoutes[i].PinRouteModel.RouteName,
                    allRoutes[i].Driver.ConvertToDriverViewModel(),
                    allRoutes[i].CarModel,
                    allRoutes[i].PortNumber,
                    allRoutes[i].LoadingLevel,
                    allRoutes[i].PreRideAnnotation,
                    allRoutes[i].PinRouteModel.NumberOfCustomers,
                    allRoutes[i].PinRouteModel.ScheduledRouteStart.TimeOfDay,
                    allRoutes[i].PinRouteModel.ScheduledRouteEnd.TimeOfDay,
                    allRoutes[i].NumberOfColdBoxes,
                    allRoutes[i].RestPicking,
                    allRoutes[i].NumberOfFrozenBoxes,
                    allRoutes[i].NumberOfBreadBoxes,
                    allRoutes[i].ControlIsDone
                    ));
            }
            return returnModels;
        }
    }
}