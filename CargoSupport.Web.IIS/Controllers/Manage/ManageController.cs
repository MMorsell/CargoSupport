using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Extensions;
using CargoSupport.Helpers;
using CargoSupport.Hubs;
using CargoSupport.Interfaces;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.PinModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class ManageController : Controller
    {
        private readonly IMongoDbService _dbService;
        private readonly IHubContext<ChatHub> _chatHub;

        public ManageController(IMongoDbService dbService, IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
            this._dbService = dbService;
        }

        public IActionResult GetFromPin()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetFromPin(PinIdModel model)
        {
            var ph = new PinHelper(_dbService);
            List<PinRouteModel> routes = await ph.RetrieveRoutesFromActualPin(model.PinId);

            var anyExistingIdOfRouteInDatabase = await ph.AnyPinRouteModelExistInDatabase(routes);

            if (anyExistingIdOfRouteInDatabase != 0)
            {
                return BadRequest($"This order has already been downloaded from Pin");
            }
            await ph.PopulateRoutesWithDriversAndSaveResultToDatabase(routes);
            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public IActionResult UpdatePinDataByOrder()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdatePinDataByOrder(PinIdModel model)
        {
            DateTime.TryParse(model.Date, out DateTime date);

            if (model.Date != date.ToString(@"yyyy-MM-dd"))
            {
                return BadRequest($"fromDate is not valid, expecting 2020-01-01, recieved: '{model.Date}'");
            }

            var _ph = new PinHelper(_dbService);
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
            var ph = new PinHelper(_dbService);

            var allSelectOptions = await ph.GetAllUniqueRoutesBetweenDatesWithNames(DateTime.Now.AddDays(-1), DateTime.Now.AddDays(2));
            return View(new OrderOptionViewModel { RoutesToSelectFrom = allSelectOptions });
        }

        [HttpPost]
        [Route("Manage/AddResourceRoute")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> AddResourceRoute(OrderOptionViewModel orderOptionViewModel)
        {
            if (orderOptionViewModel.SelectedOrderId == "Välj")
            {
                return BadRequest("Att koppla resursturen till 'Välj' går inte");
            }

            var date = DateTime.Now.SetHour(6);

            var routesOfTheDay = await _dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenTableName, date);

            var numberOfResourceRoutes = routesOfTheDay.Count(route => route.IsResourceRoute &&
                route.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            var existingRoute = routesOfTheDay.FirstOrDefault(r => r.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (existingRoute == null)
            {
                return BadRequest($"RuttId {orderOptionViewModel.SelectedOrderId} Gick inte att koppla till någon order i databasen på datum {date}");
            }

            var ph = new PinHelper(_dbService);
            await ph.InsertNewResourceRoute($"Resurs {numberOfResourceRoutes + 1}", date, orderOptionViewModel.SelectedOrderId, existingRoute.PinRouteModel.ParentOrderName);
            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> DeleteRoutesByOrderId()
        {
            var ph = new PinHelper(_dbService);

            var allSelectOptions = await ph.GetAllUniqueRoutesBetweenDatesWithNames(DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));
            return View(new OrderOptionViewModel { RoutesToSelectFrom = allSelectOptions });
        }

        [HttpPost]
        [Route("Manage/DeleteRoutesByOrderId")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteRoutesByOrderId(OrderOptionViewModel orderOptionViewModel)
        {
            if (orderOptionViewModel.SelectedOrderId == "Välj")
            {
                return BadRequest("Att ta bort 'Välj' går inte");
            }

            var routesOfOrder = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));

            var matchingRoutes = routesOfOrder.Where(
                data => data.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (!matchingRoutes.Any())
            {
                return BadRequest($"No route with parent order id {orderOptionViewModel.SelectedOrderId} existed in database");
            }
            foreach (var route in matchingRoutes)
            {
                await _dbService.DeleteRecord<DataModel>(Constants.MongoDb.OutputScreenTableName, route._Id);
            }

            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> MoveOrderDateById()
        {
            var ph = new PinHelper(_dbService);

            var allSelectOptions = await ph.GetAllUniqueRoutesBetweenDatesWithNames(DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));
            return View(new OrderOptionViewModel { RoutesToSelectFrom = allSelectOptions });
        }

        [HttpPost]
        [Route("Manage/MoveOrderDateById")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> MoveOrderDateById(OrderOptionViewModel orderOptionViewModel)
        {
            if (orderOptionViewModel.SelectedOrderId == "Välj")
            {
                return BadRequest("Att koppla resursturen till 'Välj går inte'");
            }

            DateTime.TryParse(orderOptionViewModel.DateTimeString, out DateTime date);

            if (date.ToString(@"yyyy-MM-dd") != orderOptionViewModel.DateTimeString)
            {
                return BadRequest($"dateString is not valid, expecting 2020-01-01, recieved: '{orderOptionViewModel.DateTimeString}'");
            }

            var routesOfOrder = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenTableName, DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));

            var matchingRoutes = routesOfOrder.Where(
               data => data.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (!matchingRoutes.Any())
            {
                return BadRequest($"No route with parent order id {orderOptionViewModel.SelectedOrderId} existed in database");
            }
            foreach (var route in matchingRoutes)
            {
                route.DateOfRoute = date.SetHour(6);
                await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenTableName, route);
            }
            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }
    }
}