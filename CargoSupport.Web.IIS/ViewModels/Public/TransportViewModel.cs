using CargoSupport.Enums;
using CargoSupport.Models;
using System;
using System.Collections.Generic;

namespace CargoSupport.ViewModels.Public
{
    public class TransportViewModel
    {
        public TransportViewModel(
            string _id,
            double kilos,
            string routeName,
            DriverViewModel driverModel,
            string carNumberString,
            int portNumber,
            LoadingLevel loadingIsDone,
            string preRideAnnotation,
            string postRideAnnotation,
            double numberOfCustomers,
            DateTime pinStartTime,
            DateTime pinStopTime,
            List<PickingVerifyIntModel> numberOfColdBoxes,
            List<PickingVerifyBooleanModel> restPicking,
            List<PickingVerifyIntModel> numberOfFrozenBoxes,
            List<PickingVerifyIntModel> numberOfBreadBoxes,
            List<PickingVerifyBooleanModel> controlIsDone,
            bool isResourceRoute)
        {
            _Id = _id;
            Kilos = kilos;
            RouteName = routeName;
            Driver = driverModel;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingLevel = loadingIsDone;
            PreRideAnnotation = preRideAnnotation;
            PostRideAnnotation = postRideAnnotation;
            NumberOfCustomers = numberOfCustomers;
            PinStartTime = pinStartTime;
            PinEndTime = pinStopTime;
            IsResourceRoute = isResourceRoute;
            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes, controlIsDone);
        }

        public TransportViewModel(
            string _id,
            string routeName,
            DriverViewModel driverModel,
            string carNumberString,
            int portNumber,
            LoadingLevel loadingLevel,
            string preRideAnnotation,
            double numberOfCustomers,
            DateTime pinStartTime,
            DateTime pinStopTime,
            List<PickingVerifyIntModel> numberOfColdBoxes,
            List<PickingVerifyBooleanModel> restPicking,
            List<PickingVerifyIntModel> numberOfFrozenBoxes,
            List<PickingVerifyIntModel> numberOfBreadBoxes,
            List<PickingVerifyBooleanModel> controlIsDone)
        {
            _Id = _id;
            RouteName = routeName;
            Driver = driverModel;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingLevel = loadingLevel;
            PreRideAnnotation = preRideAnnotation;
            NumberOfCustomers = numberOfCustomers;
            PinStartTime = pinStartTime;
            PinEndTime = pinStopTime;
            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes, controlIsDone);
        }

        [Obsolete("Used only for api")]
        public TransportViewModel()
        {
        }

        private void SetPickingValues(List<PickingVerifyIntModel> numberOfColdBoxes, List<PickingVerifyBooleanModel> restPicking, List<PickingVerifyIntModel> numberOfFrozenBoxes, List<PickingVerifyIntModel> numberOfBreadBoxes, List<PickingVerifyBooleanModel> controlIsDone)
        {
            if (numberOfColdBoxes.Count > 0)
            {
                NumberOfColdBoxes = numberOfColdBoxes[0].Value;
            }
            else
            {
                NumberOfColdBoxes = 0;
            }

            if (restPicking.Count > 0)
            {
                RestPlock = restPicking[0].Value;
            }
            else
            {
                RestPlock = false;
            }

            if (controlIsDone.Count > 0)
            {
                ControlIsDone = controlIsDone[0].Value;
            }
            else
            {
                ControlIsDone = false;
            }

            if (numberOfFrozenBoxes.Count > 0)
            {
                NumberOfFrozenBoxes = numberOfFrozenBoxes[0].Value;
            }
            else
            {
                NumberOfFrozenBoxes = 0;
            }

            if (numberOfBreadBoxes.Count > 0)
            {
                NumberOfBreadBoxes = numberOfBreadBoxes[0].Value;
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
        public DateTime PinStartTime { get; set; }
        public DateTime PinEndTime { get; set; }
        public double NumberOfCustomers { get; set; }
        public int NumberOfColdBoxes { get; set; }
        public bool RestPlock { get; set; }
        public int NumberOfFrozenBoxes { get; set; }
        public int NumberOfBreadBoxes { get; set; }
        public bool ControlIsDone { get; set; }
        public double Kilos { get; set; }
        public string HubId { get; set; }
        public bool IsResourceRoute { get; set; }
    }
}