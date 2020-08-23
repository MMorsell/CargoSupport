using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.QuinyxModels;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
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
            return result.ToList();
        }

        public async Task<List<DataModel>> GetAllRoutesToday(string tableName)
        {
            var collection = _database.GetCollection<DataModel>(tableName);

            var today = DateTime.Today; //2017-03-31 00:00:00.000

            var filterBuilder = Builders<DataModel>.Filter;
            var filter = filterBuilder.Gte(x => x.DateOfRoute, today);
            return await collection.FindAsync(filter).Result.ToListAsync();
        }

        public async Task<T> GetRecordById<T>(string tableName, Guid guid)
        {
            var collection = _database.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("Id", guid);
            var result = await collection.FindAsync(filter);

            return result.First();
        }

        public async Task UpsertRecord<T>(string tableName, Guid guid, T record)
        {
            var collection = _database.GetCollection<T>(tableName);

            await collection.ReplaceOneAsync(
                new BsonDocument("Id", guid),
                record,
                new ReplaceOptions
                {
                    IsUpsert = true,
                });
        }

        public async Task UpsertMultiplePinRouteModelRecords(string tableName, List<DataModel> pinRouteModels)
        {
            var collection = _database.GetCollection<DataModel>(tableName);

            for (int i = 0; i < pinRouteModels.Count; i++)
            {
                await collection.ReplaceOneAsync(
                new BsonDocument("Id", pinRouteModels[i].Id),
                pinRouteModels[i],
                new ReplaceOptions
                {
                    IsUpsert = true,
                });
            }
        }

        public async Task DeleteRecord<T>(string tableName, Guid guid)
        {
            var collection = _database.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("Id", guid);
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