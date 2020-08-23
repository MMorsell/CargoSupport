using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Helpers;
using CargoSupport.Models;
using CargoSupport.Models.PinModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize]
    public class ManageController : Controller
    {
        public async Task<IActionResult> index()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Index(PinIdModel model)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            var _ph = new PinHelper();
            List<PinRouteModel> routes = _ph.RetrieveRoutesFromActualPin(model.PinId).Result;
            _ph.PopulateRoutesWithDriversAndSaveResultToDatabase(routes).Wait();
            return View();
        }
    }
}