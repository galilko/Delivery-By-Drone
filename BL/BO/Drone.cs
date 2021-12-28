using System;

namespace BO
{
    public class Drone
    {
        public int? Id { get; set; }
        public string Model { get; set; }
        public WeightCategories Weight { get; set; }
        public double BatteryStatus { get; set; }
        public DroneStatusCategories Status { get; set; }
        public ParcelInTransfer CurrentParcel { get; set; }
        public Location CurrentLocation { get; set; }

        public override string ToString()
        {
            string result = String.Format("{0}\t\t\t{1}\t{2}\n", "Id", ":", Id);
            result += String.Format("{0}\t\t\t{1}\t{2}\n", "Model", ":", Model);
            result += String.Format("{0}\t\t\t{1}\t{2}\n", "Weight", ":", Weight);
            result += String.Format("{0}\t\t{1}\t{2}\n", "Battery Status", ":", BatteryStatus);
            result += String.Format("{0}\t\t\t{1}\t{2}\n", "Status", ":", Status);
            result += String.Format("{0}\t\t{1}\t{2}\n", "Current Location", ":", CurrentLocation);
            if (CurrentParcel != null)
            {
                result += String.Format("{0}\t{1}\n", "Parcel In Tranfer", ":");
                result += CurrentParcel;
            }
            return result;
        }
    }
}
