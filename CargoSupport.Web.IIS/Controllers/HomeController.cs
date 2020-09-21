using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CargoSupport.Models;
using CargoSupport.Helpers;
using Microsoft.AspNetCore.Http;
using static CargoSupport.Helpers.AuthorizeHelper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CargoSupport.Models.Auth;
using AspNetCore.Identity.MongoDbCore.Models;
using Microsoft.AspNetCore.Identity;
using CargoSupport.Interfaces;

namespace CargoSupport.Web.IIS.Controllers
{
    public class HomeController : Controller
    {
        public async Task<IActionResult> Transport()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        public async Task<IActionResult> Plock()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.MedarbetareAndUp)]
        public async Task<IActionResult> Medarbetare()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}