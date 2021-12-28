using System.ComponentModel;


namespace DO
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

    public enum Priorities
    {
        [Description("normal")]
        Normal,
        [Description("fast")]
        Fast,
        [Description("emergency")]
        Emergency
    }
    class Enums
    {
    }
}

