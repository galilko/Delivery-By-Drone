using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    class DataSource
    {
        static IDal dal;
        internal static List<Parcel?> ParcelsList = new List<Parcel?>();
        internal static List<Customer?> CustomersList = new List<Customer?>();
        internal static List<BaseStation?> BaseStationsList = new List<BaseStation?>();
        internal static List<Drone?> DronesList = new List<Drone?>();
        internal static List<DroneCharge?> DroneChargeList = new List<DroneCharge?>();

        internal class Config
        {
            internal static int NewParcelId = 100000;
            internal static double BatteryUseFREE { get { return 1; } }
            internal static double BatteryUseLight { get { return 2; } }
            internal static double BatteryUseMedium { get { return 3; } }
            internal static double BatteryUseHeavy { get { return 4; } }
            internal static double BatteryChargeRate { get { return 5; } }
        }

        static Random rand = new Random(DateTime.Now.Millisecond);
        /// <summary>
        /// Generic function that return random enum
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        static T RandomEnumValue<T>()
        {
            var v = Enum.GetValues(typeof(T));
            return (T)v.GetValue(rand.Next(v.Length));
        }
        /// <summary>
        /// random coordinate close to coord
        /// </summary>
        /// <param name="coord">coordinate</param>
        /// <returns>random coordinate close to coord</returns>
        static double getRandomCoordinate(double coord) => coord + rand.NextDouble() / 10;
        
        
        /// <summary>
        /// Initialize the data
        /// </summary>
        public static void Initialize(IDal dal)
        {
            DataSource.dal = dal;
            createDrones(30); //Initialize drones
            createBaseStations(10); //Initialize base-stations
            createCustomers(20); //Initialize customers
            createParcels(50); //initialize parcels
           
            /*for (int i = 0; i <= 50; i++)
            {
                TimeSpan tSpan = new TimeSpan(0, rand.Next(0, 24), rand.Next(0, 60), 0);
                Parcel parcel = new Parcel()
                {
                    Id = Config.NewParcelId++,
                    SenderId = (int)CustomersList[i]?.Id,
                    TargetId = (int)CustomersList[9 - i]?.Id,
                    Weight = RandomEnumValue<WeightCategories>(),
                    Priority = (Priorities)rand.Next(3),
                    Requested = DateTime.Now
                };

                if (rand.Next(2) == 1)
                {
                    parcel.Scheduled = parcel.Requested + tSpan;
                    parcel.DroneId = (int)DronesList[rand.Next(DronesList.Count)]?.Id;
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
            }*/

        }

        private static void createCustomers(int n)
        {
            String[] maleNames = { "Aaron", "Shoham", "Gal", "Yossef", "David", "Eyal", "Michael", "Matan", "Shaul", "Dvir" };
            String[] lastNames = { "Cohen", "Gabay", "Levi", "Weiss", "Miletzki", "Shachor", "Haviv", "Golan" };
            int newCustomerId = 100000000;
            for (int i = 0; i < n; i++)
            {
                newCustomerId += rand.Next(11, 1000);
                dal.AddCustomer(new Customer()
                {
                    Id = newCustomerId,
                    Name = maleNames[rand.Next(maleNames.Length)] + " " + lastNames[rand.Next(lastNames.Length)],
                    Phone = $"0{rand.Next(50, 58)} - {rand.Next(1000000, 10000000)}",
                    Latitude = getRandomCoordinate(31.7),
                    Longitude = getRandomCoordinate(35.1),
                    IsActive = true
                });
            }
        }
      
        private static void createBaseStations(int n)
        {
            int newBSId = 1000;
            dal.AddBaseStation(new BaseStation() { Id = newBSId++, Name = "Jerusalem Central Station", Latitude = 31.789280, Longitude = 35.202142, FreeChargeSlots = 20, IsActive=true });
            dal.AddBaseStation(new BaseStation() { Id = newBSId++, Name = "Tel Aviv Central Station", Latitude = 32.056312, Longitude = 34.779888, FreeChargeSlots = 20, IsActive=true });
            for (int i = 2; i < n; i++)
            {
                dal.AddBaseStation(new BaseStation()
                {
                    Id = newBSId++,
                    Name = "Base Station" + i,
                    Latitude = getRandomCoordinate(31.71),
                    Longitude = getRandomCoordinate(35.18),
                    FreeChargeSlots = 20,
                    IsActive = true
                });
            }
            
        }

        private static void createDrones(int n)
        {
            int newDroneId = 100000;
            for (int i = 0; i < n; i++)
            {
                dal.AddDrone(new Drone()
                {
                    Id = newDroneId++,
                    Model = "DRN-" + rand.Next(10),
                    MaxWeight = (WeightCategories)rand.Next(1, 4)
                });
            }
        }

        static void createParcels(int n)
        {
            for (int i = 0; i < n; i++)
            {
                int droneId = 0;
                DateTime? requestTime = getRandomDateTime(DateTime.Now, 200, false);
                DateTime? scheduledTime = null, pickedTime = null, deliverdTime = null;
                int status = rand.Next(100);

                if (status >= 50)
                {
                    var droneIds = DronesList
                        .Where(d => !ParcelsList.Any(p => p?.DroneId == d?.Id && p?.Scheduled != null && p?.Delivered == null))
                        .Select(d => d?.Id ?? 0);
                    scheduledTime = getRandomDateTime(requestTime, 20);
                    if (droneIds.Any() && status < 60)
                    {
                        var ids = droneIds.ToArray();
                        droneId = ids[rand.Next(ids.Length)];
                        if (status >= 55)
                            pickedTime = getRandomDateTime(scheduledTime, 20);
                    }
                    else
                    {
                        pickedTime = getRandomDateTime(scheduledTime, 20);
                        deliverdTime = getRandomDateTime(pickedTime.Value, 15);
                        droneId = DronesList[rand.Next(DronesList.Count)]?.Id ?? 0;
                    }
                }

                int sender = (int)CustomersList[rand.Next(CustomersList.Count)]?.Id;
                var targets = CustomersList.Where(c => c?.Id != sender).ToList();
                dal.AddParcel(new Parcel()
                {
                    SenderId = sender,
                    TargetId = (int)targets[rand.Next(targets.Count)]?.Id,
                    Weight = (WeightCategories)rand.Next(1, 4),
                    Priority = (Priorities)rand.Next(3),
                    DroneId = droneId,
                    Requested = (DateTime)requestTime,
                    Scheduled = scheduledTime,
                    PickedUp = pickedTime,
                    Delivered = deliverdTime
                });
            }
        }

        static DateTime? getRandomDateTime(DateTime? dt, int minute, bool after = true)
        {
            int afterOrBefore = (after) ? 1 : -1;
            return dt?.AddMinutes(afterOrBefore * rand.Next(minute));
        }

    }
}