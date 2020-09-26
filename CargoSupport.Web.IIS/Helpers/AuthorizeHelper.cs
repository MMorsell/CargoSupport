using CargoSupport.Interfaces;
using CargoSupport.Models.Auth;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public static class AuthorizeHelper
    {
        public static async Task<UserViewModel> ConvertToUserViewModel(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var allRoles = await userManager.GetRolesAsync(user);

            return new UserViewModel
            {
                IdAsString = user.Id.ToString(),
                UserName = user.UserName,
                RolesCombined = GetAllRolesCombined(allRoles)
            };
        }

        private static string GetAllRolesCombined(IList<string> allRoles)
        {
            string delimiter = ",";
            if (allRoles.Count == 0)
            {
                return "";
            }
            else
            {
                return allRoles.Aggregate((role, nxtRole) => role + delimiter + nxtRole);
            }
        }

        public static async Task<bool> AddOrUpdateUserRoleLevel(WhitelistModel authModel, ClaimsPrincipal userWithPermissionsToAdd, IMongoDbService dbService)
        {
            if (authModel.NameWithDomain == "")
            {
                return false;
            }

            var matchingRecord = await dbService.GetRecordById<WhitelistModel>(Constants.MongoDb.WhitelistTable, authModel._Id);

            if (matchingRecord == null)
            {
                await dbService.InsertRecord(Constants.MongoDb.WhitelistTable, authModel);
                return true;
            }
            else
            {
                await dbService.UpsertWhitelistRecordById(Constants.MongoDb.WhitelistTable, authModel);
                return true;
            }
        }

        public static async Task<bool> AddOrUpdateCarModel(CarModel carModel, ClaimsPrincipal userWithPermissionsToAdd, IMongoDbService dbService)
        {
            if (carModel.Name.Trim() == "")
            {
                return false;
            }

            var matchingRecord = await dbService.GetRecordById<CarModel>(Constants.MongoDb.CarTableName, carModel._Id);

            if (matchingRecord == null)
            {
                await dbService.InsertRecord(Constants.MongoDb.CarTableName, carModel);
                return true;
            }
            else
            {
                await dbService.UpsertCarRecordById(Constants.MongoDb.CarTableName, carModel);
                return true;
            }
        }
    }
}