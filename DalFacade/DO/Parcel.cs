using System;


namespace DO
{
    public struct Parcel
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public int TargetId { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public int DroneId { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Scheduled { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }

        public override string ToString()
        {
            string result = "";
            result += $"Id:\t\t {Id}\n";
            result += $"Sender Id:\t {SenderId}\n";
            result += $"Target Id:\t {TargetId}\n";
            result += $"Weight:\t\t {Weight}\n";
            result += $"Priority:\t {Priority}\n";
            result += $"Requested:\t {Requested}\n";
            result += $"Drone Id:\t {DroneId}\n";
            result += $"Scheduled:\t ";
            result += (Scheduled == null) ? "Not yet\n" : $"{Scheduled}\n";
            result += $"Picked Up:\t ";
            result += (PickedUp == null) ? "Not yet\n" : $"{PickedUp}\n";
            result += $"Delivered:\t ";
            result += (Delivered == null) ? "Not yet\n" : $"{Delivered}\n";
            return result;
        }
    }
}

