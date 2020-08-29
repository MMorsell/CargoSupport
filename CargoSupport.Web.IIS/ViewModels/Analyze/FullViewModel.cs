using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Analyze
{
    public class FullViewModel
    {
        public SlimViewModel SlimViewModel { get; set; }
        /*
         * Static startingValues
         */
        public xyz[] CustomerPositionData { get; set; }
        public xy[] KiloData { get; set; }
        public xy[] DistanceData { get; set; }
        public xy[] CustomersData { get; set; }
    }

    public class xyz
    {
        public string x { get; set; }
        public string y { get; set; }
        public string z { get; set; }
    }

    public class xy
    {
        public string x { get; set; }
        public double y { get; set; }
    }
}