using System.ComponentModel;


namespace DO
{
    public enum WeightCategories
    {
        Light = 1,
        Medium = 2,
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

