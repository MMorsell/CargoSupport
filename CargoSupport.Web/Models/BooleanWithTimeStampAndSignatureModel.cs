using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Web.Models
{
    public class BooleanWithTimeStampAndSignatureModel
    {
        public bool State { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Signature { get; set; }
    }
}