using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        struct BaseStation
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Longitude { get; set; }
            public double Lattitude { get; set; }
            public int ChargeSlots { get; set; }
            public override string ToString()
            {
                string result = "";
                result += $"Id: {Id} \n ";
                result += $"Name: {Name} \n ";
                result += $"Longitude: {Longitude} \n ";
                result += $"Lattitude: {Lattitude} \n ";
                result += $"Num of charge slots: {ChargeSlots} \n ";
                return result;
            }
        }
    }
}
