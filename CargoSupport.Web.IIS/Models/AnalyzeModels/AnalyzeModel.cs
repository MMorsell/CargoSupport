using CargoSupport.Models.DatabaseModels;
using CargoSupport.Models.QuinyxModels;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CargoSupport.Models
{
    public class AnalyzeModel
    {
        public AnalyzeModel(DataModel pinRouteModel)
        {
            _routesForThisDriver = new List<DataModel>()
            {
                pinRouteModel
            };
            Driver = pinRouteModel.Driver;
        }

        private List<DataModel> _routesForThisDriver { get; set; }
        public QuinyxModel Driver { get; set; }

        public string FullName { get; set; }
        public double WeekAvrWeight { get; set; }
        public double WeekAvrCustomers { get; set; }
        public double WeekAvrDistance { get; set; }

        public void AddPinRouteModelIfItHasNotBeenAdded(DataModel pinRouteModel)
        {
            if (_routesForThisDriver.FirstOrDefault(a => a._Id.Equals(pinRouteModel._Id)) == null)
            {
                _routesForThisDriver.Add(pinRouteModel);
            }
        }

        public void PupulatePublicProperties()
        {
            var from = DateTime.Now.AddDays(-8);
            var to = DateTime.Now.AddDays(1);
            List<DataModel> matchingRoutes = _routesForThisDriver
                .Select(route => route)
                .Where(r => r.DateOfRoute > DateTime.Now.AddDays(-8) &&
                r.DateOfRoute < DateTime.Now.AddDays(1)).ToList();

            FullName = Driver.GetDriverName();
            WeekAvrWeight = matchingRoutes.Average(r => r.PinRouteModel.Weight);
            WeekAvrCustomers = matchingRoutes.Average(r => r.PinRouteModel.NumberOfCustomers);
            WeekAvrDistance = matchingRoutes.Average(r => r.PinRouteModel.DistanceInMeters);
            Driver = null;
        }
    }
}