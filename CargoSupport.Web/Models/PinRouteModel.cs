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

        [BsonId]
        public Guid Id { get; set; }

        [DisplayName("aaa")]
        public string RouteName { get; set; }

        public QuinyxWorkerModel Driver { get; set; }
        public TimeSpan EstimatedRouteStart { get; set; }
        public TimeSpan EstimatedRouteEnd { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CurrentDate { get; set; }

        public int NumberOfColdBoxes { get; set; }
        public int NumberOfFrozenBoxes { get; set; }
        public bool LoadingIsDone { get; set; } = false;
        public string PreRideAnnotation { get; set; }
        public string PostRideAnnotation { get; set; }
    }
}