namespace IBL.BO
{
    class DroneAtParcel
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public Location CurrentLocation { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}