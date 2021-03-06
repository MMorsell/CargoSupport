﻿using System.Linq;
using System.Threading.Tasks;
using CargoSupport.Interfaces;
using CargoSupport.Models;
using CargoSupport.Models.DatabaseModels;
using CargoSupport.ViewModels.Manange;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static CargoSupport.Helpers.AuthorizeHelper;

namespace CargoSupport.Web.IIS.Controllers.Manage
{
    [Authorize(Roles = Constants.MinRoleLevel.TransportLedareAndUp)]
    public class CarController : Controller
    {
        private readonly IMongoDbService _dbService;

        public CarController(IMongoDbService dbService)
        {
            this._dbService = dbService;
        }

        [Route("Car")]
        public async Task<ActionResult> Index()
        {
            var allCars = await _dbService.GetAllRecords<CarModel>(Constants.MongoDb.CarCollectionName);
            return View(new UpsertCarViewModel() { CurrentCar = new CarModel(), ExistingCars = allCars.Where(car => !car.IsRetired).ToList() });
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateAsync(CarModel newCarModel)
        {
            if (ModelState.IsValid)
            {
                if (await AddOrUpdateCarModel(newCarModel, _dbService))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
                }
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }

        // GET: CarController/Edit/5
        public async Task<ActionResult> EditAsync(string id)
        {
            var existingCar = await _dbService.GetRecordById<CarModel>(Constants.MongoDb.CarCollectionName, id);

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
            if (ModelState.IsValid)
            {
                if (await AddOrUpdateCarModel(newCarModel, _dbService))
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
                }
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }

        // GET: CarController/Delete/5
        public async Task<ActionResult> DeleteAsync(string id)
        {
            var existingCar = await _dbService.GetRecordById<CarModel>(Constants.MongoDb.CarCollectionName, id);

            if (existingCar == null)
            {
                return NotFound();
            }

            return View(existingCar);
        }

        // POST: CarController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteAsync(CarModel newCarModel)
        {
            if (ModelState.IsValid)
            {
                var existingCar = await _dbService.GetRecordById<CarModel>(Constants.MongoDb.CarCollectionName, newCarModel._Id);

                if (existingCar == null)
                {
                    return NotFound();
                }
                existingCar.IsRetired = true;
                await _dbService.UpsertCarRecordById(Constants.MongoDb.CarCollectionName, existingCar);
                return RedirectToAction("Index");
            }
            else
            {
                return View("Error", new ErrorViewModel { Message = "Åtgärden misslyckades" });
            }
        }
    }
}