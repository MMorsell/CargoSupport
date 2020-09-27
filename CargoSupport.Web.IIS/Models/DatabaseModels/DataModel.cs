using CargoSupport.Enums;
using CargoSupport.Models.PinModels;
using CargoSupport.Models.QuinyxModels;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CargoSupport.Models.DatabaseModels
{
    public class DataModel
    {
        public DataModel()
        {
            ControlIsDone = new List<PickingVerifyBooleanModel>();
            RestPicking = new List<PickingVerifyBooleanModel>();
            NumberOfColdBoxes = new List<PickingVerifyIntModel>();
            NumberOfFrozenBoxes = new List<PickingVerifyIntModel>();
            NumberOfBreadBoxes = new List<PickingVerifyIntModel>();
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }

        /*
         * From Pin
         */

        [Required]
        public PinRouteModel PinRouteModel { get; set; }

        /*
         * From Quinyx
         */

        [Required]
        public QuinyxModel Driver { get; set; }

        /*
         * Custom
         */
        public bool IsResourceRoute { get; set; } = false;
        public string CarModel { get; set; } = "Ej Satt";
        public int PortNumber { get; set; } = 0;
        public LoadingLevel LoadingLevel { get; set; } = LoadingLevel.Ej_påbörjad;
        public DateTime DateOfRoute { get; set; }
        public List<PickingVerifyBooleanModel> ControlIsDone { get; set; }
        public List<PickingVerifyIntModel> NumberOfColdBoxes { get; set; }
        public List<PickingVerifyBooleanModel> RestPicking { get; set; }
        public List<PickingVerifyIntModel> NumberOfFrozenBoxes { get; set; }
        public List<PickingVerifyIntModel> NumberOfBreadBoxes { get; set; }
        public string PreRideAnnotation { get; set; } = "";
        public string PostRideAnnotation { get; set; } = "";
    }
}