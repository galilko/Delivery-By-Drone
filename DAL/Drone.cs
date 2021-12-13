using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


    namespace DO
    {
        public struct Drone
        {
            public int Id { get; set; }
            public string Model { get; set; }
            public WeightCategories MaxWeight { get; set; }
            public override string ToString()
            {
                string result = "";
                result += $"Id:\t\t {Id}\n";
                result += $"Model:\t\t {Model}\n";
                result += $"Max weight:\t {MaxWeight}\n";
                return result;
            }
        }
    }

