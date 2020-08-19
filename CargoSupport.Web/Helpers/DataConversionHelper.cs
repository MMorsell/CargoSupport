using CargoSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Helpers
{
    public static class DataConversionHelper
    {
        public static List<AnalyzeModel> ConvertPinRouteModelToAnalyzeModel(List<PinRouteModel> pinRouteModels)
        {
            var analyzeModels = new List<AnalyzeModel>();

            foreach (var pinRouteModel in pinRouteModels)
            {
                if (pinRouteModel.Driver != null)
                {
                    var existingRecordToGroupWith = analyzeModels.FirstOrDefault(prm => prm.Driver.QuinyxId.Equals(pinRouteModel.Driver.QuinyxId));

                    if (existingRecordToGroupWith == null)
                    {
                        analyzeModels.Add(new AnalyzeModel(pinRouteModel));
                    }
                    else
                    {
                        existingRecordToGroupWith.AddPinRouteModelIfItHasNotBeenAdded(pinRouteModel);
                    }
                }
            }

            foreach (var analyzeModel in analyzeModels)
            {
                analyzeModel.PupulatePublicProperties();
            }
            return analyzeModels;
        }
    }

    public class AnalyzeModel
    {
        public AnalyzeModel(PinRouteModel pinRouteModel)
        {
            _routesForThisDriver = new List<PinRouteModel>()
            {
                pinRouteModel
            };
            Driver = pinRouteModel.Driver;
        }

        private List<PinRouteModel> _routesForThisDriver { get; set; }
        public QuinyxWorkerModel Driver { get; set; }

        public string FullName { get; set; }
        public double WeekAvrWeight { get; set; }
        public double WeekAvrCustomers { get; set; }
        public double WeekAvrDistance { get; set; }

        public void AddPinRouteModelIfItHasNotBeenAdded(PinRouteModel pinRouteModel)
        {
            if (_routesForThisDriver.FirstOrDefault(a => a.Id.Equals(pinRouteModel.Id)) == null)
            {
                _routesForThisDriver.Add(pinRouteModel);
            }
        }

        public void PupulatePublicProperties()
        {
            var from = DateTime.Now.AddDays(-8);
            var to = DateTime.Now.AddDays(1);
            List<PinRouteModel> matchingRoutes = _routesForThisDriver
                .Select(route => route)
                .Where(r => r.CurrentDate > DateTime.Now.AddDays(-8) &&
                r.CurrentDate < DateTime.Now.AddDays(1)).ToList();

            FullName = Driver.FullName;
            WeekAvrWeight = matchingRoutes.Average(r => r.Weight);
            WeekAvrCustomers = matchingRoutes.Average(r => r.NumberOfCustomers);
            WeekAvrDistance = matchingRoutes.Average(r => r.Distance);
            Driver = null;
        }
    }
}