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
        enum WeightCategories
        {
            [Description("light")]
            Light,
            [Description("medium")]
            Medium,
            [Description("heavy")]
            Heavy
        }
        enum DroneStatusCategories
        {
            [Description("free")]
            Free,
            [Description("maintenance")]
            Maintenance,
            [Description("delivery")]
            Delivery
        }
        enum Priorities
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
