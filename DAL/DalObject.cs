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
        public void PrintAllCustomers()
        {
            for (int i = 0; i < DataSource.Config.CustomerIndex; i++)
                Console.WriteLine(DataSource.CustomersArr[i].ToString());
        }
        public void PrintAllDrones()
        {
            for (int i = 0; i < DataSource.Config.DroneIndex; i++)
                Console.WriteLine(DataSource.DronesArr[i].ToString());
        }
        public void PrintAllBaseStations()
        {
            for (int i = 0; i < DataSource.Config.BaseStationIndex; i++)
                Console.WriteLine(DataSource.BaseStationsArr[i].ToString());
        }
        public void PrintAllParcels()
        {
            for (int i = 0; i < DataSource.Config.ParcelIndex; i++)
                Console.WriteLine(DataSource.ParcelsArr[i].ToString());
        }

        public void AddBaseStation(string name, double latitude, double longitude, int freeSlots)
        {
            if (DataSource.Config.BaseStationIndex < 5)
            {
                
                /*BaseStation b = new BaseStation()
                {
                    Id = DataSource.Config.BaseStationIndex,
                    Name = "Eilat IceMall",
                    Lattitude = 29.55411169296884,
                    Longitude = 34.96563532708392,
                    FreeChargeSlots = 5
                };*/
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

    }
}
