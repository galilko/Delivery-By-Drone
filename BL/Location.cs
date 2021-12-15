namespace BO
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
        public override string ToString() => $"({DalApi.Converter.LatitudeToSexadecimal(Latitude)} , {DalApi.Converter.LongitudeToSexadecimal(Longitude)})";

        /* internal double CalcDistance(Location senderLocation)
         {
             return Math.Sqrt(Math.Pow(this.Latitude - senderLocation.Latitude, 2) + Math.Pow(this.Longitude - senderLocation.Longitude, 2));
         }*/
    }
}
