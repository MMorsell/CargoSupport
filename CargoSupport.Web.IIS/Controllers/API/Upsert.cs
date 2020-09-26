using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Hubs;
using CargoSupport.Interfaces;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.QuinyxModels;
using CargoSupport.ViewModels;
using CargoSupport.ViewModels.Public;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace CargoSupport.Web.IIS.Controllers.API
{
    [Route("api/v1/upsert/[action]")]
    [ApiController]
    public class Upsert : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly ILogger _logger;
        private readonly IQuinyxHelper _quinyxHelper;
        private readonly IMongoDbService _dbService;

        public Upsert(IHubContext<ChatHub> chatHub, ILoggerFactory logger, IQuinyxHelper quinyxHelper, IMongoDbService dbService)
        {
            _chatHub = chatHub;
            _quinyxHelper = quinyxHelper;
            this._dbService = dbService;
            _logger = logger.CreateLogger("UpsertApiv1");
        }

        [HttpPost]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> UpsertTransport([FromBody] TransportViewModel transportViewModel)
        {
            var existingRecord = await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenTableName, transportViewModel._Id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            await UpsertTransportProperties(transportViewModel, existingRecord);

            return Ok();
        }

        private async Task UpsertTransportProperties(TransportViewModel transportViewModel, DataModel existingRecord)
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
                await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, existingRecord);
                await _chatHub.Clients.All.SendAsync("ReceiveMessage");
            }
        }

        private QuinyxModel TryGetDriverInfo(int newDriverID, DateTime date, QuinyxModel fallBackDriver)
        {
            if (newDriverID != fallBackDriver.Id)
            {
                var driversThatWorksOnThisDate = _quinyxHelper.GetAllDriversSorted(date);
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
        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        public async Task<ActionResult> UpsertStorage([FromBody] StorageViewModel storageViewModel)
        {
            var existingRecord = await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenTableName, storageViewModel._Id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            await UpsertStorageProperties(storageViewModel, existingRecord);

            return Ok();
        }

        private async Task UpsertStorageProperties(StorageViewModel storageViewModel, DataModel existingRecord)
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
                await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, existingRecord);
                await _chatHub.Clients.All.SendAsync("ReceiveMessage");
            }
        }
    }
}