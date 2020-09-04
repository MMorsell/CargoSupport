using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    public class UserController : Controller
    {
        [Route("User")]
        public async Task<ActionResult> Index()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            var db = new CargoSupport.Helpers.MongoDbHelper(Constants.MongoDb.DatabaseName);
            var allUsers = await db.GetAllRecords<WhitelistModel>(Constants.MongoDb.WhitelistTable);
            return View(new UpsertUserViewModel() { CurrentUser = new WhitelistModel(), ExistingUsers = allUsers });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(WhitelistModel newUserModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            if (await AddOrUpdateUserRoleLevel(newUserModel, HttpContext.User))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }

        public async Task<ActionResult> EditAsync(string id)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            var db = new CargoSupport.Helpers.MongoDbHelper(Constants.MongoDb.DatabaseName);
            var existingUser = await db.GetRecordById<WhitelistModel>(Constants.MongoDb.WhitelistTable, id);

            if (existingUser == null)
            {
                return NotFound();
            }

            return View(existingUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(WhitelistModel newUserModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            if (await AddOrUpdateUserRoleLevel(newUserModel, HttpContext.User))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }

        public async Task<ActionResult> DeleteAsync(string id)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            var db = new CargoSupport.Helpers.MongoDbHelper(Constants.MongoDb.DatabaseName);
            var existingUser = await db.GetRecordById<WhitelistModel>(Constants.MongoDb.WhitelistTable, id);

            if (existingUser == null)
            {
                return NotFound();
            }

            return View(existingUser);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(WhitelistModel newUserModel)
        {
            var db = new CargoSupport.Helpers.MongoDbHelper(Constants.MongoDb.DatabaseName);
            await db.DeleteRecord<WhitelistModel>(Constants.MongoDb.WhitelistTable, newUserModel._Id);

            return RedirectToAction("Index");
        }
    }
}