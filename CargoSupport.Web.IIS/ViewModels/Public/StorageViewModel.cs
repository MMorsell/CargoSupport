using CargoSupport.Enums;
using CargoSupport.Models;
using System;
using System.Collections.Generic;

namespace CargoSupport.ViewModels.Public
{
    public class StorageViewModel
    {
        public StorageViewModel(
            string _id,
            TimeSpan pinStartTime,
            TimeSpan pinStopTime,
            string routeName,
            string carNumberString,
            int portNumber,
            LoadingLevel loadingLevel,
            double numberOfCustomers,
            List<PickingVerifyBooleanModel> controlIsDone,
            List<PickingVerifyIntModel> numberOfColdBoxes,
            List<PickingVerifyBooleanModel> restPicking,
            List<PickingVerifyIntModel> numberOfFrozenBoxes,
            List<PickingVerifyIntModel> numberOfBreadBoxes,
            bool isExteded)
        {
            _Id = _id;
            PinStartTimeString = pinStartTime.ToString(@"hh\:mm");
            PinEndTimeString = pinStopTime.ToString(@"hh\:mm");
            RouteName = routeName;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingLevel = loadingLevel;
            NumberOfCustomers = numberOfCustomers;

            if (isExteded)
            {
                ListNumberOfColdBoxes = numberOfColdBoxes;
                ListRestPicking = restPicking;
                ListNumberOfFrozenBoxes = numberOfFrozenBoxes;
                ListNumberOfBreadBoxes = numberOfBreadBoxes;
                ListControlIsDone = controlIsDone;
            }
            else
            {
                SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes, controlIsDone);
            }
        }

        [Obsolete("Used only for api")]
        public StorageViewModel()
        {
        }

        private void SetPickingValues(List<PickingVerifyIntModel> numberOfColdBoxes, List<PickingVerifyBooleanModel> restPicking, List<PickingVerifyIntModel> numberOfFrozenBoxes, List<PickingVerifyIntModel> numberOfBreadBoxes, List<PickingVerifyBooleanModel> controlIsDone)
        {
            if (restPicking.Count > 0)
            {
                RestPicking = restPicking[0];
            }
            else
            {
                RestPicking = new PickingVerifyBooleanModel(false);
            }

            if (controlIsDone.Count > 0)
            {
                ControlIsDone = controlIsDone[0];
            }
            else
            {
                ControlIsDone = new PickingVerifyBooleanModel(false);
            }

            if (numberOfColdBoxes.Count > 0)
            {
                NumberOfColdBoxes = numberOfColdBoxes[0];
            }
            else
            {
                NumberOfColdBoxes = new PickingVerifyIntModel(0);
            }

            if (numberOfFrozenBoxes.Count > 0)
            {
                NumberOfFrozenBoxes = numberOfFrozenBoxes[0];
            }
            else
            {
                NumberOfFrozenBoxes = new PickingVerifyIntModel(0);
            }

            if (numberOfBreadBoxes.Count > 0)
            {
                NumberOfBreadBoxes = numberOfBreadBoxes[0];
            }
            else
            {
                NumberOfBreadBoxes = new PickingVerifyIntModel(0);
            }
        }

        public string _Id { get; set; }
        public string PinStartTimeString { get; set; }
        public string PinEndTimeString { get; set; }
        public string RouteName { get; set; }
        public string CarNumber { get; set; }
        public int PortNumber { get; set; }
        public LoadingLevel LoadingLevel { get; set; }
        public double NumberOfCustomers { get; set; }
        public PickingVerifyIntModel NumberOfColdBoxes { get; set; }
        public PickingVerifyBooleanModel RestPicking { get; set; }
        public PickingVerifyIntModel NumberOfFrozenBoxes { get; set; }
        public PickingVerifyIntModel NumberOfBreadBoxes { get; set; }
        public PickingVerifyBooleanModel ControlIsDone { get; set; }
        public List<PickingVerifyIntModel> ListNumberOfColdBoxes { get; set; }
        public List<PickingVerifyBooleanModel> ListRestPicking { get; set; }
        public List<PickingVerifyIntModel> ListNumberOfFrozenBoxes { get; set; }
        public List<PickingVerifyIntModel> ListNumberOfBreadBoxes { get; set; }
        public List<PickingVerifyBooleanModel> ListControlIsDone { get; set; }
        public string HubId { get; set; }
    }
}