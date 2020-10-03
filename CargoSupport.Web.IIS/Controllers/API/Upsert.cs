using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Hubs;
using CargoSupport.Interfaces;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.QuinyxModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Serilog;

namespace CargoSupport.Web.IIS.Controllers.API
{
    [Route("api/v1/upsert/[action]")]
    [ApiController]
    public class Upsert : ControllerBase
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IQuinyxHelper _quinyxHelper;
        private readonly IMongoDbService _dbService;

        public Upsert(IHubContext<ChatHub> chatHub, IQuinyxHelper quinyxHelper, IMongoDbService dbService)
        {
            _chatHub = chatHub;
            _quinyxHelper = quinyxHelper;
            this._dbService = dbService;
        }

        [HttpPost]
        [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
        public async Task<ActionResult> UpsertTransport([FromBody] Dictionary<string, object> upsertDictionary)
        {
            //Keys
            //* driver: driver_select
            //* preRideAnnotation: preRideInput
            //* postRideAnnotation: postRideInput
            //* portNumber: port_selectBox
            //* carNumber: carNumber_selectBox
            //* loadingLevel: convert_loadingLevel_toSelectbox

            var existingRecord = await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenTableName, upsertDictionary["_Id"].ToString());

            if (existingRecord == null)
            {
                return NotFound();
            }

            await UpsertTransportProperties(upsertDictionary, existingRecord);

            return Ok();
        }

        private async Task UpsertTransportProperties(Dictionary<string, object> upsertDirectory, DataModel existingRecord)
        {
            var update = false;
            var updateKeyValuePair = upsertDirectory.FirstOrDefault(pair => pair.Key != "hubId" && pair.Key != "_Id");

            switch (updateKeyValuePair.Key.ToLowerInvariant())
            {
                case "driver_select":
                    if (int.Parse(updateKeyValuePair.Value.ToString()) != 0)
                    {
                        if (int.Parse(updateKeyValuePair.Value.ToString()) == -1)
                        {
                            existingRecord.Driver = new QuinyxModel();
                            upsertDirectory.Add("driver_fullName", "");
                            update = true;
                        }
                        else
                        {
                            existingRecord.Driver = TryGetDriverInfo(int.Parse(updateKeyValuePair.Value.ToString()), existingRecord.DateOfRoute, existingRecord.Driver);
                            upsertDirectory.Add("driver_fullName", existingRecord.Driver.GetDriverName());
                            update = true;
                        }
                    }
                    break;

                case "prerideinput":
                    if (updateKeyValuePair.Value != null)
                    {
                        existingRecord.PreRideAnnotation = updateKeyValuePair.Value.ToString();
                        update = true;
                    }
                    break;

                case "postrideinput":
                    if (updateKeyValuePair.Value != null)
                    {
                        existingRecord.PostRideAnnotation = updateKeyValuePair.Value.ToString();
                        update = true;
                    }
                    break;

                case "port_selectbox":
                    if (int.Parse(updateKeyValuePair.Value.ToString()) != -1)
                    {
                        existingRecord.PortNumber = int.Parse(updateKeyValuePair.Value.ToString());
                        update = true;
                    }
                    break;

                case "carnumber_selectbox":
                    if (updateKeyValuePair.Value != null)
                    {
                        existingRecord.CarModel = updateKeyValuePair.Value.ToString();
                        update = true;
                    }
                    break;

                case "convert_loadinglevel_toselectbox":
                    if (int.Parse(updateKeyValuePair.Value.ToString()) != -1)
                    {
                        existingRecord.LoadingLevel = (LoadingLevel)Enum.Parse(typeof(LoadingLevel), updateKeyValuePair.Value.ToString(), true);
                        update = true;
                    }
                    break;

                default:
                    Log.Logger.Error($"Unable to identify update property type of '{updateKeyValuePair.Key}'");
                    break;
            }

            if (update)
            {
                await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, existingRecord);
                await _chatHub.Clients.AllExcept(upsertDirectory["hubId"].ToString()).SendAsync("Upsert", upsertDirectory);
            }
        }

        private QuinyxModel TryGetDriverInfo(int newDriverID, DateTime date, QuinyxModel fallBackDriver)
        {
            if (newDriverID != fallBackDriver.Id)
            {
                var driversThatWorksOnThisDate = _quinyxHelper.GetAllDriversSorted(date, false);
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
        public async Task<ActionResult> UpsertStorage([FromBody] Dictionary<string, object> upsertDictionary)
        {
            //Keys
            //numberOfColdBoxes: numberOfColdBoxes_input
            //restPicking: restPicking_input
            //numberOfFrozenBoxes: numberOfFrozenBoxes_input
            //numberOfBreadBoxes: numberOfBreadBoxes_input
            //controlIsDone: controlIsDone_input
            var existingRecord = await _dbService.GetRecordById<DataModel>(Constants.MongoDb.OutputScreenTableName, upsertDictionary["_Id"].ToString());

            if (existingRecord == null)
            {
                return NotFound();
            }

            await UpsertStorageProperties(upsertDictionary, existingRecord);

            return Ok();
        }

        private async Task UpsertStorageProperties(Dictionary<string, object> upsertDirectory, DataModel existingRecord)
        {
            var update = false;
            var signature = upsertDirectory["signature"];
            upsertDirectory.Remove("signature");
            var updateKeyValuePair = upsertDirectory.FirstOrDefault(pair => pair.Key != "hubId" && pair.Key != "_Id");

            switch (updateKeyValuePair.Key.ToLowerInvariant())
            {
                case "numberofcoldboxes_input":
                    existingRecord.NumberOfColdBoxes.Insert(0, new PickingVerifyIntModel(int.Parse(updateKeyValuePair.Value.ToString()), signature.ToString()));
                    update = true;
                    break;

                case "numberoffrozenboxes_input":
                    existingRecord.NumberOfFrozenBoxes.Insert(0, new PickingVerifyIntModel(int.Parse(updateKeyValuePair.Value.ToString()), signature.ToString()));
                    update = true;
                    break;

                case "numberofbreadboxes_input":
                    existingRecord.NumberOfBreadBoxes.Insert(0, new PickingVerifyIntModel(int.Parse(updateKeyValuePair.Value.ToString()), signature.ToString()));
                    update = true;
                    break;

                case "restpicking_input":
                    existingRecord.RestPicking.Insert(0, new PickingVerifyBooleanModel(bool.Parse(updateKeyValuePair.Value.ToString()), signature.ToString()));
                    update = true;
                    break;

                case "controlisdone_input":
                    existingRecord.ControlIsDone.Insert(0, new PickingVerifyBooleanModel(bool.Parse(updateKeyValuePair.Value.ToString()), signature.ToString()));
                    update = true;
                    break;

                default:
                    Log.Logger.Error($"Unable to identify update property type of '{updateKeyValuePair.Key}'");
                    break;
            }

            if (update)
            {
                await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, existingRecord);
                await _chatHub.Clients.AllExcept(upsertDirectory["hubId"].ToString()).SendAsync("Upsert", upsertDirectory);
            }
        }
    }
}