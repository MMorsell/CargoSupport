using CargoSupport.Extensions;
using CargoSupport.Helpers;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using Microsoft.AspNetCore.Mvc.DataAnnotations;
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
                route.ParentOrderName = orderRecord.name;
                route.ParentOrderId = orderRecord.id;
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
                    //BE AWARE: Mongodb save removes timezone so it is needed to add two hours since the time is set to 00:00 in .Date therefore records move one day back
                    DateOfRoute = pinRouteModels[i].ScheduledRouteStart
                });
            }
            await PopulateAllRoutesWithDriversAndSaveToDatabase(dbModelCollection, dbModelCollection[0].DateOfRoute);
        }

        private async Task PopulateAllRoutesWithDriversAndSaveToDatabase(List<DataModel> allRoutesForToday, DateTime dateToGetDriversFrom)
        {
            //allRoutesForToday = allRoutesForToday.OrderBy(r => r.PinRouteModel.ScheduledRouteStart).ToList();
            //List<QuinyxModel> allDriversForToday = await _quinyxHelper.GetAllDriversSorted(dateToGetDriversFrom);

            for (int i = 0; i < allRoutesForToday.Count; i++)
            {
                //if (i < allDriversForToday.Count)
                //{
                //    allRoutesForToday[i].Driver = allDriversForToday[i];
                //}
                //else
                //{
                allRoutesForToday[i].Driver = new QuinyxModel();
                //}
            }

            await _dbHelper.InsertMultipleRecords(Constants.MongoDb.OutputScreenTableName, allRoutesForToday);
        }

        public async Task UpdateExistingRecordsIfThereIsOne(List<PinRouteModel> pinRouteModels)
        {
            foreach (var pinModel in pinRouteModels)
            {
                var existingRecord = await _dbHelper.GetRecordByPinId(Constants.MongoDb.OutputScreenTableName, pinModel);

                if (existingRecord != null)
                {
                    existingRecord.PinRouteModel = pinModel;
                    await _dbHelper.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, existingRecord);
                }
            }
        }

        internal async Task InsertNewResourceRoute(string routeName, DateTime date, string parentOrderIdToBindTo, string parentOrderName)
        {
            date = date.SetHour(6);
            var newResourceRoute = new DataModel
            {
                Driver = new QuinyxModel(),
                PinRouteModel = new PinRouteModel(),
            };
            newResourceRoute.IsResourceRoute = true;
            newResourceRoute.DateOfRoute = date;
            newResourceRoute.PinRouteModel.RouteName = routeName;
            newResourceRoute.PinRouteModel.ScheduledRouteStart = DateTime.Now.SetHour(23);
            newResourceRoute.PinRouteModel.ScheduledRouteEnd = DateTime.Now.SetHour(23).SetMinute(59);
            newResourceRoute.PinRouteModel.ParentOrderId = parentOrderIdToBindTo;
            newResourceRoute.PinRouteModel.ParentOrderName = parentOrderName;

            await _dbHelper.InsertRecord(Constants.MongoDb.OutputScreenTableName, newResourceRoute);
        }

        public async Task<List<string>> GetAllOrderIdsAsStringForThisDay(DateTime date)
        {
            var allRecords = await _dbHelper.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date);

            var res = allRecords.Select(rec => rec.PinRouteModel.ParentOrderId).Distinct().ToList();
            return res;
        }

        public async Task<int> AnyPinRouteModelExistInDatabase(List<PinRouteModel> pinRouteModels)
        {
            foreach (var pinModel in pinRouteModels)
            {
                var existingRecord = await _dbHelper.GetRecordByPinId(Constants.MongoDb.OutputScreenTableName, pinModel);

                if (existingRecord != null)
                {
                    return existingRecord.PinRouteModel.RouteId;
                }
            }
            return 0;
        }

        public async Task<Dictionary<string, string>> GetAllUniqueRoutesBetweenDatesWithNames(DateTime from, DateTime to)
        {
            var returnDictionary = new Dictionary<string, string>();
            var allRecords = await _dbHelper.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, from, to);

            var allIds = allRecords.Select(rec => rec.PinRouteModel.ParentOrderId).Where(r => r != null && r != "" && r != "0").Distinct().ToList();
            var allNames = allRecords.Select(rec => rec.PinRouteModel.ParentOrderName).Where(r => r != null && r != "" && r != "0").Distinct().ToList();
            for (int i = 0; i < allIds.Count; i++)
            {
                returnDictionary.Add(allIds[i], allNames[i]);
            }

            return returnDictionary;
        }
    }
}