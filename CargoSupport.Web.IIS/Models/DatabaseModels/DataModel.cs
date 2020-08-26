using CargoSupport.Enums;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models.DatabaseModels
{
    public class DataModel
    {
        public DataModel()
        {
            NumberOfColdBoxes = new List<PickingVerifyModel>();
            RestPicking = new List<PickingVerifyModel>();
            NumberOfFrozenBoxes = new List<PickingVerifyModel>();
            NumberOfBreadBoxes = new List<PickingVerifyModel>();
            CarModel = new CarNumber();
        }

        [BsonId]
        public Guid Id { get; set; }

        /*
         * From Pin
         */
        public PinRouteModel PinRouteModel { get; set; }

        /*
         * From Quinyx
         */
        public QuinyxModel Driver { get; set; }

        /*
         * Custom
         */
        public bool IsResourceRoute { get; set; } = false;
        public CarNumber CarModel { get; set; }
        public int PortNumber { get; set; }
        public LoadingLevel LoadingLevel { get; set; } = LoadingLevel.Ej_påbörjad;
        public DateTime DateOfRoute { get; set; }
        public bool ControlIsDone { get; set; } = false;
        public List<PickingVerifyModel> NumberOfColdBoxes { get; set; }
        public List<PickingVerifyModel> RestPicking { get; set; }
        public List<PickingVerifyModel> NumberOfFrozenBoxes { get; set; }
        public List<PickingVerifyModel> NumberOfBreadBoxes { get; set; }
        public string PreRideAnnotation { get; set; } = "";
        public string PostRideAnnotation { get; set; } = "";
        public string EstimatedRouteStartString { get { return PinRouteModel.ScheduledRouteStart.ToString(@"hh\:mm\:ss"); } }
        public string EstimatedRouteEndString { get { return PinRouteModel.ScheduledRouteEnd.ToString(@"hh\:mm\:ss"); } }
    }
}