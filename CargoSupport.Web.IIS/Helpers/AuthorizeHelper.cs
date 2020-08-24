using CargoSupport.Enums;
using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class AuthorizeHelper
    {
        public static async Task<bool> IsAuthorized(List<RoleLevel> authRoleLevels, ClaimsPrincipal user)
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
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, userWithPermissionsToAdd) == false)
            {
                return false;
            }

            if (authModel.NameWithDomain == "")
            {
                return false;
            }

            var dbConnection = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var wlRecords = await dbConnection.GetAllRecords<WhitelistModel>(Constants.MongoDb.WhitelistTable);

            var matchingRecord = wlRecords.FirstOrDefault(rec => rec.NameWithDomain.Equals(authModel.NameWithDomain));

            if (matchingRecord == null)
            {
                await dbConnection.InsertRecord<WhitelistModel>(Constants.MongoDb.WhitelistTable, new WhitelistModel { NameWithDomain = authModel.NameWithDomain, RoleLevel = authModel.RoleLevel });
                return true;
            }
            else
            {
                matchingRecord.RoleLevel = authModel.RoleLevel;
                await dbConnection.UpsertRecord<WhitelistModel>(Constants.MongoDb.WhitelistTable, matchingRecord.Id, matchingRecord);
                return true;
            }
        }
    }
}