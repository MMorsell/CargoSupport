using CargoSupport.Models;
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
        private IMongoDatabase _database { get; set; }

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

        public async Task<List<QuinyxWorkerModel>> GetAllDriversForTodaySorted(string tableName)
        {
            var collection = _database.GetCollection<QuinyxWorkerModel>(tableName);

            var today = DateTime.Today; //2017-03-31 00:00:00.000

            var filterBuilder = Builders<QuinyxWorkerModel>.Filter;
            var filter = filterBuilder.Gte(x => x.CurrentDate, today);
            var result = await collection.FindAsync(filter).Result.ToListAsync();

            result = result.OrderBy(e => e.StartShiftTime).ThenBy(e => e.EndShiftTime).ToList();

            return result;
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

        public async Task DeleteRecord<T>(string tableName, Guid guid)
        {
            var collection = _database.GetCollection<T>(tableName);
            var filter = Builders<T>.Filter.Eq("Id", guid);
            await collection.DeleteOneAsync(filter);
        }
    }
}