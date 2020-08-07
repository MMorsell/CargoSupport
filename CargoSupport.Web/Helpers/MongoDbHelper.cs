using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Helpers
{
    public class MongoDbHelper
    {
        private IMongoDatabase _database { get; set; }

        public MongoDbHelper(string databaseName)
        {
            _database = new MongoClient().GetDatabase(databaseName);
        }

        public async Task InsertRecord<T>(string tableName, T record)
        {
            var collection = _database.GetCollection<T>(tableName);
            await collection.InsertOneAsync(record);
        }

        public async Task<List<T>> GetMultipleRecords<T>(string tableName)
        {
            var collection = _database.GetCollection<T>(tableName);
            var result = await collection.FindAsync(new BsonDocument());
            return result.ToList();
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