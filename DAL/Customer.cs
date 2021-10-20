using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public double Longitude { get; set; }
            public double Lattitude { get; set; }
            public override string ToString()
            {
                string str = "";
                str += $"Id:\t\t {Id}\n";
                str += $"Name:\t\t {Name}\n";
                str += $"Phone:\t\t {Phone}\n";
                str += $"Longitude:\t {Longitude}\n";
                str += $"Lattitude:\t {Lattitude}\n";
                return str;
            }
        }
    }
}
