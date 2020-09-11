using CargoSupport.Enums;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CargoSupport.Models.DatabaseModels
{
    public class WhitelistModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _Id { get; set; }

        public string NameWithDomain { get; set; }
        public RoleLevel RoleLevel { get; set; }
    }
}