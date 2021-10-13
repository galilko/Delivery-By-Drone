using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        class Customer
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public double Longitude { get; set; }
            public double Lattitude { get; set; }
            public override string ToString()
            {
                string str = "";
                str += $"Id: {Id}";
                str += $"Name: {Name}";
                str += $"Phone: {Phone}";
                str += $"Longitude: {Longitude}";
                str += $"Lattitude: {Lattitude}";
                return str;
            }
        }
    }
}
