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
        private List<PinRouteModel> _currentRoutes;

        public void RetrieveRoutesForToday()
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);

            _currentRoutes = new List<PinRouteModel>
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
                new PinRouteModel
                {
                    RouteName = "T03",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                },
                new PinRouteModel
                {
                    RouteName = "T04",
                    EstimatedRouteStart = DateTime.Now.TimeOfDay,
                    EstimatedRouteEnd = DateTime.Now.AddHours(5).TimeOfDay,
                    NumberOfColdBoxes = 5,
                    NumberOfFrozenBoxes = 50,
                }
            };

            _dbHelper.InsertMultipleRecords(Constants.MongoDb.OutputScreenTableName, _currentRoutes).Wait();
        }

        public void PopulateAllRoutesWithDrivers()
        {
        }
    }
}