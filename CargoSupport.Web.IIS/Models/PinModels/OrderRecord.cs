using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models.PinModels
{
    public class OrderRecord
    {
        public string id { get; set; }
        public List<PinOrderModel> routes { get; set; }
    }

    public class PinOrderModel
    {
        [JsonProperty("id")]
        public int RouteId { get; set; }

        //[JsonProperty("url")]
        //public string PinDirectUrl { get; set; }
    }
}