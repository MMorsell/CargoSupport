using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CargoSupport.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace CargoSupport.Web.IIS.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Transport()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.PlockAndUp)]
        public IActionResult Plock()
        {
            return View();
        }

        [Authorize(Roles = Constants.MinRoleLevel.MedarbetareAndUp)]
        public IActionResult Medarbetare()
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