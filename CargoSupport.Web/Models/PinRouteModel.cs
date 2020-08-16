using CargoSupport.Web.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.ComponentModel;

namespace CargoSupport.Models
{
    public class PinRouteModel : IDateTime
    {
        public PinRouteModel()
        {
            CurrentDate = DateTime.Now;
        }

        public PinRouteModel(DateTime newDateTime)
        {
            CurrentDate = newDateTime;
        }

        [BsonId]
        public Guid Id { get; set; }

        public string RouteName { get; set; }
        public QuinyxWorkerModel Driver { get; set; }
        public TimeSpan EstimatedRouteStart { get; set; }
        public TimeSpan EstimatedRouteEnd { get; set; }
        public string EstimatedRouteStartString { get { return EstimatedRouteStart.ToString(@"hh\:mm\:ss"); } }
        public string EstimatedRouteEndString { get { return EstimatedRouteEnd.ToString(@"hh\:mm\:ss"); } }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CurrentDate { get; set; }

        public int NumberOfColdBoxes { get; set; }
        public int NumberOfFrozenBoxes { get; set; }
        public bool LoadingIsDone { get; set; } = false;
        public string PreRideAnnotation { get; set; }
        public string PostRideAnnotation { get; set; }
    }
}