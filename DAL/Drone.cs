using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        class Drone
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }
            public DroneStatusCategories Status { get; set; }
            public double Battery { get; set; }
            public override string ToString()
            {
                string result = "";
                result += $"Id: {Id} \n ";
                result += $"Model: {Model} \n ";
                result += $"Max weight: {MaxWeight} \n ";
                result += $"Drone status: {Status} \n ";
                result += $"Battery status: {Battery} \n ";
                return result;
            }
        }
    }
}
