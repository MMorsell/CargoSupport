using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Models;
using CargoSupport.Interfaces;
using CargoSupport.Models;
using CargoSupport.Models.Auth;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using CargoSupport.Web.IIS.ViewModels.Auth;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    //[Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class UserController : Controller
    {
        private readonly IMongoDbService _dbService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<MongoIdentityRole> _roleManager;

        public UserController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<MongoIdentityRole> roleManager,
            IMongoDbService dbService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            this._dbService = dbService;
        }

        [Route("User")]
        public async Task<ActionResult> Index()
        {
            var userViewModels = new List<UserViewModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                userViewModels.Add(await user.ConvertToUserViewModel(_userManager));
            }
            return View(userViewModels);
        }

        public ActionResult Create()
        {
            var allRoles = _roleManager.Roles.AsEnumerable().Select(role => role.Name).ToList();

            return View(new CreateViewModel() { Roles = allRoles });
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
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            return View(await existingUser.ConvertToUserViewModel(_userManager));
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