using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Dal
{
    public class DalXml : IDal
    {
        #region thread-safe DalXml singleton
        private DalXml() { }
        static readonly object padlock = new object();
        static DalXml instance = null;
        internal static DalXml Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new DalXml();
                    }
                    return instance;
                }
            }
        }
        #endregion

        internal class Config
        {
            internal static int NewParcelId = 100000;
            internal static double BatteryUseFREE { get { return 0.5; } }
            internal static double BatteryUseLight { get { return 2.5; } }
            internal static double BatteryUseMedium { get { return 3.5; } }
            internal static double BatteryUseHeavy { get { return 5.5; } }
            internal static double BatteryChargeRate { get { return 20.5; } }
        }

        public double[] GetBatteryUse()
        {
            return new double[5] { Config.BatteryUseFREE, Config.BatteryUseLight,
                Config.BatteryUseMedium, Config.BatteryUseHeavy, Config.BatteryChargeRate};
        }
        #region DS XML Files

        private readonly string parcelsPath = @"ParcelsXml.xml"; //XMLSerializer
        private readonly string baseStationsPath = @"BaseStationsXml.xml"; //XMLSerializer
        private readonly string customersPath = @"CustomersXml.xml"; //XElement
        private readonly string dronesPath = @"DronesXml.xml"; //XMLSerializer
        private readonly string dronesInChargePath = @"DronesInChargeXml.xml"; //XMLSerializer

        #endregion
        #region done
        public void AddBaseStation(BaseStation newBaseStation)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            if (!baseStationsList.FirstOrDefault(b => b.Id == newBaseStation.Id).Equals(default(BaseStation)))
                throw new DO.BaseStationException($"Duplicate Base Station Id {newBaseStation.Id}");
            baseStationsList.Add(newBaseStation);
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }

        public void AddCustomer(Customer newCustomer)
        {
            /*
            List<Customer> customersList = XMLTools.LoadListFromXMLSerializer<Customer>(customersPath);
            if (!customersList.FirstOrDefault(c => c.Id == newCustomer.Id).Equals(default(Customer)))
                throw new DO.CustomerException($"Duplicate Customer Id {newCustomer.Id}");
            customersList.Add(newCustomer);
            XMLTools.SaveListToXMLSerializer(customersList, customersPath);*/
            XElement customersRootElem = XMLTools.LoadListFromXMLElement(customersPath);

            XElement c1 = (from c in customersRootElem.Elements()
                             where int.Parse(c.Element("Id").Value) == newCustomer.Id
                             select c)
                             .FirstOrDefault();

            if (c1 != null)
                throw new DO.CustomerException($"Duplicate Customer Id {newCustomer.Id}");

            XElement customerElem = new XElement("Customer", new XElement("Id", newCustomer.Id),
                                  new XElement("Name", newCustomer.Name),
                                  new XElement("Phone", newCustomer.Phone),
                                  new XElement("Lattitude", newCustomer.Lattitude),
                                  new XElement("Longitude", newCustomer.Longitude),
                                  new XElement("IsActive", true));
           
            customersRootElem.Add(customerElem);
            XMLTools.SaveListToXMLElement(customersRootElem, customersPath);
        }

        public void AddDrone(Drone newDrone)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            if (!dronesList.FirstOrDefault(d => d.Id == newDrone.Id).Equals(default(Drone)))
                throw new DO.DroneException($"Duplicate Drone Id {newDrone.Id}");
            dronesList.Add(newDrone);
            XMLTools.SaveListToXMLSerializer(dronesList, dronesPath);
        }

        public void AddParcel(Parcel newParcel)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.FirstOrDefault(p => p.Id == newParcel.Id).Equals(default(Parcel)))
                throw new DO.ParcelException($"Duplicate Parcel Id {newParcel.Id}");
            parcelsList.Add(newParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }

        public IEnumerable<BaseStation> AllBaseStations(Func<BaseStation, bool> predicate = null)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            if (predicate == null)
                return from bs in baseStationsList
                       where bs.IsActive is true
                       select bs;
            else
                return from bs in baseStationsList
                       where bs.IsActive is true
                       where predicate(bs)
                       select bs;
        }

        public IEnumerable<Customer> AllCustomers(Func<Customer, bool> predicate = null)
        {
            /*  List<Customer> customersList = XMLTools.LoadListFromXMLSerializer<Customer>(customersPath);
              if (predicate == null)
                  return from customer in customersList
                         where customer.IsActive is true
                         select customer;
              else
                  return from customer in customersList
                         where customer.IsActive is true
                         where predicate(customer)
                         select customer;*/
            XElement customersRootElem = XMLTools.LoadListFromXMLElement(customersPath);
            if (predicate == null)
                return (from c in customersRootElem.Elements()
                    let c1 = new Customer()
                    {
                        Id = Int32.Parse(c.Element("Id").Value),
                        Name = c.Element("Name").Value,
                        Phone = c.Element("Phone").Value,
                        Lattitude = double.Parse(c.Element("Lattitude").Value),
                        Longitude = double.Parse(c.Element("Longitude").Value),
                        IsActive = bool.Parse(c.Element("IsActive").Value)
                    }
                    where c1.IsActive is true
                    select c1
                   );
            else
                return (from c in customersRootElem.Elements()
                        let c1 = new Customer()
                        {
                            Id = Int32.Parse(c.Element("Id").Value),
                            Name = c.Element("Name").Value,
                            Phone = c.Element("Phone").Value,
                            Lattitude = double.Parse(c.Element("Lattitude").Value),
                            Longitude = double.Parse(c.Element("Longitude").Value),
                            IsActive = bool.Parse(c.Element("IsActive").Value)
                        }
                        where predicate(c1)
                        where c1.IsActive is true
                        select c1
                   );
        }

        public IEnumerable<Drone> AllDrones()
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            return from drone in dronesList
                   select drone;
        }

        public IEnumerable<Parcel> AllParcels(Func<Parcel, bool> predicate = null)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if(predicate == null)
                return from parcel in parcelsList
                       select parcel;
            else
                return from parcel in parcelsList
                       where predicate(parcel)
                       select parcel;
        }
        
        public BaseStation FindBaseStation(int id)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            BaseStation bs = baseStationsList.Find(b => b.Id == id && b.IsActive == true);
            if (!bs.Equals(default(BaseStation)))
                return bs; 
            else
                throw new DO.BaseStationException($"Missing Base-Station id: {id}");
        }

        public Customer FindCustomer(int? id)
        {
            /* List<Customer> customersList = XMLTools.LoadListFromXMLSerializer<Customer>(customersPath);
            Customer customer = customersList.Find(c => c.Id == id && c.IsActive == true);
            if (!customer.Equals(default(Customer)))
                return customer;
            else
                throw new DO.CustomerException($"Missing Customer id: {id}");*/
            XElement customersRootElem = XMLTools.LoadListFromXMLElement(customersPath);

            Customer? customer = (from c in customersRootElem.Elements()
                        where int.Parse(c.Element("Id").Value) == id
                        select new Customer()
                        {
                            Id = Int32.Parse(c.Element("Id").Value),
                            Name = c.Element("Name").Value,
                            Phone = c.Element("Phone").Value,
                            Lattitude = double.Parse(c.Element("Lattitude").Value),
                            Longitude = double.Parse(c.Element("Longitude").Value),
                            IsActive = bool.Parse(c.Element("IsActive").Value)
                        }
                        ).FirstOrDefault();

            if (customer == null)
                throw new DO.CustomerException($"Missing Customer id: {id}");

            return (Customer)customer;

        }

        public Drone FindDrone(int? id)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            Drone drone = dronesList.Find(d => d.Id == id);
            if (!drone.Equals(default(Drone)))
                return drone;
            else
                throw new DO.DroneException($"Missing Drone id: {id}");
        }

        public Parcel FindParcel(int id)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            Parcel parcel = parcelsList.Find(p => p.Id == id);
            if (!parcel.Equals(default(Parcel)))
                return parcel;
            else
                throw new DO.ParcelException($"Missing Parcel id: {id}");
        }

        public void DeleteBaseStation(int id)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            BaseStation? bs = FindBaseStation(id);
            if (bs != null)
            {
                BaseStation inActiveBS = (BaseStation)bs;
                baseStationsList.Remove((BaseStation)bs);
                inActiveBS.IsActive = false;
                baseStationsList.Add(inActiveBS);
            }
            else
                throw new DO.BaseStationException($"Missing Base Station Id {id}");
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }
        
        public void DeleteDrone(int? id)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            Drone? drone = dronesList.Find(d => d.Id == id);
            if (drone != null)
            {
                dronesList.Remove((Drone)drone);
            }
            else
                throw new DO.ParcelException($"Missing Drone Id {id}");
            XMLTools.SaveListToXMLSerializer(dronesList, dronesPath);
        }

        public void DeleteCustomer(int id)
        {
            /*List<Customer> customersList = XMLTools.LoadListFromXMLSerializer<Customer>(customersPath);
            Customer? customer = FindCustomer(id);
            if (customer != null)
            {
                Customer inActiveC = (Customer)customer;
                customersList.Remove((Customer)customer);
                inActiveC.IsActive = false;
                customersList.Add(inActiveC);
            }
            else
                throw new DO.CustomerException($"Missing Customer Id {id}");
            XMLTools.SaveListToXMLSerializer(customersList, customersPath);*/
            XElement customersRootElem = XMLTools.LoadListFromXMLElement(customersPath);

            XElement customer = (from c in customersRootElem.Elements()
                            where int.Parse(c.Element("Id").Value) == id
                            where bool.Parse(c.Element("IsActive").Value) is true
                            select c).FirstOrDefault();

            if (customer != null)
            {
                customer.Element("IsActive").Value = "false"; //<==>   Inactive customer from customersRootElem
                XMLTools.SaveListToXMLElement(customersRootElem, customersPath);
            }
            else
                throw new DO.CustomerException($"Missing Customer Id {id}");
        }

        public void DeleteParcel(int id)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            Parcel? parcel = parcelsList.Find(p => p.Id == id);
            if (parcel != null)
            {
                parcelsList.Remove((Parcel)parcel);
            }
            else
                throw new DO.ParcelException($"Missing Parcel Id {id}");
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }

        public void ChargeDrone(int? droneId, int baseStationId)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            List<DroneCharge> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            if (!dronesList.Exists(d => d.Id == droneId))
                throw new DroneException($"Drone {droneId} doesn't exist");
            if (!baseStationsList.Exists(b => b.Id == baseStationId && b.IsActive))
                throw new BaseStationException($"Base Station {baseStationId} doesn't exist");
            dronesInChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
            BaseStation bs = FindBaseStation(baseStationId);
            BaseStation updatedBS = bs;
            baseStationsList.Remove(bs);
            updatedBS.FreeChargeSlots--;
            baseStationsList.Add(updatedBS);
            XMLTools.SaveListToXMLSerializer(dronesInChargeList, dronesInChargePath);
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }

        public void ReleaseDroneFromCharge(int? droneId)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            List<DroneCharge> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            if (!dronesList.Exists(d => d.Id == droneId))
                throw new DroneException($"Drone {droneId} doesn't exist");
            DroneCharge dc = dronesInChargeList.Find(dc => dc.DroneId == droneId);
            BaseStation bs = FindBaseStation(dc.StationId);
            BaseStation updatedBS = bs;
            dronesInChargeList.Remove(dc);
            baseStationsList.Remove(bs);
            updatedBS.FreeChargeSlots++;
            baseStationsList.Add(updatedBS);
            XMLTools.SaveListToXMLSerializer(dronesInChargeList, dronesInChargePath);
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }

        public void DeliverAParcel(int parcelId)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.Exists(p => p.Id == parcelId))
                throw new ParcelException($"Parcel {parcelId} doesn't exist");
            Parcel parcel = FindParcel(parcelId);
            Parcel updatedParcel = parcel;
            parcelsList.Remove(parcel);
            updatedParcel.Delivered = DateTime.Now;
            parcelsList.Add(updatedParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }

        public void UpdateCustomer(int? customerId, string newName, string newPhone)
        {
            /*List<Customer> customersList = XMLTools.LoadListFromXMLSerializer<Customer>(customersPath);
            Customer? customer = customersList.Find(c => c.Id == customerId);
            if (customer != null)
            {
                Customer newCustomer = (Customer)customer;
                customersList.Remove((Customer)customer);
                if (!string.IsNullOrEmpty(newName)) newCustomer.Name = newName;
                if (!string.IsNullOrEmpty(newPhone)) newCustomer.Phone = newPhone;
                customersList.Add(newCustomer);
            }
            else
                throw new DO.CustomerException($"Missing Customer Id {customerId}");
            XMLTools.SaveListToXMLSerializer(customersList, customersPath);*/
            XElement customersRootElem = XMLTools.LoadListFromXMLElement(customersPath);

            XElement customer = (from c in customersRootElem.Elements()
                            where int.Parse(c.Element("Id").Value) == customerId
                            where bool.Parse(c.Element("IsActive").Value) is true
                            select c).FirstOrDefault();

            if (customer != null)
            {
                if (!string.IsNullOrEmpty(newName)) customer.Element("Name").Value = newName;
                if (!string.IsNullOrEmpty(newPhone)) customer.Element("Phone").Value = newPhone;
                XMLTools.SaveListToXMLElement(customersRootElem, customersPath);
            }
            else
                throw new DO.CustomerException($"Missing Customer Id {customerId}");
        }

        public void UpdateDroneModel(int? droneId, string newName)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            Drone? drone = dronesList.Find(d => d.Id == droneId);
            if (drone != null)
            {
                Drone newDrone = (Drone)drone;
                dronesList.Remove((Drone)drone);
                if (!string.IsNullOrEmpty(newName)) newDrone.Model = newName;
                dronesList.Add(newDrone);
            }
            else
                throw new DO.DroneException($"Missing Drone Id {droneId}");

            XMLTools.SaveListToXMLSerializer(dronesList, dronesPath);
        }

        public void UpdateBaseStation(int baseStationId, string newName, int slotsCount)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            List<DroneCharge> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            BaseStation? bs = baseStationsList.Find(b => b.Id == baseStationId);
            if (bs != null)
            {
                BaseStation newBS = (BaseStation)bs;
                baseStationsList.Remove((BaseStation)bs);
                if (!string.IsNullOrEmpty(newName)) newBS.Name = newName;
                if (slotsCount != 0)
                    newBS.FreeChargeSlots =  slotsCount - dronesInChargeList.Where(x => x.StationId == baseStationId).Count(); ;
                baseStationsList.Add(newBS);
            }
            else
                throw new DO.BaseStationException($"Missing Customer Base-Station {baseStationId}");

            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }

        #endregion



        public IEnumerable<BaseStation> FreeSlotsBaseStations()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Parcel> NoneScheduledParcels()
        {
            throw new NotImplementedException();
        }

        public void PickingUpAParcel(int parcelId)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.Exists(p => p.Id == parcelId))
                throw new ParcelException($"Parcel {parcelId} doesn't exist");
            Parcel parcel = FindParcel(parcelId);
            Parcel updatedParcel = parcel;
            parcelsList.Remove(parcel);
            updatedParcel.PickedUp = DateTime.Now;
            parcelsList.Add(updatedParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }

        public void ScheduleDroneForParcel(int parcelId, int? droneId)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.Exists(p => p.Id == parcelId))
                throw new ParcelException($"Parcel {parcelId} doesn't exist");
            if (!dronesList.Exists(d => d.Id == droneId))
                throw new DroneException($"Drone {droneId} doesn't exist");
            Parcel parcel = FindParcel(parcelId);
            Parcel updatedParcel = parcel;
            parcelsList.Remove(parcel);
            updatedParcel.DroneId = droneId;
            updatedParcel.Scheduled = DateTime.Now;
            parcelsList.Add(updatedParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }

        public IEnumerable<DroneCharge> GetListOfInChargeDrones()
        {
            List<DroneCharge> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            return from dc in dronesInChargeList
                   select dc;
        }
        
    }
}
