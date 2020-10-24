using System;

namespace CargoSupport.Models
{
    public class PickingVerifyBooleanModel
    {
        public PickingVerifyBooleanModel()
        {
            _timestamp = DateTime.Now;
        }

        public PickingVerifyBooleanModel(bool input)
        {
            Value = input;
            _timestamp = DateTime.Now;
        }

        public PickingVerifyBooleanModel(bool input, string signature)
        {
            Value = input;
            Signature = signature;
            _timestamp = DateTime.Now;
        }

        public DateTime _timestamp { get; set; }
        public bool Value { get; set; } = false;
        public string Signature { get; set; }
    }
}