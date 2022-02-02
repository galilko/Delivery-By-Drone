using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using BlApi;

namespace BL
{
    sealed internal class BL : BlApi.IBL
    {
        internal readonly DalApi.IDal MyDal = DalApi.DalFactory.GetDal();
        internal readonly double[] BatteryUsages;
        internal const int DRONE_FREE = 0;
        internal const int DRONE_LIGHT = 1;
        internal const int DRONE_MEDIUM = 2;
        internal const int DRONE_HEAVY = 3;
        internal const int DRONE_CHARGE = 4;
        List<DroneToList> blDrones { get; set; }

        #region thread-safe BL singleton
        static readonly object padlock = new object();
        static BL instance = null;
        internal static BL Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new BL();
                    }
                    return instance;
                }
            }
        }
        private BL()
        {
            BatteryUsages = MyDal.GetBatteryUsages();
            initializeDrones();
        }

        private void initializeDrones()
        {
            Random rand = new Random(DateTime.Now.Millisecond);
            blDrones = (from d in MyDal.GetDrones()
                        let drone = (DO.Drone)d
                        select new DroneToList
                        {
                            Id = drone.Id,
                            Model = drone.Model,
                            Weight = (WeightCategories)drone.MaxWeight,
                            TransferdParcel = null,
                            IsActive = true
                        }).ToList();

            foreach (var drone in blDrones)
            {
                int? parcelId = MyDal.GetParcels().FirstOrDefault(p => p?.DroneId == drone.Id
                                                                   && p?.Scheduled != null
                                                                   && p?.Delivered == null)?.Id;
                if (parcelId != null)
                {
                    DO.Parcel parcel = MyDal.GetParcel((int)parcelId);
                    drone.Status = DroneStatusCategories.Delivery;
                    if (parcel.PickedUp == null)
                    {
                        drone.Location = FindClosestBaseStation(GetCustomer(parcel.SenderId), false).Location;
                        {/*
                        double minBattery = drone.RequiredBattery(this, (int)parcel.Id);
                         var Reciever = Customers.Find(x => x.Id == myParcel.TargetId);
                        Location RecieverLocation = new Location() { Latitude = Reciever.Latitude, Longitude = Reciever.Longitude };
                        double distanceWithParcel = LocationFuncs.DistanceBetweenTwoLocations(myDrone.Location, RecieverLocation);//מרחק בין רחפן ליעד
                        double distanceWithoutParcel = LocationFuncs.DistanceBetweenTwoLocations(RecieverLocation, LocationFuncs.ClosestBaseStationLocation(BaseStations, RecieverLocation));//מרחק בין יעד לתחנה קרובה
                        double minBattery = distanceWithoutParcel * BatteryUsages[DRONE_FREE];// בלי חבילה עליו
                        if (myParcel.Weight == DO.WeightCategories.Light)
                            minBattery += distanceWithParcel * BatteryUsages[DRONE_LIGHT];
                        else if (myParcel.Weight == DO.WeightCategories.Medium)
                            minBattery += distanceWithParcel * BatteryUsages[DRONE_MEDIUM];
                        else if (myParcel.Weight == DO.WeightCategories.Heavy)
                            minBattery += distanceWithParcel * BatteryUsages[DRONE_HEAVY];
                        myDrone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;
                        */
                        }
                    }
                    else
                    {
                        drone.Location = GetCustomer(parcel.SenderId).Location;
                    }
                    double minBattery = drone.RequiredBattery(this, (int)parcelId);
                    drone.BatteryStatus = minBattery + rand.NextDouble() * (100 - minBattery);
                    drone.TransferdParcel = parcelId;
                }
                else
                {
                    drone.Status = (DroneStatusCategories)rand.Next(0, 2);
                    if (drone.Status == DroneStatusCategories.Maintenance)
                    {
                        DO.BaseStation randomBS = (DO.BaseStation)MyDal.GetBaseStations().ToList()[rand.Next(MyDal.GetBaseStations().ToList().Count)];
                        drone.Location = new Location() { Latitude = randomBS.Latitude, Longitude = randomBS.Longitude };
                        drone.BatteryStatus = rand.NextDouble() * 20;
                        drone.StartCharge = DateTime.Now;
                        MyDal.ChargeDrone((int)drone.Id, randomBS.Id);
                    }
                    if (drone.Status == DroneStatusCategories.Free)
                    {
                        List<int> targetsIds = new();
                        foreach (DO.Parcel parcel in MyDal.GetParcels())
                        {
                            if (parcel.Delivered != null && !targetsIds.Exists(x => x == parcel.TargetId))
                                targetsIds.Add(parcel.TargetId);
                        }
                        if (targetsIds.Count > 0)
                        {
                            drone.Location = GetCustomer(targetsIds[rand.Next(targetsIds.Count)]).Location;
                            //DO.Customer randomCustomer = (DO.Customer)targetsIds[rand.Next(targetsIds.Count)];
                            //new Location() { Latitude = randomCustomer.Latitude, Longitude = randomCustomer.Longitude };
                        }
                        else
                        {
                            drone.Location = new()
                            {
                                Latitude = 31.71 + rand.NextDouble() / 10,
                                Longitude = 35.18 + rand.NextDouble() / 10
                                //Latitude = rand.NextDouble() * (33.4188709641265 - 29.49970431757609) + 29.49970431757609,
                                //Longitude = rand.NextDouble() * (35.89927249423983 - 34.26371323423407) + 34.26371323423407
                            };
                        }
                        double distanceWithoutParcel = drone.Distance(FindClosestBaseStation(drone, false));//מרחק בין יעד לתחנה קרובה
                        double minBattery = distanceWithoutParcel * BatteryUsages[DRONE_FREE];// בלי חבילה עליו
                        drone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;

                    }
                }

            }
        }
        #endregion

        internal BaseStation FindClosestBaseStation(ILocatable fromLocatable, bool forCharge) =>
    //from all base-stations -- if not for charge the FreeChargeSlots doesn't matter
    (from s in MyDal.GetBaseStations(bs => !forCharge || bs?.FreeChargeSlots > 0)
     let l = (DO.BaseStation)s
     // build a bl BS
     let bs = new BaseStation
     {
         Id = l.Id,
         Name = l.Name,
         FreeChargeSlots = l.FreeChargeSlots,
         // assign all drones in charge in bs
         DronesInCharge = (from dc in MyDal.GetDronesInCharge(dc => dc?.StationId == l.Id)
                           let id = dc?.DroneId ?? 0 // if droneId is null then id = 0
                           let drone = blDrones.Find(d => d.Id == id) // find droneToList with this id
                           select new DroneInCharge { Id = id, BatteryStatus = drone.BatteryStatus }).ToList(),
         Location = new Location { Latitude = l.Latitude, Longitude = l.Longitude }
     }
     // select the BS with the minimum distance from Location "fromLocatable"
     select new { Distance = fromLocatable.Distance(bs), Station = bs } // anonymous type
    ).Aggregate(new { Distance = double.PositiveInfinity, Station = (BaseStation)null },
                (closest, next) => next.Distance < closest.Distance ? next : closest, best => best.Station);

        #region Request (Get) single objects
        public Customer GetCustomer(int customerId)
        {
            try
            {
                lock (MyDal)
                {
                    DO.Customer dalCustomer = MyDal.GetCustomer(customerId); //find the customer data layer throwgh id and fit the details to BL layer
                    return Converter.GetBlCustomer(dalCustomer);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlFindItemException($"cannot find customer {customerId}:", ex);
            }
        }
        public Parcel GetParcel(int parcelId)
        {
            try
            {
                lock (MyDal)
                {
                    DO.Parcel dalParcel = MyDal.GetParcel(parcelId);
                    return Converter.GetBlParcel(dalParcel, blDrones);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlFindItemException($"cannot Find parcel {parcelId}:", ex);
            }
        }
        public BaseStation GetBaseStation(int baseStationId)
        {
            try
            {
                lock (MyDal)
                {
                    DO.BaseStation dalBs = MyDal.GetBaseStation(baseStationId);
                    return Converter.GetBlBS(dalBs, blDrones);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot Find base station {baseStationId}:", ex);
            }

        }
        public Drone GetDrone(int droneId)
        {
            try
            {
                DroneToList dtl = blDrones.Find(x => x.Id == droneId);
                return Converter.GetBlDrone(dtl);
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlFindItemException($"cannot find drone {droneId}:", ex);
            }
        }

        public BaseStationToList GetBSToList(int id)
        {
            return (from item in GetBaseStations().ToList()
                    where item.Id == id
                    select item).FirstOrDefault();
        }
        public CustomerToList GetCustomerToList(int id)
        {
            return (from item in GetCustomers().ToList()
                    where item.Id == id
                    select item).FirstOrDefault();
        }
        public ParcelToList GetParcelToList(int id)
        {
            return (from item in GetParcels().ToList()
                    where item.Id == id
                    select item).FirstOrDefault();
        }
        public DroneToList GetDroneToList(int id) => blDrones.Find(d => d.Id == id);
        #endregion

        #region Request (Get) collections
        #region parcels
        public IEnumerable<ParcelToList> GetParcels()
        {
            List<ParcelToList> myList = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.Parcel parcel in MyDal.GetParcels().ToList())
                        myList.Add(Converter.GetParcelToList(parcel));
                    if (myList.Count == 0)
                        throw new BlViewItemsListException("Parcel list is empty");
                    return myList;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }

        public IEnumerable<ParcelToList> ParcelsByStatus(ParcelStatus myStatus)
        {
            try
            {
                return GetParcels().Where(x => x.Status == myStatus);
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        public IEnumerable<ParcelToList> NoneScheduledParcels()
        {
            List<ParcelToList> myList = new(); //takes all parcels from myDal end set them on myList
            try
            {
                return GetParcels().Where(x => x.Status == ParcelStatus.Defined);
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }

        public IEnumerable<ParcelToList> ParcelsByDates(DateTime? fromDate, DateTime? toDate)
        {
            List<ParcelToList> myList = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.Parcel parcel in MyDal.GetParcels().ToList().Where(p => p?.Requested >= fromDate && p?.Requested <= toDate))
                        myList.Add(Converter.GetParcelToList(parcel));
                    return myList;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        public IEnumerable<ParcelToList> GetSentParcels(int id)
        {
            List<ParcelToList> myList = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.Parcel parcel in MyDal.GetParcels(x => x?.SenderId == id))
                        myList.Add(Converter.GetParcelToList(parcel));
                    return myList;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        public IEnumerable<ParcelToList> GetRecievedParcels(int id)
        {
            List<ParcelToList> myList = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.Parcel parcel in MyDal.GetParcels(x => x?.TargetId == id))
                        myList.Add(Converter.GetParcelToList(parcel));
                    return myList;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        #endregion
        public IEnumerable<CustomerToList> GetCustomers()
        {
            List<CustomerToList> customers = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.Customer customer in MyDal.GetCustomers())
                        customers.Add(Converter.GetCustomerToList(customer));
                    if (customers.Count == 0)
                        throw new BlViewItemsListException("Customers list is empty");
                    return customers;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show base stations list:", ex);
            }
        }
        public IEnumerable<BaseStationToList> GetBaseStations()
        {
            List<BaseStationToList> myList = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.BaseStation bs in MyDal.GetBaseStations()) //takes all BlBaseStations from myDal end set them on myList
                        myList.Add(Converter.GetBSToList(bs));
                    if (myList.Count == 0)
                        throw new BlViewItemsListException("Base-station list is empty");
                    return myList;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show base stations list:", ex);
            }
        }
        public IEnumerable<BaseStationToList> FreeSlotsBaseStations()
        {
            List<BaseStationToList> baseStations = new();
            try
            {
                lock (MyDal)
                {
                    foreach (DO.BaseStation bs in MyDal.GetBaseStations(x => x?.FreeChargeSlots > 0)) //takes all BlBaseStations from myDal end set them on myList
                        baseStations.Add(Converter.GetBSToList(bs));
                    if (baseStations.Count == 0)
                        throw new BlViewItemsListException("Base-station list is empty");
                    return baseStations;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlViewItemsListException($"cannot show Free Slots Base Stations list:", ex);
            }

        }
        public IEnumerable<DroneToList> GetDrones(Func<DroneToList, bool> predicate = null)
        {
            if (predicate == null)
                return blDrones.Where(x => x.IsActive == true).ToList();

            return (from item in blDrones
                    where item.IsActive is true
                    where predicate(item)
                    select item);
        }

        #endregion

        #region DELETE
        public void DeleteParcel(int parcelId)
        {
            try
            {
                lock (MyDal)
                {
                    if (GetParcel(parcelId).Scheduled == null)
                        MyDal.DeleteParcel(parcelId);
                    else
                        throw new BlDeleteItemException($"Parcel {parcelId} was scheduled:");
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlDeleteItemException($"cannot Find parcel {parcelId}:", ex);
            }
        }
        public void DeleteCustomer(int id)

        /// <summary>
        /// delete (inactivate) a base station by Id 
        /// </summary>
        /// <param name="id">base station Id</param>
        {
            try
            {
                lock (MyDal)
                    MyDal.DeleteCustomer(id);
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlDeleteEntityException($"cannot delete customer {id}:", ex);
            }
        }
        public void DeleteDrone(int id)
        {
            try
            {
                lock (MyDal)
                {
                    if (blDrones.Find(x => x.Id == id).Status == DroneStatusCategories.Maintenance)
                        releaseDrone(id);
                    MyDal.DeleteDrone(id);
                    blDrones.Find(x => x.Id == id).IsActive = false;
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlDeleteEntityException($"cannot delete drone {id}:", ex);
            }
        }
        public void DeleteBaseStation(int id)
        {
            try
            {
                lock (MyDal)
                {
                    if(MyDal.GetDronesInCharge().Where(x => x?.StationId == id).Count() > 0)
                        throw new BlDeleteItemException($"cannot delete base station {id}. There are drones in charge");
                    MyDal.DeleteBaseStation(id);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlDeleteEntityException($"cannot delete base station {id}:", ex);
            }
        }
        #endregion

        #region ADD
        public void AddParcel(Parcel myParcel)
        {
            try
            {
                lock (MyDal)
                {
                    MyDal.GetCustomer(myParcel.Sender.Id);
                    MyDal.GetCustomer(myParcel.Target.Id);
                    MyDal.AddParcel(Converter.GetDalParcel(myParcel)); // convert myparcel to store it in data layer
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlAddEntityException($"cannot add Parcel id:", ex);
            }
        }
        public void AddBaseStation(BaseStation myBaseStation)
        {
            try
            {
                lock (MyDal)
                    MyDal.AddBaseStation(Converter.GetDalBS(myBaseStation));
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlAddEntityException($"cannot add base station {myBaseStation.Id}:", ex);
            }
        }
        public void AddCustomer(Customer myCustomer)
        {
            try
            {
                lock (MyDal)
                    MyDal.AddCustomer(Converter.GetDalCustomer(myCustomer));
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlAddEntityException($"cannot add customer {myCustomer.Id}:", ex);
            }
        }
        public void AddDrone(DroneToList myDrone, int baseStationId)
        {
            try
            {
                lock (MyDal)
                {
                    DO.BaseStation bs = MyDal.GetBaseStation(baseStationId);
                    Random rand = new(DateTime.Now.Millisecond);
                    myDrone.BatteryStatus = rand.NextDouble() * 20 + 20; // random battery
                    myDrone.Status = DroneStatusCategories.Maintenance;
                    myDrone.Location = new Location() { Latitude = bs.Latitude, Longitude = bs.Longitude };
                    myDrone.TransferdParcel = 0;
                    myDrone.IsActive = true;
                    MyDal.AddDrone(Converter.GetDalDrone(myDrone)); // add drone to data
                    MyDal.ChargeDrone((int)myDrone.Id, bs.Id);
                    blDrones.Add(myDrone);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlAddEntityException($"cannot add drone id:", ex);
            }
        }

        #endregion

        #region UPDATE
        public void UpdateBaseStation(int baseStationId, string newName, int slotsCount)
        {
            try
            {
                lock (MyDal)
                {
                    int busySlots = MyDal.GetDronesInCharge().Where(x => x?.StationId == baseStationId).Count();
                    var bs = MyDal.GetBaseStation(baseStationId);
                    if (busySlots > slotsCount || slotsCount < 0)
                        throw new BlUpdateEntityException($"cannot update base station {baseStationId}. Too much drones in charge");
                    MyDal.UpdateBaseStation(baseStationId, newName, slotsCount);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot update base station {baseStationId}:", ex);
            }
        }
        public void UpdateCustomer(int customerId, string newName, string newPhone)
        {
            try
            {
                lock (MyDal)
                    MyDal.UpdateCustomer(customerId, newName, newPhone);
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot update customer {customerId}:", ex);
            }
        }
        public void UpdateDroneModel(int droneId, string newName)
        {
            try
            {
                lock (MyDal)
                    MyDal.UpdateDroneModel(droneId, newName);   // change the model of a dron in data layer by dron id
                blDrones.Find(x => x.Id == droneId).Model = newName;
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot update drone {droneId}:", ex);
            }

        }

        #endregion

        #region BaseStation charging
        public void ChargeDrone(int droneId)
        {
            try
            {
                lock (MyDal)
                {
                    MyDal.GetDrone(droneId);
                    DroneToList myDrone = blDrones.Find(x => x.Id == droneId);// find the drone in blDrones list
                    if (myDrone.Status != DroneStatusCategories.Free)    // check that the drone is free
                        throw new BlUpdateEntityException("The chosen drone isn't free");
                    else
                    {
                        List<DO.BaseStation?> dalBsList = MyDal.GetBaseStations().OrderBy(x => myDrone.Distance(new BaseStation() { Location = new Location() { Latitude = x?.Latitude, Longitude = x?.Longitude } })).ToList();
                        foreach (DO.BaseStation baseStation in dalBsList)
                        {
                            var bs = new BaseStation() { Location = new() { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude } };
                            double batteryNeed = myDrone.Distance(bs) * BatteryUsages[DRONE_FREE]; //batteryNeed is the total calculation between the distance multiplied with the use of battery when the drone free
                            if (myDrone.BatteryStatus > batteryNeed)    //if the battery of the drone enough
                                if (baseStation.FreeChargeSlots > 0) // if the base station free  update the details ChargeDrone at base station and the drone details 
                                {
                                    MyDal.ChargeDrone((int)myDrone.Id, baseStation.Id);
                                    myDrone.Status = DroneStatusCategories.Maintenance;
                                    myDrone.StartCharge = DateTime.Now;
                                    myDrone.BatteryStatus -= batteryNeed;
                                    myDrone.Location = new Location() { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude };
                                    return;
                                }
                                else //continue to the next base station
                                    continue;
                            else
                                throw new BlUpdateEntityException($"There isn't enough battery to arrive to the closest base station");
                        }
                        throw new BlUpdateEntityException($"There aren't free charge slots for {droneId}");
                    }
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot charge drone {droneId}:", ex);
            }
        }
        public void releaseDrone(int droneId)
        {
            try
            {
                lock (MyDal)
                {
                    MyDal.GetDrone(droneId);
                    if (blDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Maintenance) // check if the drone are Maintenance
                        throw new BlUpdateEntityException("The chosen drone isn't in charge");
                    MyDal.ReleaseDroneFromCharge(droneId);
                    var myDrone = blDrones.Find(x => x.Id == droneId);
                    myDrone.Status = DroneStatusCategories.Free;
                    TimeSpan t = DateTime.Now - myDrone.StartCharge;
                    myDrone.BatteryStatus = Math.Min(myDrone.BatteryStatus + t.TotalHours * BatteryUsages[DRONE_CHARGE], 100.0);
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot release drone {droneId}:", ex);
            }
        }
        #endregion


        #region Handle Parcel
        public void ScheduleDroneForParcel(int droneId)
        {
            try
            {
                lock (MyDal)
                {
                    MyDal.GetDrone(droneId);
                    DroneToList myDrone = blDrones.Find(x => x.Id == droneId);                               // check if the dron are free
                    if (blDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Free)
                        throw new BlUpdateEntityException($"Drone {droneId} isn't free");
                    else if (blDrones.Find(x => x.Id == droneId).IsActive == false)
                        throw new BlUpdateEntityException("The chosen drone isn't active");
                    else
                    {
                        // make a list parcels of all parcels from emergency to normal that their weight or equal or smaller then the drone can carry
                        List<DO.Parcel?> parcels = MyDal.GetParcels(x => x?.Scheduled == null).OrderByDescending(x => x?.Priority).Where(x => x?.Weight <= (DO.WeightCategories)myDrone.Weight).OrderByDescending(x => x?.Weight).ToList();
                        parcels = parcels.OrderBy(x => myDrone.Distance(GetCustomer((int)x?.SenderId))).ToList();
                        if (parcels.Count > 0)                                       // sorted the parcels list that created from the nearest parcel to the drone location to The farthest
                        {
                            foreach (DO.Parcel item in parcels)
                            {
                                double batteryNeedToBase = myDrone.RequiredBattery(this, item.Id);
                                if (batteryNeedToBase < myDrone.BatteryStatus)
                                {                                                                     // if the battery of the drone enough to complit the delivering and get to the base station
                                    MyDal.ScheduleDroneForParcel(item.Id, (int)myDrone.Id);
                                    blDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Delivery;
                                    blDrones.Find(x => x.Id == droneId).TransferdParcel = item.Id;
                                    return;
                                }
                            }
                            throw new BlUpdateEntityException($"There aren't match parcels to schedule because your charging");
                        }
                        else
                            throw new BlUpdateEntityException($"There aren't match parcels to schedule");
                    }
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot schedule drone {droneId}:", ex);
            }
        }
        public void PickingUpAParcel(int droneId)
        {
            try
            {
                lock (MyDal)
                {
                    MyDal.GetDrone(droneId);
                    foreach (DO.Parcel item in MyDal.GetParcels())
                    {
                        if (item.DroneId == droneId && item.Scheduled != null && item.PickedUp == null)              // updat the details of the drone and the parcel and the battery of the drone and is location
                        {
                            DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                            ParcelInTransfer myParcel = Converter.GetParcelInTransfer(item);
                            MyDal.PickingUpAParcel(item.Id);
                            blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUsages[DRONE_FREE] * myDrone.Distance(GetCustomer(item.SenderId));
                            blDrones.Find(x => x.Id == droneId).Location = myParcel.CollectionLocation;
                            return;
                        }
                    }
                    throw new BlUpdateEntityException($"There isn't parcel to pick up for drone {droneId}");
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot pick up {droneId}:", ex);
            }
        }
        public void DeliverAParcel(int droneId)
        {
            try
            {
                lock (MyDal)
                {
                    MyDal.GetDrone(droneId);                                   //find the drone and find the parcels that he deliverd and canculat the values for the battery and the location of the drone
                    foreach (DO.Parcel item in MyDal.GetParcels())
                    {
                        if (item.DroneId == droneId && item.PickedUp != null && item.Delivered == null)
                        {
                            DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                            ParcelInTransfer myParcel = Converter.GetParcelInTransfer(item);
                            MyDal.DeliverAParcel(item.Id);
                            if (myParcel.Weight == WeightCategories.Light)
                                blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUsages[DRONE_LIGHT] * myParcel.TransportDistance;
                            else if (myParcel.Weight == WeightCategories.Medium)
                                blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUsages[DRONE_MEDIUM] * myParcel.TransportDistance;         // to every case of whight
                            else if (myParcel.Weight == WeightCategories.Heavy)
                                blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUsages[DRONE_HEAVY] * myParcel.TransportDistance;
                            blDrones.Find(x => x.Id == droneId).Location = myParcel.DeliveryDestinationLocation;
                            blDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Free;
                            blDrones.Find(x => x.Id == droneId).TransferdParcel = 0;
                            return;
                        }
                    }
                    throw new BlUpdateEntityException($"There isn't parcel to deliver for drone {droneId}");
                }
            }
            catch (DO.ExistIdException ex)
            {
                throw new BlUpdateEntityException($"cannot pick up {droneId}:", ex);
            }
        }
        public int GetNextParcelId()
        {
            lock (MyDal)
                return MyDal.GetNextParcelId();
        }
        #endregion

        public void StartDroneSimulator(int id, Action update, Func<bool> checkStop) => new DroneSimulator(this, id, update, checkStop);

    }
}

