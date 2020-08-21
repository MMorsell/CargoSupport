using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Models.PublicModels
{
    public class Public_Transport
    {
        public Public_Transport(
            string routeName,
            TimeSpan loadUpTime,
            string fullName,
            string deliveryComment,
            string portNumber,
            string car,
            bool isLoaded,
            int numberOfCustomers,
            TimeSpan time,
            int numberOfBoxes,
            string restPicking,
            string time_Freezer,
            int numberOfFreezingBoxes,
            string bTime,
            string bread)
        {
            RouteName = routeName;
            LoadUpTime = loadUpTime;
            FullName = fullName;
            DeliveryComment = deliveryComment;
            PortNumber = portNumber;
            Car = car;
            IsLoaded = isLoaded;
            NumberOfCustomers = numberOfCustomers;
            Time = time;
            NumberOfBoxes = numberOfBoxes;
            RestPicking = restPicking;
            Time_Freezer = time_Freezer;
            NumberOfFreezingBoxes = numberOfFreezingBoxes;
            BTime = bTime;
            Bread = bread;
        }

        public string RouteName { get; private set; }
        public TimeSpan LoadUpTime { get; private set; }
        public string FullName { get; private set; }
        public string DeliveryComment { get; private set; }
        public string PortNumber { get; private set; }
        public string Car { get; private set; }
        public bool IsLoaded { get; private set; }
        public int NumberOfCustomers { get; private set; }
        public TimeSpan Time { get; private set; }
        public int NumberOfBoxes { get; private set; }
        public string RestPicking { get; private set; }
        public string Time_Freezer { get; private set; }
        public int NumberOfFreezingBoxes { get; private set; }
        public string BTime { get; private set; }
        public string Bread { get; private set; }
    }
}