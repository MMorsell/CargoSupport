using CargoSupport.Enums;
using CargoSupport.Models;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Public
{
    public class TransportViewModel
    {
        //TidFrys = tidFrys.ToString(@"hh\:mm");

        public TransportViewModel(string _id, string routeName, DriverViewModel driverModel, string carNumberString, int portNumber, LoadingLevel loadingIsDone, string preRideAnnotation, string postRideAnnotation, double numberOfCustomers, TimeSpan pinStartTime, TimeSpan pinStopTime, List<PickingVerifyModel> numberOfColdBoxes, List<PickingVerifyModel> restPicking, List<PickingVerifyModel> numberOfFrozenBoxes, List<PickingVerifyModel> numberOfBreadBoxes, bool controlIsDone)
        {
            _Id = _id;
            RouteName = routeName;
            Driver = driverModel;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingLevel = loadingIsDone;
            PreRideAnnotation = preRideAnnotation;
            PostRideAnnotation = postRideAnnotation;
            NumberOfCustomers = numberOfCustomers;
            PinStartTimeString = pinStartTime.ToString(@"hh\:mm");
            PinEndTimeString = pinStopTime.ToString(@"hh\:mm");
            ControlIsDone = controlIsDone;
            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes);
        }

        public TransportViewModel(string routeName, DriverViewModel driverModel, string carNumberString, int portNumber, LoadingLevel loadingLevel, string preRideAnnotation, double numberOfCustomers, List<PickingVerifyModel> numberOfColdBoxes, List<PickingVerifyModel> restPicking, List<PickingVerifyModel> numberOfFrozenBoxes, List<PickingVerifyModel> numberOfBreadBoxes, bool controlIsDone)
        {
            RouteName = routeName;
            Driver = driverModel;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingLevel = loadingLevel;
            PreRideAnnotation = preRideAnnotation;
            NumberOfCustomers = numberOfCustomers;
            ControlIsDone = controlIsDone;
            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes);
        }

        [Obsolete]
        public TransportViewModel()
        {
        }

        private void SetPickingValues(List<PickingVerifyModel> numberOfColdBoxes, List<PickingVerifyModel> restPicking, List<PickingVerifyModel> numberOfFrozenBoxes, List<PickingVerifyModel> numberOfBreadBoxes)
        {
            if (numberOfColdBoxes.Count > 0)
            {
                NumberOfColdBoxes = (int)numberOfColdBoxes[0].Value;
            }
            else
            {
                NumberOfColdBoxes = 0;
            }

            if (restPicking.Count > 0)
            {
                RestPlock = (bool)restPicking[0].Value;
            }
            else
            {
                RestPlock = false;
            }

            if (numberOfFrozenBoxes.Count > 0)
            {
                NumberOfFrozenBoxes = (int)numberOfFrozenBoxes[0].Value;
            }
            else
            {
                NumberOfFrozenBoxes = 0;
            }

            if (numberOfBreadBoxes.Count > 0)
            {
                NumberOfBreadBoxes = (int)numberOfBreadBoxes[0].Value;
            }
            else
            {
                NumberOfBreadBoxes = 0;
            }
        }

        public string _Id { get; set; }
        public string RouteName { get; set; }
        public DriverViewModel Driver { get; set; }
        public string CarNumber { get; set; }
        public int PortNumber { get; set; }
        public LoadingLevel LoadingLevel { get; set; }
        public string PreRideAnnotation { get; set; } = "";
        public string PostRideAnnotation { get; set; } = "";
        public string PinStartTimeString { get; set; }
        public string PinEndTimeString { get; set; }
        public double NumberOfCustomers { get; set; }
        public int NumberOfColdBoxes { get; set; }
        public bool RestPlock { get; set; }
        public int NumberOfFrozenBoxes { get; set; }
        public int NumberOfBreadBoxes { get; set; }
        public bool ControlIsDone { get; set; }
    }
}