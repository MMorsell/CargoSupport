using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Helpers;
using CargoSupport.Web.Models;
using CargoSupport.Web.Models.PinModels;
using Microsoft.AspNetCore.Mvc;

namespace CargoSupport.Web.Controllers.Manage
{
    public class ManageController : Controller
    {
        public IActionResult index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Index(PinIdModel model)
        {
            var _ph = new PinHelper();
            List<PinRouteModel> routes = _ph.RetrieveRoutesFromActualPin(model.PinId).Result;
            _ph.PopulateRoutesWithDriversAndSaveResultToDatabase(routes).Wait();
            return View();
        }
    }
}