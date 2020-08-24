using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CargoSupport.Models
{
    public class PickingVerifyModel
    {
        public PickingVerifyModel()
        {
            _timestamp = DateTime.Now;
        }

        private DateTime _timestamp;
        public object Value { get; set; }
        public string Signature { get; set; }
    }
}