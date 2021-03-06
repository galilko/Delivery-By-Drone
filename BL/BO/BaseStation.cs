using System.Collections.Generic;

namespace BO
{
    public class BaseStation : ILocatable
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public Location Location { get; set; }
        public int? FreeChargeSlots { get; set; }
        public List<DroneInCharge> DronesInCharge { get; set; }
        public override string ToString()
        {
            string result = "";
            result += $"Id:\t\t\t {Id}\n";
            result += $"Name:\t\t\t {Name}\n";
            result += $"Location:\t\t {Location}\n";
            result += $"Num of charge slots:\t {FreeChargeSlots}\n";
            if (DronesInCharge.Count > 0)
                foreach (var item in DronesInCharge)
                    result += $"{item.ToString()}\n";
            else
                result += "There aren't drones in charge\n";
            return result;
        }
    }
}
