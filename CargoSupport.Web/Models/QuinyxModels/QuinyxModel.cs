using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CargoSupport.Web.Models.QuinyxModels
{
    public class QuinyxModel
    {
        public int PersonId { get; set; }

        public string BadgeNo { get; set; }

        public string begTimeString { get; set; }

        public string endTimeString { get; set; }

        public TimeSpan begTime { get; set; }

        public TimeSpan endTime { get; set; }

        public string categoryName { get; set; }

        public decimal hours { get; set; }

        public int costCentre { get; set; }

        public string GetDriverName()
        {
            return "";
        }

        public ExtendedInformationModel ExtendedInformationModel { get; set; }
    }
}