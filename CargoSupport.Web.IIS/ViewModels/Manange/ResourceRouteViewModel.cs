using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Manange
{
    public class ResourceRouteViewModel
    {
        public ResourceRouteViewModel()
        {
            RouteNamesOfToday = new Dictionary<string, string>();
        }

        public Dictionary<string, string> RouteNamesOfToday { get; set; }
        public string OrderID { get; set; }
    }
}