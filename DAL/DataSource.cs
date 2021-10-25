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
        internal static List<DroneCharge> DroneChargeList = new List<DroneCharge>();

        internal class Config
        {
            internal static int DroneIndex = 0;
            internal static int BaseStationIndex = 0;
            internal static int CustomerIndex = 0;
            internal static int ParcelIndex = 0;
            internal static int NewParcelId = 100000;
        }

        static Random _R = new Random();
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public static void Initialize()
        {
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                DronesArr[i] = new Drone()
                {
                    Id = rand.Next(100,999),
                    Model = "model" + i.ToString(),
                    MaxWeight = RandomEnumValue<WeightCategories>(),
                    Status = RandomEnumValue<DroneStatusCategories>(),
                    Battery = rand.NextDouble() * 100
                };
            }
            BaseStationsArr[0] = new BaseStation() { Id = Config.BaseStationIndex++, Name = "Jerusalem Central Station", Lattitude = 31.789280, Longitude = 35.202142, FreeChargeSlots = 4 };
            BaseStationsArr[1] = new BaseStation() { Id = Config.BaseStationIndex++, Name = "Tel Aviv Central Station", Lattitude = 32.056312, Longitude = 34.779888, FreeChargeSlots = 5 };

            String[] maleNames = { "Aaron", "Shoham", "Gal", "Yossef", "David", "Eyal", "Michael", "Matan", "Shaul", "Dvir" };
            String[] lastNames = { "Cohen", "Gabay", "Levi", "Weiss", "Miletzki" };

            for (int i = 0; i < 10; i++)
            {
                CustomersArr[i] = new Customer()
                {
                    Id = rand.Next(100000000, 999999999),
                    Name = maleNames[rand.Next(maleNames.Length)] + " " + lastNames[rand.Next(lastNames.Length)],
                    Lattitude = rand.NextDouble() * (33.4188709641265 - 29.49970431757609) + 29.49970431757609,
                    Longitude = rand.NextDouble() * (35.89927249423983 - 34.26371323423407) + 34.26371323423407,
                    Phone = "05" + rand.Next(0, 99999999).ToString().Insert(1,"-")
                };
                Config.CustomerIndex++;
            }

            for (int i = 0; i < 10; i++)
            {
                //TimeSpan tSpan = new TimeSpan(0, rand.Next(0, 24), rand.Next(0, 60), 0);
                ParcelsArr[i] = new Parcel()
                {
                    Id = Config.NewParcelId++,
                    SenderId = CustomersArr[i].Id,
                    TargetId = CustomersArr[9 - i].Id,
                    Weight = RandomEnumValue<WeightCategories>(),
                    Priority = RandomEnumValue<Priorities>(),
                    Requested = DateTime.Now,
                };
                Config.ParcelIndex++;
            }
        }
    }
}