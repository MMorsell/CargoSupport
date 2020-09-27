using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace CargoSupport.Models.DatabaseModels
{
    public class CarModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string RegistrationNumber { get; set; }

        [Required]
        public int MaxWeight { get; set; }

        [Required]
        public bool CanBeSelected { get; set; }
    }
}