using Newtonsoft.Json;
using System.Collections.Generic;

namespace CargoSupport.Models.PinModels
{
    public class OrderRecord
    {
        public string name { get; set; }
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