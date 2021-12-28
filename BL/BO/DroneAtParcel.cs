namespace BO
{
    public class DroneAtParcel
    {
        public int? Id { get; set; }
        public double BatteryStatus { get; set; }
        public Location CurrentLocation { get; set; }
        public override string ToString() => $"id: {Id}\tBattery Status: {BatteryStatus}\tCurrent Location: {CurrentLocation} ";
    }
}