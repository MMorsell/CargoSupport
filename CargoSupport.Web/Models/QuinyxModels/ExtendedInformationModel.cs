using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Models.QuinyxModels
{
    public class ExtendedInformationModel
    {
        public string BadgeNo { get; set; }
        public string GivenName { get; set; }
        public string FamilyName { get; set; }
        public int StaffCat { get; set; }
        public string StaffCatName { get; set; }
        public int Section { get; set; }
        public int CostCentre { get; set; }
        public string ReportingTo { get; set; }
    }
}