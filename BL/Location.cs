using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Location
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Location() { }
        public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
        public override string ToString() => $"(Latitude: {IDAL.Converter.LatitudeToSexadecimal(Latitude)} , Longitude: {IDAL.Converter.LongitudeToSexadecimal(Longitude)})";

        internal double CalcDistance(Location senderLocation)
        {
            return Math.Sqrt(Math.Pow(this.Latitude - senderLocation.Latitude, 2) + Math.Pow(this.Longitude - senderLocation.Longitude, 2));
        }
    }
}
