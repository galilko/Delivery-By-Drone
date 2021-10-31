using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;

namespace DalObject
{
    public class DalObject
    {
        public DalObject()
        {
            DataSource.Initialize();
        }

        /// <summary>
        /// schedule a drone for parcel delivery
        /// </summary>
        /// <param name="parcelId">parcel id for deliver</param>
        /// <param name="droneId">drone id for schedule</param>
        public void ScheduleDroneForParcel(int parcelId, int droneId)
        {
            Parcel found = DataSource.ParcelsList.FirstOrDefault(p => p.Id == parcelId);
            if (!found.Equals(default(Parcel)))
            {
                Drone found1 = DataSource.DronesList.FirstOrDefault(p => p.Id == droneId);
                if (!found1.Equals(default(Drone)))
                {
                    DataSource.ParcelsList.Remove(found);
                    found.DroneId = droneId;
                    found.Scheduled = DateTime.Now;
                    DataSource.ParcelsList.Add(found);
                }
            }
        }
        /// <summary>
        /// picking up a parcel by its drone
        /// </summary>
        /// <param name="parcelId">parcel id to picking up</param>
        public void PickingUpAParcel(int parcelId)
        {
            Parcel found = DataSource.ParcelsList.FirstOrDefault(p => p.Id == parcelId);
            if (!found.Equals(default(Parcel)))
            {
                DataSource.ParcelsList.Remove(found);
                found.PickedUp = DateTime.Now;
                DataSource.ParcelsList.Add(found);
                Drone found1 = DataSource.DronesList.FirstOrDefault(p => p.Id == found.DroneId);
                if (!found1.Equals(default(Drone)))
                {
                    DataSource.DronesList.Remove(found1);
                    found1.Status = DroneStatusCategories.Delivery;
                    DataSource.DronesList.Add(found1);
                }
            }
        }
        /// <summary>
        /// finish delivery of a parcel
        /// </summary>
        /// <param name="parcelId">parcel id to deliver</param>
        public void DeliverAParcel(int parcelId)
        {
            Parcel found = DataSource.ParcelsList.FirstOrDefault(p => p.Id == parcelId);
            if (!found.Equals(default(Parcel)))
            {
                DataSource.ParcelsList.Remove(found);
                found.Delivered = DateTime.Now;
                DataSource.ParcelsList.Add(found);
                Drone found1 = DataSource.DronesList.FirstOrDefault(p => p.Id == found.DroneId);
                if (!found1.Equals(default(Drone)))
                {
                    DataSource.DronesList.Remove(found1);
                    found1.Status = DroneStatusCategories.Free;
                    DataSource.DronesList.Add(found1);
                }
            }
        }
        /// <summary>
        /// connect a drone to charging at base-station
        /// </summary>
        /// <param name="droneId">drone's id to charge</param>
        /// <param name="baseStationId">requested base-station</param>
        public void ChargeDrone(int droneId, int baseStationId)
        {
            Drone found = DataSource.DronesList.FirstOrDefault(p => p.Id == droneId);
            if (!found.Equals(default(Drone)))
            {
                BaseStation found1 = DataSource.BaseStationsList.FirstOrDefault(p => p.Id == baseStationId);
                if (!found1.Equals(default(BaseStation)))
                {
                    DataSource.DronesList.Remove(found);
                    DataSource.BaseStationsList.Remove(found1);
                    found.Status = DroneStatusCategories.Maintenance;
                    found1.FreeChargeSlots--;
                    DataSource.BaseStationsList.Add(found1);
                    DataSource.DronesList.Add(found);
                    DataSource.DroneChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
                }
            }
        }
        /// <summary>
        /// release a drone from charging
        /// </summary>
        /// <param name="droneId">drone's id to release</param>
        public void ReleaseDroneFromCharge(int droneId)
        {
            for (int i = 0; i < DataSource.Config.DroneIndex; i++)
                if (DataSource.DronesList[i].Id == droneId)
                {
                    DataSource.DronesList[i].Status = DroneStatusCategories.Free;
                    DroneCharge droneCharge = DataSource.DroneChargeList.Find(myDroneCharge => myDroneCharge.DroneId == droneId);
                    for (int j = 0; j < DataSource.Config.BaseStationIndex; j++)
                        if (DataSource.BaseStationsList[j].Id == droneCharge.StationId)
                        {
                            DataSource.BaseStationsList[j].FreeChargeSlots++;
                            DataSource.DroneChargeList.Remove(droneCharge);
                            break;
                        }
                }
        }

