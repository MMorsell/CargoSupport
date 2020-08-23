using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class PinHelper
    {
        private readonly MongoDbHelper _dbHelper;
        private readonly ApiRequestHelper _apiRequestHelper;
        private readonly QuinyxHelper _quinyxHelper;

        public PinHelper()
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
            _apiRequestHelper = new ApiRequestHelper();
            _quinyxHelper = new QuinyxHelper();
        }

        /// <summary>
        /// Returns all PinRouteModels for the orderId specified
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<List<PinRouteModel>> RetrieveRoutesFromActualPin(int orderId)
        {
            var orderRecord = await _apiRequestHelper.GetSingleResult<OrderRecord>(Constants.PinApi.GetOrder(orderId));

            var routeResult = await _apiRequestHelper.GetRoutesBatchParalellAsync(orderRecord.routes.Select(r => r.RouteId).ToList());

            Parallel.ForEach(routeResult, (route) =>
            {
                route.CalculateProperties();
            });

            return routeResult;
        }

        public async Task PopulateRoutesWithDriversAndSaveResultToDatabase(List<PinRouteModel> pinRouteModels)
        {
            var dbModelCollection = new List<DataModel>();
            for (int i = 0; i < pinRouteModels.Count; i++)
            {
                dbModelCollection.Add(new DataModel
                {
                    PinRouteModel = pinRouteModels[i],
                    DateOfRoute = pinRouteModels[i].ScheduledRouteStart
                });
            }
            await PopulateAllRoutesWithDriversAndSaveToDatabase(dbModelCollection, DateTime.Now.AddDays(1));
        }

        private async Task PopulateAllRoutesWithDriversAndSaveToDatabase(List<DataModel> allRoutesForToday, DateTime dateToGetDriversFrom)
        {
            allRoutesForToday = allRoutesForToday.OrderBy(r => r.PinRouteModel.ScheduledRouteStart).ToList();
            List<QuinyxModel> allDriversForToday = _quinyxHelper.GetAllDriversSorted(dateToGetDriversFrom);

            for (int i = 0; i < allRoutesForToday.Count; i++)
            {
                if (i < allDriversForToday.Count)
                {
                    allRoutesForToday[i].Driver = allDriversForToday[i];
                }
                else
                {
                    allRoutesForToday[i].Driver = allDriversForToday[0];
                }
            }

            await _dbHelper.InsertMultipleRecords(Constants.MongoDb.OutputScreenTableName, allRoutesForToday);

            foreach (var route in allRoutesForToday)
            {
                Console.WriteLine($"{route.PinRouteModel.RouteName} {route.PinRouteModel.ScheduledRouteStart} - {route.Driver.begTime}/{route.Driver.endTime}");
            }
        }
    }
}