using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IBL.BO;



public class BL
{
    IDAL.IDal MyDal;
    internal static double BatteryUseFREE;
    internal static double BatteryUseLight;
    internal static double BatteryUseMedium;
    internal static double BatteryUseHeavy;
    internal static double BatteryChargeRate;
    internal static BL b;
    List<DroneToList> BlDrones;

    public BL()
    {
        BlDrones = new();
        MyDal = new DalObject.DalObject();
        //אחוזים לק"מ
        BatteryUseFREE = MyDal.GetBatteryUse()[0];
        BatteryUseLight = MyDal.GetBatteryUse()[1];
        BatteryUseMedium = MyDal.GetBatteryUse()[2];
        BatteryUseHeavy = MyDal.GetBatteryUse()[3];
        //אחוזים לשעה
        BatteryChargeRate = MyDal.GetBatteryUse()[4];
        Random rand = new Random();
        List<IDAL.DO.Drone> Drones = (List<IDAL.DO.Drone>)MyDal.AllDrones();
        List<IDAL.DO.Parcel> Parcels = (List<IDAL.DO.Parcel>)MyDal.AllParcels();
        List<IDAL.DO.Customer> Customers = (List<IDAL.DO.Customer>)MyDal.AllCustomers();
        List<IDAL.DO.BaseStation> BaseStations = (List<IDAL.DO.BaseStation>)MyDal.AllBaseStations();
        foreach (var item in Drones)
        {
            DroneToList myDrone = new();
            myDrone.Id = item.Id;
            myDrone.Model = item.Model;
            myDrone.Weight = (WeightCategories)item.MaxWeight;
            var myParcel = Parcels.Find(x => x.DroneId == myDrone.Id && x.Delivered == DateTime.MinValue);
            if (!myParcel.Equals(default(IDAL.DO.Parcel)))
            {
                myDrone.Status = DroneStatusCategories.Delivery;
                var Sender = Customers.Find(x => x.Id == myParcel.SenderId);
                Location SenderLocation = new() { Latitude = Sender.Lattitude, Longitude = Sender.Lattitude };
                if (myParcel.PickedUp == DateTime.MinValue)
                {
                    Location MyLocation = LocationFuncs.ClosestBaseStationLocation(BaseStations, SenderLocation);
                    myDrone.CurrentLocation = MyLocation;

                    var Reciever = Customers.Find(x => x.Id == myParcel.TargetId);
                    Location RecieverLocation = new Location(Reciever.Lattitude, Reciever.Longitude);
                    double distanceWithParcel = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, RecieverLocation);//מרחק בין רחפן ליעד
                    double distanceWithoutParcel = LocationFuncs.DistanceBetweenTwoLocations(RecieverLocation, LocationFuncs.ClosestBaseStationLocation(BaseStations, RecieverLocation));//מרחק בין יעד לתחנה קרובה
                    double minBattery = distanceWithoutParcel * BatteryUseFREE;// בלי חבילה עליו
                    if (myParcel.Weight == IDAL.DO.WeightCategories.Light)
                        minBattery += distanceWithParcel * BatteryUseLight;
                    else if (myParcel.Weight == IDAL.DO.WeightCategories.Medium)
                        minBattery += distanceWithParcel * BatteryUseMedium;
                    else if (myParcel.Weight == IDAL.DO.WeightCategories.Heavy)
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
                    IDAL.DO.BaseStation randomBS = BaseStations[rand.Next(BaseStations.Count)];
                    myDrone.CurrentLocation = new Location(randomBS.Lattitude, randomBS.Longitude);
                    myDrone.BatteryStatus = rand.NextDouble() * 20;
                }

                if (myDrone.Status == DroneStatusCategories.Free)
                {
                    List<IDAL.DO.Customer> Recievers = new();
                    foreach (var parcel in Parcels)
                    {
                        if (parcel.Delivered != DateTime.MinValue && Recievers.Find(x => x.Id == parcel.TargetId).Equals(default(IDAL.DO.Customer)))
                            Recievers.Add(Customers.Find(y => y.Id == parcel.TargetId));
                    }
                    if (Recievers.Count > 0)
                    {
                        IDAL.DO.Customer randomCustomer = Recievers[rand.Next(Recievers.Count)];
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
                myDrone.TransferdParcelsCount = Parcels.Where(x => x.DroneId == myDrone.Id).Count();
            }
                BlDrones.Add(myDrone);

        }
    }

    #region PARCEL FUNCS
    public void AddParcel(Parcel myParcel)
    {
        try
        {
            //  if ((int)myParcel.Weight > 3 || (int)myParcel.Weight < 1)
            //    throw new InvalidEnumValueException($"Invalid Weight Categories{(int)myParcel.Weight}");
            MyDal.FindCustomer(myParcel.Sender.Id);
            MyDal.FindCustomer(myParcel.Target.Id);
            myParcel.Requested = DateTime.Now;
            myParcel.Scheduled = myParcel.PickedUp = myParcel.Delivered = DateTime.MinValue;
            myParcel.DroneAtParcel = new DroneAtParcel();
            MyDal.AddParcel(Converter.ConvertBlParcelToDalParcel(myParcel));
        }
        catch (Exception ex)
        {
            throw new BlAddEntityException($"cannot add Parcel id:", ex);
        }
    }
    public Parcel FindParcel(int parcelId)
    {
        try
        {
            IDAL.DO.Parcel dalParcel = MyDal.FindParcel(parcelId);
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
            if (blParcel.Scheduled != DateTime.MinValue)
            {
                blParcel.DroneAtParcel = new DroneAtParcel()
                {
                    Id = dalParcel.DroneId,
                    BatteryStatus = BlDrones.Find(x => x.Id == dalParcel.DroneId).BatteryStatus,
                    CurrentLocation = BlDrones.Find(x => x.Id == dalParcel.DroneId).CurrentLocation
                };
            }
            return blParcel;
        }
        catch (Exception ex)
        {
            throw new BlFindItemException($"cannot Find parcel {parcelId}:", ex);
        }
    }
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
    public IEnumerable<ParcelToList> NoneScheduledParcels()
    {
        var myList = AllBlParcels().Where(x => x.Status == ParcelStatus.Defined).ToList();
        if(myList.Count == 0)
            throw new BlViewItemsListException("None-Scheduled Parcels list is empty");
        return myList;
    }
    #endregion
    #region BASE-STATION FUNCS
    public void AddBaseStation(BaseStation myBaseStation)
    {
        try
        {
            MyDal.AddBaseStation(Converter.ConvertBlBsToDalBs(myBaseStation));
        }
        catch (Exception ex)
        {
            throw new BlAddEntityException($"cannot add base station {myBaseStation.Id}:", ex);
        }
        myBaseStation.DronesInCharge = new List<DroneInCharge>();
    }
    public BaseStation FindBaseStation(int baseStationId)
    {
        try
        {
            IDAL.DO.BaseStation dalBs = MyDal.FindBaseStation(baseStationId);
            BaseStation blBs = new();
            blBs.DronesInCharge = new();
            blBs.Id = dalBs.Id;
            blBs.Name = dalBs.Name;
            blBs.BSLocation = new Location(dalBs.Lattitude, dalBs.Longitude);
            blBs.FreeChargeSlots = dalBs.FreeChargeSlots;
            List<IDAL.DO.DroneCharge> myList = MyDal.GetListOfInChargeDrones().Where(x => x.StationId == baseStationId).ToList();
            foreach (var item in myList)
            {
                blBs.DronesInCharge.Add(new DroneInCharge()
                {
                    Id = item.DroneId,
                    BatteryStatus = BlDrones.Find(x => x.Id == item.DroneId).BatteryStatus
                });
            }
            return blBs;
        }
        catch (Exception ex)
        {
            throw new BlUpdateEntityException($"cannot Find base station {baseStationId}:", ex);
        }

    }
    public IEnumerable<BaseStationToList> AllBlBaseStations()
    {
        List<BaseStationToList> myList = new();
        try
        {
            foreach (var item in MyDal.AllBaseStations())
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
            throw new BlViewItemsListException($"cannot show base stations list:", ex);
        }
        if (myList.Count == 0)
            throw new BlViewItemsListException("Base-station list is empty");
        return myList;
    }
    public IEnumerable<BaseStationToList> FreeSlotsBaseStations()
    {
        try
        {
            var myList = AllBlBaseStations().Where(x => x.FreeChargeSlots > 0).ToList();
            if (myList.Count == 0)
                throw new BlViewItemsListException("None-Scheduled Parcels list is empty");
            return myList;
        }
        catch (Exception ex)
        {
            throw new BlViewItemsListException($"cannot show Free Slots Base Stations list:", ex);
        }
    }
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
    public Customer FindCustomer(int customerId)
    {
        try
        {
            IDAL.DO.Customer dalCustomer = MyDal.FindCustomer(customerId);
            Customer blCustomer = new()
            {
                Id = dalCustomer.Id,
                Name = dalCustomer.Name,
                CustomerLocation = new Location(dalCustomer.Lattitude, dalCustomer.Longitude),
                PhoneNumber = dalCustomer.Phone
            };
            foreach (var item in MyDal.AllParcels().Where(x => x.SenderId == customerId))
            {
                blCustomer.ParcelFromCustomerList = new List<ParcelAtCustomer>();
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
            foreach (var item in MyDal.AllParcels().Where(x => x.TargetId == customerId))
            {
                blCustomer.ParcelToCustomerList = new List<ParcelAtCustomer>();
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
            return blCustomer;
        }
        catch (Exception ex)
        {
            throw new BlFindItemException($"cannot find customer {customerId}:", ex);
        }
    }
    public IEnumerable<CustomerToList> AllBlCustomers()
    {
        List<CustomerToList> myList = new();
        try
        {
            foreach (var item in MyDal.AllCustomers())
                myList.Add(new CustomerToList()
                {
                    Id = item.Id,
                    Name = item.Name,
                    PhoneNumber = item.Phone,
                    SentAndSuppliedParcels = MyDal.AllParcels().Where(x => x.SenderId == item.Id && x.Delivered != DateTime.MinValue).Count(),
                    SentAndNotSuppliedParcels = MyDal.AllParcels().Where(x => x.SenderId == item.Id && x.Delivered == DateTime.MinValue).Count(),
                    RecievedParcels = MyDal.AllParcels().Where(x => x.TargetId == item.Id && x.Delivered != DateTime.MinValue).Count(),
                    InProcessParcelsToCustomer = MyDal.AllParcels().Where(x => x.TargetId == item.Id && x.Delivered == DateTime.MinValue).Count(),
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
    public void UpdateCustomer(int customerId, string newName, string newPhone)
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
    public void AddDrone(DroneToList myDrone, int baseStationId)
    {
        try
        {
            IDAL.DO.BaseStation bs = MyDal.FindBaseStation(baseStationId);
            if (bs.Equals(null))
                throw new BaseStationBLException($"Base station {baseStationId} doesn't found");
            else
            {
                Random rand = new();
                myDrone.BatteryStatus = rand.NextDouble() * 20 + 20;
                myDrone.Status = DroneStatusCategories.Maintenance;
                myDrone.CurrentLocation = new Location(bs.Lattitude, bs.Longitude);
                myDrone.TransferdParcelsCount = 0;
                MyDal.AddDrone(Converter.ConvertBlDroneToDalDrone(myDrone));
                BlDrones.Add(myDrone);
            }
        }
        catch (Exception ex)
        {
            throw new BlAddEntityException($"cannot add drone id:", ex);
        }
    }
    public Drone FindDrone(int droneId)
    {
        try
        {
            DroneToList myDrone = BlDrones.Find(x => x.Id == droneId);
            Drone drone = new()
            {
                Id = myDrone.Id,
                Model = myDrone.Model,
                Weight = myDrone.Weight,
                BatteryStatus = myDrone.BatteryStatus,
                Status = myDrone.Status,
                CurrentLocation = myDrone.CurrentLocation
            };
            if (drone.Status == DroneStatusCategories.Delivery)
            {
                IDAL.DO.Parcel dalParcel = MyDal.AllParcels().First(x => x.DroneId == droneId && x.Delivered == DateTime.MinValue);
                ParcelInTransfer parcelInTransfer = new()
                {
                    Id = dalParcel.Id,
                    Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = MyDal.FindCustomer(dalParcel.SenderId).Name },
                    Reciever = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = MyDal.FindCustomer(dalParcel.TargetId).Name },
                    Weight = (WeightCategories)dalParcel.Weight,
                    CollectionLocation = new() { Latitude = MyDal.FindCustomer(dalParcel.SenderId).Lattitude, Longitude = MyDal.FindCustomer(dalParcel.SenderId).Longitude },
                    DeliveryDestinationLocation = new() { Latitude = MyDal.FindCustomer(dalParcel.TargetId).Lattitude, Longitude = MyDal.FindCustomer(dalParcel.TargetId).Longitude },
                    Priority = (Priorities)dalParcel.Priority,
                };
                parcelInTransfer.TransportDistance = LocationFuncs.DistanceBetweenTwoLocations(parcelInTransfer.CollectionLocation, parcelInTransfer.DeliveryDestinationLocation);
                if (dalParcel.PickedUp == DateTime.MinValue)
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
    public IEnumerable<DroneToList> AllBlDrones()
    {
        return BlDrones;
    }
    public void UpdateDroneModel(int droneId, string newName)
    {
        try
        {
            MyDal.UpdateDroneModel(droneId, newName);
            BlDrones.Find(x => x.Id == droneId).Model = newName;
        }
        catch(Exception ex)
        {
            throw new BlUpdateEntityException($"cannot update drone {droneId}:", ex);
        }

    }
    public void ChargeDrone(int droneId)
    {
        try
        {
            if (BlDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Free)
                throw new BlUpdateEntityException("The chosen drone isn't free");
            else
            {
                DroneToList myDrone = BlDrones.Find(x => x.Id == droneId);
                List<IDAL.DO.BaseStation> dalBsList = MyDal.AllBaseStations().OrderBy(x => LocationFuncs.DistanceBetweenTwoLocations(new Location(x.Lattitude, x.Longitude), (myDrone.CurrentLocation))).ToList();
                foreach (var baseStation in dalBsList)
                {
                    double batteryNeed = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, new Location(baseStation.Lattitude, baseStation.Longitude)) * BatteryUseFREE;
                    if (myDrone.BatteryStatus > batteryNeed)
                        if (baseStation.FreeChargeSlots > 0)
                        {
                            MyDal.ChargeDrone(myDrone.Id, baseStation.Id);
                            BlDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Maintenance;
                            BlDrones.Find(x => x.Id == droneId).BatteryStatus -= batteryNeed;
                            BlDrones.Find(x => x.Id == droneId).CurrentLocation = new Location(baseStation.Lattitude, baseStation.Longitude);
                            return;
                        }
                        else
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
    public void releaseDrone(int droneId, TimeSpan tSpanInCharge)
    {
        try
        {
            if (BlDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Maintenance)
                throw new BlUpdateEntityException("The chosen drone isn't in charge");
            MyDal.ReleaseDroneFromCharge(droneId);
            BlDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Free;
            BlDrones.Find(x => x.Id == droneId).BatteryStatus = Math.Min(BlDrones.Find(x => x.Id == droneId).BatteryStatus + tSpanInCharge.TotalHours * BatteryChargeRate, 100.0);
        }
        catch (Exception ex)
        {
            throw new BlUpdateEntityException($"cannot release drone {droneId}:", ex);
        }
    }
    public void ScheduleDroneForParcel(int droneId)
    {
        try
        {
            MyDal.FindDrone(droneId);
            DroneToList myDrone = BlDrones.Find(x => x.Id == droneId);
            if (BlDrones.Find(x => x.Id == droneId).Status != DroneStatusCategories.Free)
                throw new BlUpdateEntityException($"Drone {droneId} isn't free");
            else
            {
                List<IDAL.DO.Parcel> parcels = MyDal.NoneScheduledParcels().OrderByDescending(x => x.Priority).Where(x => x.Weight <= (IDAL.DO.WeightCategories)myDrone.Weight).OrderByDescending(x => x.Weight).ToList();
                parcels = parcels.OrderBy(x => LocationFuncs.DistanceBetweenTwoLocations(new Location() { Latitude = MyDal.FindCustomer(x.SenderId).Lattitude, Longitude = MyDal.FindCustomer(x.SenderId).Longitude }, myDrone.CurrentLocation)).ToList();
                if (parcels.Count > 0)
                {
                    foreach (var item in parcels)
                    {
                        ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.AllCustomers().ToList());
                        //Location sourceLocation = new Location() { Latitude = MyDal.FindCustomer(item.SenderId).Lattitude, Longitude = MyDal.FindCustomer(item.SenderId).Longitude };
                        //Location targetLocation = new Location() { Latitude = MyDal.FindCustomer(item.TargetId).Lattitude, Longitude = MyDal.FindCustomer(item.TargetId).Longitude };
                        double distanceToSource = LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, myParcel.CollectionLocation);
                        double distanceToBase = LocationFuncs.DistanceBetweenTwoLocations(myParcel.DeliveryDestinationLocation, LocationFuncs.ClosestBaseStationLocation(MyDal.AllBaseStations().ToList(), myParcel.DeliveryDestinationLocation));
                        double batteryNeedToBase = BatteryUseFREE * (distanceToSource + distanceToBase);
                        if (item.Weight == IDAL.DO.WeightCategories.Light)
                            batteryNeedToBase += myParcel.TransportDistance * BatteryUseLight;
                        else if (item.Weight == IDAL.DO.WeightCategories.Medium)
                            batteryNeedToBase += myParcel.TransportDistance * BatteryUseMedium;
                        else if (item.Weight == IDAL.DO.WeightCategories.Heavy)
                            batteryNeedToBase += myParcel.TransportDistance * BatteryUseHeavy;
                        if (batteryNeedToBase < myDrone.BatteryStatus)
                        {
                            MyDal.ScheduleDroneForParcel(item.Id, myDrone.Id);
                            BlDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Delivery;
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
    public void PickingUpAParcel(int droneId)
    {
        try
        {
            MyDal.FindDrone(droneId);
            foreach (var item in MyDal.AllParcels())
            {
                if (item.DroneId == droneId && item.Scheduled != DateTime.MinValue && item.PickedUp == DateTime.MinValue)
                {
                    DroneToList myDrone = BlDrones.Find(x => x.Id == droneId);
                    ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.AllCustomers().ToList());
                    MyDal.PickingUpAParcel(item.Id);
                    BlDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseFREE * LocationFuncs.DistanceBetweenTwoLocations(myDrone.CurrentLocation, myParcel.CollectionLocation);
                    BlDrones.Find(x => x.Id == droneId).CurrentLocation = myParcel.CollectionLocation;
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
    public void DeliverAParcel(int droneId)
    {
        try
        {
            MyDal.FindDrone(droneId);
            foreach (var item in MyDal.AllParcels())
            {
                if (item.DroneId == droneId && item.PickedUp != DateTime.MinValue && item.Delivered == DateTime.MinValue)
                {
                    DroneToList myDrone = BlDrones.Find(x => x.Id == droneId);
                    ParcelInTransfer myParcel = Converter.ConvertDalParcelToBlParcelInTranfer(item, MyDal.AllCustomers().ToList());
                    MyDal.DeliverAParcel(item.Id);
                    if(myParcel.Weight == WeightCategories.Light)
                        BlDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseLight * myParcel.TransportDistance;
                    else if(myParcel.Weight == WeightCategories.Medium)
                        BlDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseMedium * myParcel.TransportDistance;
                    else if (myParcel.Weight == WeightCategories.Heavy)
                        BlDrones.Find(x => x.Id == droneId).BatteryStatus -= BatteryUseHeavy * myParcel.TransportDistance;
                    BlDrones.Find(x => x.Id == droneId).CurrentLocation = myParcel.DeliveryDestinationLocation;
                    BlDrones.Find(x => x.Id == droneId).Status = DroneStatusCategories.Free;
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

