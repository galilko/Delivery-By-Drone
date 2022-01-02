using DO;
using System;
using System.Collections.Generic;

namespace Dal
{
    class DataSource
    {

        internal static List<Parcel> ParcelsList = new List<Parcel>();
        internal static List<Customer> CustomersList = new List<Customer>();
        internal static List<BaseStation> BaseStationsList = new List<BaseStation>();
        internal static List<Drone> DronesList = new List<Drone>();
        internal static List<DroneCharge> DroneChargeList = new List<DroneCharge>();

        internal class Config
        {
            internal static int NewParcelId = 100000;
            internal static double BatteryUseFREE { get { return 0.5; } }
            internal static double BatteryUseLight { get { return 2.5; } }
            internal static double BatteryUseMedium { get { return 3.5; } }
            internal static double BatteryUseHeavy { get { return 5.5; } }
            internal static double BatteryChargeRate { get { return 20.5; } }
        }

        static Random _R = new Random(DateTime.Now.Millisecond);
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(_R.Next(v.Length));
        }

        public static void Initialize()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            //initialize drones
            for (int i = 0; i < 20; i++)
            {
                DronesList.Add(new Drone()
                {
                    Id = rand.Next(1000000, 10000000),
                    Model = "Model" + i.ToString(),
                    MaxWeight = RandomEnumValue<WeightCategories>()
                });
            }
            //initialize base-stations
            BaseStationsList.Add(new BaseStation() { Id = 1000, Name = "Jerusalem Central Station", Lattitude = 31.789280, Longitude = 35.202142, FreeChargeSlots = 20, IsActive=true });
            BaseStationsList.Add(new BaseStation() { Id = 1001, Name = "Tel Aviv Central Station", Lattitude = 32.056312, Longitude = 34.779888, FreeChargeSlots = 20, IsActive=true });
            //initialize customers
            String[] maleNames = { "Aaron", "Shoham", "Gal", "Yossef", "David", "Eyal", "Michael", "Matan", "Shaul", "Dvir" };
            String[] lastNames = { "Cohen", "Gabay", "Levi", "Weiss", "Miletzki" };
            for (int i = 0; i < 10; i++)
            {
                CustomersList.Add(new Customer()
                {
                    Id = rand.Next(100000000, 999999999),
                    Name = maleNames[rand.Next(maleNames.Length)] + " " + lastNames[rand.Next(lastNames.Length)],
                    Lattitude = rand.NextDouble() * (33.4188709641265 - 29.49970431757609) + 29.49970431757609,
                    Longitude = rand.NextDouble() * (35.89927249423983 - 34.26371323423407) + 34.26371323423407,
                    Phone = "05" + rand.Next(0, 99999999).ToString().Insert(1, "-"),
                    IsActive = true
                }
                );
            }
            //initialize parcels
            for (int i = 0; i < 10; i++)
            {
                TimeSpan tSpan = new TimeSpan(0, rand.Next(0, 24), rand.Next(0, 60), 0);
                Parcel parcel = new Parcel()
                {
                    Id = Config.NewParcelId++,
                    SenderId = CustomersList[i].Id,
                    TargetId = CustomersList[9 - i].Id,
                    Weight = RandomEnumValue<WeightCategories>(),
                    Priority = (Priorities)rand.Next(1, 4),
                    Requested = DateTime.Now
                };

                if (rand.Next(2) == 1)
                {
                    parcel.Scheduled = parcel.Requested + tSpan;
                    parcel.DroneId = DronesList[rand.Next(DronesList.Count)].Id;
                    if (rand.Next(2) == 1)
                    {
                        parcel.PickedUp = parcel.Scheduled + tSpan;
                        if (rand.Next(2) == 1)
                            parcel.Delivered = parcel.PickedUp + tSpan;
                        else
                            parcel.Delivered = null;
                    }
                    else
                        parcel.PickedUp = parcel.Delivered = null;
                }
                else
                {
                    parcel.Scheduled = parcel.PickedUp = parcel.Delivered = null;
                    parcel.DroneId = 0;
                }
                ParcelsList.Add(parcel);
            }

        }

    }
}