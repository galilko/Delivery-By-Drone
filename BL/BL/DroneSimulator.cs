/*using System;
using BO;
using System.Threading;
using static BlApi.BL;
using System.Linq;
using static System.Math;

namespace BL
{
    internal class DroneSimulator
    {
        enum Maintenance { Starting, Going, Charging }
        private const double VELOCITY = 1.0;
        private const int DELAY = 500;
        private const double TIME_STEP = DELAY / 1000.0;
        private const double STEP = VELOCITY / TIME_STEP;

        public DroneSimulator(BlApi.BL blImp, int droneId, Action updateDrone, Func<bool> checkStop)
        {
            var bl = blImp;
            var dal = bl.MyDal;
            var drone = bl.GetDroneToList(droneId);
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
                parcel = dal.FindParcel(id);
                batteryUsage = (int)Enum.Parse(typeof(BatteryUsage), parcel?.Weight.ToString());
                pickedUp = parcel?.PickedUp is not null;
                customer = bl.FindCustomer((int)(pickedUp ? parcel?.TargetId : parcel?.SenderId));
            }

            do
            {
                //(var next, var id) = drone.nextAction(bl);

                switch (drone)
                {
                    case DroneToList { Status: DroneStatusCategories.Free }:
                        if (!sleepDelayTime()) break;

                        lock (bl) lock (dal)
                            {
                                parcelId = bl.MyDal.AllParcels(p => p.Scheduled == null
                                                                  && (WeightCategories)(p.Weight) <= drone.Weight
                                                                  && drone.RequiredBattery(bl, (int)p.Id) < drone.BatteryStatus)
                                                 .OrderByDescending(p => p.Priority)
                                                 .ThenByDescending(p => p.Weight)
                                                 .FirstOrDefault()?.Id;
                                switch (parcelId, drone.BatteryStatus)
                                {
                                    case (null, 1.0):
                                        break;

                                    case (null, _):
                                        baseStationId = bl.FindClosestBaseStation(drone, charge: true)?.Id;
                                        if (baseStationId != null)
                                        {
                                            drone.Status = DroneStatusCategories.Maintenance;
                                            maintenance = Maintenance.Starting;
                                            dal.BaseStationDroneIn((int)baseStationId);
                                            dal.ChargeDrone(droneId, (int)baseStationId);
                                        }
                                        break;
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

                    case DroneToList { Status: DroneStatusCategories.Maintenance }:
                        switch (maintenance)
                        {
                            case Maintenance.Starting:
                                lock (bl) lock (dal)
                                    {
                                        try { bs = bl.FindBaseStation(baseStationId ?? dal.GetDroneChargeBaseStationId(drone.Id)); }
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
                                if (drone.BatteryStatus == 1.0)
                                    lock (bl) lock (dal)
                                        {
                                            drone.Status = DroneStatusCategories.Free;
                                            dal.ReleaseDroneFromCharge(droneId);
                                            //dal.BaseStationDroneOut(bs.Id);
                                        }
                                else
                                {
                                    if (!sleepDelayTime()) break;
                                    lock (bl) drone.BatteryStatus = Min(1.0, drone.BatteryStatus + bl.BatteryUsages[DRONE_CHARGE] * TIME_STEP);
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
                                        customer = bl.FindCustomer((int)parcel?.TargetId);
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
                                double lat = drone.Location.Latitude + (customer.Location.Latitude - drone.Location.Latitude) * proportion;
                                double lon = drone.Location.Longitude + (customer.Location.Longitude - drone.Location.Longitude) * proportion;
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

        private static bool sleepDelayTime()
        {
            try { Thread.Sleep(DELAY); } catch (ThreadInterruptedException) { return false; }
            return true;
        }
    }
}
*/