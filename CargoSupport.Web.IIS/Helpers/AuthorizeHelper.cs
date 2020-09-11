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
        public static async Task<bool> IsNotAuthorized(List<RoleLevel> authRoleLevels, ClaimsPrincipal user)
        {
            //string userName = user.FindFirstValue(ClaimTypes.Name);

            //if (userName == "")
            //{
            //    return false;
            //}

            //var dbConnection = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            //var wlRecords = await dbConnection.GetAllRecords<WhitelistModel>(Constants.MongoDb.WhitelistTable);

            //var matchingRecord = wlRecords.FirstOrDefault(rec => rec.NameWithDomain.Equals(userName));

            //if (matchingRecord == null)
            //{
            //    return false;
            //}

            //if (authRoleLevels.Contains(matchingRecord.RoleLevel))
            //{
            //    return true;
            //}
            return true;
        }

        public static async Task<bool> AddOrUpdateUserRoleLevel(WhitelistModel authModel, ClaimsPrincipal userWithPermissionsToAdd)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, userWithPermissionsToAdd))
            {
                return false;
            }

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
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, userWithPermissionsToAdd))
            {
                return false;
            }

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