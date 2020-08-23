using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.ViewModels.Public
{
    public class TransportViewModel
    {
        public TransportViewModel(
            Guid id,
            string routeName,
            string driverFullName,
            TimeSpan estimatedRouteStart,
            bool routHasStarted,
            int numberOfColdBoxes,
            int numberOfFrozenBoxes,
            string preRideAnnotation,
            int portNumber,
            int carNumber,
            double numberOfCustomers,
            TimeSpan tid,
            string restPlock,
            TimeSpan tidFrys)
        {
            Id = id;
            RouteName = routeName;
            DriverFullName = driverFullName;
            EstimatedRouteStart = estimatedRouteStart.ToString(@"hh\:mm");
            RouteHasStarted = routHasStarted;
            NumberOfColdBoxes = numberOfColdBoxes;
            NumberOfFrozenBoxes = numberOfFrozenBoxes;
            PreRideAnnotation = preRideAnnotation;
            PortNumber = portNumber;
            CarNumber = carNumber;
            NumberOfCustomers = numberOfCustomers;
            Tid = tid.ToString(@"hh\:mm");
            RestPlock = restPlock;
            TidFrys = tidFrys.ToString(@"hh\:mm");
        }

        public Guid Id { get; private set; }
        public string RouteName { get; private set; }
        public string DriverFullName { get; private set; }
        public string PreRideAnnotation { get; private set; }
        public int PortNumber { get; private set; }
        public int CarNumber { get; private set; }
        public bool RouteHasStarted { get; private set; }
        public double NumberOfCustomers { get; private set; }

        public string Tid { get; private set; }
        public int NumberOfColdBoxes { get; private set; }
        public string RestPlock { get; private set; }
        public string TidFrys { get; private set; }
        public int NumberOfFrozenBoxes { get; private set; }

        public string EstimatedRouteStart { get; private set; }
    }
}