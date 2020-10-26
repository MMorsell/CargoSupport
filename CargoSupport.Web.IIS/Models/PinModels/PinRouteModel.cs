using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CargoSupport.Models.PinModels
{
    public class PinRouteModel
    {
        [JsonProperty("id")]
        [Required]
        public int RouteId { get; set; }

        [JsonProperty("name")]
        [Required]
        public string RouteName { get; set; }

        [JsonProperty("url")]
        public string PinDirectLink { get; set; }

        public string ParentOrderId { get; set; } = "";
        public string ParentOrderName { get; set; }

        /*
         * Sub-objects
         */

        [JsonProperty("customer")]
        [Required]
        public List<PinCustomerModel> Customers { get; set; } = new List<PinCustomerModel>();

        [JsonProperty("route_info")]
        [Required]
        public PinRouteInfoModel RouteInfoModel { get; set; } = new PinRouteInfoModel();

        [JsonProperty("order")]
        [Required]
        public PinOrderInfoModel PinOrderInfoModel { get; set; } = new PinOrderInfoModel();

        /*
         * Intergers
         */

        public double NumberOfCustomers { get; private set; } = 0;
        public double Weight { get; private set; } = 0;
        public double DistanceInMeters { get; private set; } = 0;

        /*
         * Booleans
         */

        [JsonProperty("started")]
        public bool RouteHasStarted { get; set; } = false;

        [JsonProperty("ended")]
        public bool RouteHasEnded { get; set; } = false;

        /*
         * DateTime and TimeSpan
         */

        [JsonProperty("scheduled_start_time")]
        public DateTime ScheduledRouteStart { get; set; }

        public DateTime ScheduledRouteEnd { get; set; }

        [JsonProperty("actual_start_time")]
        public string ActualRouteStart { get; set; }

        public DateTime ActualRouteStartAsDate { get; set; }

        public void CalculateProperties()
        {
            Weight = Customers.Sum(s => s.PinCustomerDeliveryInfo.weight);
            NumberOfCustomers = Customers.Count;
            int.TryParse(RouteInfoModel.DistanceInMeters, out int result);
            DistanceInMeters = result;
            ScheduledRouteEnd = ScheduledRouteStart.AddSeconds(double.Parse(RouteInfoModel.duration)).ToUniversalTime();

            if (RouteHasStarted && ActualRouteStart != null)
            {
                DateTime.TryParse(ActualRouteStart, out DateTime parsedResult);
                ActualRouteStartAsDate = parsedResult.ToUniversalTime();
            }

            foreach (var customer in Customers)
            {
                var resArr = customer.tracking_number.Split('-');
                if (resArr.Length > 1)
                {
                    customer.tracking_number_splitted = resArr[1];
                }
            }

            ScheduledRouteStart = ScheduledRouteStart.ToUniversalTime();
        }
    }

    public class PinOrderInfoModel
    {
        public string id { get; set; }
        public string name { get; set; }
        public string scheduled_date { get; set; }
        public string delivery_group { get; set; }
    }

    public class PinRouteInfoModel
    {
        public string start_time { get; set; } = "23:58";
        public string end_time { get; set; } = "23:59";
        public string duration { get; set; } = "0";

        [JsonProperty("distance")]
        public string DistanceInMeters { get; set; } = "0";
    }

    public class PinCustomerModel
    {
        [JsonProperty("id")]
        public int id { get; set; }

        [JsonProperty("position_in_route")]
        public int PositionInRoute { get; set; } = 0;

        public string tracking_number { get; set; } = "NO_TRACKINGNUMBER_SET";
        public string tracking_number_splitted { get; set; } = "NO_tracking_number_splitted";
        public string vehicle_tags { get; set; } = "";
        public string position_lng { get; set; } = "NOT_SET";
        public string position_lat { get; set; } = "NOT_SET";
        public CustomerReportModel CustomerReportModel { get; set; } = new CustomerReportModel();
        public CustomerServiceModel CustomerServiceModel { get; set; } = new CustomerServiceModel();

        [JsonProperty("delivery_info")]
        public PinCustomerDeliveryInfo PinCustomerDeliveryInfo { get; set; }
    }

    public class PinCustomerDeliveryInfo
    {
        public string timewindow_start { get; set; }
        public string timewindow_end { get; set; }
        public string time_estimated { get; set; }
        public string time_handled { get; set; }
        public int weight { get; set; }
        public int stop_time { get; set; }
        public object actual_stop_time { get; set; }
        public int? age_verification { get; set; }
    }
}