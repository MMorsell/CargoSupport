using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CargoSupport.Web.Models.QuinyxModels
{
    public class QuinyxModel
    {
        public int Id { get; set; }

        public string BadgeNo { get; set; }

        public string begTimeString { get; set; }

        public string endTimeString { get; set; }

        public TimeSpan begTime { get; set; }

        public TimeSpan endTime { get; set; }

        public int CategoryId { get; set; }

        public int Section { get; set; }

        public string SectionName { get; set; }

        public decimal hours { get; set; }

        public int CostCentre { get; set; }

        public int ManagerId { get; set; }

        public string GetDriverName()
        {
            if (ExtendedInformationModel != null)
            {
                return $"{ExtendedInformationModel.GivenName} {ExtendedInformationModel.FamilyName}";
            }
            else
            {
                return "";
            }
        }

        public ExtendedInformationModel ExtendedInformationModel { get; set; }
        public string CategoryName { get; internal set; }
    }
}