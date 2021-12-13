using System.ComponentModel;

namespace BO
{
    public enum WeightCategories
    {
        [Description("light")]
        Light = 1,
        [Description("medium")]
        Medium = 2,
        [Description("heavy")]
        Heavy = 3
    }
    public enum DroneStatusCategories
    {
        [Description("free to get a Parcel")]
        Free = 0,
        [Description("maintenance")]
        Maintenance = 1,
        [Description("delivery")]
        Delivery = 2
    }
    public enum Priorities
    {
        [Description("normal")]
        Normal = 1,
        [Description("fast")]
        Fast = 2,
        [Description("emergency")]
        Emergency = 3
    }
    public enum ParcelStatus
    {
        [Description("defined")]
        Defined,
        [Description("scheduled")]
        Scheduled,
        [Description("picked up")]
        PickedUp,
        [Description("delivered")]
        Delivered
    }
}