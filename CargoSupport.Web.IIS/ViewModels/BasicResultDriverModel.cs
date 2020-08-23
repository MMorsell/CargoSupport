using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models
{
    public class BasicResultDriverModel
    {
        public string FullName { get; set; }
        public int QuinyxId { get; set; }
        public double WeekAvrWeight { get; set; }
        public double WeekAvrCustomers { get; set; }
        public double WeekAvrDistance { get; set; }
    }
}