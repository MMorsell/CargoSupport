using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class ManageController : Controller
    {
        private readonly IMongoDbService _dbService;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IConfiguration _configuration;

        public ManageController(
            IMongoDbService dbService,
            IHubContext<ChatHub> chatHub,
            IConfiguration configuration)
        {
            _chatHub = chatHub;
            this._configuration = configuration;
            this._dbService = dbService;
        }

        public IActionResult GetFromPin()
        {
            return View();
        }

        // GET: ManageController/Register
        public ActionResult RegisterCustomerReports(object file)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCustomerReport(IFormFile file)
        {
            //var uploads = Path.Combine(_environment.WebRootPath, "uploads");
            //if (file.Length > 0)
            //{
            //    using (var fileStream = new FileStream(Path.Combine(uploads, file.FileName), FileMode.Create))
            //    {
            //        await file.CopyToAsync(fileStream);
            //    }
            //}
            return Ok();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> UploadCustomerReport(object files)
        //{
        //    //foreach (IFormFile source in files)
        //    //{
        //    //    string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.Trim('"');

        //    //    filename = this.EnsureCorrectFilename(filename);

        //    //    //using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename)))
        //    //    //    await source.CopyToAsync(output);
        //    //}

        //    return this.View();
        //}

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> GetFromPin(PinIdModel model)
        {
            var ph = new PinHelper(_dbService, _configuration);
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

            var _ph = new PinHelper(_dbService, _configuration);
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
            var ph = new PinHelper(_dbService, _configuration);

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

            //Get days of orders to select from
            var routesOfTheDay = await _dbService.GetAllRecordsBetweenDates(
                Constants.MongoDb.OutputScreenCollectionName,
                DateTime.Now.AddDays(-2).SetHour(6),
                DateTime.Now.AddDays(3).SetHour(6));

            if (!routesOfTheDay
                    .Any(route => route.PinRouteModel.ParentOrderId
                    .Equals(orderOptionViewModel.SelectedOrderId)))
            {
                return BadRequest($"RuttId {orderOptionViewModel.SelectedOrderId} Gick inte att koppla till någon order i databasen");
            }

            var existingRouteInSameOrder = routesOfTheDay
                .FirstOrDefault(route => route.PinRouteModel.ParentOrderId
                    .Equals(orderOptionViewModel.SelectedOrderId));

            var numberOfExistingResourceRoutes = routesOfTheDay.Count(route => route.IsResourceRoute &&
                route.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            var allRoutesInOrder = routesOfTheDay.Where(route =>
                route.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            DateTime resourceOrderStart = GetLastOrderStart(allRoutesInOrder).Add(new TimeSpan(0, 0, 5, 0, 0));
            DateTime resourceOrderFin = GetLastOrderFin(allRoutesInOrder).Add(new TimeSpan(0, 0, 5, 0, 0));

            var ph = new PinHelper(_dbService, _configuration);
            await ph.InsertNewResourceRoute(
                $"Resurs {existingRouteInSameOrder.PinRouteModel.ParentOrderName} " +
                $"{numberOfExistingResourceRoutes + 1}",
                existingRouteInSameOrder.DateOfRoute,
                resourceOrderStart,
                resourceOrderFin,
                orderOptionViewModel.SelectedOrderId,
                existingRouteInSameOrder.PinRouteModel.ParentOrderName);

            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        private DateTime GetLastOrderFin(IEnumerable<DataModel> routesInOrderExcludingResourceRoutes)
        {
            return routesInOrderExcludingResourceRoutes
                .Max(route => route.PinRouteModel.ScheduledRouteEnd);
        }

        private DateTime GetLastOrderStart(IEnumerable<DataModel> routesInOrderExcludingResourceRoutes)
        {
            return routesInOrderExcludingResourceRoutes
                .Max(route => route.PinRouteModel.ScheduledRouteStart);
        }

        public async Task<IActionResult> DeleteRoutesByOrderId()
        {
            var ph = new PinHelper(_dbService, _configuration);

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

            var routesOfOrder = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));

            var matchingRoutes = routesOfOrder.Where(
                data => data.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (!matchingRoutes.Any())
            {
                return BadRequest($"No route with parent order id {orderOptionViewModel.SelectedOrderId} existed in database");
            }
            foreach (var route in matchingRoutes)
            {
                await _dbService.DeleteRecord<DataModel>(Constants.MongoDb.OutputScreenCollectionName, route._Id);
            }

            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }

        public async Task<IActionResult> MoveOrderDateById()
        {
            var ph = new PinHelper(_dbService, _configuration);

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

            var routesOfOrder = await _dbService.GetAllRecordsBetweenDates(Constants.MongoDb.OutputScreenCollectionName, DateTime.Now.AddDays(-8).SetHour(6), DateTime.Now.AddDays(8).SetHour(6));

            var matchingRoutes = routesOfOrder.Where(
               data => data.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            if (!matchingRoutes.Any())
            {
                return BadRequest($"No route with parent order id {orderOptionViewModel.SelectedOrderId} existed in database");
            }
            foreach (var route in matchingRoutes)
            {
                route.DateOfRoute = date.SetHour(6);
                await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenCollectionName, route);
            }
            await _chatHub.Clients.All.SendAsync("ReloadDataTable");
            return RedirectToAction(nameof(HomeController.Transport), "Home");
        }
    }
}