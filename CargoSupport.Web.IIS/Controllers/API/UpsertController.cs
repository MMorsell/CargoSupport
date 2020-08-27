using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Helpers;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels;
using CargoSupport.ViewModels.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UpsertController : ControllerBase
    {
        public UpsertController()
        {
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

        private static async Task UpsertTransportProperties(TransportViewModel transportViewModel, MongoDbHelper dbConnection, DataModel existingRecord)
        {
            var update = false;
            //existingRecord.Driver.Id = transportViewModel.Driver.Id;
            //existingRecord.CarModel = transportViewModel.CarNumber;
            if (transportViewModel.PortNumber != -1)
            {
                existingRecord.PortNumber = transportViewModel.PortNumber;
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
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpsertStorage([FromBody] StorageViewModel storageViewModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return Ok();
        }

        private static async Task UpsertStorage(TransportViewModel transportViewModel, MongoDbHelper dbConnection, DataModel existingRecord)
        {
            //var update = false;
            ////existingRecord.Driver.Id = transportViewModel.Driver.Id;
            ////existingRecord.CarModel = transportViewModel.CarNumber;
            //if (transportViewModel.PortNumber != -1)
            //{
            //    existingRecord.PortNumber = transportViewModel.PortNumber;
            //    update = true;
            //}
            //else if ((int)transportViewModel.LoadingLevel != -1)
            //{
            //    existingRecord.LoadingLevel = transportViewModel.LoadingLevel;
            //    update = true;
            //}
            //else if (transportViewModel.PreRideAnnotation != "")
            //{
            //    existingRecord.PreRideAnnotation = transportViewModel.PreRideAnnotation;
            //    update = true;
            //}
            //else if (transportViewModel.PostRideAnnotation != "")
            //{
            //    existingRecord.PostRideAnnotation = transportViewModel.PostRideAnnotation;
            //    update = true;
            //}

            //if (update)
            //{
            //    await dbConnection.UpsertRecordByNativeGuid(Constants.MongoDb.OutputScreenTableName, existingRecord.Id, existingRecord);
            //}
        }
    }
}