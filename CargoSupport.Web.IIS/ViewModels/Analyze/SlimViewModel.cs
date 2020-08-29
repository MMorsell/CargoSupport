using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Analyze
{
    public class SlimViewModel
    {
        public string DriverFullName { get; set; }
        public int QuinyxId { get; set; }
        public double AvrWeight { get; set; }
        public double AvrCustomers { get; set; }
        public double AvrDrivingDistance { get; set; }
    }
}
