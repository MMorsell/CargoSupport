using CargoSupport.Models.DatabaseModels;
using System.Collections.Generic;

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