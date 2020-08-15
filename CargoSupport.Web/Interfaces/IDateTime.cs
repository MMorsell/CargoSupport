using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Interfaces
{
    public interface IDateTime
    {
        public DateTime CurrentDate { get; set; }
    }
}