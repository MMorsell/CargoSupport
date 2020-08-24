using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models
{
    public class CarNumber
    {
        public int Number { get; set; }
        public string OwnCarDepiction { get; set; } = "";
        public bool HasOwnCar { get; set; } = false;

        internal string GetValue()
        {
            if (OwnCarDepiction != "")
            {
                return OwnCarDepiction;
            }
            else
            {
                return $"{Number}";
            }
        }
    }
}