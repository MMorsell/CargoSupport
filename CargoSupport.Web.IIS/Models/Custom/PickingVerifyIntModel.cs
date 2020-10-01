using System;

namespace CargoSupport.Models
{
    public class PickingVerifyIntModel
    {
        public PickingVerifyIntModel()
        {
            _timestamp = DateTime.Now;
        }

        public PickingVerifyIntModel(int input)
        {
            Value = input;
            _timestamp = DateTime.Now;
        }

        public PickingVerifyIntModel(int input, string signature)
        {
            Value = input;
            Signature = signature;
            _timestamp = DateTime.Now;
        }

        public DateTime _timestamp;
        public int Value { get; set; } = 0;
        public string Signature { get; set; }
    }
}