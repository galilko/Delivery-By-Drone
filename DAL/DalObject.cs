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

        public void AddBaseStation(string name, double latitude, double longitude, int freeSlots)
        {
            if (DataSource.Config.BaseStationIndex < 5)
            {
                DataSource.BaseStationsArr[DataSource.Config.BaseStationIndex] = new BaseStation()
                {
                    Id = DataSource.Config.BaseStationIndex++,
                    Name = name,
                    Lattitude = latitude,
                    Longitude = longitude,
                    FreeChargeSlots = freeSlots
                };

            }
        }
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


        public BaseStation FindBaseStation(int id)
        {
            foreach (var item in DataSource.BaseStationsArr)
                if (item.Id == id)
                    return item;
            return default;
        }
        public Drone FindDrone(int id)
        {
            foreach (var item in DataSource.DronesArr)
                if (item.Id == id)
                    return item;
            return default;
        }
        public Customer FindCustomer(int id)
        {
            foreach (var item in DataSource.CustomersArr)
                if (item.Id == id)
                    return item;
            return default;
        }
        public Parcel FindParcel(int id)
        {
            foreach (var item in DataSource.ParcelsArr)
                if (item.Id == id)
                    return item;
            return default;
        }

        public Customer[] AllCustomers()
        {
            return DataSource.CustomersArr.Skip(0).Take(DataSource.Config.CustomerIndex).ToArray();
        }
        public Drone[] AllDrones()
        {
            return DataSource.DronesArr.Skip(0).Take(DataSource.Config.DroneIndex).ToArray();
        }
        public BaseStation[] AllBaseStations()
        {
            return DataSource.BaseStationsArr.Skip(0).Take(DataSource.Config.BaseStationIndex).ToArray();
        }
        public Parcel[] AllParcels()
        {
            return DataSource.ParcelsArr.Skip(0).Take(DataSource.Config.ParcelIndex).ToArray();
        }
        public Parcel[] NoneScheduledParcels()
        {
            List<Parcel> tmpList = new();
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                if (DataSource.ParcelsArr[i].Scheduled == DateTime.MinValue)
                    tmpList.Add(DataSource.ParcelsArr[i]);
            return tmpList.ToArray();
        }
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
