using CargoSupport.Extensions;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class MongoDbHelper
    {
        private readonly IMongoDatabase _database;

        public MongoDbHelper(string databaseName)
        {
            _database = new MongoClient(Constants.MongoDb.ConnectionString).GetDatabase(databaseName);
        }

        public async Task InsertRecord<T>(string tableName, T record)
        {
            var collection = _database.GetCollection<T>(tableName);
            await collection.InsertOneAsync(record);
        }

        public async Task InsertMultipleRecords<T>(string tableName, List<T> records)
        {
            var collection = _database.GetCollection<T>(tableName);
            await collection.InsertManyAsync(records);
        }

        public async Task<List<T>> GetAllRecords<T>(string tableName)
        {
            var collection = _database.GetCollection<T>(tableName);
            var result = await collection.FindAsync(Builders<T>.Filter.Empty).Result.ToListAsync();
            return result;
        }

        public async Task UpsertDataRecord(string tableName, DataModel record)
        {
            var collection = _database.GetCollection<DataModel>(tableName);

            await collection.ReplaceOneAsync(p => p._Id == record._Id,
                            record,
                            new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertCarRecordById(string tableName, CarModel carModel)
        {
            var collection = _database.GetCollection<CarModel>(tableName);

            await collection.ReplaceOneAsync(p => p._Id == carModel._Id,
                            carModel,
                            new ReplaceOptions { IsUpsert = true });
        }

        public async Task UpsertWhitelistRecordById(string tableName, WhitelistModel whiteListModel)
        {
            var collection = _database.GetCollection<WhitelistModel>(tableName);

            await collection.ReplaceOneAsync(p => p._Id == whiteListModel._Id,
                            whiteListModel,
                            new ReplaceOptions { IsUpsert = true });
        }

        public async Task<T> GetRecordByStringId<T>(string tableName, string id)
        {
            var collection = _database.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("_Id", id);
            var result = await collection.FindAsync(filter);

            return result.First();
        }

        public async Task<T> GetRecordById<T>(string tableName, string id)
        {
            var collection = _database.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("_Id", id);
            var result = await collection.Find(filter).FirstOrDefaultAsync();

            return result;
        }

        public async Task<List<DataModel>> GetAllRecordsByDateAndDriverId(string tableName, int driverId, DateTime from, DateTime to)
        {
            DateTime minDate = from.SetHour(0).SetMinute(0);
            DateTime maxDate = to.SetHour(23).SetMinute(59);
            var collection = _database.GetCollection<DataModel>(tableName);

            var filterBuilder = Builders<DataModel>.Filter;
            var filter = filterBuilder.Where(x => x.Driver.Id.Equals(driverId) && x.DateOfRoute <= maxDate && x.DateOfRoute >= minDate);
            return await collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<List<DataModel>> GetAllRecordsByDriverId(string tableName, int driverId)
        {
            var collection = _database.GetCollection<DataModel>(tableName);

            var filterBuilder = Builders<DataModel>.Filter;
            var filter = filterBuilder.Where(x => x.Driver.Id.Equals(driverId));
            return await collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<List<DataModel>> GetAllRecordsByDate(string tableName, DateTime date)
        {
            DateTime minDate = date.SetHour(0).SetMinute(0);
            DateTime maxDate = date.SetHour(23).SetMinute(59);
            var collection = _database.GetCollection<DataModel>(tableName);

            var filterBuilder = Builders<DataModel>.Filter;
            var filter = filterBuilder.Where(x => x.DateOfRoute >= minDate && x.DateOfRoute <= maxDate);
            var res = await collection.FindAsync(filter).Result.ToListAsync();
            return res;
        }

        public async Task<List<DataModel>> GetAllRecordsBetweenDates(string tableName, DateTime from, DateTime to)
        {
            DateTime minDate = from.SetHour(0).SetMinute(0);
            DateTime maxDate = to.SetHour(23).SetMinute(59);
            var collection = _database.GetCollection<DataModel>(tableName);

            var filterBuilder = Builders<DataModel>.Filter;
            var filter = filterBuilder.Where(x => x.DateOfRoute <= maxDate && x.DateOfRoute >= minDate);
            return await collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<DataModel> GetRecordByPinId(string tableName, PinRouteModel pinModel)
        {
            var collection = _database.GetCollection<DataModel>(tableName);
            var result = await collection.Find(_ => _.PinRouteModel.RouteId == pinModel.RouteId).FirstOrDefaultAsync();

            return result;
        }

        public async Task UpsertMultiplePinRouteModelRecords(string tableName, List<DataModel> pinRouteModels)
        {
            var collection = _database.GetCollection<DataModel>(tableName);

            for (int i = 0; i < pinRouteModels.Count; i++)
            {
                await collection.ReplaceOneAsync(
                new BsonDocument("_Id", pinRouteModels[i]._Id),
                pinRouteModels[i],
                new ReplaceOptions
                {
                    IsUpsert = true,
                });
            }
        }

        public async Task DeleteRecord<T>(string tableName, string id)
        {
            var collection = _database.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("_Id", id);
            await collection.DeleteOneAsync(filter);
        }

        public async Task BackupData<T>(string collectionName, string backupCollectionName)
        {
            IMongoCollection<T> collection = _database.GetCollection<T>(collectionName);
            var result = await collection.FindAsync(Builders<T>.Filter.Empty).Result.ToListAsync();
            await _database.CreateCollectionAsync(backupCollectionName);

            IMongoCollection<T> backupCollection = _database.GetCollection<T>(backupCollectionName);
            await backupCollection.InsertManyAsync(result);
        }
    }
}