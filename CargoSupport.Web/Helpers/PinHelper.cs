using CargoSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Helpers
{
    public class PinHelper
    {
        private MongoDbHelper _dbHelper;

        public PinHelper()
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
        }

        public async Task RetrieveRoutesForToday()
        {
            var currentRoutes = new List<PinRouteModel>
            {
                new PinRouteModel
                {
                    RouteName = "T01",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 33,
                    Weight = 66,
                    Distance = 256
                },
                new PinRouteModel
                {
                    RouteName = "T02",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 15,
                    Weight = 67,
                    Distance = 221
                },
                new PinRouteModel(DateTime.Now.AddDays(-2))
                {
                    RouteName = "T03",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 55,
                    Weight = 15,
                    Distance = 778
                },
                new PinRouteModel(DateTime.Now.AddDays(-2))
                {
                    RouteName = "T04",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 25,
                    Weight = 678,
                    Distance = 100
                },
                new PinRouteModel(DateTime.Now.AddDays(-1))
                {
                    RouteName = "T05",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 21,
                    Weight = 54,
                    Distance = 200
                },
                new PinRouteModel(DateTime.Now.AddDays(-7))
                {
                    RouteName = "T06",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 45,
                    Weight = 552,
                    Distance = 420
                },
                new PinRouteModel(DateTime.Now.AddDays(-9))
                {
                    RouteName = "T07",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 15,
                    Weight = 226,
                    Distance = 300
                },
                new PinRouteModel(DateTime.Now.AddDays(-11))
                {
                    RouteName = "T08",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                    NumberOfCustomers = 32,
                    Weight = 223,
                    Distance = 100
                }
            };

            await PopulateAllRoutesWithDrivers(currentRoutes);
        }

        public async Task PopulateAllRoutesWithDrivers(List<PinRouteModel> allRoutesForToday)
        {
            var allDriversForToday = await _dbHelper.GetAllDriversForTodaySorted(Constants.MongoDb.QuinyxWorkerTableName);

            if (allDriversForToday.Count == 0)
            {
                var qh = new QuinyxHelper();
                await qh.PopulateWithWorkersToday();
                allDriversForToday = await _dbHelper.GetAllDriversForTodaySorted(Constants.MongoDb.QuinyxWorkerTableName);
            }

            for (int i = 0; i < allRoutesForToday.Count; i++)
            {
                if (i < allDriversForToday.Count)
                {
                    allRoutesForToday[i].Driver = allDriversForToday[i];
                }
                else
                {
                    allRoutesForToday[i].Driver = allDriversForToday[0];
                }
            }

            await _dbHelper.InsertMultipleRecords(Constants.MongoDb.OutputScreenTableName, allRoutesForToday);
        }
    }
}