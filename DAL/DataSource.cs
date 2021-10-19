using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using IDAL.DO;

namespace DalObject
{
    class DataSource
    {
        internal static Drone[] DronesArr = new Drone[10];
        internal static BaseStation[] BaseStationsArr = new BaseStation[5];
        internal static Customer[] CustomersArr = new Customer[100];
        internal static Parcel[] ParcelsArr = new Parcel[1000];

        internal class Config
        {
            internal static int DroneIndex = 0;
            internal static int BaseStationIndex = 0;
            internal static int CustomerIndex = 0;
            internal static int ParcelIndex = 0;
            internal static int NewParcelId = 0;
        }

        public static void Initialize()
        {
            //   Drone[] drones[0] = {1, "",decimal, decimal, 55}
            DronesArr[0] = new Drone() { Id = Config.DroneIndex++, Model = "a", MaxWeight = WeightCategories.Heavy, Status = DroneStatusCategories.Free, Battery = 25.2 };
            DronesArr[1] = new Drone() { Id = Config.DroneIndex++, Model = "b", MaxWeight = WeightCategories.Heavy, Status = DroneStatusCategories.Free, Battery = 45.2 };
            DronesArr[2] = new Drone() { Id = Config.DroneIndex++, Model = "c", MaxWeight = WeightCategories.Light, Status = DroneStatusCategories.Delivery, Battery = 82.54 };
            DronesArr[3] = new Drone() { Id = Config.DroneIndex++, Model = "d", MaxWeight = WeightCategories.Light, Status = DroneStatusCategories.Maintenance, Battery = 85.32 };
            DronesArr[4] = new Drone() { Id = Config.DroneIndex++, Model = "e", MaxWeight = WeightCategories.Medium, Status = DroneStatusCategories.Delivery, Battery = 0 };

            BaseStationsArr[0] = new BaseStation() { Id = Config.BaseStationIndex++, Name = "Jerusalem Central Station", Lattitude = 31.789280, Longitude = 35.202142, FreeChargeSlots = 4 };
            BaseStationsArr[1] = new BaseStation() { Id = Config.BaseStationIndex++, Name = "Tel Aviv Central Station", Lattitude = 32.056312, Longitude = 34.779888, FreeChargeSlots = 5 };

            Random rand = new Random();
            for (int i = 0; i < 10; i++)
            {
                CustomersArr[i] = new Customer()
                {
                    Id = rand.Next(100000000, 999999999),
                    Name = "Gal Gabay",
                    Lattitude = rand.NextDouble() * (33.4188709641265 - 29.49970431757609) + 29.49970431757609,
                    Longitude = rand.NextDouble() * (35.89927249423983 - 34.26371323423407) + 34.26371323423407,
                    Phone = "05" + rand.Next(0, 99999999).ToString()
                };
                Config.CustomerIndex++;
            }

            for (int i = 0; i < 10; i++)
            {
                TimeSpan tSpan = new TimeSpan(0, rand.Next(0, 24), rand.Next(0, 60), 0);
                ParcelsArr[i] = new Parcel()
                {
                    Id = Config.NewParcelId++,
                    SenderId = CustomersArr[i].Id,
                    TargetId = CustomersArr[9 - i].Id,
                    Weight = WeightCategories.Heavy,
                    Priority = Priorities.Emergency,
                    Requested = DateTime.Now,
                    DroneId = rand.Next(0, Config.DroneIndex - 1),
                    Scheduled = DateTime.Now + tSpan,
                    PickedUp = DateTime.Now + tSpan + tSpan,
                    Delivered = DateTime.Now + tSpan + tSpan + tSpan
                };
                Config.ParcelIndex++;
            }
        }
    }
}