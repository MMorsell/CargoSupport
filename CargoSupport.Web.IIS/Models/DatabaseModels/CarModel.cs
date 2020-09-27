using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CargoSupport.Models.DatabaseModels
{
    public class CarModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }

        public string Name { get; set; }
        public string RegistrationNumber { get; set; }
        public int MaxWeight { get; set; }
        public bool CanBeSelected { get; set; }
    }
}