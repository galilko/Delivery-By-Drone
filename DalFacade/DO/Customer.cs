using DalApi;

namespace DO
{
    public struct Customer
    {
        public int Id { get; init; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsActive { get; set; }
        public override string ToString()
        {
            string str = "";
            str += $"Id:\t\t {Id}\n";
            str += $"Name:\t\t {Name}\n";
            str += $"Phone:\t\t {Phone}\n";
            str += $"Latitude:\t {Converter.LatitudeToSexadecimal(Latitude)}\n";
            str += $"Longitude:\t {Converter.LongitudeToSexadecimal(Longitude)}\n";
            return str;
        }

    }
}

/*
# You have decimal degrees (-73.9874°) instead of degrees, minutes, and seconds (-73° 59’ 14.64")
# The whole units of degrees will remain the same (-73.9874° longitude, start with 73°)
# Multiply the decimal by 60 (0.9874 * 60 = 59.244)
# The whole number becomes the minutes (59’)
# Take the remaining decimal and multiply by 60. (0.244 * 60 = 14.64)
# The resulting number becomes the seconds (14.64"). Seconds can remain as a decimal.
# Take your three sets of numbers and put them together, using the symbols for degrees (°), minutes (’), and seconds (") (-73° 59’ 14.64" longitude)
*/