using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Enums;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    public class CarController : Controller
    {
        [Route("Car")]
        public async Task<ActionResult> Index()
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            var db = new CargoSupport.Helpers.MongoDbHelper(Constants.MongoDb.DatabaseName);
            var allCars = await db.GetAllRecords<CarModel>(Constants.MongoDb.CarTableName);
            return View(new UpsertCarViewModel() { CurrentCar = new CarModel(), ExistingCars = allCars });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(CarModel newCarModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            if (await AddOrUpdateCarModel(newCarModel, HttpContext.User))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }

        // GET: CarController/Edit/5
        public async Task<ActionResult> EditAsync(string id)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }
            var db = new CargoSupport.Helpers.MongoDbHelper(Constants.MongoDb.DatabaseName);
            var existingCar = await db.GetRecordById<CarModel>(Constants.MongoDb.CarTableName, id);

            if (existingCar == null)
            {
                return NotFound();
            }

            return View(existingCar);
        }

        // POST: CarController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> EditAsync(CarModel newCarModel)
        {
            if (await IsAuthorized(new List<RoleLevel> { RoleLevel.SuperUser }, HttpContext.User) == false)
            {
                return Unauthorized();
            }

            if (await AddOrUpdateCarModel(newCarModel, HttpContext.User))
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }

        /*
         * Will not allow deletions by users for now, saving this if it changes in the future
         */
        //// GET: CarController/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //// POST: CarController/Delete/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Delete(int id, IFormCollection collection)
        //{
        //    try
        //    {
        //        return RedirectToAction(nameof(Index));
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}