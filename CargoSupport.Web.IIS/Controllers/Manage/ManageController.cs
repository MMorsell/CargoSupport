using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Extensions;
using CargoSupport.Helpers;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.ViewModels.Manange;
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
            var ph = new PinHelper();
            List<PinRouteModel> routes = await ph.RetrieveRoutesFromActualPin(model.PinId);

            var anyExistingIdOfRouteInDatabase = await ph.AnyPinRouteModelExistInDatabase(routes);

            if (anyExistingIdOfRouteInDatabase != 0)
            {
                return BadRequest($"This order has already been downloaded from Pin");
                //return View("Error", new ErrorViewModel { Message = $"Åtgärden misslyckades eftersom någon av orderns rutter redan fanns i systemet: ruttid:'{anyExistingIdOfRouteInDatabase}' hittades" });
            }
            await ph.PopulateRoutesWithDriversAndSaveResultToDatabase(routes);

            return View("../Home/Transport");
        }

        public async Task<IActionResult> UpdatePinDataByOrder()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePinDataByOrder(PinIdModel model)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            DateTime.TryParse(model.Date, out DateTime date);

            if (model.Date != date.ToString(@"yyyy-MM-dd"))
            {
                return BadRequest($"fromDate is not valid, expecting 2020-01-01, recieved: '{model.Date}'");
            }

            var _ph = new PinHelper();
            var existingIds = await _ph.GetAllOrderIdsAsStringForThisDay(date);

            foreach (var existingId in existingIds)
            {
                int.TryParse(existingId, out int result);

                if (result != 0)
                {
                    List<PinRouteModel> routes = await _ph.RetrieveRoutesFromActualPin(result);
                    await _ph.UpdateExistingRecordsIfThereIsOne(routes);
                }
            }

            return View("../Home/Transport");
        }

        [HttpPost]
        [Route("Manage/AddResourceRoute")]
        public async Task<ActionResult> AddResourceRoute()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            var date = DateTime.Now.SetHour(6);
            var db = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var routesOfTheDay = await db.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date);
            var numberOfResourceRoutes = routesOfTheDay.Where(route => route.IsResourceRoute == true).Count();

            var ph = new PinHelper();
            await ph.InsertNewResourceRoute($"Resurs {numberOfResourceRoutes + 1}", date);
            return Ok();
        }
    }
}