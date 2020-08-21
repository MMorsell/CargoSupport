using CargoSupport.Web.Models.PinModels;
using CargoSupport.Web.Models.QuinyxModels;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Models.DatabaseModels
{
    public class DataModel
    {
        [BsonId]
        public Guid Id { get; set; }

        /*
         * From Pin
         */
        public PinRouteModel PinRouteModel { get; set; }

        /*
         * From Quinyx
         */
        public QuinyxModel Driver { get; set; }

        /*
         * Custom
         */
        public DateTime DateOfRoute { get; set; }
        public int NumberOfColdBoxes { get; set; }
        public int NumberOfFrozenBoxes { get; set; }
        public string PreRideAnnotation { get; set; }
        public string PostRideAnnotation { get; set; }
        public string EstimatedRouteStartString { get { return PinRouteModel.ScheduledRouteStart.ToString(@"hh\:mm\:ss"); } }

        //TODO: Detta måste parsas ut
        public string EstimatedRouteEndString { get { return PinRouteModel.ScheduledRouteStart.ToString(@"hh\:mm\:ss"); } }
    }
}