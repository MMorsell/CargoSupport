using CargoSupport.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CargoSupport.Models.QuinyxModels
{
    public class QuinyxModel
    {
        public int Id { get; set; } = -1;

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
                return $"Name not found for driver with id '{Id}' and badgeId '{BadgeNo}'";
            }
        }

        public DriverViewModel ConvertToDriverViewModel()
        {
            string fullName = "";
            int active = 1;
            if (ExtendedInformationModel != null)
            {
                fullName = $"{ExtendedInformationModel.GivenName} {ExtendedInformationModel.FamilyName}";
                active = ExtendedInformationModel.Active;
            }
            return new DriverViewModel(begTime, endTime)
            {
                FullName = fullName,
                Active = active,
                Id = this.Id,
            };
        }

        public ExtendedInformationModel ExtendedInformationModel { get; set; }
        public string CategoryName { get; internal set; }

        public string FullNameWithTime => $"{ExtendedInformationModel.GivenName} {ExtendedInformationModel.FamilyName} {begTime.ToString(@"hh\:mm")}-{endTime.ToString(@"hh\:mm")}";
    }
}