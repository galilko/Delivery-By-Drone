﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    class DroneInCharge
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }

        public override string ToString() => ToolStringClass.ToStringProperty(this); 
    }
}
