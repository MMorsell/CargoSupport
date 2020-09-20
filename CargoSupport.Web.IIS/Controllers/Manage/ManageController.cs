using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Extensions;
using CargoSupport.Helpers;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    public class ManageController : Controller
    {
        public async Task<IActionResult> GetFromPin()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> GetFromPin(PinIdModel model)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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

            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> UpdatePinDataByOrder()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> UpdatePinDataByOrder(PinIdModel model)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
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

            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> AddResourceRoute()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            var ph = new PinHelper();

            var allSelectOptions = await ph.GetAllUniqueRoutesBetweenDatesWithNames(DateTime.Now, DateTime.Now);
            return View(new OrderOptionViewModel { RoutesToSelectFrom = allSelectOptions });
        }

        [HttpPost]
        [Route("Manage/AddResourceRoute")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddResourceRoute(OrderOptionViewModel orderOptionViewModel)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            if (orderOptionViewModel.SelectedOrderId == "Välj")
            {
                return BadRequest("Att koppla resursturen till 'Välj' går inte");
            }

            var date = DateTime.Now.SetHour(6);
            var db = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var routesOfTheDay = await db.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date);

            var numberOfResourceRoutes = routesOfTheDay.Count(route => route.IsResourceRoute &&
                route.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            var existingRoute = routesOfTheDay.FirstOrDefault(r => r.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (existingRoute == null)
            {
                return BadRequest($"RuttId {orderOptionViewModel.SelectedOrderId} Gick inte att koppla till någon order i databasen på datum {date}");
            }

            var ph = new PinHelper();
            await ph.InsertNewResourceRoute($"Resurs {numberOfResourceRoutes + 1}", date, orderOptionViewModel.SelectedOrderId, existingRoute.PinRouteModel.ParentOrderName);
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> DeleteRoutesByOrderId()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            var ph = new PinHelper();

            var allSelectOptions = await ph.GetAllUniqueRoutesBetweenDatesWithNames(DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));
            return View(new OrderOptionViewModel { RoutesToSelectFrom = allSelectOptions });
        }

        [HttpPost]
        [Route("Manage/DeleteRoutesByOrderId")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRoutesByOrderId(OrderOptionViewModel orderOptionViewModel)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            if (orderOptionViewModel.SelectedOrderId == "Välj")
            {
                return BadRequest("Att ta bort 'Välj' går inte");
            }

            var db = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var routesOfOrder = await db.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));

            var matchingRoutes = routesOfOrder.Where(
                data => data.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (!matchingRoutes.Any())
            {
                return BadRequest($"No route with parent order id {orderOptionViewModel.SelectedOrderId} existed in database");
            }
            foreach (var route in matchingRoutes)
            {
                await db.DeleteRecord<DataModel>(Constants.MongoDb.OutputScreenTableName, route._Id);
            }

            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> MoveOrderDateById()
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }
            var ph = new PinHelper();

            var allSelectOptions = await ph.GetAllUniqueRoutesBetweenDatesWithNames(DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));
            return View(new OrderOptionViewModel { RoutesToSelectFrom = allSelectOptions });
        }

        [HttpPost]
        [Route("Manage/MoveOrderDateById")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveOrderDateById(OrderOptionViewModel orderOptionViewModel)
        {
            if (await IsNotAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User))
            {
                return Unauthorized();
            }

            if (orderOptionViewModel.SelectedOrderId == "Välj")
            {
                return BadRequest("Att koppla resursturen till 'Välj går inte'");
            }

            DateTime.TryParse(orderOptionViewModel.DateTimeString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != orderOptionViewModel.DateTimeString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{orderOptionViewModel.DateTimeString}'");
            }

            var db = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            var routesOfOrder = await db.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));

            var matchingRoutes = routesOfOrder.Where(
               data => data.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (!matchingRoutes.Any())
            {
                return BadRequest($"No route with parent order id {orderOptionViewModel.SelectedOrderId} existed in database");
            }
            foreach (var route in matchingRoutes)
            {
                route.DateOfRoute = date.SetHour(6);
                await db.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, route);
            }

            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }
    }
}