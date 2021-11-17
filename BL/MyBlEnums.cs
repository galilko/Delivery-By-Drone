using System.ComponentModel;

namespace IBL.BO
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