using System;
using BO;
using System.Threading;
using static BL.BL;
using System.Linq;
using static System.Math;
using System.Runtime.Serialization;

namespace BL
{
    internal class DroneSimulator
    {
        enum Maintenance { Starting, Going, Charging }
        private const double VELOCITY = 1.0; // speed
        private const int DELAY = 500; 
        private const double TIME_STEP = DELAY / 1000.0; // 0.5
        private const double STEP = VELOCITY / TIME_STEP; // 2

        public DroneSimulator(BL bl1, int droneId, Action updateDrone, Func<bool> checkStop)
        {
            var bl = bl1;
            var dal = bl.MyDal;
            var drone = bl.GetDroneToList(droneId); // simulated drone
            int? parcelId = null;
            int? baseStationId = null;
            BaseStation bs = null;
            double distance = 0.0;
            int batteryUsage = 0;
            DO.Parcel? parcel = null;
            bool pickedUp = false;
            Customer customer = null;
            Maintenance maintenance = Maintenance.Starting;

            void initDelivery(int id)
            {
                parcel = dal.GetParcel(id);
                batteryUsage = (int)Enum.Parse(typeof(BatteryUsage), parcel?.Weight.ToString());
                pickedUp = parcel?.PickedUp is not null; // if parcel was picked up => pickedUp = true
                customer = bl.GetCustomer((int)(pickedUp ? parcel?.TargetId : parcel?.SenderId));
            }

            do
            {
                switch (drone)
                {
                    // if the drone is available
                    case DroneToList { Status: DroneStatusCategories.Free }:
                        if (!sleepDelayTime()) break;
                        lock (bl) lock (dal)
                            {
                                // Assign to parcelId the most match parcel according to conditions
                                parcelId = bl.MyDal.GetParcels(p => p?.Scheduled == null
                                                                  && (WeightCategories)(p?.Weight) <= drone.Weight
                                                                  && drone.RequiredBattery(bl, (int)p?.Id) < drone.BatteryStatus)
                                                 .OrderByDescending(p => p?.Priority)
                                                 .ThenByDescending(p => p?.Weight)
                                                 .FirstOrDefault()?.Id;
                                // 
                                switch (parcelId, drone.BatteryStatus)
                                {
                                    // there isn't parcel to pick up for this drone and its charging is full
                                    // the problem is with the parcels and isn't with drone
                                    // maybe there is none-scheduled but not-match parcels
                                    case (null, 100.0):
                                        break;

                                    // there isn't parcel to pick up for this drone and its charging isn't full
                                    // maybe the drone don't have enough battery
                                    case (null, _):
                                        baseStationId = bl.FindClosestBaseStation(drone, forCharge: true)?.Id;
                                        if (baseStationId != null)
                                        {
                                            drone.Status = DroneStatusCategories.Maintenance;
                                            maintenance = Maintenance.Starting;
                                            dal.ChargeDrone(droneId, (int)baseStationId);
                                        }
                                        break;
                                    // there is a match parcel for this drone
                                    case (_, _):
                                        try
                                        {
                                            dal.ScheduleDroneForParcel((int)parcelId, droneId);
                                            drone.TransferdParcel = (int)parcelId;
                                            initDelivery((int)parcelId);
                                            drone.Status = DroneStatusCategories.Delivery;
                                        }
                                        catch (DO.ExistIdException ex) { throw new BadStatusException("Internal error getting parcel", ex); }
                                        break;
                                }
                            }
                        break;

                    // if the drone is in maintenance
                    case DroneToList { Status: DroneStatusCategories.Maintenance }:
                        switch (maintenance)
                        {
                            case Maintenance.Starting:
                                lock (bl) lock (dal)
                                    {
                                        try { bs = bl.GetBaseStation(baseStationId ?? dal.GetDroneChargeBaseStationId((int)drone.Id)); }
                                        catch (DO.ExistIdException ex) { throw new BadStatusException("Internal error base station", ex); }
                                        distance = drone.Distance(bs);
                                        maintenance = Maintenance.Going;
                                    }
                                break;

                            case Maintenance.Going:
                                if (distance < 0.01)
                                    lock (bl)
                                    {
                                        drone.Location = bs.Location;
                                        maintenance = Maintenance.Charging;
                                    }
                                else
                                {
                                    if (!sleepDelayTime()) break;
                                    lock (bl)
                                    {
                                        double delta = distance < STEP ? distance : STEP;
                                        distance -= delta;
                                        drone.BatteryStatus = Max(0.0, drone.BatteryStatus - delta * bl.BatteryUsages[DRONE_FREE]);
                                    }
                                }
                                break;

                            case Maintenance.Charging:
                                if (drone.BatteryStatus == 100.0)
                                    lock (bl) lock (dal)
                                        {
                                            drone.Status = DroneStatusCategories.Free;
                                            dal.ReleaseDroneFromCharge(droneId);
                                        }
                                else
                                {
                                    if (!sleepDelayTime()) break;
                                    lock (bl) drone.BatteryStatus = Min(100.0, drone.BatteryStatus + bl.BatteryUsages[DRONE_CHARGE] * TIME_STEP);
                                }
                                break;
                            default:
                                throw new BadStatusException("Internal error: wrong maintenance substate");
                        }
                        break;

                    case DroneToList { Status: DroneStatusCategories.Delivery }:
                        lock (bl) lock (dal)
                            {
                                try { if (parcelId == null) initDelivery((int)drone.TransferdParcel); }
                                catch (DO.ExistIdException ex) { throw new BadStatusException("Internal error getting parcel", ex); }
                                distance = drone.Distance(customer);
                            }

                        if (distance < 0.01 || drone.BatteryStatus == 0.0)
                            lock (bl) lock (dal)
                                {
                                    drone.Location = customer.Location;
                                    if (pickedUp)
                                    {
                                        dal.DeliverAParcel((int)parcel?.Id);
                                        drone.Status = DroneStatusCategories.Free;

                                    }
                                    else
                                    {
                                        dal.PickingUpAParcel((int)parcel?.Id);
                                        customer = bl.GetCustomer((int)parcel?.TargetId);
                                        pickedUp = true;
                                    }
                                }
                        else
                        {
                            if (!sleepDelayTime()) break;
                            lock (bl)
                            {
                                double delta = distance < STEP ? distance : STEP;
                                double proportion = delta / distance;
                                drone.BatteryStatus= Max(0.0, drone.BatteryStatus- delta * bl.BatteryUsages[pickedUp ? batteryUsage : DRONE_FREE]);
                                double? lat = drone.Location.Latitude + (customer.Location.Latitude - drone.Location.Latitude) * proportion;
                                double? lon = drone.Location.Longitude + (customer.Location.Longitude - drone.Location.Longitude) * proportion;
                                drone.Location = new() { Latitude = lat, Longitude = lon };
                            }
                        }
                        break;

                    default:
                        throw new BadStatusException("Internal error: not available after Delivery...");

                }
                updateDrone();
            } while (!checkStop());
        }

        /// <summary>
        /// Sent thread to sleep and return true if succeed
        /// </summary>
        /// <returns></returns>
        private static bool sleepDelayTime()
        {
            try { Thread.Sleep(DELAY); }
            catch (ThreadInterruptedException) { return false; }
            return true;
        }
    }

}
