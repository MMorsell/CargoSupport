using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Helpers;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    public class ManageController : Controller
    {
        public async Task<IActionResult> GetFromPin()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetFromPin(PinIdModel model)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            //var _ph = new PinHelper();
            //List<PinRouteModel> routes = _ph.RetrieveRoutesFromActualPin(model.PinId).Result;
            //_ph.PopulateRoutesWithDriversAndSaveResultToDatabase(routes).Wait();
            return View("../Home/Index");
        }

        public async Task<IActionResult> AddOrUpdateUser()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdateUser(WhitelistModel authModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            //return Json(new { status = "success", message = "customer created" });
            if (await AddOrUpdateUserRoleLevel(authModel, HttpContext.User))
            {
                return View("../Home/Index");
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }
    }
}