using CargoSupport.Models.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Manange
{
    public class UpsertCarViewModel
    {
        public UpsertCarViewModel()
        {
            ExistingCars = new List<CarModel>();
        }

        public CarModel CurrentCar { get; set; }
        public List<CarModel> ExistingCars { get; set; }
    }
}