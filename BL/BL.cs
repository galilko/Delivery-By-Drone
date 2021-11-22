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
                    Location MyLocation = ClosestBaseStationLocation(BaseStations, SenderLocation);
                    myDrone.CurrentLocation = MyLocation;

                    var Reciever = Customers.Find(x => x.Id == myParcel.TargetId);
                    Location RecieverLocation = new Location(Reciever.Lattitude, Reciever.Longitude);
                    double distanceWithParcel = myDrone.CurrentLocation.CalcDistance(RecieverLocation);//מרחק בין רחפן ליעד
                    double distanceWithoutParcel = RecieverLocation.CalcDistance(ClosestBaseStationLocation(BaseStations, RecieverLocation));//מרחק בין יעד לתחנה קרובה
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
     
                        IDAL.DO.Customer randomCustomer = Recievers[rand.Next(Recievers.Count)];
                        myDrone.CurrentLocation = new Location(randomCustomer.Lattitude, randomCustomer.Longitude);

                        double distanceWithoutParcel = myDrone.CurrentLocation.CalcDistance(ClosestBaseStationLocation(BaseStations, myDrone.CurrentLocation));//מרחק בין יעד לתחנה קרובה
                        double minBattery = distanceWithoutParcel * BatteryUseFREE;// בלי חבילה עליו
                        myDrone.BatteryStatus = rand.NextDouble() * (100 - minBattery) + minBattery;

                }
                myDrone.TransferdParcelsCount = Parcels.Where(x => x.DroneId == myDrone.Id).Count();
            }
                BlDrones.Add(myDrone);

        }
    }
    private static Location ClosestBaseStationLocation(List<IDAL.DO.BaseStation> BaseStations, Location MyLocation)
    {
        Location ClosestLocation = new Location();
        double MinDistance = double.MaxValue;
        foreach (var bs in BaseStations)
        {
            Location BSLocation = new() { Latitude = bs.Lattitude, Longitude = bs.Longitude };
            if (BSLocation.CalcDistance(MyLocation) < MinDistance)
            {
                MinDistance = BSLocation.CalcDistance(MyLocation);
                ClosestLocation = BSLocation;
            }
        }
        return ClosestLocation;
    }
    public void AddBaseStation(BaseStation myBaseStation)
    {
        try
        {
            myBaseStation.DronesInCharge = new List<DroneInCharge>();
            MyDal.AddBaseStation(Converter.ConvertBlBsToDalBs(myBaseStation));
        }
        catch (Exception ex)
        {
            throw new BlAddEntityException($"cannot add base station id:", ex);
        }
    }

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

    public void AddCustomer(Customer myCustomer)
    {
        try
        {
            MyDal.AddCustomer(Converter.ConvertBlCustomerToDalCustomer(myCustomer));
        }
        catch (Exception ex)
        {
            throw new BlAddEntityException($"cannot add customer id:", ex);
        }
    }

    public void AddParcel(Parcel myParcel)
    {
        try
        {
            myParcel.Requested = DateTime.Now;
            myParcel.Scheduled = myParcel.PickedUp = myParcel.Delivered = DateTime.MinValue;
            myParcel.DroneAtParcel = new DroneAtParcel();
            MyDal.AddParcel(Converter.ConvertBlParcelToDalParcel(myParcel));
        }
        catch (Exception ex)
        {
            throw new BlAddEntityException($"cannot add parcel id:", ex);
        }
    }

    public IEnumerable<BaseStationToList> AllBlBaseStations()
    { 
        List<BaseStationToList> myList = new();
        foreach (var item in MyDal.AllBaseStations())
            myList.Add(new BaseStationToList()
            {
                Id = item.Id,
                Name = item.Name,
                FreeChargeSlots = item.FreeChargeSlots,
                BusyChargeSlots = 0
            });
        return myList;
    }

    public IEnumerable<DroneToList> AllBlDrones()
    {
        return BlDrones;
    }

    public IEnumerable<CustomerToList> AllBlCustomers()
    {
        List<CustomerToList> myList = new();
        foreach (var item in MyDal.AllCustomers())
            myList.Add(new CustomerToList()
            {
                Id = item.Id,
                Name = item.Name,
                PhoneNumber = item.Phone,
                SentAndSuppliedParcels = MyDal.AllParcels().Where(x=>x.SenderId == item.Id && x.Delivered != DateTime.MinValue).Count(),
                SentAndNotSuppliedParcels = MyDal.AllParcels().Where(x=>x.SenderId == item.Id && x.Delivered == DateTime.MinValue).Count(),
                RecievedParcels = MyDal.AllParcels().Where(x => x.TargetId == item.Id && x.Delivered != DateTime.MinValue).Count(),
                InProcessParcelsToCustomer = MyDal.AllParcels().Where(x => x.TargetId == item.Id && x.Delivered == DateTime.MinValue).Count(),
            });
        return myList;
    }

    public IEnumerable<ParcelToList> AllBlParcels()
    {
        List<ParcelToList> myList = new();
        foreach (var item in MyDal.AllParcels())
            myList.Add(new ParcelToList()
            {
                Id = item.Id,
                SenderName = (MyDal.AllCustomers().First(x=>x.Id == item.SenderId)).Name,
                TargetName = (MyDal.AllCustomers().First(x=>x.Id == item.TargetId)).Name,
                Weight = (WeightCategories)item.Weight,
                Priority = (Priorities)item.Priority,
                Status = CalculateParcelStatus(item),
            });
        return myList;
    }

    private ParcelStatus CalculateParcelStatus(IDAL.DO.Parcel parcel)
    {
        if (parcel.Delivered != DateTime.MinValue)
            return ParcelStatus.Delivered;
        else if (parcel.PickedUp != DateTime.MinValue)
            return ParcelStatus.PickedUp;
        else if (parcel.Scheduled != DateTime.MinValue)
            return ParcelStatus.Scheduled;
        else
            return ParcelStatus.Defined;
    }

    public IEnumerable<ParcelToList> NoneScheduledParcels()
    {
        return AllBlParcels().Where(x => x.Status == ParcelStatus.Defined).ToList();
    }

    public IEnumerable<BaseStationToList> FreeSlotsBaseStations()
    {
        return AllBlBaseStations().Where(x => x.FreeChargeSlots > 0).ToList();
    }
    public void ChangeModelDrone(int DroneId , String model)
    {
        try
        {
            MyDal.ChangeModelDrone(DroneId, model);
        }
        catch(Exception ex)
        {
            throw new BlAddEntityException($"cannot update drone id:", ex);
        }
    }

    /*public BaseStation FindBaseStation(int baseStationId)
    {
        IDAL.DO.BaseStation dalBs = MyDal.FindBaseStation(baseStationId);
        BaseStation blBs = new();
        blBs.Id = dalBs.Id;
        blBs.Name = dalBs.Name;
        blBs.BSLocation = new Location(dalBs.Lattitude, dalBs.Longitude);
        blBs.FreeChargeSlots = dalBs.FreeChargeSlots;
        blBs.DronesInCharge = new List<DroneInCharge>
    }*/
}

