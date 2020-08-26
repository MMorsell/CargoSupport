using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.ViewModels.Public;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.API
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UpsertController : ControllerBase
    {
        [HttpPost]
        public async Task<ActionResult> UpsertTransport(TransportViewModel transportViewModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return Ok();
        }

        [HttpPost]
        public async Task<ActionResult> UpsertTransport()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            return Ok();
        }
    }
}