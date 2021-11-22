using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    class LocationFuncs
    {
        internal static double DistanceBetweenTwoLocations(Location l1, Location l2)
        {
            return Math.Sqrt(Math.Pow(l1.Latitude - l2.Latitude, 2) + Math.Pow(l1.Longitude - l2.Longitude, 2));
        }

        internal static Location ClosestBaseStationLocation(List<IDAL.DO.BaseStation> BaseStations, Location MyLocation)
        {
            Location ClosestLocation = new Location();
            double MinDistance = double.MaxValue;
            foreach (var bs in BaseStations)
            {
                Location BSLocation = new() { Latitude = bs.Lattitude, Longitude = bs.Longitude };
                if (LocationFuncs.DistanceBetweenTwoLocations(BSLocation, MyLocation) < MinDistance)
                {
                    MinDistance = LocationFuncs.DistanceBetweenTwoLocations(BSLocation, MyLocation);
                    ClosestLocation = BSLocation;
                }
            }
            return ClosestLocation;
        }

    }
}
