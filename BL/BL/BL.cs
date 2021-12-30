using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BlApi
{
    sealed internal class BL : IBL
    {
        private DalApi.IDal MyDal;
        private static double BatteryUseFREE;
        private static double BatteryUseLight;
        private static double BatteryUseMedium;
        private static double BatteryUseHeavy;
        private static double BatteryChargeRate;
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
            MyDal = DalApi.DalFactory.GetDal();
            //אחוזים לק"מ
            BatteryUseFREE = MyDal.GetBatteryUse()[0];
            BatteryUseLight = MyDal.GetBatteryUse()[1];
            BatteryUseMedium = MyDal.GetBatteryUse()[2];
            BatteryUseHeavy = MyDal.GetBatteryUse()[3];
            //אחוזים לשעה
            BatteryChargeRate = MyDal.GetBatteryUse()[4];
            Random rand = new Random(DateTime.Now.Millisecond);
            List<DO.Drone> Drones = (List<DO.Drone>)MyDal.AllDrones();
            List<DO.Parcel> Parcels = (List<DO.Parcel>)MyDal.AllParcels();
            List<DO.Customer> Customers = (List<DO.Customer>)MyDal.AllCustomers();
            List<DO.BaseStation> BaseStations = (List<DO.BaseStation>)MyDal.AllBaseStations();
            foreach (var item in Drones.ToList())
            {
                DroneToList myDrone = new();
                myDrone.Id = item.Id;
                myDrone.Model = item.Model;
                myDrone.Weight = (WeightCategories)item.MaxWeight;
                myDrone.IsActive = true;
                var myParcel = Parcels.Find(x => x.DroneId == myDrone.Id && x.Delivered == null);
                if (!myParcel.Equals(default(DO.Parcel)))
                {
                    myDrone.Status = DroneStatusCategories.Delivery;
                    myDrone.BatteryStatus = rand.NextDouble() * 50 + 50;
                    myDrone.TransferdParcel = myParcel.Id;
                    var Sender = Customers.Find(x => x.Id == myParcel.SenderId);
                    Location SenderLocation = new() { Latitude = Sender.Lattitude, Longitude = Sender.Lattitude };
                    if (myParcel.PickedUp == null)
                    {
                        Location MyLocation = LocationFuncs.ClosestBaseStationLocation(BaseStations, SenderLocation);
                        myDrone.CurrentLocation = MyLocation;

                        var Reciever = Customers.Find(x => x.Id == myParcel.TargetId);
                        Location RecieverLocation = new Location(Reciever.Lattitude, Reciever.Longitude);
                        double distanceWithParcel = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, RecieverLocation);//מרחק בין רחפן ליעד
                        double distanceWithoutParcel = LocationFuncs.DistanceBetweenTwoLocations(RecieverLocation, LocationFuncs.ClosestBaseStationLocation(BaseStations, RecieverLocation));//מרחק בין יעד לתחנה קרובה
                        double minBattery = distanceWithoutParcel * BatteryUseFREE;// בלי חבילה עליו
                        if (myParcel.Weight == DO.WeightCategories.Light)
                            minBattery += distanceWithParcel * BatteryUseLight;
                        else if (myParcel.Weight == DO.WeightCategories.Medium)
                            minBattery += distanceWithParcel * BatteryUseMedium;
                        else if (myParcel.Weight == DO.WeightCategories.Heavy)
                            minBattery += distanceWithParcel * BatteryUseHeavy;
                        myDrone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;
                    }
                    else
                    {
                        myDrone.CurrentLocation = SenderLocation;
                    }
                }
                else
                {
                    myDrone.Status = (DroneStatusCategories)rand.Next(0, 2);
                    if (myDrone.Status == DroneStatusCategories.Maintenance)
                    {
                        DO.BaseStation randomBS = BaseStations[rand.Next(BaseStations.Count)];
                        myDrone.CurrentLocation = new Location() { Latitude = randomBS.Lattitude, Longitude = randomBS.Longitude };
                        myDrone.BatteryStatus = rand.NextDouble() * 20;
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
                            myDrone.CurrentLocation = new Location(randomCustomer.Lattitude, randomCustomer.Longitude);
                        }
                        else
                        {
                            myDrone.CurrentLocation = new()
                            {
                                Latitude = rand.NextDouble() * (33.4188709641265 - 29.49970431757609) + 29.49970431757609,
                                Longitude = rand.NextDouble() * (35.89927249423983 - 34.26371323423407) + 34.26371323423407
                            };
                        }
                        double distanceWithoutParcel = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, LocationFuncs.ClosestBaseStationLocation(BaseStations, myDrone.CurrentLocation));//מרחק בין יעד לתחנה קרובה
                        double minBattery = distanceWithoutParcel * BatteryUseFREE;// בלי חבילה עליו
                        myDrone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;

                    }
                    myDrone.TransferdParcel = Parcels.Find(x => x.DroneId == myDrone.Id && x.Delivered == null).Id;
                }
                blDrones.Add(myDrone);

            }
        }
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
                //  if ((int)myParcel.Weight > 3 || (int)myParcel.Weight < 1)
                //    throw new InvalidEnumValueException($"Invalid Weight Categories{(int)myParcel.Weight}");
                MyDal.FindCustomer(myParcel.Sender.Id);
                MyDal.FindCustomer(myParcel.Target.Id);
                myParcel.Requested = DateTime.Now;
                myParcel.Scheduled = myParcel.PickedUp = myParcel.Delivered = null;
                myParcel.DroneAtParcel = new DroneAtParcel();
                MyDal.AddParcel(Converter.ConvertBlParcelToDalParcel(myParcel)); // convert myparcel to store it in data layer
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
        public Parcel FindParcel(int parcelId)
        {
            try
            {
                //extract the parcel from data layer and fit the details to the BL layer 
                DO.Parcel dalParcel = MyDal.FindParcel(parcelId);
                Parcel blParcel = new()
                {
                    Id = dalParcel.Id,
                    Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = MyDal.FindCustomer(dalParcel.SenderId).Name },
                    Target = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = MyDal.FindCustomer(dalParcel.TargetId).Name },
                    Weight = (WeightCategories)dalParcel.Weight,
                    Priority = (Priorities)dalParcel.Priority,
                    Requested = dalParcel.Requested,
                    Scheduled = dalParcel.Scheduled,
                    PickedUp = dalParcel.PickedUp,
                    Delivered = dalParcel.Delivered
                };
                if (blParcel.Scheduled != null)
                {
                    blParcel.DroneAtParcel = new DroneAtParcel()
                    {
                        Id = dalParcel.DroneId,
                        BatteryStatus = blDrones.Find(x => x.Id == dalParcel.DroneId).BatteryStatus,
                        CurrentLocation = blDrones.Find(x => x.Id == dalParcel.DroneId).CurrentLocation
                    };
                }
                return blParcel;
            }
            catch (Exception ex)
            {
                throw new BlFindItemException($"cannot Find parcel {parcelId}:", ex);
            }
        }
        /// <summary>
        /// return list of parcels through BL
        /// </summary>
        /// <returns>returns the list of all parcels</returns>
        public IEnumerable<ParcelToList> AllBlParcels()
        {
            List<ParcelToList> myList = new(); 
            try
            {
                foreach (var item in MyDal.AllParcels())
                    myList.Add(new ParcelToList()
                    {
                        Id = item.Id,
                        SenderName = (MyDal.AllCustomers().First(x => x.Id == item.SenderId)).Name,
                        TargetName = (MyDal.AllCustomers().First(x => x.Id == item.TargetId)).Name,
                        Weight = (WeightCategories)item.Weight,
                        Priority = (Priorities)item.Priority,
                        Status = Converter.CalculateParcelStatus(item),
                    });
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
            if (myList.Count == 0)
                throw new BlViewItemsListException("Parcel list is empty");

            return myList;
        }
        /// <summary>
        /// return list of parcels through BL by Status
        /// </summary>
        /// <returns>returns the list of all parcels</returns>
        public IEnumerable<ParcelToList> ParcelsByStatus(ParcelStatus myStatus)
        {
            try
            {
                return AllBlParcels().Where(x => x.Status == myStatus);
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
                foreach (var item in MyDal.AllParcels(x => x.Scheduled == null))
                    myList.Add(new ParcelToList()
                    {
                        Id = item.Id,
                        SenderName = (MyDal.AllCustomers().First(x => x.Id == item.SenderId)).Name,
                        TargetName = (MyDal.AllCustomers().First(x => x.Id == item.TargetId)).Name,
                        Weight = (WeightCategories)item.Weight,
                        Priority = (Priorities)item.Priority,
                        Status = Converter.CalculateParcelStatus(item),
                    });
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show parcels list:", ex);
            }
            if (myList.Count == 0)
                throw new BlViewItemsListException("None-Scheduled Parcels list is empty");
            return myList;
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
                MyDal.AddBaseStation(new DO.BaseStation()
                {
                    Id = (int)myBaseStation.Id,
                    Name = myBaseStation.Name,
                    Lattitude = (double)myBaseStation.BSLocation.Latitude,
                    Longitude = (double)myBaseStation.BSLocation.Longitude,
                    FreeChargeSlots = (int)myBaseStation.FreeChargeSlots,
                    IsActive = true
                });// use with AddBaseStation from mydal to set basestation from the user to datasorce
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
        public BaseStation FindBaseStation(int baseStationId)
        {
            try
            {
                DO.BaseStation dalBs = MyDal.FindBaseStation(baseStationId);
                BaseStation blBs = new();
                blBs.DronesInCharge = new();
                blBs.Id = dalBs.Id;                                                                 // find the wanted BaseStation and fit him to BL BaseStation
                blBs.Name = dalBs.Name;
                blBs.BSLocation = new Location(dalBs.Lattitude, dalBs.Longitude);
                blBs.FreeChargeSlots = dalBs.FreeChargeSlots;
                List<DO.DroneCharge> myList = MyDal.GetListOfInChargeDrones().Where(x => x.StationId == baseStationId).ToList(); //update the blBs.DronesInCharge with all the drones that chargeing on the BaseStation that found 
                foreach (var item in myList)
                {
                    blBs.DronesInCharge.Add(new DroneInCharge()
                    {
                        Id = item.DroneId,
                        BatteryStatus = blDrones.Find(x => x.Id == item.DroneId).BatteryStatus
                    });
                }
                return blBs;
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
        public IEnumerable<BaseStationToList> AllBlBaseStations()
        {
            List<BaseStationToList> myList = new();
            try
            {
                foreach (var item in MyDal.AllBaseStations()) //takes all BlBaseStations from myDal end set them on myList
                    myList.Add(new BaseStationToList()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        FreeChargeSlots = item.FreeChargeSlots,
                        BusyChargeSlots = MyDal.GetListOfInChargeDrones().Where(x=>x.StationId==item.Id).Count()
                    });
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show base stations list:", ex);
            }
            if (myList.Count == 0)
                throw new BlViewItemsListException("Base-station list is empty");
            return myList;
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
            List<BaseStationToList> myList = new();
            try
            {
                foreach (var item in MyDal.AllBaseStations(x => x.FreeChargeSlots > 0)) //takes all BlBaseStations from myDal end set them on myList
                    myList.Add(new BaseStationToList()
                    {
                        Id = item.Id,
                        Name = item.Name,
                        FreeChargeSlots = item.FreeChargeSlots,
                        BusyChargeSlots = 0
                    });
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show Free Slots Base Stations list:", ex);
            }
            if (myList.Count == 0)
                throw new BlViewItemsListException("Base-station list is empty");
            return myList;

        }
        /// <summary>
        /// change the name and the number of slots in base station through BL
        /// </summary>
        /// <param name="baseStationId">baseStationId for identified the base-station</param>
        /// <param name="newName">newName to replace the old one</param>
        /// <param name="slotsCount">newCount to replace the number of the old slots</param>
        public void UpdateBaseStation(int baseStationId, string newName, int slotsCount)
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
                MyDal.AddCustomer(Converter.ConvertBlCustomerToDalCustomer(myCustomer));
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
        public Customer FindCustomer(int? customerId)
        {
            try
            {
                DO.Customer dalCustomer = MyDal.FindCustomer(customerId); //find the customer data layer throwgh id and fit the details to BL layer
                Customer blCustomer = new()
                {
                    Id = dalCustomer.Id,
                    Name = dalCustomer.Name,
                    CustomerLocation = new Location(dalCustomer.Lattitude, dalCustomer.Longitude),
                    PhoneNumber = dalCustomer.Phone
                };
                blCustomer.ParcelFromCustomerList = new List<ParcelAtCustomer>();
                foreach (var item in MyDal.AllParcels().Where(x => x.SenderId == customerId)) // creat a list into  blCustomer of all the parcel that he hever send and all the details of hevry parcel
                {
                    blCustomer.ParcelFromCustomerList.Add(new ParcelAtCustomer()
                    {
                        Id = item.Id,
                        Weight = (WeightCategories)item.Weight,
                        Priority = (Priorities)item.Priority,
                        Status = Converter.CalculateParcelStatus(item),
                        CustomerAtParcel = new CustomerAtParcel()
                        {
                            Id = item.TargetId,
                            Name = MyDal.FindCustomer(item.TargetId).Name
                        }
                    });
                }
                blCustomer.ParcelToCustomerList = new List<ParcelAtCustomer>();
                foreach (var item in MyDal.AllParcels().Where(x => x.TargetId == customerId))//creat a list into  blCustomer of all the parcel that he hever have received and all the details of hevry parcel
                {
                    blCustomer.ParcelToCustomerList.Add(new ParcelAtCustomer()
                    {
                        Id = item.Id,
                        Weight = (WeightCategories)item.Weight,
                        Priority = (Priorities)item.Priority,
                        Status = Converter.CalculateParcelStatus(item),
                        CustomerAtParcel = new CustomerAtParcel()
                        {
                            Id = item.SenderId,
                            Name = MyDal.FindCustomer(item.SenderId).Name
                        }
                    });
                }
                return blCustomer;  // returns the updeted customer 
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
        public IEnumerable<CustomerToList> AllBlCustomers()
        {
            List<CustomerToList> myList = new();
            try
            {
                foreach (var item in MyDal.AllCustomers())
                    myList.Add(new CustomerToList()
                    {
                        Id = item.Id,                                                  //extarct all customers from data layer and fit them to a list on BL 
                        Name = item.Name,
                        PhoneNumber = item.Phone,
                        SentAndSuppliedParcels = MyDal.AllParcels().Where(x => x.SenderId == item.Id && x.Delivered != null).Count(),
                        SentAndNotSuppliedParcels = MyDal.AllParcels().Where(x => x.SenderId == item.Id && x.Delivered == null).Count(),
                        RecievedParcels = MyDal.AllParcels().Where(x => x.TargetId == item.Id && x.Delivered != null).Count(),
                        InProcessParcelsToCustomer = MyDal.AllParcels().Where(x => x.TargetId == item.Id && x.Delivered == null).Count(),
                    });
            }
            catch (Exception ex)
            {
                throw new BlViewItemsListException($"cannot show base stations list:", ex);
            }
            if (myList.Count == 0)
                throw new BlViewItemsListException("Customers list is empty");
            return myList;
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
                DO.BaseStation bs = MyDal.FindBaseStation(baseStationId);                              // luoking if the base station exsist
                if (bs.Equals(null))
                    throw new BaseStationBLException($"Base station {baseStationId} doesn't found");
                else
                {
                    Random rand = new(DateTime.Now.Millisecond);
                    myDrone.BatteryStatus = rand.NextDouble() * 20 + 20;                                    // put valus in to myDrone and insert to a list in BL
                    myDrone.Status = DroneStatusCategories.Maintenance;
                    myDrone.CurrentLocation = new Location(bs.Lattitude, bs.Longitude);
                    myDrone.TransferdParcel = 0;
                    MyDal.AddDrone(Converter.ConvertBlDroneToDalDrone(myDrone));     // set the new drone in data layer
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
        public Drone FindDrone(int? droneId)
        {
            try
            {
                DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                Drone drone = new()
                {
                    Id = myDrone.Id,
                    Model = myDrone.Model,                                             //finds a dron by id from list that in BL and set the details on Drone show
                    Weight = myDrone.Weight,
                    BatteryStatus = myDrone.BatteryStatus,
                    Status = myDrone.Status,
                    CurrentLocation = myDrone.CurrentLocation
                };
                if (drone.Status == DroneStatusCategories.Delivery)                      //if the drone executing delivery uptate the details of ParcelInTransfer 
                {
                    DO.Parcel dalParcel = MyDal.AllParcels().First(x => x.DroneId == droneId && x.Delivered == null); // find the parcel that the drone delivering
                    ParcelInTransfer parcelInTransfer = new()
                    {
                        Id = dalParcel.Id,                                                                                                 // update the details in  ParcelInTransfer 
                        Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = MyDal.FindCustomer(dalParcel.SenderId).Name },
                        Reciever = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = MyDal.FindCustomer(dalParcel.TargetId).Name },
                        Weight = (WeightCategories)dalParcel.Weight,
                        CollectionLocation = new() { Latitude = MyDal.FindCustomer(dalParcel.SenderId).Lattitude, Longitude = MyDal.FindCustomer(dalParcel.SenderId).Longitude },
                        DeliveryDestinationLocation = new() { Latitude = MyDal.FindCustomer(dalParcel.TargetId).Lattitude, Longitude = MyDal.FindCustomer(dalParcel.TargetId).Longitude },
                        Priority = (Priorities)dalParcel.Priority,
                    };
                    parcelInTransfer.TransportDistance = LocationFuncs.DistanceBetweenTwoLocations(parcelInTransfer.CollectionLocation, parcelInTransfer.DeliveryDestinationLocation); // use in Distance calculation function that creat for calculat distance between two locations
                    if (dalParcel.PickedUp == null) // update the status of  parcelInTransfer to be false if the parcel not pickup or true if are pickup                                                                                                                      
                        parcelInTransfer.Status = false;
                    else
                        parcelInTransfer.Status = true;
                    drone.CurrentParcel = parcelInTransfer;
                }
                return drone;
            }
            catch (Exception ex)
            {
                throw new BlFindItemException($"cannot find drone {droneId}:", ex);
            }
        }


        /// <summary>
        /// collect all drones from data base
        /// </summary>
        /// <returns>return array of drones from BL</returns>
        public IEnumerable<DroneToList> AllBlDrones(Func<DroneToList, bool> predicate = null)
        {
            if (predicate == null)
                return blDrones;

            //return blDrones.Where(predicate);
            return (from item in blDrones
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
                MyDal.FindDrone(droneId);
                if (blDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Free)    // check befor that the drone are free
                    throw new BlUpdateEntityException("The chosen drone isn't free");
                else
                {
                    DroneToList myDrone = blDrones.Find(x => x.Id == droneId);// find the drone that wonted to charge and creat a list of all base station that sorted from the nearest base station to the farthest from the current location of the dron
                    List<DO.BaseStation> dalBsList = MyDal.AllBaseStations().OrderBy(x => LocationFuncs.DistanceBetweenTwoLocations(new Location(x.Lattitude, x.Longitude), (myDrone.CurrentLocation))).ToList();
                    foreach (var baseStation in dalBsList)
                    {
                        double batteryNeed = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, new Location(baseStation.Lattitude, baseStation.Longitude)) * BatteryUseFREE; //batteryNeed is the total calculation between the distance multiplied with the use of battery when the drone free
                        if (myDrone.BatteryStatus > batteryNeed)    //if the battery of the drone enough
                            if (baseStation.FreeChargeSlots > 0) // if the base station free  update the details ChargeDrone at base station and the drone details 
                            {
                                MyDal.ChargeDrone(myDrone.Id, baseStation.Id);
                                blDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Maintenance;
                                blDrones.Find(x => x.Id == droneId).BatteryStatus -= batteryNeed;
                                blDrones.Find(x => x.Id == droneId).CurrentLocation = new Location(baseStation.Lattitude, baseStation.Longitude);
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
        public void releaseDrone(int? droneId, TimeSpan tSpanInCharge)
        {
            try
            {
                MyDal.FindDrone(droneId);
                if (blDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Maintenance) // check if the drone are Maintenance
                    throw new BlUpdateEntityException("The chosen drone isn't in charge");
                if (tSpanInCharge.TotalMinutes < 0)
                    throw new BlUpdateEntityException("The chosen time is invalid");
                MyDal.ReleaseDroneFromCharge(droneId);                                                                //updat the details in the drone 
                blDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Free;
                blDrones.Find(x => x.Id == droneId).BatteryStatus = Math.Min(blDrones.Find(x => x.Id == droneId).BatteryStatus + tSpanInCharge.TotalHours * BatteryChargeRate, 100.0);
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
                MyDal.FindDrone(droneId);
                DroneToList myDrone = blDrones.Find(x => x.Id == droneId);                               // check if the dron are free
                if (blDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Free)
                    throw new BlUpdateEntityException($"Drone {droneId} isn't free");
                //if()
                else
                {                                                                                           // make a list parcels of all parcels from emergency to normal that their whight or equal or smaller then the drone can carry
                    List<DO.Parcel> parcels = MyDal.NoneScheduledParcels().OrderByDescending(x => x.Priority).Where(x => x.Weight <= (DO.WeightCategories)myDrone.Weight).OrderByDescending(x => x.Weight).ToList();
                    parcels = parcels.OrderBy(x => LocationFuncs.DistanceBetweenTwoLocations(new Location() { Latitude = MyDal.FindCustomer(x.SenderId).Lattitude, Longitude = MyDal.FindCustomer(x.SenderId).Longitude }, myDrone.CurrentLocation)).ToList();
                    if (parcels.Count > 0)                                       // sorted the parcels list that created from the nearest parcel to the drone location to The farthest
                    {
                        foreach (var item in parcels)
                        {
                            ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.AllCustomers().ToList()); //set the details of the parcel in ParcelInTransfer show
                            double distanceToSource = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, myParcel.CollectionLocation); //calculate the distance between drone to parcel location  
                            double distanceToBase = LocationFuncs.DistanceBetweenTwoLocations(myParcel.DeliveryDestinationLocation, LocationFuncs.ClosestBaseStationLocation(MyDal.AllBaseStations().ToList(), myParcel.DeliveryDestinationLocation)); // calculate the dustance between from delivery to the closest base station
                            double batteryNeedToBase = BatteryUseFREE * (distanceToSource + distanceToBase); //calculate the total use of battery to make the delivery and to the base station
                            if (item.Weight == DO.WeightCategories.Light)
                                batteryNeedToBase += myParcel.TransportDistance * BatteryUseLight;
                            else if (item.Weight == DO.WeightCategories.Medium)                       // every case of waight of parcel we do diferent calculat
                                batteryNeedToBase += myParcel.TransportDistance * BatteryUseMedium;
                            else if (item.Weight == DO.WeightCategories.Heavy)
                                batteryNeedToBase += myParcel.TransportDistance * BatteryUseHeavy;
                            if (batteryNeedToBase < myDrone.BatteryStatus)
                            {                                                                     // if the battery of the drone enough to complit the delivering and get to the base station
                                MyDal.ScheduleDroneForParcel(item.Id, myDrone.Id);
                                blDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Delivery;
                                blDrones.Find(x => x.Id == droneId).TransferdParcel = myParcel.Id;
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
                MyDal.FindDrone(droneId);
                foreach (var item in MyDal.AllParcels())
                {
                    if (item.DroneId == droneId && item.Scheduled != null && item.PickedUp == null)              // updat the details of the drone and the parcel and the battery of the drone and is location
                    {
                        DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                        ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.AllCustomers().ToList());
                        MyDal.PickingUpAParcel(item.Id);
                        blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseFREE * LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, myParcel.CollectionLocation);
                        blDrones.Find(x => x.Id == droneId).CurrentLocation = myParcel.CollectionLocation;
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
                MyDal.FindDrone(droneId);                                   //find the drone and find the parcels that he deliverd and canculat the values for the battery and the location of the drone
                foreach (var item in MyDal.AllParcels())
                {
                    if (item.DroneId == droneId && item.PickedUp != null && item.Delivered == null)
                    {
                        DroneToList myDrone = blDrones.Find(x => x.Id == droneId);
                        ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.AllCustomers().ToList());
                        MyDal.DeliverAParcel(item.Id);
                        if (myParcel.Weight == WeightCategories.Light)
                            blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseLight * myParcel.TransportDistance;
                        else if (myParcel.Weight == WeightCategories.Medium)
                            blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseMedium * myParcel.TransportDistance;         // to every case of whight
                        else if (myParcel.Weight == WeightCategories.Heavy)
                            blDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseHeavy * myParcel.TransportDistance;
                        blDrones.Find(x => x.Id == droneId).CurrentLocation = myParcel.DeliveryDestinationLocation;
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


        #endregion

    }
}

