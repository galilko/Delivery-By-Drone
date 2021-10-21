using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct BaseStation
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Lattitude { get; set; }
            public double Longitude { get; set; }
            public int FreeChargeSlots { get; set; }
            public override string ToString()
            {
                string result = "";
                result += $"Id:\t\t\t {Id}\n";
                result += $"Name:\t\t\t {Name}\n";
                result += $"Lattitude:\t\t {Converter.LatitudeToSexadecimal(Lattitude)}\n";
                result += $"Longitude:\t\t {Converter.LongitudeToSexadecimal(Longitude)}\n";
                result += $"Num of charge slots:\t {FreeChargeSlots}\n";
                return result;
            }
        }
    }
}
