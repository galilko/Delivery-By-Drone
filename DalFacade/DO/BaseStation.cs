using DalApi;

namespace DO
{
    public struct BaseStation
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int FreeChargeSlots { get; set; }
        public bool IsActive { get; set; }
        public override string ToString()
        {
            string result = "";
            result += $"Id:\t\t\t {Id}\n";
            result += $"Name:\t\t\t {Name}\n";
            result += $"Latitude:\t\t {Converter.LatitudeToSexadecimal(Latitude)}\n";
            result += $"Longitude:\t\t {Converter.LongitudeToSexadecimal(Longitude)}\n";
            result += $"Num of charge slots:\t {FreeChargeSlots}\n";
            return result;
        }
    }
}

