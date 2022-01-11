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
        /*private static double BatteryUsages[DRONE_FREE];
        private static double BatteryUsages[DRONE_LIGHT];
        private static double BatteryUsages[DRONE_MEDIUM];
        private static double BatteryUsages[DRONE_HEAVY];
        private static double BatteryUsages[DRONE_CHARGE];*/
        private List<DroneToList> blDrones;

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
            blDrones = new();
            //  MyDal = DalApi.DalFactory.GetDal();
            BatteryUsages = MyDal.GetBatteryUse();
            //אחוזים לק"מ
            /*BatteryUsages[DRONE_FREE] = MyDal.GetBatteryUse()[0];
            BatteryUsages[DRONE_LIGHT] = MyDal.GetBatteryUse()[1];
            BatteryUsages[DRONE_MEDIUM] = MyDal.GetBatteryUse()[2];
            BatteryUsages[DRONE_HEAVY] = MyDal.GetBatteryUse()[3];
            //אחוזים לשעה
            BatteryUsages[DRONE_CHARGE] = MyDal.GetBatteryUse()[4];*/
            Random rand = new Random(DateTime.Now.Millisecond);
            List<DO.Drone> Drones = MyDal.GetDrones().ToList();
            List<DO.Parcel> Parcels = MyDal.GetParcels().ToList();
            List<DO.Customer> Customers = MyDal.GetCustomers().ToList();
            List<DO.BaseStation> BaseStations = MyDal.GetBaseStations().ToList();
            foreach (var item in Drones.ToList())
            {
                DroneToList myDrone = new();
                myDrone.Id = item.Id;
                myDrone.Model = item.Model;
                myDrone.Weight = (WeightCategories)item.MaxWeight;
                myDrone.IsActive = true;
                DO.Parcel myParcel = Parcels.Find(x => x.DroneId == myDrone.Id && x.Delivered == null);
                if (!myParcel.Equals(default(DO.Parcel)))
                {
                    myDrone.Status = DroneStatusCategories.Delivery;
                    myDrone.BatteryStatus = rand.NextDouble() * 50 + 50; // random battery 50-100
                    myDrone.TransferdParcel = (int)(myParcel.Id);
                    var Sender = Customers.Find(x => x.Id == myParcel.SenderId);
                    Location SenderLocation = new() { Latitude = Sender.Latitude, Longitude = Sender.Latitude };
                    if (myParcel.PickedUp == null)
                    {
                        Location MyLocation = FindClosestBaseStation(GetCustomer(Sender.Id)).Location;
                        myDrone.Location = MyLocation;
                        double minBattery = myDrone.RequiredBattery(this, (int)myParcel.Id);
                        /*
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
                            minBattery += distanceWithParcel * BatteryUsages[DRONE_HEAVY];*/
                        myDrone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;
                    }
                    else
                    {
                        myDrone.Location = SenderLocation;
                    }
                }
                else
                {
                    myDrone.Status = (DroneStatusCategories)rand.Next(0, 2);
                    if (myDrone.Status == DroneStatusCategories.Maintenance)
                    {
                        DO.BaseStation randomBS = BaseStations[rand.Next(BaseStations.Count)];
                        myDrone.Location = new Location() { Latitude = randomBS.Latitude, Longitude = randomBS.Longitude };
                        myDrone.BatteryStatus = rand.NextDouble() * 20;
                        myDrone.StartCharge = DateTime.Now;
                        MyDal.ChargeDrone(item.Id, randomBS.Id);
                    }

                    if (myDrone.Status == DroneStatusCategories.Free)
                    {
                        List<DO.Customer> Recievers = new();
                        foreach (var parcel in Parcels)
                        {
                            if (parcel.Delivered != null && Recievers.Find(x => x.Id == parcel.TargetId).Equals(default(DO.Customer)))
                                Recievers.Add(Customers.Find(y => y.Id == parcel.TargetId));
                        }
                        if (Recievers.Count > 0)
                        {
                            DO.Customer randomCustomer = Recievers[rand.Next(Recievers.Count)];
                            myDrone.Location = new Location() { Latitude = randomCustomer.Latitude, Longitude = randomCustomer.Longitude };
                        }
                        else
                        {
                            myDrone.Location = new()
                            {
                                Latitude = rand.NextDouble() * (33.4188709641265 - 29.49970431757609) + 29.49970431757609,
                                Longitude = rand.NextDouble() * (35.89927249423983 - 34.26371323423407) + 34.26371323423407
                            };
                        }
                        Drone drone = new() { Location = myDrone.Location };
                        double distanceWithoutParcel = drone.Distance(FindClosestBaseStation(drone));//מרחק בין יעד לתחנה קרובה
                        double minBattery = distanceWithoutParcel * BatteryUsages[DRONE_FREE];// בלי חבילה עליו
                        myDrone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;

                    }
                    myDrone.TransferdParcel = Parcels.Find(x => x.DroneId == myDrone.Id && x.Delivered == null).Id;
                }
                blDrones.Add(myDrone);

            }
        }

        internal BaseStation FindClosestBaseStation(ILocatable fromLocatable) =>
    (from s in MyDal.GetBaseStations(bs => bs.FreeChargeSlots > 0)
     let l = (DO.BaseStation)s
     let bs = new BaseStation
     {
         Id = l.Id,
         Name = l.Name,
         FreeChargeSlots = l.FreeChargeSlots,
         DronesInCharge = (from dc in MyDal.GetListOfInChargeDrones(dc => dc.StationId == l.Id)
                            let id = dc.DroneId ?? 0
                            let drone = blDrones.Find(d => d.Id == id)
                            select new DroneInCharge { Id = id, BatteryStatus = drone.BatteryStatus }).ToList(),
         Location = new Location { Latitude = l.Latitude, Longitude = l.Longitude }
     }
     select new { Distance = fromLocatable.Distance(bs), Station = bs }
    ).Aggregate(new { Distance = double.PositiveInfinity, Station = (BaseStation)null },
                (closest, next) => next.Distance < closest.Distance ? next : closest, best => best.Station);
        #endregion
        #region PARCEL FUNCS
        /// <summary>
        /// adding a new parcel into list through BL
        /// </summary>
        /// <param name="myParcel">Parcel to add</param>
        public void AddParcel(Parcel myParcel)
        {
            try
            {
                MyDal.GetCustomer(myParcel.Sender.Id);
                MyDal.GetCustomer(myParcel.Target.Id);
                MyDal.AddParcel(Converter.GetDalParcel(myParcel)); // convert myparcel to store it in data layer
            }
            catch (Exception ex)
            {
                throw new BlAddEntityException($"cannot add Parcel id:", ex);
            }
        }
        /// <summary>
        /// find a parcel by id and return it through BL
        /// </summary>
        /// <param name="parcelId">parcelId to find</param>
        /// <returns>returns the parcel that asked</returns>
        public Parcel GetParcel(int parcelId)
        {
            try
            {
                //extract the parcel from data layer and fit the details to the BL layer 
                DO.Parcel dalParcel = MyDal.GetParcel(parcelId);
                return Converter.GetBlParcel(dalParcel, MyDal, blDrones);
            }
            catch (Exception ex)
            {
                throw new BlFindItemException($"cannot Find parcel {parcelId}:", ex);
            }
        }
        public void DeleteParcel(int parcelId)
        {
            try
            {
                if (GetParcel(parcelId).Scheduled == null)
                    MyDal.DeleteParcel(parcelId);
                else
                    throw new BlDeleteItemException($"Parcel {parcelId} was scheduled:");
            }
            catch (Exception ex)
            {
                throw new BlDeleteItemException($"cannot Find parcel {parcelId}:", ex);
            }
        }
        /// <summary>
        /// return list of parcels through BL
        /// </summary>
        /// <returns>returns the list of all parcels</returns>
        public IEnumerable<ParcelToList> GetParcels()
        {
            List<ParcelToList> myList = new();
            try
            {
                foreach (var parcel in MyDal.GetParcels().ToList())
                    myList.Add(Converter.GetParcelToList(parcel, MyDal));
                if (myList.Count == 0)
                    throw new BlViewItemsListException("Parcel list is empty");
                return myList;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        /// <summary>
        /// return list of parcels through BL by Status
        /// </summary>
        /// <returns>returns the list of all parcels</returns>
        public IEnumerable<ParcelToList> ParcelsByStatus(ParcelStatus myStatus)
        {
            try
            {
                return GetParcels().Where(x => x.Status == myStatus);
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        /// <summary>
        /// search after all parcels that defined
        /// </summary>
        /// <returns>return a list with all the parcels that defined</returns>
        public IEnumerable<ParcelToList> NoneScheduledParcels()
        {
            List<ParcelToList> myList = new(); //takes all parcels from myDal end set them on myList
            try
            {
                return GetParcels().Where(x => x.Status == ParcelStatus.Defined);
                /*foreach (var item in MyDal.AllParcels(x => x.Scheduled == null))
                    myList.Add(new ParcelToList()
                    {
                        Id = item.Id,
                        SenderName = (MyDal.AllCustomers().First(x => x.Id == item.SenderId)).Name,
                        TargetName = (MyDal.AllCustomers().First(x => x.Id == item.TargetId)).Name,
                        Weight = (WeightCategories)item.Weight,
                        Priority = (Priorities)item.Priority,
                        Status = Converter.CalculateParcelStatus(item),
                    });*/
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }

        public IEnumerable<ParcelToList> ParcelsByDates(DateTime? fromDate, DateTime? toDate)
        {
            List<ParcelToList> myList = new();
            try
            {
                foreach (var parcel in MyDal.GetParcels().ToList().Where(p=> p.Requested >= fromDate && p.Requested <= toDate))
                    myList.Add(Converter.GetParcelToList(parcel, MyDal));
                return myList;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        public IEnumerable<ParcelToList> GetSentParcels(int id)
        {
            List<ParcelToList> myList = new();
            try
            {
                foreach (var parcel in MyDal.GetParcels(x=>x.SenderId == id))
                    myList.Add(Converter.GetParcelToList(parcel, MyDal));
                return myList;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        public IEnumerable<ParcelToList> GetRecievedParcels(int id)
        {
            List<ParcelToList> myList = new();
            try
            {
                foreach (var parcel in MyDal.GetParcels(x => x.TargetId == id))
                    myList.Add(Converter.GetParcelToList(parcel, MyDal));
                return myList;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
        }
        #endregion

        #region BASE-STATION FUNCS
        /// <summary>
        /// adding a new base atation into list through BL
        /// </summary>
        /// <param name="myBaseStation">myBasestion to add</param>
        public void AddBaseStation(BaseStation myBaseStation)
        {
            try
            {
                MyDal.AddBaseStation(Converter.GetDalBS(myBaseStation));
            }
            catch (Exception ex)
            {
                throw new BlAddEntityException($"cannot add base station {myBaseStation.Id}:", ex);
            }
        }
        /// <summary>
        /// find a base-station by id and return it through BL
        /// </summary>
        /// <param name="baseStationId"> base-stationId to find</param>
        /// <returns></returns>
        public BaseStation GetBaseStation(int baseStationId)
        {
            try
            {
                DO.BaseStation dalBs = MyDal.GetBaseStation(baseStationId);
                return Converter.GetBlBS(dalBs, MyDal, blDrones);
            }
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot Find base station {baseStationId}:", ex);
            }

        }
        /// <summary>
        /// takes all base-station in data-layer
        /// </summary>
        /// <returns> return array of base-stations through BL</returns>
        public IEnumerable<BaseStationToList> GetBaseStations()
        {
            List<BaseStationToList> myList = new();
            try
            {
                foreach (var bs in MyDal.GetBaseStations()) //takes all BlBaseStations from myDal end set them on myList
                    myList.Add(Converter.GetBSToList(bs, MyDal));
                if (myList.Count == 0)
                    throw new BlViewItemsListException("Base-station list is empty");
                return myList;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show base stations list:", ex);
            }
        }
        /// <summary>
        /// delete (inactivate) a base station by Id 
        /// </summary>
        /// <param name="id">base station Id</param>
        public void DeleteBaseStation(int id)
        {
            try
            {
                MyDal.DeleteBaseStation(id);
            }
            catch (Exception ex)
            {
                throw new BlDeleteEntityException($"cannot delete base station {id}:", ex);
            }
        }
        /// <summary>
        /// chacks in every base-station for free slots
        /// </summary>
        /// <returns>reurn list of free slots on base station from BL</returns>
        public IEnumerable<BaseStationToList> FreeSlotsBaseStations()
        {
            List<BaseStationToList> baseStations = new();
            try
            {
                foreach (var bs in MyDal.GetBaseStations(x => x.FreeChargeSlots > 0)) //takes all BlBaseStations from myDal end set them on myList
                    baseStations.Add(Converter.GetBSToList(bs, MyDal));
                if (baseStations.Count == 0)
                    throw new BlViewItemsListException("Base-station list is empty");
                return baseStations;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show Free Slots Base Stations list:", ex);
            }
           
        }
        /// <summary>
        /// change the name and the number of slots in base station through BL
        /// </summary>
        /// <param name="baseStationId">baseStationId for identified the base-station</param>
        /// <param name="newName">newName to replace the old one</param>
        /// <param name="slotsCount">newCount to replace the number of the old slots</param>
        public void UpdateBaseStation(int? baseStationId, string newName, int slotsCount)
        {
            try
            {
                MyDal.UpdateBaseStation(baseStationId, newName, slotsCount);
            }
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot update base station {baseStationId}:", ex);
            }
        }
        #endregion
        #region CUSTOMER FUNCS
        /// <summary>
        /// adding a new customer into list through BL
        /// </summary>
        /// <param name="myCustomer">myCustomer to add</param>
        public void AddCustomer(Customer myCustomer)
        {
            try
            {
                MyDal.AddCustomer(Converter.GetDalCustomer(myCustomer));
            }
            catch (Exception ex)
            {
                throw new BlAddEntityException($"cannot add customer {myCustomer.Id}:", ex);
            }
        }
        /// <summary>
        /// find a customer by id and return it through BL
        /// </summary>
        /// <param name="customerId">customerId to find</param>
        /// <returns>return if he find one that matching</returns>
        public Customer GetCustomer(int? customerId)
        {
            try
            {
                DO.Customer dalCustomer = MyDal.GetCustomer(customerId); //find the customer data layer throwgh id and fit the details to BL layer
                return Converter.GetBlCustomer(dalCustomer, MyDal);
            }
            catch (Exception ex)
            {
                throw new BlFindItemException($"cannot find customer {customerId}:", ex);
            }
        }
        /// <summary>
        /// takes all customers from data base 
        /// </summary>
        /// <returns> return array of customers through BL </returns>
        public IEnumerable<CustomerToList> GetCustomers()
        {
            List<CustomerToList> customers = new();
            try
            {
                foreach (var customer in MyDal.GetCustomers())
                    customers.Add(Converter.GetCustomerToList(customer, MyDal));
                if (customers.Count == 0)
                    throw new BlViewItemsListException("Customers list is empty");
                return customers;
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show base stations list:", ex);
            }
        }
        /// <summary>
        /// delete (inactivate) a customer by Id 
        /// </summary>
        /// <param name="id">base station Id</param>
        public void DeleteCustomer(int id)
        {
            try
            {
                MyDal.DeleteCustomer(id);
            }
            catch (Exception ex)
            {
                throw new BlDeleteEntityException($"cannot delete customer {id}:", ex);
            }
        }
        /// <summary>
        /// change the name and the phon number
        /// </summary>
        /// <param name="customerId">customerId for identified the customer</param>
        /// <param name="newName">newName to change</param>
        /// <param name="newPhone">newPhone to change</param>
        public void UpdateCustomer(int? customerId, string newName, string newPhone)
        {
            try
            {
                MyDal.UpdateCustomer(customerId, newName, newPhone);
            }
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot update customer {customerId}:", ex);
            }
        }
        #endregion

        #region DRONE FUNCS
        /// <summary>
        /// adding a new drone into list through BL
        /// </summary>
        /// <param name="myDrone">myDrone to add</param>
        /// <param name="baseStationId">baseStationId to set the myDrone there</param>
        public void AddDrone(DroneToList myDrone, int baseStationId)
        {
            try
            {
                DO.BaseStation bs = MyDal.GetBaseStation(baseStationId);                              // luoking if the base station exsist
                if (bs.Equals(null))
                    throw new BaseStationBLException($"Base station {baseStationId} doesn't found");
                else
                {
                    Random rand = new(DateTime.Now.Millisecond);
                    myDrone.BatteryStatus = rand.NextDouble() * 20 + 20; // put valus in to myDrone and insert to a list in BL
                    myDrone.Status = DroneStatusCategories.Maintenance;
                    myDrone.Location = new Location() { Latitude = bs.Latitude, Longitude = bs.Longitude };
                    myDrone.TransferdParcel = 0;
                    myDrone.IsActive = true;
                    MyDal.AddDrone(Converter.GetDalDrone(myDrone)); // set the new drone in data layer
                    MyDal.ChargeDrone(myDrone.Id, bs.Id);
                    blDrones.Add(myDrone);
                }
            }
            catch (Exception ex)
            {
                throw new BlAddEntityException($"cannot add drone id:", ex);
            }
        }
        /// <summary>
        /// find a drone by id and return it from BL
        /// </summary>
        /// <param name="droneId">droneId to find</param>
        /// <returns>returns if he find one</returns>
        public Drone GetDrone(int? droneId)
        {
            try
            {
                DroneToList dtl = blDrones.Find(x => x.Id == droneId);
                return Converter.GetBlDrone(dtl, MyDal);
            }
            catch (Exception ex)
            {
                throw new BlFindItemException($"cannot find drone {droneId}:", ex);
            }
        }
        /// <summary>
        /// delete (inactivate) a drone by Id 
        /// </summary>
        /// <param name="id">base station Id</param>
        public void DeleteDrone(int? id)
        {
            try
            {
                if (blDrones.Find(x => x.Id == id).Status == DroneStatusCategories.Maintenance)
                    releaseDrone(id);
                MyDal.DeleteDrone(id);
                blDrones.Find(x => x.Id == id).IsActive = false;
            }
            catch (Exception ex)
            {
                throw new BlDeleteEntityException($"cannot delete drone {id}:", ex);
            }
        }
        public DroneToList GetDroneToList(int? id) => blDrones.Find(d => d.Id == id);
        /// <summary>
        /// collect all drones from data base
        /// </summary>
        /// <returns>return array of drones from BL</returns>
        public IEnumerable<DroneToList> GetDrones(Func<DroneToList, bool> predicate = null)
        {
            if (predicate == null)
                return blDrones.Where(x=>x.IsActive == true).ToList();

            return (from item in blDrones
                    where item.IsActive is true
                    where predicate(item)
                    select item);
        }
        /// <summary>
        /// change the drone model through BL
        /// </summary>
        /// <param name="droneId">droneId to be identified the drone</param>
        /// <param name="newName">newName to change</param>
        public void UpdateDroneModel(int? droneId, string newName)
        {
            try
            {
                MyDal.UpdateDroneModel(droneId, newName);   // change the model of a dron in data layer by dron id
                blDrones.Find(x => x.Id == droneId).Model = newName;
            }
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot update drone {droneId}:", ex);
            }

        }
        /// <summary>
        /// connect a drone to charging at base-station throu BL
        /// </summary>
        /// <param name="droneId"> to connect</param>
        public void ChargeDrone(int? droneId)
        {
            try
            {
                MyDal.GetDrone(droneId);
                if (blDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Free)    // check befor that the drone are free
                    throw new BlUpdateEntityException("The chosen drone isn't free");
                else
                {
                    DroneToList myDrone = blDrones.Find(x => x.Id == droneId);// find the drone that wonted to charge and creat a list of all base station that sorted from the nearest base station to the farthest from the current location of the dron
                    List<DO.BaseStation> dalBsList = MyDal.GetBaseStations().OrderBy(x => myDrone.Distance(new BaseStation(){Location = new Location() { Latitude = x.Latitude, Longitude = x.Longitude } })).ToList();
                    foreach (var baseStation in dalBsList)
                    {
                        var bs = new BaseStation() { Location = new() { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude } };
                        double batteryNeed = myDrone.Distance(bs) * BatteryUsages[DRONE_FREE]; //batteryNeed is the total calculation between the distance multiplied with the use of battery when the drone free
                        if (myDrone.BatteryStatus > batteryNeed)    //if the battery of the drone enough
                            if (baseStation.FreeChargeSlots > 0) // if the base station free  update the details ChargeDrone at base station and the drone details 
                            {
                                MyDal.ChargeDrone(myDrone.Id, baseStation.Id);
                                blDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Maintenance;
                                blDrones.Find(x => x.Id == droneId).StartCharge = DateTime.Now;
                                blDrones.Find(x => x.Id == droneId).BatteryStatus -= batteryNeed;
                                blDrones.Find(x => x.Id == droneId).Location = new Location() { Latitude = baseStation.Latitude, Longitude = baseStation.Longitude };
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
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot charge drone {droneId}:", ex);
            }
        }
        /// <summary>
        /// release a drone from charging through BL
        /// </summary>
        /// <param name="droneId">droneId to release</param>
        /// <param name="tSpanInCharge">tSpanInCharge to calculate the amount of charge</param>
        public void releaseDrone(int? droneId)
        {
            try
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
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot release drone {droneId}:", ex);
            }
        }
        /// <summary>
        /// schedule a drone for parcel delivery through BL
        /// </summary>
        /// <param name="parcelId">parcel id for deliver</param>
        /// <param name="droneId">drone id for schedule</param>
        public void ScheduleDroneForParcel(int? droneId)
        {
            try
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
                    List<DO.Parcel> parcels = MyDal.GetParcels(x=>x.Scheduled == null).OrderByDescending(x => x.Priority).Where(x => x.Weight <= (DO.WeightCategories)myDrone.Weight).OrderByDescending(x => x.Weight).ToList();
                    parcels = parcels.OrderBy(x => myDrone.Distance(GetCustomer(x.SenderId))).ToList();
                    if (parcels.Count > 0)                                       // sorted the parcels list that created from the nearest parcel to the drone location to The farthest
                    {
                        foreach (var item in parcels)
                        {
                            //ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.GetCustomers().ToList()); //set the details of the parcel in ParcelInTransfer show
                            /*double distanceToSource = LocationFuncs.DistanceBetweenTwoLocations(myDrone.Location, myParcel.CollectionLocation); //calculate the distance between drone to parcel location  
                            double distanceToBase = LocationFuncs.DistanceBetweenTwoLocations(myParcel.DeliveryDestinationLocation, LocationFuncs.ClosestBaseStationLocation(MyDal.GetBaseStations().ToList(), myParcel.DeliveryDestinationLocation)); // calculate the dustance between from delivery to the closest base station
                            double batteryNeedToBase = BatteryUsages[DRONE_FREE] * (distanceToSource + distanceToBase); //calculate the total use of battery to make the delivery and to the base station
                            // different calculate for evert case of weight
                            if (item.Weight == DO.WeightCategories.Light)
                                batteryNeedToBase += myParcel.TransportDistance * BatteryUsages[DRONE_LIGHT];
                            else if (item.Weight == DO.WeightCategories.Medium)
                                batteryNeedToBase += myParcel.TransportDistance * BatteryUsages[DRONE_MEDIUM];
                            else if (item.Weight == DO.WeightCategories.Heavy)
                                batteryNeedToBase += myParcel.TransportDistance * BatteryUsages[DRONE_HEAVY];
                            */
                            double batteryNeedToBase = myDrone.RequiredBattery(this, item.Id);
                            if (batteryNeedToBase < myDrone.BatteryStatus)
                            {                                                                     // if the battery of the drone enough to complit the delivering and get to the base station
                                MyDal.ScheduleDroneForParcel(item.Id, myDrone.Id);
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
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot schedule drone {droneId}:", ex);
            }
        }
        /// <summary>
        /// picking up a parcel by its drone through BL
        /// </summary>
        /// <param name="droneId">droneId to Recognize </param>
        public void PickingUpAParcel(int? droneId)
        {
            try
            {
                MyDal.GetDrone(droneId);
                foreach (var item in MyDal.GetParcels())
                {
                    if (item.DroneId == droneId && item.Scheduled != null && item.PickedUp == null)              // updat the details of the drone and the parcel and the battery of the drone and is location
                    {
                        DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                        ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.GetCustomers().ToList());
                        MyDal.PickingUpAParcel(item.Id);
                        blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUsages[DRONE_FREE] * myDrone.Distance(GetCustomer(item.SenderId));
                        blDrones.Find(x => x.Id == droneId).Location = myParcel.CollectionLocation;
                        return;
                    }
                }
                throw new BlUpdateEntityException($"There isn't parcel to pick up for drone {droneId}");
            }
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot pick up {droneId}:", ex);
            }
        }
        /// <summary>
        /// finish delivery of a parcel through BL
        /// </summary>
        /// <param name="droneId">droneId to Recognize</param>
        public void DeliverAParcel(int? droneId)
        {
            try
            {
                MyDal.GetDrone(droneId);                                   //find the drone and find the parcels that he deliverd and canculat the values for the battery and the location of the drone
                foreach (var item in MyDal.GetParcels())
                {
                    if (item.DroneId == droneId && item.PickedUp != null && item.Delivered == null)
                    {
                        DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                        ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.GetCustomers().ToList());
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
            catch (Exception ex)
            {
                throw new BlUpdateEntityException($"cannot pick up {droneId}:", ex);
            }
        }

        public BaseStationToList GetBSToList(int? id)
        {
            return (from item in GetBaseStations().ToList()
                    where item.Id == id
                    select item).FirstOrDefault();
        }
        
        public CustomerToList GetCustomerToList(int? id)
        {
            return (from item in GetCustomers().ToList()
                    where item.Id == id
                    select item).FirstOrDefault();
        }
        
        public ParcelToList GetParcelToList(int? id)
        {
            return (from item in GetParcels().ToList()
                    where item.Id == id
                    select item).FirstOrDefault();
        }

        public int GetNextParcelId()
        {
            return MyDal.GetNextParcelId();
        }



        #endregion

    }
}

