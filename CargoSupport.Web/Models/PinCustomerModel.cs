using CargoSupport.Web.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Models
{
    public class PinCustomerModel
    {
        public string PinId { get; private set; }
        public DeliveryGroup DeliveryGroup { get; private set; }
        public string RouteName { get; private set; }
        public int QuinyxId { get; private set; }
        public int Weight { get; private set; }

        /*
         * Date and time properties
         */
        public DateTime DeliveryDay { get; private set; }
        public DateTime PlannedDeliveryTime { get; private set; }
        public TimeSpan FromTimeWindow { get; private set; }
        public TimeSpan ToTimeWindow { get; private set; }
        public TimeSpan ActualDeliveryTime { get; private set; }
        public TimeSpan DeliveryTimeDifferance { get; private set; }

        /*
         * GPS Locations
         */
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public double DeliveredLatitude { get; private set; }
        public double DeliveredLongitude { get; private set; }
    }
}