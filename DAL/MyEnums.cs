using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace IDAL
{
    namespace DO
    {
        public enum WeightCategories
        {
            [Description("light")]
            Light = 0,
            [Description("medium")]
            Medium = 1,
            [Description("heavy")]
            Heavy = 2
        }
        public enum DroneStatusCategories
        {
            [Description("free")]
            Free = 0,
            [Description("maintenance")]
            Maintenance = 1,
            [Description("delivery")]
            Delivery = 2
        }
        public enum Priorities
        {
            [Description("normal")]
            Normal,
            [Description("fast")]
            Fast,
            [Description("emergency")]
            Emergency
        }
        class MyEnums
        {
        }
    }
}
