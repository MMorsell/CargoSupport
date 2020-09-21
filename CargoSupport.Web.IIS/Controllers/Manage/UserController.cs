using System.Collections.Generic;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Interfaces;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class UserController : Controller
    {
        private readonly IMongoDbService _dbService;

        public UserController(IMongoDbService dbService)
        {
            this._dbService = dbService;
        }

        [Route("User")]
        public async Task<ActionResult> Index()
        {
            var allUsers = await _dbService.GetAllRecords<WhitelistModel>(Constants.MongoDb.WhitelistTable);
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
            if (await AddOrUpdateUserRoleLevel(newUserModel, HttpContext.User, _dbService))
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
            var existingUser = await _dbService.GetRecordById<WhitelistModel>(Constants.MongoDb.WhitelistTable, id);

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
            if (await AddOrUpdateUserRoleLevel(newUserModel, HttpContext.User, _dbService))
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
            var existingUser = await _dbService.GetRecordById<WhitelistModel>(Constants.MongoDb.WhitelistTable, id);

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
            await _dbService.DeleteRecord<WhitelistModel>(Constants.MongoDb.WhitelistTable, newUserModel._Id);

            return RedirectToAction("Index");
        }
    }
}