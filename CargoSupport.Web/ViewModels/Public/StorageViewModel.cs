using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.ViewModels.Public
{
    public class StorageViewModel
    {
        public StorageViewModel(
            Guid id,
            string routeName,
            TimeSpan estimatedRouteStart,
            int numberOfColdBoxes,
            int numberOfFrozenBoxes,
            double numberOfCustomers,
            TimeSpan tid,
            string restPlock,
            TimeSpan tidFrys)
        {
            Id = id;
            RouteName = routeName;
            EstimatedRouteStart = estimatedRouteStart.ToString(@"hh\:mm");
            NumberOfColdBoxes = numberOfColdBoxes;
            NumberOfFrozenBoxes = numberOfFrozenBoxes;
            NumberOfCustomers = numberOfCustomers;
            Tid = tid.ToString(@"hh\:mm");
            RestPlock = restPlock;
            TidFrys = tidFrys.ToString(@"hh\:mm");
        }

        public Guid Id { get; private set; }
        public string RouteName { get; private set; }
        public double NumberOfCustomers { get; private set; }
        public string Tid { get; private set; }
        public int NumberOfColdBoxes { get; private set; }
        public string RestPlock { get; private set; }
        public string TidFrys { get; private set; }
        public int NumberOfFrozenBoxes { get; private set; }
        public string EstimatedRouteStart { get; private set; }
    }
}