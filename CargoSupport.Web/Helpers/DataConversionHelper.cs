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
                    var existingRecordToGroupWith = analyzeModels.FirstOrDefault(prm => prm._driver.QuinyxId.Equals(pinRouteModel.Driver.QuinyxId));

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
            _driver = pinRouteModel.Driver;
        }

        private List<PinRouteModel> _routesForThisDriver { get; set; }
        public QuinyxWorkerModel _driver { get; }

        public void AddPinRouteModelIfItHasNotBeenAdded(PinRouteModel pinRouteModel)
        {
            if (_routesForThisDriver.FirstOrDefault(a => a.Id.Equals(pinRouteModel.Id)) == null)
            {
                _routesForThisDriver.Add(pinRouteModel);
            }
        }
    }
}