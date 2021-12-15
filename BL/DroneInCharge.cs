namespace BO
{
    public class DroneInCharge
    {
        public int Id { get; set; }
        public double BatteryStatus { get; set; }
        public override string ToString() => $"Id: {Id,-15} Battery Status: {BatteryStatus}";
    }
}
