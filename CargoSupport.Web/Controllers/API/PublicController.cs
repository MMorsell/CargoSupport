using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Helpers;
using CargoSupport.Web.Models.PublicModels;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CargoSupport.Web.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublicController : ControllerBase
    {
        private readonly MongoDbHelper _dbHelper;

        public PublicController()
        {
            _dbHelper = new MongoDbHelper(Constants.MongoDb.DatabaseName);
        }

        [HttpGet]
        public string Get()
        {
            return null;
            //    return JsonSerializer.Serialize(new List<Public_Transport>()
            //    {
            //}); ;
        }
    }
}