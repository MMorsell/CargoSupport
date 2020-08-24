using CargoSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Public
{
    public class TransportViewModel
    {
        //TidFrys = tidFrys.ToString(@"hh\:mm");

        public TransportViewModel(Guid id, string routeName, string driverFullName, string carNumberString, int portNumber, bool loadingIsDone, string preRideAnnotation, string postRideAnnotation, double numberOfCustomers, TimeSpan pinStartTime, TimeSpan pinStopTime, List<PickingVerifyModel> numberOfColdBoxes, List<PickingVerifyModel> restPicking, List<PickingVerifyModel> numberOfFrozenBoxes, List<PickingVerifyModel> numberOfBreadBoxes)
        {
            Id = id;
            RouteName = routeName;
            DriverFullName = driverFullName;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingIsDone = loadingIsDone;
            PreRideAnnotation = preRideAnnotation;
            PostRideAnnotation = postRideAnnotation;
            NumberOfCustomers = numberOfCustomers;
            PinStartTimeString = pinStartTime.ToString(@"hh\:mm");
            PinEndTimeString = pinStopTime.ToString(@"hh\:mm");
            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes);
        }

        public void SetPickingValues(List<PickingVerifyModel> numberOfColdBoxes, List<PickingVerifyModel> restPicking, List<PickingVerifyModel> numberOfFrozenBoxes, List<PickingVerifyModel> numberOfBreadBoxes)
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

        public Guid Id { get; private set; }
        public string RouteName { get; private set; }
        public string DriverFullName { get; private set; }
        public string CarNumber { get; private set; }
        public int PortNumber { get; private set; }
        public bool LoadingIsDone { get; set; }
        public string PreRideAnnotation { get; private set; } = "";
        public string PostRideAnnotation { get; private set; } = "";
        public string PinStartTimeString { get; set; }
        public string PinEndTimeString { get; set; }
        public double NumberOfCustomers { get; private set; }
        public int NumberOfColdBoxes { get; private set; }
        public bool RestPlock { get; private set; }
        public int NumberOfFrozenBoxes { get; private set; }
        public int NumberOfBreadBoxes { get; private set; }
    }
}