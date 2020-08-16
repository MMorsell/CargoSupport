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
                },
                new PinRouteModel
                {
                    RouteName = "T02",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel(DateTime.Now.AddDays(-2))
                {
                    RouteName = "T03",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel(DateTime.Now.AddDays(-2))
                {
                    RouteName = "T04",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel(DateTime.Now.AddDays(-1))
                {
                    RouteName = "T05",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel(DateTime.Now.AddDays(-7))
                {
                    RouteName = "T06",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel(DateTime.Now.AddDays(-9))
                {
                    RouteName = "T07",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel(DateTime.Now.AddDays(-11))
                {
                    RouteName = "T08",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                }
            };

            await PopulateAllRoutesWithDrivers(currentRoutes);
        }

        public async Task PopulateAllRoutesWithDrivers(List<PinRouteModel> allRoutesForToday)
        {
            var allDriversForToday = await _dbHelper.GetAllDriversForTodaySorted(Constants.MongoDb.QuinyxWorkerTableName);

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