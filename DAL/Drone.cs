using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Drone
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }
            public DroneStatusCategories Status { get; set; }
            public double Battery { get; set; }
            public override string ToString()
            {
                string result = "";
                result += $"Id:\t\t {Id}\n";
                result += $"Model:\t\t {Model}\n";
                result += $"Max weight:\t {MaxWeight}\n";
                result += $"Drone status:\t {Status}\n";
                result += $"Battery status:\t {Battery.ToString("F2")} %\n";
                return result;
            }
        }
    }
}
