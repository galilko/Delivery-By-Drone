using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BO
{
    public class BaseStation
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Location BSLocation { get; set; }
        public int FreeChargeSlots { get; set; }
        public List<DroneInCharge> DronesInCharge { get; set; }
        public override string ToString()
        {
            string result = "";
            result += $"Id:\t\t\t {Id}\n";
            result += $"Name:\t\t\t {Name}\n";
            result += $"Location:\t\t {BSLocation}\n";
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
