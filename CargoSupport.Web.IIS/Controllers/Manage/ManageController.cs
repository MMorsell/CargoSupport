using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Serilog;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class ManageController : Controller
    {
        private readonly IMongoDbService _dbService;
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _env;

        public ManageController(
            IMongoDbService dbService,
            IHubContext<ChatHub> chatHub,
            IConfiguration configuration,
            IWebHostEnvironment env)
        {
            _chatHub = chatHub;
            this._configuration = configuration;
            this._env = env;
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
            var ph = new PinHelper(_dbService, _configuration, _env);
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

        public ActionResult RegisterCustomerReports(object file)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCustomerReport(IFormFile file)
        {
            var actualType = FileExtensionHelper.GetContentType(file.FileName);
            if (!actualType.EndsWith("excel"))
            {
                return BadRequest($"File is not valid excel file, detected filetype: '{actualType}'");
            }
            var customerList = new List<CustomerReportModel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var fileStream = file.OpenReadStream())
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int colCount = worksheet.Dimension.End.Column;  //get Column Count
                    int rowCount = worksheet.Dimension.End.Row;     //get row count
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var customerModel = new CustomerReportModel();
                            for (int col = 1; col <= colCount; col++)
                            {
                                switch (col)
                                {
                                    case 1: customerModel.SetDateOfRoute(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    case 2: customerModel.POnumber = worksheet.Cells[row, col].Value?.ToString().Trim(); break;
                                    case 8: customerModel.SetSatisfactionNumber(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    case 9: customerModel.SetTimingNumber(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    case 10: customerModel.SetDriverNumber(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    case 11: customerModel.SetProduceNumber(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    case 12: customerModel.SetComment(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    default:
                                        break;
                                }
                            }
                            customerList.Add(customerModel);
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex, "Exception in function UploadCustomerReport when parsing an uploaded excel row");
                        }
                    }
                }
            }

            await UpdateRecordsWithCustomerReports(customerList);

            return Ok();
        }

        public ActionResult RegisterCustomerServiceReports(object file)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadCustomerServiceReport(IFormFile file)
        {
            var actualType = FileExtensionHelper.GetContentType(file.FileName);
            if (!actualType.EndsWith("excel"))
            {
                return BadRequest($"File is not valid excel file, detected filetype: '{actualType}'");
            }
            var serviceReportsList = new List<CustomerServiceModel>();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            using (var fileStream = file.OpenReadStream())
            {
                using (var package = new ExcelPackage(fileStream))
                {
                    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
                    int colCount = worksheet.Dimension.End.Column;  //get Column Count
                    int rowCount = worksheet.Dimension.End.Row;     //get row count
                    for (int row = 2; row <= rowCount; row++)
                    {
                        try
                        {
                            var customerModel = new CustomerServiceModel();
                            for (int col = 1; col <= colCount; col++)
                            {
                                switch (col)
                                {
                                    case 1: customerModel.SetNumber(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    case 2: customerModel.SetOrderNumber(worksheet.Cells[row, col].Text?.ToString().Trim()); break;
                                    case 3: customerModel.SetOpened(worksheet.Cells[row, col].Text?.ToString().Trim()); break;
                                    case 4: customerModel.SetClosed(worksheet.Cells[row, col].Text?.ToString().Trim()); break;
                                    case 5: customerModel.SetCategoryLevelComment(worksheet.Cells[row, col].Value?.ToString().Trim()); break;
                                    default:
                                        break;
                                }
                            }
                            serviceReportsList.Add(customerModel);
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex, "Exception in function UploadCustomerReport when parsing an uploaded excel row");
                        }
                    }
                }
            }

            var sw = new Stopwatch();
            sw.Start();
            Log.Debug("Start upgrading database with new customerservicemodels");
            await UpdateRecordsWithCustomerServiceReports(serviceReportsList);
            sw.Stop();
            Log.Debug($"Upgrade database with new customerservicemodels completed, elapsed seconds: {sw.Elapsed.TotalSeconds}");
            return Ok();
        }

        private async Task UpdateRecordsWithCustomerServiceReports(List<CustomerServiceModel> serviceRecordModels)
        {
            foreach (var serviceRecordModel in serviceRecordModels)
            {
                try
                {
                    var matchingDataRecord = await _dbService.GetRecordByCustomerId(Constants.MongoDb.OutputScreenCollectionName, serviceRecordModel.OrderNumber);

                    if (matchingDataRecord == null)
                    {
                        Log.Error($"No match in database for customerid '{serviceRecordModel.OrderNumber}'");
                    }
                    else
                    {
                        var matchingCustomer = matchingDataRecord.PinRouteModel.Customers
                            .FirstOrDefault(customer => customer.tracking_number_splitted == serviceRecordModel.OrderNumber);

                        matchingCustomer.CustomerServiceModel = serviceRecordModel;

                        await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenCollectionName, matchingDataRecord);
                    }
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Exception in function UpdateRecordsWithCustomerServiceReports");
                }
            }
        }

        private async Task UpdateRecordsWithCustomerReports(List<CustomerReportModel> customerList)
        {
            var customerListGroupedByDate = customerList
                .GroupBy(model => model.DateOfRoute.Date);

            var allDbGetTasks = new List<Task<List<DataModel>>>();

            foreach (var day in customerListGroupedByDate)
            {
                allDbGetTasks.Add(_dbService.GetAllRecordsByDate(Constants.MongoDb.OutputScreenCollectionName, day.Key));
            }

            await Task.WhenAll(allDbGetTasks);

            foreach (var groupOfCustomers in customerListGroupedByDate)
            {
                try
                {
                    var matchingDbTask = allDbGetTasks
                                .FirstOrDefault(task => task.Result[0].DateOfRoute.Date
                                .Equals(groupOfCustomers.Key.Date));

                    await MatchCustomerWithData(groupOfCustomers.ToList(), matchingDbTask.Result);
                }
                catch (Exception ex)
                {
                    Log.Logger.Error(ex, "Exception in function UpdateRecordsWithCustomerReports, problem when connection matching task with customerreportmodel and upserting");
                }
            }
        }

        private async Task MatchCustomerWithData(List<CustomerReportModel> customerRecordUpserts, List<DataModel> existingRecordsInDb)
        {
            foreach (var record in existingRecordsInDb)
            {
                foreach (var customer in record.PinRouteModel.Customers)
                {
                    var matchingCustomerRecord = customerRecordUpserts
                        .FirstOrDefault(upsertCustomer => upsertCustomer.POnumber
                        .Equals(customer.tracking_number));
                    if (matchingCustomerRecord != null)
                    {
                        try
                        {
                            customer.CustomerReportModel = matchingCustomerRecord;
                            await _dbService.UpsertDataRecord(Constants.MongoDb.OutputScreenCollectionName, record);
                            customerRecordUpserts.Remove(matchingCustomerRecord);
                        }
                        catch (Exception ex)
                        {
                            Log.Logger.Error(ex, "Exception in function MatchCustomerWithData");
                        }
                    }
                }
            }

            foreach (var notMatchedCustomerRecordUpserts in customerRecordUpserts)
            {
                Log.Logger.Error($"No match on PO '{notMatchedCustomerRecordUpserts.POnumber}' " +
                    $"with comment '{notMatchedCustomerRecordUpserts.Comment}'. " +
                    $"Has every order on day '{notMatchedCustomerRecordUpserts.DateOfRoute.ToLongDateString()}' been retrieved?");
            }
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

            var _ph = new PinHelper(_dbService, _configuration, _env);
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
            var ph = new PinHelper(_dbService, _configuration, _env);

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

            var numberOfRoutes = routesOfTheDay.Count(route =>
                route.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            var allRoutesInOrder = routesOfTheDay.Where(route =>
                route.PinRouteModel.ParentOrderId == orderOptionViewModel.SelectedOrderId);

            DateTime resourceOrderStart = GetLastOrderStart(allRoutesInOrder).Add(new TimeSpan(0, 0, 5, 0, 0));
            DateTime resourceOrderFin = GetLastOrderFin(allRoutesInOrder).Add(new TimeSpan(0, 0, 5, 0, 0));

            var routeName = CargoSupport.Helpers.RegexHelper.GetPrefixOfRoute(existingRouteInSameOrder.PinRouteModel.RouteName, numberOfRoutes + 1);
            if (routeName == null)
            {
                routeName = existingRouteInSameOrder.PinRouteModel.ParentOrderName;
            }

            var ph = new PinHelper(_dbService, _configuration, _env);
            await ph.InsertNewResourceRoute(
                $"{routeName} Resurs",
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
            var ph = new PinHelper(_dbService, _configuration, _env);

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
            var ph = new PinHelper(_dbService, _configuration, _env);

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