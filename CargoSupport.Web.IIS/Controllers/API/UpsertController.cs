using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Helpers;
using CargoSupport.Hubs;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.QuinyxModels;
using CargoSupport.ViewModels;
using CargoSupport.ViewModels.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UpsertController : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;

        public UpsertController(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }

        [HttpPost]
        public async Task<ActionResult> UpsertTransport([FromBody] TransportViewModel transportViewModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            var dbConnection = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var existingRecord = await dbConnection.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenTableName, transportViewModel._Id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            await UpsertTransportProperties(transportViewModel, dbConnection, existingRecord);

            return Ok();
        }

        private async Task UpsertTransportProperties(TransportViewModel transportViewModel, MongoDbHelper dbConnection, DataModel existingRecord)
        {
            var update = false;
            if (transportViewModel.Driver.Id != 0)
            {
                if (transportViewModel.Driver.Id == -1)
                {
                    existingRecord.Driver = new QuinyxModel();
                }
                else
                {
                    existingRecord.Driver = TryGetDriverInfo(transportViewModel.Driver.Id, existingRecord.DateOfRoute, existingRecord.Driver);
                }
                update = true;
            }
            else if (transportViewModel.PortNumber != -1)
            {
                existingRecord.PortNumber = transportViewModel.PortNumber;
                update = true;
            }
            else if (transportViewModel.CarNumber != null)
            {
                existingRecord.CarModel = transportViewModel.CarNumber;
                update = true;
            }
            else if ((int)transportViewModel.LoadingLevel != -1)
            {
                existingRecord.LoadingLevel = transportViewModel.LoadingLevel;
                update = true;
            }
            else if (transportViewModel.PreRideAnnotation != null)
            {
                existingRecord.PreRideAnnotation = transportViewModel.PreRideAnnotation;
                update = true;
            }
            else if (transportViewModel.PostRideAnnotation != null)
            {
                existingRecord.PostRideAnnotation = transportViewModel.PostRideAnnotation;
                update = true;
            }

            if (update)
            {
                await dbConnection.UpsertDataRecordById(Constants.MongoDb.OutputScreenTableName, existingRecord);
                await _chatHub.Clients.All.SendAsync("ReceiveMessage");
            }
        }

        private static QuinyxModel TryGetDriverInfo(int newDriverID, DateTime date, QuinyxModel fallBackDriver)
        {
            if (newDriverID != fallBackDriver.Id)
            {
                var qh = new QuinyxHelper();
                var driversThatWorksOnThisDate = qh.GetAllDriversSorted(date);
                var matchingDriver = driversThatWorksOnThisDate.Result
                    .FirstOrDefault(dr => dr.Id.Equals(newDriverID));

                if (matchingDriver != null)
                {
                    return matchingDriver;
                }
            }
            return fallBackDriver;
        }

        [HttpPost]
        public async Task<ActionResult> UpsertStorage([FromBody] StorageViewModel storageViewModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            var dbConnection = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var existingRecord = await dbConnection.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenTableName, storageViewModel._Id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            await UpsertStorageProperties(storageViewModel, dbConnection, existingRecord);

            return Ok();
        }

        private async Task UpsertStorageProperties(StorageViewModel storageViewModel, MongoDbHelper dbConnection, DataModel existingRecord)
        {
            var update = false;
            if (storageViewModel.NumberOfColdBoxes.Signature != null)
            {
                existingRecord.NumberOfColdBoxes.Insert(0, storageViewModel.NumberOfColdBoxes);
                update = true;
            }
            else if (storageViewModel.NumberOfFrozenBoxes.Signature != null)
            {
                existingRecord.NumberOfFrozenBoxes.Insert(0, storageViewModel.NumberOfFrozenBoxes);
                update = true;
            }
            else if (storageViewModel.NumberOfBreadBoxes.Signature != null)
            {
                existingRecord.NumberOfBreadBoxes.Insert(0, storageViewModel.NumberOfBreadBoxes);
                update = true;
            }
            else if (storageViewModel.RestPicking.Signature != null)
            {
                existingRecord.RestPicking.Insert(0, storageViewModel.RestPicking);
                update = true;
            }
            else if (storageViewModel.ControlIsDone.Signature != null)
            {
                existingRecord.ControlIsDone.Insert(0, storageViewModel.ControlIsDone);
                update = true;
            }

            if (update)
            {
                await dbConnection.UpsertDataRecordById(Constants.MongoDb.OutputScreenTableName, existingRecord);
                await _chatHub.Clients.All.SendAsync("ReceiveMessage");
            }
        }
    }
}