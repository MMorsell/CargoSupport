using System;

namespace CargoSupport.Models
{
    public class PickingVerifyBooleanModel
    {
        public PickingVerifyBooleanModel()
        {
            Timestamp = DateTime.Now;
        }

        public PickingVerifyBooleanModel(bool input)
        {
            Value = input;
            Timestamp = DateTime.Now;
        }

        public bool Value { get; set; } = false;
        public string Signature { get; set; }
        public DateTime Timestamp { get; set; }
    }
}