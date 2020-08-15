using CargoSupport.Web.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models
{
    public class QuinyxWorkerModel : IDateTime
    {
        public QuinyxWorkerModel()
        {
            CurrentDate = DateTime.Now;
        }

        [BsonId]
        public Guid Id { get; set; }

        public int QuinyxId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CurrentDate { get; set; }

        public TimeSpan StartShiftTime { get; set; }

        public TimeSpan EndShiftTime { get; set; }

        public int CurrentRouteWeight { get; set; }
        public int TotalWeightThisWeek { get; set; }
    }
}