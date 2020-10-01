using AspNetCore.Identity.MongoDbCore.Models;
using CargoSupport.Interfaces;
using CargoSupport.Models.Auth;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public static class AuthorizeHelper
    {
        public static Dictionary<string, string> GetAllRolesToDictionary(this RoleManager<MongoIdentityRole> roleManager)
        {
            return roleManager.Roles
                .AsEnumerable() //Need this for anonymous select to work
                .Select(p => new { key = p.Id.ToString(), value = p.Name })
                .ToDictionary(kvp => kvp.key, kvp => kvp.value);
        }

        public static bool UserIsInRole(this RoleManager<MongoIdentityRole> roleManager, Guid roleId, ApplicationUser currentUser)
        {
            return currentUser.Roles.Any(role => role.Equals(roleId));
        }

        public static async Task<UserViewModel> ConvertToUserViewModel(this ApplicationUser user, UserManager<ApplicationUser> userManager)
        {
            var allRoles = await userManager.GetRolesAsync(user);

            return new UserViewModel
            {
                IdAsString = user.Id.ToString(),
                UserName = user.UserName,
                FullName = user.FullName,
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

        public static async Task<bool> AddOrUpdateCarModel(CarModel carModel, IMongoDbService dbService)
        {
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