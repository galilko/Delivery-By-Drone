using System;
using System.Collections.Generic;

namespace BO
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
            return Math.Sqrt(Math.Pow((double)p1.Latitude - (double)p2.Latitude, 2) + Math.Pow((double)p1.Longitude - (double)p2.Longitude, 2));
        }

        internal static Location ClosestBaseStationLocation(List<DO.BaseStation> BaseStations, Location MyLocation)
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
