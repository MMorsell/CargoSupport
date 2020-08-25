using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels
{
    public class DriverViewModel
    {
        public DriverViewModel()
        {
        }

        public DriverViewModel(TimeSpan begTime, TimeSpan endTime)
        {
            BegTime = begTime;
            EndTime = endTime;
            BegTimeString = BegTime.ToString(@"hh\:mm");
            EndTimeString = EndTime.ToString(@"hh\:mm");
        }

        public int Id { get; set; }
        public string FullName { get; set; }
        public TimeSpan BegTime { get; private set; }
        public TimeSpan EndTime { get; private set; }
        public int Active { get; set; }
        public string BegTimeString { get; private set; }
        public string EndTimeString { get; private set; }
    }
}