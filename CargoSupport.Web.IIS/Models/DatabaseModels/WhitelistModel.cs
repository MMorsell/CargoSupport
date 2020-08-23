using CargoSupport.Enums;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models.DatabaseModels
{
    public class WhitelistModel
    {
        [BsonId]
        public Guid Id { get; set; }

        public string NameWithDomain { get; set; }
        public RoleLevel RoleLevel { get; set; }
    }
}