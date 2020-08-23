using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models
{
    public class TimeSpanWithTimeStampAndSignatureModel
    {
        public TimeSpan State { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Signature { get; set; }
    }
}