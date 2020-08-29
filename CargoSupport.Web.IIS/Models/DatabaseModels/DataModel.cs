using CargoSupport.Enums;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using MongoDB.Bson;
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
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }

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
        public string CarModel { get; set; } = "Ej Satt";
        public int PortNumber { get; set; } = 0;
        public LoadingLevel LoadingLevel { get; set; } = LoadingLevel.Ej_påbörjad;
        public DateTime DateOfRoute { get; set; }
        public bool ControlIsDone { get; set; } = false;
        public List<PickingVerifyModel> NumberOfColdBoxes { get; set; }
        public List<PickingVerifyModel> RestPicking { get; set; }
        public List<PickingVerifyModel> NumberOfFrozenBoxes { get; set; }
        public List<PickingVerifyModel> NumberOfBreadBoxes { get; set; }
        public string PreRideAnnotation { get; set; } = "";
        public string PostRideAnnotation { get; set; } = "";
    }
}