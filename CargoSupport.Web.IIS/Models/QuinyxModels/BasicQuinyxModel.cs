using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models.QuinyxModels
{
    public class BasicQuinyxModel
    {
        public int Id { get; set; }
        public string begTimeString { get; set; }
        public string endTimeString { get; set; }
        public TimeSpan begTime { get; set; }
        public TimeSpan endTime { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public string FullName => $"{GivenName} {FamilyName}";
        public int Active { get; set; }
        public int StaffCat { get; set; }
    }
}