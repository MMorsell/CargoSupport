using CargoSupport.Enums;
using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public static class AuthorizeHelper
    {
        public static async Task<bool> AddOrUpdateUserRoleLevel(WhitelistModel authModel, ClaimsPrincipal userWithPermissionsToAdd)
        {
            if (authModel.NameWithDomain == "")
            {
                return false;
            }

            var dbConnection = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var matchingRecord = await dbConnection.GetRecordById<WhitelistModel>(Constants.MongoDb.WhitelistTable, authModel._Id);

            if (matchingRecord == null)
            {
                await dbConnection.InsertRecord(Constants.MongoDb.WhitelistTable, authModel);
                return true;
            }
            else
            {
                await dbConnection.UpsertWhitelistRecordById(Constants.MongoDb.WhitelistTable, authModel);
                return true;
            }
        }

        public static async Task<bool> AddOrUpdateCarModel(CarModel carModel, ClaimsPrincipal userWithPermissionsToAdd)
        {
            if (carModel.Name.Trim() == "")
            {
                return false;
            }

            var dbConnection = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var matchingRecord = await dbConnection.GetRecordById<CarModel>(Constants.MongoDb.CarTableName, carModel._Id);

            if (matchingRecord == null)
            {
                await dbConnection.InsertRecord(Constants.MongoDb.CarTableName, carModel);
                return true;
            }
            else
            {
                await dbConnection.UpsertCarRecordById(Constants.MongoDb.CarTableName, carModel);
                return true;
            }
        }
    }
}