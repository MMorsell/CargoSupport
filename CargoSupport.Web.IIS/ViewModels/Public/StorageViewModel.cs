﻿using CargoSupport.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.ViewModels.Public
{
    public class StorageViewModel
    {
        public StorageViewModel(
            Guid id,
            string routeName,
            string carNumberString,
            int portNumber,
            bool loadingIsDone,
            double numberOfCustomers,
            bool controlIsDone,
            List<PickingVerifyModel> numberOfColdBoxes,
            List<PickingVerifyModel> restPicking,
            List<PickingVerifyModel> numberOfFrozenBoxes,
            List<PickingVerifyModel> numberOfBreadBoxes)
        {
            Id = id;
            RouteName = routeName;
            CarNumber = carNumberString;
            PortNumber = portNumber;
            LoadingIsDone = loadingIsDone;
            NumberOfCustomers = numberOfCustomers;
            ControlIsDone = controlIsDone;

            SetPickingValues(numberOfColdBoxes, restPicking, numberOfFrozenBoxes, numberOfBreadBoxes);
        }

        private void SetPickingValues(List<PickingVerifyModel> numberOfColdBoxes, List<PickingVerifyModel> restPicking, List<PickingVerifyModel> numberOfFrozenBoxes, List<PickingVerifyModel> numberOfBreadBoxes)
        {
            if (numberOfColdBoxes.Count > 0)
            {
                NumberOfColdBoxes = numberOfColdBoxes[0];
            }
            else
            {
                NumberOfColdBoxes = new PickingVerifyModel(0);
            }

            if (restPicking.Count > 0)
            {
                RestPicking = restPicking[0];
            }
            else
            {
                RestPicking = new PickingVerifyModel(false);
            }

            if (numberOfFrozenBoxes.Count > 0)
            {
                NumberOfFrozenBoxes = numberOfFrozenBoxes[0];
            }
            else
            {
                NumberOfFrozenBoxes = new PickingVerifyModel(0);
            }

            if (numberOfBreadBoxes.Count > 0)
            {
                NumberOfBreadBoxes = numberOfBreadBoxes[0];
            }
            else
            {
                NumberOfBreadBoxes = new PickingVerifyModel(0);
            }
        }

        public Guid Id { get; private set; }
        public string RouteName { get; private set; }
        public string CarNumber { get; private set; }
        public int PortNumber { get; private set; }
        public bool LoadingIsDone { get; set; }
        public double NumberOfCustomers { get; private set; }
        public PickingVerifyModel NumberOfColdBoxes { get; set; }
        public PickingVerifyModel RestPicking { get; set; }
        public PickingVerifyModel NumberOfFrozenBoxes { get; set; }
        public PickingVerifyModel NumberOfBreadBoxes { get; set; }
        public bool ControlIsDone { get; set; }
    }
}