        /// <summary>
        /// adding a new base atation into array
        /// </summary>
        /// <param name="name"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="freeSlots"></param>
        public void AddBaseStation(string name, double latitude, double longitude, int freeSlots)
        {
            if (DataSource.Config.BaseStationIndex < 5)
            {
                DataSource.BaseStationsList[DataSource.Config.BaseStationIndex] = new BaseStation()
                {
                    Id = 10 + DataSource.Config.BaseStationIndex++,
                    Name = name,
                    Lattitude = latitude,
                    Longitude = longitude,
                    FreeChargeSlots = freeSlots
                };

            }
        }
        /// <summary>
        /// adding a new drone into array
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="weight"></param>
        /// <param name="status"></param>
        /// <param name="battery"></param>
        public void AddDrone(int id, string model, WeightCategories weight, DroneStatusCategories status, double battery)
        {
            if (DataSource.Config.BaseStationIndex < 10)
            {
                DataSource.DronesList[DataSource.Config.DroneIndex++] = new Drone()
                {
                    Id = id,
                    Model = model,
                    MaxWeight = weight,
                    Status = status,
                    Battery = battery
                };
            }
        }
        /// <summary>
        /// adding a new customer into array
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void AddCustomer(int id, string name, string phone, double latitude, double longitude)
        {
            if (DataSource.Config.BaseStationIndex < 100)
            {
                DataSource.CustomersList[DataSource.Config.CustomerIndex++] = new Customer()
                {
                    Id = id,
                    Name = name,
                    Phone = phone,
                    Lattitude = latitude,
                    Longitude = longitude
                };
            }
        }
        /// <summary>
        /// adding a new parcel into array
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="targetId"></param>
        /// <param name="weight"></param>
        /// <param name="priority"></param>
        public void AddParcel(int senderId, int targetId, WeightCategories weight, Priorities priority)
        {
            if (DataSource.Config.BaseStationIndex < 1000)
            {
                DataSource.ParcelsList[DataSource.Config.ParcelIndex++] = new Parcel()
                {
                    Id = DataSource.Config.NewParcelId++,
                    SenderId = senderId,
                    TargetId = targetId,
                    Weight = weight,
                    Priority = priority,
                    Requested = DateTime.Now
                };
            }
        }

        /// <summary>
        /// find a base-station by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseStation FindBaseStation(int id)
        {
            foreach (var item in DataSource.BaseStationsList)
                if (item.Id == id)
                    return item;
            return default;
        }
        /// <summary>
        /// find a drone by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Drone FindDrone(int id)
        {
            foreach (var item in DataSource.DronesList)
                if (item.Id == id)
                    return item;
            return default;
        }
        /// <summary>
        /// find a customer by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer FindCustomer(int id)
        {
            foreach (var item in DataSource.CustomersList)
                if (item.Id == id)
                    return item;
            return default;
        }
        /// <summary>
        /// find a parcel by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Parcel FindParcel(int id)
        {
            foreach (var item in DataSource.ParcelsList)
                if (item.Id == id)
                    return item;
            return default;
        }

        /// <summary>
        /// return array of customers
        /// </summary>
        /// <returns></returns>
        public Customer[] AllCustomers()
        {
            return DataSource.CustomersList.Skip(0).Take(DataSource.Config.CustomerIndex).ToArray();
        }
        /// <summary>
        /// return array of drones
        /// </summary>
        /// <returns></returns>
        public Drone[] AllDrones()
        {
            return DataSource.DronesList.Skip(0).Take(DataSource.Config.DroneIndex).ToArray();
        }
        /// <summary>
        /// return array of base-stations
        /// </summary>
        /// <returns></returns>
        public BaseStation[] AllBaseStations()
        {
            return DataSource.BaseStationsList.Skip(0).Take(DataSource.Config.BaseStationIndex).ToArray();
        }
        /// <summary>
        /// return array of parcels
        /// </summary>
        /// <returns></returns>
        public Parcel[] AllParcels()
        {
            return DataSource.ParcelsList.Skip(0).Take(DataSource.Config.ParcelIndex).ToArray();
        }
        /// <summary>
        /// return array of none-scheduled parcels
        /// </summary>
        /// <returns></returns>
        public Parcel[] NoneScheduledParcels()
        {
            List<Parcel> tmpList = new();
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                if (DataSource.ParcelsList[i].Scheduled == DateTime.MinValue)
                    tmpList.Add(DataSource.ParcelsList[i]);
            return tmpList.ToArray();
        }
        /// <summary>
        /// return array of base-stations with free-slots
        /// </summary>
        /// <returns></returns>
        public BaseStation[] FreeSlotsBaseStations()
        {
            List<BaseStation> tmpList = new();
            for (int i = 0; i < DataSource.Config.BaseStationIndex; i++)
                if (DataSource.BaseStationsList[i].FreeChargeSlots > 0)
                    tmpList.Add(DataSource.BaseStationsList[i]);
            return tmpList.ToArray();
        }


    }
}
