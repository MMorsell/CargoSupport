﻿using CargoSupport.Enums;
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

        public TransportViewModel(string _id, double kilos, string routeName, DriverViewModel driverModel, string carNumberString, int portNumber, LoadingLevel loadingIsDone, string preRideAnnotation, string postRideAnnotation, double numberOfCustomers, TimeSpan pinStartTime, TimeSpan pinStopTime, List<PickingVerifyIntModel> numberOfColdBoxes, List<PickingVerifyBooleanModel> restPicking, List<PickingVerifyIntModel> numberOfFrozenBoxes, List<PickingVerifyIntModel> numberOfBreadBoxes, List<PickingVerifyBooleanModel> controlIsDone)
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
            PinStartTimeString = pinStartTime.ToString(@"hh\:mm");
            PinEndTimeString = pinStopTime.ToString(@"hh\:mm");
            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes, controlIsDone);
        }

        public TransportViewModel(string routeName, DriverViewModel driverModel, string carNumberString, int portNumber, LoadingLevel loadingLevel, string preRideAnnotation, double numberOfCustomers, TimeSpan pinStartTime, TimeSpan pinStopTime, List<PickingVerifyIntModel> numberOfColdBoxes, List<PickingVerifyBooleanModel> restPicking, List<PickingVerifyIntModel> numberOfFrozenBoxes, List<PickingVerifyIntModel> numberOfBreadBoxes, List<PickingVerifyBooleanModel> controlIsDone)
        {
            RouteName = routeName;
            Driver = driverModel;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingLevel = loadingLevel;
            PreRideAnnotation = preRideAnnotation;
            NumberOfCustomers = numberOfCustomers;
            PinStartTimeString = pinStartTime.ToString(@"hh\:mm");
            PinEndTimeString = pinStopTime.ToString(@"hh\:mm");
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
                NumberOfColdBoxes = (int)numberOfColdBoxes[0].Value;
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
        public double Kilos { get; set; }
        public string HubId { get; set; }
    }
}