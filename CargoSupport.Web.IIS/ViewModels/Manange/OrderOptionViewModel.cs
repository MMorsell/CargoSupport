using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Manange
{
    public class OrderOptionViewModel
    {
        public OrderOptionViewModel()
        {
            RoutesToSelectFrom = new Dictionary<string, string>();
        }

        public Dictionary<string, string> RoutesToSelectFrom { get; set; }
        public string SelectedOrderId { get; set; }
        public string DateTimeString { get; set; }
    }
}