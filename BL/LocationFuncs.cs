using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    class LocationFuncs
    {
        /// <summary>
        /// return the distance between two points
        /// </summary>
        /// <param name="l1">point 1</param>
        /// <param name="l2">point 2</param>
        /// <returns> double distance</returns>
        internal static double DistanceBetweenTwoLocations(Location p1, Location p2)
        {
            return Math.Sqrt(Math.Pow(p1.Latitude - p2.Latitude, 2) + Math.Pow(p1.Longitude - p2.Longitude, 2));
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
