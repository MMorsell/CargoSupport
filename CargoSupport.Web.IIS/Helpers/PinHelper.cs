using CargoSupport.Extensions;
using CargoSupport.Interfaces;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class PinHelper
    {
        private readonly ApiRequestHelper _apiRequestHelper;
        private readonly IMongoDbService _dbService;

        public PinHelper(IMongoDbService dbService, IConfiguration configuration)
        {
            _apiRequestHelper = new ApiRequestHelper(configuration);
            _dbService = dbService;
        }

        /// <summary>
        /// Returns all PinRouteModels for the orderId specified
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        public async Task<List<PinRouteModel>> RetrieveRoutesFromActualPin(int orderId)
        {
            try
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
            catch (Exception ex)
            {
                Log.Logger.Error(ex, $"Exception in function RetrieveRoutesFromActualPin");
                return new List<PinRouteModel>();
            }
        }

        public async Task PopulateRoutesWithDriversAndSaveResultToDatabase(List<PinRouteModel> pinRouteModels)
        {
            try
            {
                var dbModelCollection = new List<DataModel>();
                for (int i = 0; i < pinRouteModels.Count; i++)
                {
                    dbModelCollection.Add(new DataModel
                    {
                        PinRouteModel = pinRouteModels[i],
                        //BE AWARE: Mongodb save removes timezone so it is needed to add two hours since the time is set to 00:00 in .Date therefore records move one day back
                        DateOfRoute = pinRouteModels[i].ScheduledRouteStart,
                        Driver = new QuinyxModel()
                    });
                }
                await _dbService.InsertMultipleRecords(Constants.MongoDb.OutputScreenCollectionName, dbModelCollection);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function PopulateRoutesWithDriversAndSaveResultToDatabase");
            }
        }

        public async Task UpdateExistingRecordsIfThereIsOne(List<PinRouteModel> pinRouteModels)
        {
            try
            {
                foreach (var pinModel in pinRouteModels)
                {
                    var existingRecord = await _dbService.GetRecordByPinId(Constants.MongoDb.OutputScreenCollectionName, pinModel);

                    if (existingRecord != null)
                    {
                        existingRecord.PinRouteModel = pinModel;
                        await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenCollectionName, existingRecord);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function UpdateExistingRecordsIfThereIsOne");
            }
        }

        internal async Task InsertNewResourceRoute(string routeName, DateTime date, string parentOrderIdToBindTo, string parentOrderName)
        {
            try
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

                await _dbService.InsertRecord(Constants.MongoDb.OutputScreenCollectionName, newResourceRoute);
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function InsertNewResourceRoute");
            }
        }

        public async Task<List<string>> GetAllOrderIdsAsStringForThisDay(DateTime date)
        {
            try
            {
                var allRecords = await _dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenCollectionName, date);

                var res = allRecords.Select(rec => rec.PinRouteModel.ParentOrderId).Distinct().ToList();
                return res;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function GetAllOrderIdsAsStringForThisDay");
                return new List<string>();
            }
        }

        public async Task<int> AnyPinRouteModelExistInDatabase(List<PinRouteModel> pinRouteModels)
        {
            try
            {
                foreach (var pinModel in pinRouteModels)
                {
                    var existingRecord = await _dbService.GetRecordByPinId(Constants.MongoDb.OutputScreenCollectionName, pinModel);

                    if (existingRecord != null)
                    {
                        return existingRecord.PinRouteModel.RouteId;
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function AnyPinRouteModelExistInDatabase");
                return 0;
            }
        }

        public async Task<Dictionary<string, string>> GetAllUniqueRoutesBetweenDatesWithNames(DateTime from, DateTime to)
        {
            try
            {
                var returnDictionary = new Dictionary<string, string>();
                var allRecords = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, from, to);

                var allIds = allRecords.Select(rec => rec.PinRouteModel.ParentOrderId).Where(r => r != null && r != "" && r != "0").Distinct().ToList();
                var allNames = allRecords.Select(rec => rec.PinRouteModel.ParentOrderName).Where(r => r != null && r != "" && r != "0").Distinct().ToList();
                for (int i = 0; i < allIds.Count; i++)
                {
                    returnDictionary.Add(allIds[i], allNames[i]);
                }

                return returnDictionary;
            }
            catch (Exception ex)
            {
                Log.Logger.Error(ex, "Exception in function GetAllUniqueRoutesBetweenDatesWithNames");
                return new Dictionary<string, string>();
            }
        }
    }
}