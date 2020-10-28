using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CargoSupport.Models.DatabaseModels;
using Microsoft.AspNetCore.Mvc;
using CargoSupport.ViewModels.Public;
using System.Linq;
using CargoSupport.Models.QuinyxModels;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;
using CargoSupport.Interfaces;

namespace CargoSupport.Web.Controllers.API
{
    [Route("api/v1/public/[action]")]
    [ApiController]
    public class Public : ControllerBase
    {
        private readonly IQuinyxHelper _quinyxHelper;
        private readonly IMongoDbService _dbService;

        public Public(IQuinyxHelper quinyxHelper, IMongoDbService dbService)
        {
            _quinyxHelper = quinyxHelper;
            this._dbService = dbService;
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> GetTransport(string dateString)
        {
            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var carOptionsTask = _dbService.GetAllRecords<CarModel>(Constants.MongoDb.CarCollectionName);
            var driversThatWorksOnThisDateTask = _quinyxHelper.GetAllDriversSortedToArray(date, false);
            var dataBaseResTask = ConvertToTransport(await _dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenCollectionName, date));

            await Task.WhenAll(carOptionsTask, driversThatWorksOnThisDateTask, dataBaseResTask);
            return Ok(new ReturnModel
            {
                data = dataBaseResTask.Result,
                selectValues = driversThatWorksOnThisDateTask.Result,
                carOptions = carOptionsTask.Result.Where(car => car.CanBeSelected && !car.IsRetired).ToArray()
            });
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> GetTransportSingleRecord(string recordId)
        {
            var dataBaseResTask = await ConvertToTransport(
                new List<DataModel> { await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenCollectionName, recordId) });

            return Ok(dataBaseResTask.ToArray());
        }

        public class ReturnModel
        {
            public TransportViewModel[] data { get; set; }
            public QuinyxModel[] selectValues { get; set; }
            public CarModel[] carOptions { get; set; }
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.MedarbetareAndUp)]
        public async Task<ActionResult> GetPublic(string dateString)
        {
            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var res = ConvertToPublic(await _dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenCollectionName, date));
            return Ok(res.Result.ToArray());
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.MedarbetareAndUp)]
        public async Task<ActionResult> GetPublicSingleRecord(string recordId)
        {
            var res = await ConvertToPublic(new List<DataModel> { await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenCollectionName, recordId) });
            return Ok(res.ToArray());
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        public async Task<ActionResult> GetStorage(string dateString)
        {
            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var res = ConvertToStorage(await _dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenCollectionName, date), false);
            return Ok(res.Result.ToArray());
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.SuperUserAndPlockAnalys)]
        public async Task<ActionResult> GetStorageExtended(string dateString)
        {
            DateTime.TryParse(dateString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != dateString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{dateString}'");
            }

            var res = ConvertToStorage(await _dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenCollectionName, date), true);
            return Ok(res.Result.ToArray());
        }

        [HttpGet]
        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        public async Task<ActionResult> GetStorageSingleRecord(string recordId)
        {
            var res = await ConvertToStorage(new List<DataModel> { await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenCollectionName, recordId) }, false);
            return Ok(res.ToArray());
        }

        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        private async Task<List<StorageViewModel>> ConvertToStorage(List<DataModel> allRoutes, bool isExtended)
        {
            allRoutes = allRoutes.Where(route => !route.IsResourceRoute).ToList();
            allRoutes = await _quinyxHelper.AddNamesToData(allRoutes);
            var returnModels = new List<StorageViewModel>();
            for (int i = 0; i < allRoutes.Count; i++)
            {
                returnModels.Add(new StorageViewModel(
                    allRoutes[i]._Id,
                    allRoutes[i].PinRouteModel.ScheduledRouteStart,
                    allRoutes[i].PinRouteModel.ScheduledRouteEnd,
                    allRoutes[i].PinRouteModel.RouteName,
                    allRoutes[i].CarModel,
                    allRoutes[i].PortNumber,
                    allRoutes[i].LoadingLevel,
                    allRoutes[i].PinRouteModel.NumberOfCustomers,
                    allRoutes[i].ControlIsDone,
                    allRoutes[i].NumberOfColdBoxes,
                    allRoutes[i].RestPicking,
                    allRoutes[i].NumberOfFrozenBoxes,
                    allRoutes[i].NumberOfBreadBoxes,
                    isExtended
                    ));
            }

            return returnModels;
        }

        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        private async Task<TransportViewModel[]> ConvertToTransport(List<DataModel> allRoutes)
        {
            allRoutes = await _quinyxHelper.AddNamesToData(allRoutes);
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
                    route.PinRouteModel.ScheduledRouteStart,
                    route.PinRouteModel.ScheduledRouteEnd,
                    route.NumberOfColdBoxes,
                    route.RestPicking,
                    route.NumberOfFrozenBoxes,
                    route.NumberOfBreadBoxes,
                    route.ControlIsDone,
                    route.IsResourceRoute
                    ));
            });

            return returnModels.ToArray();
        }

        [Authorize(Roles = Constants.MinRoleLevel.MedarbetareAndUp)]
        private async Task<List<TransportViewModel>> ConvertToPublic(List<DataModel> allRoutes)
        {
            allRoutes = await _quinyxHelper.AddNamesToData(allRoutes);
            var returnModels = new List<TransportViewModel>();
            for (int i = 0; i < allRoutes.Count; i++)
            {
                returnModels.Add(new TransportViewModel(
                    allRoutes[i]._Id,
                    allRoutes[i].PinRouteModel.RouteName,
                    allRoutes[i].Driver.ConvertToDriverViewModel(),
                    allRoutes[i].CarModel,
                    allRoutes[i].PortNumber,
                    allRoutes[i].LoadingLevel,
                    allRoutes[i].PreRideAnnotation,
                    allRoutes[i].PinRouteModel.NumberOfCustomers,
                    allRoutes[i].PinRouteModel.ScheduledRouteStart,
                    allRoutes[i].PinRouteModel.ScheduledRouteEnd,
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