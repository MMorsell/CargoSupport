﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Helpers;
using CargoSupport.Web.Models.DatabaseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using System.Security.Claims;
using System.Diagnostics;
using CargoSupport.Web.ViewModels.Public;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CargoSupport.Web.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly MongoDbHelper _dbHelper;
        private readonly QuinyxHelper _qnHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PublicController(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _qnHelper = new QuinyxHelper();
        }

        [HttpGet]
        public async Task<ActionResult> GetTransport(string date)
        {
            var res = ConvertToTransport(await _dbHelper.GetAllRecords<DataModel>(Constants.MongoDb.OutputScreenTableName));
            return Ok(res.ToArray());
        }

        [HttpGet]
        public async Task<ActionResult> GetStorage(string date)
        {
            var res = ConvertToStorage(await _dbHelper.GetAllRecords<DataModel>(Constants.MongoDb.OutputScreenTableName));
            return Ok(res.ToArray());
        }

        [HttpPost]
        public async Task<ActionResult> UpdateRecord([FromBody] object model)
        {
            return Ok();
        }

        private List<StorageViewModel> ConvertToStorage(List<DataModel> allRoutes)
        {
            allRoutes = _qnHelper.AddNamesToData(allRoutes);
            var returnModels = new List<StorageViewModel>();
            for (int i = 0; i < allRoutes.Count; i++)
            {
                returnModels.Add(new StorageViewModel(
                    allRoutes[i].Id,
                    allRoutes[i].PinRouteModel.RouteName,
                    allRoutes[i].PinRouteModel.ScheduledRouteStart.TimeOfDay,
                    allRoutes[i].NumberOfColdBoxes,
                    allRoutes[i].NumberOfFrozenBoxes,
                    allRoutes[i].PinRouteModel.NumberOfCustomers,
                    DateTime.Now.TimeOfDay,
                    "restplock",
                    DateTime.Now.TimeOfDay
                    ));
            }
            return returnModels;
        }

        private List<TransportViewModel> ConvertToTransport(List<DataModel> allRoutes)
        {
            allRoutes = _qnHelper.AddNamesToData(allRoutes);
            var returnModels = new List<TransportViewModel>();
            for (int i = 0; i < allRoutes.Count; i++)
            {
                returnModels.Add(new TransportViewModel(
                    allRoutes[i].Id,
                    allRoutes[i].PinRouteModel.RouteName,
                    allRoutes[i].Driver.GetDriverName(),
                    allRoutes[i].PinRouteModel.ScheduledRouteStart.TimeOfDay,
                    allRoutes[i].PinRouteModel.RouteHasStarted,
                    allRoutes[i].NumberOfColdBoxes,
                    allRoutes[i].NumberOfFrozenBoxes,
                    allRoutes[i].PreRideAnnotation,
                    0,
                    0,
                    allRoutes[i].PinRouteModel.NumberOfCustomers,
                    DateTime.Now.TimeOfDay,
                    "restplock",
                    DateTime.Now.TimeOfDay
                    ));
            }
            return returnModels;
        }
    }
}