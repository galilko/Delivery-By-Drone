using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    class BaseStation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location BSLocation { get; set; }
        public int FreeChargeSlots { get; set; }
        public List<DroneInCharge> DronesInCharge { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}
