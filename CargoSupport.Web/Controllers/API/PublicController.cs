using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using CargoSupport.Helpers;
using CargoSupport.Web.Models.Public;
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

        // GET: api/<PublicController>
        [HttpGet]
        public string Get()
        {
            return JsonSerializer.Serialize(new List<Public_Transport>()
            {
                new Public_Transport(
                    "K01",
                    new TimeSpan(1, 10, 49, 00, 0),
                    "Fayaz Ali",
                    "",
                    "",
                    "108",
                    false,
                    17,
                    new TimeSpan(1, 8, 10, 00, 0),
                    52,
                    "08:24",
                    "07:09",
                    19,
                    "",
                    "Brööd"),

                new Public_Transport(
                    "K02",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Rasmus Ali",
                    "Låne 10",
                    "",
                    "110",
                    false,
                    17,
                    new TimeSpan(1, 8, 13, 00, 0),
                    50,
                    "08:24",
                    "07:24",
                    19,
                    "",
                    "Brööd"),

                new Public_Transport(
                    "K03",
                    new TimeSpan(1, 11, 16, 00, 0),
                    "Lina Ali",
                    "Retur löp 132 och efter stopp 10",
                    "",
                    "202",
                    true,
                    17,
                    new TimeSpan(1, 8, 16, 00, 0),
                    48,
                    "09:00",
                    "07:24",
                    21,
                    "",
                    ""),

                new Public_Transport(
                    "K04",
                    new TimeSpan(1, 11, 18, 00, 0),
                    "Ramon Ali",
                    "",
                    "",
                    "205",
                    false,
                    22,
                    new TimeSpan(1, 8, 18, 00, 0),
                    54,
                    "08:24",
                    "07:26",
                    21,
                    "",
                    ""),

                new Public_Transport(
                    "K05",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Mikael Ali",
                    "",
                    "",
                    "206",
                    false,
                    20,
                    new TimeSpan(1, 8, 22, 00, 0),
                    55,
                    "08:24",
                    "07:24",
                    17,
                    "",
                    "Brööd"),

                new Public_Transport(
                    "K06",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Selcuk Ali",
                    "",
                    "",
                    "207",
                    true,
                    17,
                    new TimeSpan(1, 8, 25, 00, 0),
                    57,
                    "08:24",
                    "07:24",
                    17,
                    "",
                    "Brööd"),

                new Public_Transport(
                    "K07",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Tobias Ali",
                    "Tele 301",
                    "",
                    "210",
                    false,
                    16,
                    new TimeSpan(1, 8, 30, 00, 0),
                    47,
                    "klar",
                    "07:24",
                    17,
                    "",
                    ""),

                new Public_Transport(
                    "K08",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Patryk Ali",
                    "Tele 305",
                    "",
                    "Egen",
                    true,
                    25,
                    new TimeSpan(1, 9, 19, 00, 0),
                    56,
                    "klar",
                    "07:24",
                    25,
                    "",
                    ""),

                new Public_Transport(
                    "K09",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Joseph Ali",
                    "Tele 501",
                    "",
                    "Egen",
                    true,
                    22,
                    new TimeSpan(1, 9, 5, 00, 0),
                    56,
                    "08:24",
                    "07:24",
                    24,
                    "",
                    ""),

                new Public_Transport(
                    "K10",
                    new TimeSpan(1, 10, 58, 00, 0),
                    "Esmael Ali",
                    "",
                    "",
                    "211",
                    false,
                    16,
                    new TimeSpan(1, 9, 27, 00, 0),
                    41,
                    "klar",
                    "07:24",
                    19,
                    "",
                    "Brööd"),
        }); ;
        }
    }
}