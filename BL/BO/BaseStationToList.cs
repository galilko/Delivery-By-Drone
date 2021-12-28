using System;

namespace BO
{
    public class BaseStationToList
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public int FreeChargeSlots { get; set; }
        public int BusyChargeSlots { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}
