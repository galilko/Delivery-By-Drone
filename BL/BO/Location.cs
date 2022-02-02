namespace BO
{
    public class Location
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Location() { }
        /*public Location(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }*/
        public override string ToString() => $"({DalApi.Converter.LatitudeToSexadecimal((double)Latitude)} , {DalApi.Converter.LongitudeToSexadecimal((double)Longitude)})";
    }
}
