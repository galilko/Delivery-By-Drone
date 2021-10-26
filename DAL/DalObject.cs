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
            
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                if (DataSource.ParcelsArr[i].Id == parcelId)
                {
                    for (int j = 0; j < DataSource.Config.DroneIndex; j++)
                        if (DataSource.DronesArr[j].Id == droneId)
                        {
                            DataSource.ParcelsArr[i].DroneId = droneId;
                            DataSource.ParcelsArr[i].Scheduled = DateTime.Now;
                            break;
                        }
                    break;
                }
        }
        /// <summary>
        /// picking up a parcel by its drone
        /// </summary>
        /// <param name="parcelId">parcel id to picking up</param>
        public void PickingUpAParcel(int parcelId)
        {
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                if (DataSource.ParcelsArr[i].Id == parcelId)
                {
                    DataSource.ParcelsArr[i].PickedUp = DateTime.Now;
                    for (int j = 0; j < DataSource.Config.DroneIndex; j++)
                        if (DataSource.DronesArr[j].Id == DataSource.ParcelsArr[i].DroneId)
                        {
                            DataSource.DronesArr[j].Status = DroneStatusCategories.Delivery;
                            break; 
                        }
                }
        }
        /// <summary>
        /// finish delivery of a parcel
        /// </summary>
        /// <param name="parcelId">parcel id to deliver</param>
        public void DeliverAParcel(int parcelId)
        {
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                if (DataSource.ParcelsArr[i].Id == parcelId)
                {
                    DataSource.ParcelsArr[i].Delivered = DateTime.Now;
                    for (int j = 0; j < DataSource.Config.DroneIndex; j++)
                        if (DataSource.DronesArr[j].Id == DataSource.ParcelsArr[i].DroneId)
                        {
                            DataSource.DronesArr[j].Status = DroneStatusCategories.Free;
                            break;
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
            for (int i = 0; i < DataSource.Config.DroneIndex; i++)
                if (DataSource.DronesArr[i].Id == droneId)
                    for (int j = 0; j < DataSource.Config.BaseStationIndex; j++)
                        if (DataSource.BaseStationsArr[j].Id == baseStationId)
                        {
                            DataSource.DronesArr[i].Status = DroneStatusCategories.Maintenance;
                            DataSource.DroneChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
                            DataSource.BaseStationsArr[j].FreeChargeSlots--;
                            break;
                        }
        }
        /// <summary>
        /// release a drone from charging
        /// </summary>
        /// <param name="droneId">drone's id to release</param>
        public void ReleaseDroneFromCharge(int droneId)
        {
            for (int i = 0; i < DataSource.Config.DroneIndex; i++)
                if (DataSource.DronesArr[i].Id == droneId)
                {
                    DataSource.DronesArr[i].Status = DroneStatusCategories.Free;
                    DroneCharge droneCharge = DataSource.DroneChargeList.Find(myDroneCharge => myDroneCharge.DroneId == droneId);
                    for (int j = 0; j < DataSource.Config.BaseStationIndex; j++)
                        if (DataSource.BaseStationsArr[j].Id == droneCharge.StationId)
                        {
                            DataSource.BaseStationsArr[j].FreeChargeSlots++;
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
                DataSource.BaseStationsArr[DataSource.Config.BaseStationIndex] = new BaseStation()
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
                DataSource.DronesArr[DataSource.Config.DroneIndex++] = new Drone()
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
                DataSource.CustomersArr[DataSource.Config.CustomerIndex++] = new Customer()
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
                DataSource.ParcelsArr[DataSource.Config.ParcelIndex++] = new Parcel()
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
            foreach (var item in DataSource.BaseStationsArr)
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
            foreach (var item in DataSource.DronesArr)
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
            foreach (var item in DataSource.CustomersArr)
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
            foreach (var item in DataSource.ParcelsArr)
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
            return DataSource.CustomersArr.Skip(0).Take(DataSource.Config.CustomerIndex).ToArray();
        }
        /// <summary>
        /// return array of drones
        /// </summary>
        /// <returns></returns>
        public Drone[] AllDrones()
        {
            return DataSource.DronesArr.Skip(0).Take(DataSource.Config.DroneIndex).ToArray();
        }
        /// <summary>
        /// return array of base-stations
        /// </summary>
        /// <returns></returns>
        public BaseStation[] AllBaseStations()
        {
            return DataSource.BaseStationsArr.Skip(0).Take(DataSource.Config.BaseStationIndex).ToArray();
        }
        /// <summary>
        /// return array of parcels
        /// </summary>
        /// <returns></returns>
        public Parcel[] AllParcels()
        {
            return DataSource.ParcelsArr.Skip(0).Take(DataSource.Config.ParcelIndex).ToArray();
        }
        /// <summary>
        /// return array of none-scheduled parcels
        /// </summary>
        /// <returns></returns>
        public Parcel[] NoneScheduledParcels()
        {
            List<Parcel> tmpList = new();
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                if (DataSource.ParcelsArr[i].Scheduled == DateTime.MinValue)
                    tmpList.Add(DataSource.ParcelsArr[i]);
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
                if (DataSource.BaseStationsArr[i].FreeChargeSlots > 0)
                    tmpList.Add(DataSource.BaseStationsArr[i]);
            return tmpList.ToArray();
        }


    }
}
