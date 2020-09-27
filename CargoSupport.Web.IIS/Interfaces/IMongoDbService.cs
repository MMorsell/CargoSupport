using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CargoSupport.Interfaces
{
    public interface IMongoDbService
    {
        Task BackupData<T>(string collectionName, string backupCollectionName);

        Task DeleteRecord<T>(string tableName, string id);

        Task<List<T>> GetAllRecords<T>(string tableName);

        Task<List<DataModel>> GetAllRecordsBetweenDates(string tableName, DateTime from, DateTime to);

        Task<List<DataModel>> GetAllRecordsByDate(string tableName, DateTime date);

        Task<List<DataModel>> GetAllRecordsByDateAndDriverId(string tableName, int driverId, DateTime from, DateTime to);

        Task<List<DataModel>> GetAllRecordsByDriverId(string tableName, int driverId);

        Task<T> GetRecordById<T>(string tableName, string id);

        Task<DataModel> GetRecordByPinId(string tableName, PinRouteModel pinModel);

        Task<T> GetRecordByStringId<T>(string tableName, string id);

        Task InsertMultipleRecords<T>(string tableName, List<T> records);

        Task InsertRecord<T>(string tableName, T record);

        Task UpsertCarRecordById(string tableName, CarModel carModel);

        Task UpsertDataRecord(string tableName, DataModel record);

        Task UpsertMultiplePinRouteModelRecords(string tableName, List<DataModel> pinRouteModels);
    }
}