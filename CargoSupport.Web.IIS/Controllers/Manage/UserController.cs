using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCore.Identity.MongoDbCore.Models;
using CargoSupport.Interfaces;
using CargoSupport.Models.Auth;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
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

        //Client does not want editable users
        //public async Task<ActionResult> EditAsync(string id)
        //{
        //    var existingUser = await _userManager.FindByIdAsync(id);

        //    if (existingUser == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewBag.Roles = new SelectList(_roleManager.Roles, "ROLEID", "ROLENAME");
        //    return View(await existingUser.ConvertToUserViewModel(_userManager));
        //}

        public async Task<ActionResult> DeleteAsync(string id)
        {
            var existingUser = await _userManager.FindByIdAsync(id);

            if (existingUser == null)
            {
                return NotFound();
            }

            return View(await existingUser.ConvertToUserViewModel(_userManager));
        }
    }
}