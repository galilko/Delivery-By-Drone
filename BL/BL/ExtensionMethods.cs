using System;
using BO;
using System.Linq;
using static BL.BL;

namespace BL
{
    internal static class ExtensionMethods
    {
        /// <summary>
        /// calculate the accurate distance between two locations in KM
        /// </summary>
        /// <param name="from">source location</param>
        /// <param name="to">target location</param>
        /// <returns></returns>
        internal static double Distance(this ILocatable from, ILocatable to)
        {
            int R = 6371 * 1000; // metres
            double? phi1 = from.Location.Latitude * Math.PI / 180; // φ, λ in radians
            double? phi2 = to.Location.Latitude * Math.PI / 180;
            double? deltaPhi = (to.Location.Latitude - from.Location.Latitude) * Math.PI / 180;
            double? deltaLambda = (to.Location.Longitude - from.Location.Longitude) * Math.PI / 180;
                  
            double a = Math.Sin((double)deltaPhi / 2) * Math.Sin((double)deltaPhi / 2) +
                       Math.Cos((double)phi1) * Math.Cos((double)phi2) *
                       Math.Sin((double)deltaLambda / 2) * Math.Sin((double)deltaLambda / 2);
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            double d = R * c / 1000; // in kilometres
            return d;
        }
        /// <summary>
        /// Calculate the required battery to deliver a parcel for drone
        /// </summary>
        /// <param name="drone"></param>
        /// <param name="bl"></param>
        /// <param name="parcelId"></param>
        /// <returns></returns>
        internal static double RequiredBattery(this ILocatable drone, BL bl, int parcelId)
        {
            DO.Parcel parcel = bl.MyDal.GetParcel(parcelId);
            Customer sender = bl.GetCustomer(parcel.SenderId);
            Customer target = bl.GetCustomer(parcel.TargetId);
            double battery = bl.BatteryUsages[(int)Enum.Parse(typeof(BatteryUsage), parcel.Weight.ToString())] * sender.Distance(target); // required battery from sender to target
            battery += bl.BatteryUsages[DRONE_FREE] * target.Distance(bl.FindClosestBaseStation(target, false)); // required battery from target to closest base-station
            if (parcel.PickedUp is null)
                battery += bl.BatteryUsages[DRONE_FREE] * drone.Distance(sender); // required battery from drone to sender
            return battery;
        }
        /*
        internal static (DroneStatuses, int) nextActionA(this DroneForList drone, BlImp bl)
        {
            DO.Parcel? parcel;
            return drone.Status switch
            {
                DroneStatuses.Available =>
                        (parcel = bl.Dal.GetParcels(p => p?.Scheduled == null
                                        && (WeightCategories)(p?.Weight) <= drone.MaxWeight
                                        && drone.RequiredBattery(bl, (int)p?.Id) < drone.Battery)
                                                .OrderByDescending(p => p?.Priority)
                                                .ThenByDescending(p => p?.Weight).FirstOrDefault()) == null
                        ? (drone.Battery == 1.0 ? (DroneStatuses.Available, 0)
                                                : (DroneStatuses.Maintenance, bl.FindClosestBaseStation(drone, charge: true).Id))
                        : (DroneStatuses.Delivery, (int)parcel?.Id),

                DroneStatuses.Delivery =>
                    bl.Dal.GetParcel((int)drone.DeliveryId).Delivered != null ? (DroneStatuses.Available, 0)
                                                                              : (DroneStatuses.Delivery, (int)drone.DeliveryId),

                DroneStatuses.Maintenance => (DroneStatuses.Available, 0),

                DroneStatuses.None => (DroneStatuses.None, 0),

                _ => (DroneStatuses.None, 0)
            };
        }
        */

    }
}

