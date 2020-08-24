using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels
{
    public class DriverViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public TimeSpan BegTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int Active { get; set; }
        public string BegTimeString => BegTime.ToString(@"hh\:mm");
        public string EndTimeString => EndTime.ToString(@"hh\:mm");
    }
}