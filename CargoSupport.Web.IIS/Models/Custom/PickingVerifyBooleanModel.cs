﻿using System;

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

        public DateTime _timestamp;
        public bool Value { get; set; } = false;
        public string Signature { get; set; }
    }
}