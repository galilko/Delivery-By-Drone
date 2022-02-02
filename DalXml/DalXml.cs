using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Dal
{
    public class DalXml : IDal
    {
        #region thread-safe DalXml singleton
        private DalXml()
        {
            List<DroneCharge> dronesInCharge = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            List<BaseStation> baseStations = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            foreach (var dc in dronesInCharge)
            {
                BaseStation? baseStation = baseStations.Find(bs => bs.Id == dc.StationId);
                if (baseStation != null)
                {
                    BaseStation updatedBS = (BaseStation)baseStation;
                    baseStations.Remove((BaseStation)baseStation);
                    updatedBS.FreeChargeSlots++;
                    baseStations.Add(updatedBS);
                }
            }
            dronesInCharge.Clear();
            XMLTools.SaveListToXMLSerializer(dronesInCharge, dronesInChargePath);
            XMLTools.SaveListToXMLSerializer(baseStations, baseStationsPath);
        }
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


       
        #region DS XML Files

        private readonly string parcelsPath = @"ParcelsXml.xml"; //XMLSerializer
        private readonly string baseStationsPath = @"BaseStationsXml.xml"; //XMLSerializer
        private readonly string customersPath = @"CustomersXml.xml"; //XElement
        private readonly string dronesPath = @"DronesXml.xml"; //XMLSerializer
        private readonly string dronesInChargePath = @"DronesInChargeXml.xml"; //XMLSerializer
        private readonly string configPath = @"Config.xml"; //XMLSerializer

        #endregion

        public double[] GetBatteryUsages()
        {
            return XMLTools.LoadListFromXMLElement(configPath).Element("BatteryUsages").Elements()
               .Select(e => Convert.ToDouble(e.Value)).ToArray();
            //return new double[5] { Config.BatteryUseFREE, Config.BatteryUseLight,
            //  Config.BatteryUseMedium, Config.BatteryUseHeavy, Config.BatteryChargeRate};
        }

        #region ADD
        public void AddBaseStation(BaseStation newBaseStation)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            if (!baseStationsList.FirstOrDefault(b => b.Id == newBaseStation.Id).Equals(default(BaseStation)))
                throw new DO.ExistIdException($"Duplicate Base Station Id {newBaseStation.Id}");
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
                throw new DO.ExistIdException($"Duplicate Customer Id {newCustomer.Id}");

            XElement customerElem = new XElement("Customer", new XElement("Id", newCustomer.Id),
                                  new XElement("Name", newCustomer.Name),
                                  new XElement("Phone", newCustomer.Phone),
                                  new XElement("Latitude", newCustomer.Latitude),
                                  new XElement("Longitude", newCustomer.Longitude),
                                  new XElement("IsActive", true));
           
            customersRootElem.Add(customerElem);
            XMLTools.SaveListToXMLElement(customersRootElem, customersPath);
        }

        public void AddDrone(Drone newDrone)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            if (!dronesList.FirstOrDefault(d => d.Id == newDrone.Id).Equals(default(Drone)))
                throw new DO.ExistIdException($"Duplicate Drone Id {newDrone.Id}");
            dronesList.Add(newDrone);
            XMLTools.SaveListToXMLSerializer(dronesList, dronesPath);
        }

        public void AddParcel(Parcel newParcel)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.FirstOrDefault(p => p.Id == newParcel.Id).Equals(default(Parcel)))
                throw new DO.ExistIdException($"Duplicate Parcel Id {newParcel.Id}");
            XElement t = XElement.Load(configPath);
            int id = Convert.ToInt32(t.Element("NewParcelId").Value);
            t.Element("NewParcelId").Value = (id + 1).ToString(); 
            newParcel.Id = id;
            parcelsList.Add(newParcel);
            t.Save(configPath);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }
        #endregion

        #region Request (Get) collections
        public IEnumerable<BaseStation?> GetBaseStations(Func<BaseStation?, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXMLSerializer<BaseStation?>(baseStationsPath).Where(bs => bs?.IsActive == true).OrderBy(x => x?.Id) :
                XMLTools.LoadListFromXMLSerializer<BaseStation?>(baseStationsPath).Where(predicate).Where(bs => bs?.IsActive == true).OrderBy(x => x?.Id);
        public IEnumerable<Customer> GetCustomers(Func<Customer, bool> predicate = null)
        {
            XElement customersRootElem = XMLTools.LoadListFromXMLElement(customersPath);
            if (predicate == null)
                return (from c in customersRootElem.Elements()
                    let c1 = new Customer()
                    {
                        Id = Int32.Parse(c.Element("Id").Value),
                        Name = c.Element("Name").Value,
                        Phone = c.Element("Phone").Value,
                        Latitude = double.Parse(c.Element("Latitude").Value),
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
                            Latitude = double.Parse(c.Element("Latitude").Value),
                            Longitude = double.Parse(c.Element("Longitude").Value),
                            IsActive = bool.Parse(c.Element("IsActive").Value)
                        }
                        where predicate(c1)
                        where c1.IsActive is true
                        select c1
                   );
        }
        public IEnumerable<Customer?> GetCustomers(Func<Customer?, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXMLSerializer<Customer?>(customersPath).Where(c => c?.IsActive == true).OrderBy(x => x?.Id) :
                XMLTools.LoadListFromXMLSerializer<Customer?>(customersPath).Where(predicate).Where(c => c?.IsActive == true).OrderBy(x => x?.Id);
        public IEnumerable<Drone?> GetDrones(Func<Drone?, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXMLSerializer<Drone?>(dronesPath).OrderBy(x => x?.Id) :
                XMLTools.LoadListFromXMLSerializer<Drone?>(dronesPath).Where(predicate).OrderBy(x => x?.Id);
        public IEnumerable<Parcel?> GetParcels(Func<Parcel?, bool> predicate = null) =>
            predicate == null ?
                XMLTools.LoadListFromXMLSerializer<Parcel?>(parcelsPath).OrderBy(x=>x?.Id):
                XMLTools.LoadListFromXMLSerializer<Parcel?>(parcelsPath).Where(predicate).OrderBy(x => x?.Id);
        public IEnumerable<DroneCharge?> GetDronesInCharge(Func<DroneCharge?, bool> predicate = null)
        {
            List<DroneCharge?> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge?>(dronesInChargePath);
            if (predicate == null)
                return from dc in dronesInChargeList
                      select dc;
            else
                return from dc in dronesInChargeList
                       where predicate(dc)
                       select dc;
        }
        #endregion

        #region Request (Get) single objects
        public BaseStation GetBaseStation(int id)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            BaseStation bs = baseStationsList.Find(b => b.Id == id && b.IsActive == true);
            if (!bs.Equals(default(BaseStation)))
                return bs; 
            else
                throw new DO.ExistIdException($"Missing Base-Station id: {id}");
        }
        public Customer GetCustomer(int id)
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
                            Latitude = double.Parse(c.Element("Latitude").Value),
                            Longitude = double.Parse(c.Element("Longitude").Value),
                            IsActive = bool.Parse(c.Element("IsActive").Value)
                        }
                        ).FirstOrDefault();

            if (customer == null)
                throw new DO.ExistIdException($"Missing Customer id: {id}");

            return (Customer)customer;

        }
        public Drone GetDrone(int id)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            Drone drone = dronesList.Find(d => d.Id == id);
            if (!drone.Equals(default(Drone)))
                return drone;
            else
                throw new DO.ExistIdException($"Missing Drone id: {id}");
        }
        public Parcel GetParcel(int id)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            Parcel parcel = parcelsList.Find(p => p.Id == id);
            if (!parcel.Equals(default(Parcel)))
                return parcel;
            else
                throw new DO.ExistIdException($"Missing Parcel id: {id}");
        }
        #endregion

        #region DELETE
        public void DeleteBaseStation(int id)
        {
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            BaseStation? bs = GetBaseStation(id);
            if (bs != null)
            {
                BaseStation inActiveBS = (BaseStation)bs;
                baseStationsList.Remove((BaseStation)bs);
                inActiveBS.IsActive = false;
                baseStationsList.Add(inActiveBS);
            }
            else
                throw new DO.ExistIdException($"Missing Base Station Id {id}");
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }
        
        public void DeleteDrone(int id)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            Drone? drone = dronesList.Find(d => d.Id == id);
            if (drone != null)
            {
                dronesList.Remove((Drone)drone);
            }
            else
                throw new DO.ExistIdException($"Missing Drone Id {id}");
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
                throw new DO.ExistIdException($"Missing Customer Id {id}");
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
                throw new DO.ExistIdException($"Missing Parcel Id {id}");
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }
        #endregion

        #region Handling charging
        public void ChargeDrone(int droneId, int baseStationId)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            List<DroneCharge> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            if (!dronesList.Exists(d => d.Id == droneId))
                throw new ExistIdException($"Drone {droneId} doesn't exist");
            if (!baseStationsList.Exists(b => b.Id == baseStationId && b.IsActive))
                throw new ExistIdException($"Base Station {baseStationId} doesn't exist");
            dronesInChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
            BaseStation bs = GetBaseStation(baseStationId);
            BaseStation updatedBS = bs;
            baseStationsList.Remove(bs);
            updatedBS.FreeChargeSlots--;
            baseStationsList.Add(updatedBS);
            XMLTools.SaveListToXMLSerializer(dronesInChargeList, dronesInChargePath);
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }

        public void ReleaseDroneFromCharge(int droneId)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            List<BaseStation> baseStationsList = XMLTools.LoadListFromXMLSerializer<BaseStation>(baseStationsPath);
            List<DroneCharge> dronesInChargeList = XMLTools.LoadListFromXMLSerializer<DroneCharge>(dronesInChargePath);
            if (!dronesList.Exists(d => d.Id == droneId))
                throw new ExistIdException($"Drone {droneId} doesn't exist");
            DroneCharge dc = dronesInChargeList.Find(dc => dc.DroneId == droneId);
            BaseStation bs = GetBaseStation(dc.StationId);
            BaseStation updatedBS = bs;
            dronesInChargeList.Remove(dc);
            baseStationsList.Remove(bs);
            updatedBS.FreeChargeSlots++;
            baseStationsList.Add(updatedBS);
            XMLTools.SaveListToXMLSerializer(dronesInChargeList, dronesInChargePath);
            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }
        #endregion

        #region Handling delivery
        public void ScheduleDroneForParcel(int parcelId, int droneId)
        {
            List<Drone> dronesList = XMLTools.LoadListFromXMLSerializer<Drone>(dronesPath);
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.Exists(p => p.Id == parcelId))
                throw new ExistIdException($"Parcel {parcelId} doesn't exist");
            if (!dronesList.Exists(d => d.Id == droneId))
                throw new ExistIdException($"Drone {droneId} doesn't exist");
            Parcel parcel = GetParcel(parcelId);
            Parcel updatedParcel = parcel;
            parcelsList.Remove(parcel);
            updatedParcel.DroneId = droneId;
            updatedParcel.Scheduled = DateTime.Now;
            parcelsList.Add(updatedParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }
        public void PickingUpAParcel(int parcelId)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.Exists(p => p.Id == parcelId))
                throw new ExistIdException($"Parcel {parcelId} doesn't exist");
            Parcel parcel = GetParcel(parcelId);
            Parcel updatedParcel = parcel;
            parcelsList.Remove(parcel);
            updatedParcel.PickedUp = DateTime.Now;
            parcelsList.Add(updatedParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }
        public void DeliverAParcel(int parcelId)
        {
            List<Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<Parcel>(parcelsPath);
            if (!parcelsList.Exists(p => p.Id == parcelId))
                throw new ExistIdException($"Parcel {parcelId} doesn't exist");
            Parcel parcel = GetParcel(parcelId);
            Parcel updatedParcel = parcel;
            parcelsList.Remove(parcel);
            updatedParcel.Delivered = DateTime.Now;
            parcelsList.Add(updatedParcel);
            XMLTools.SaveListToXMLSerializer(parcelsList, parcelsPath);
        }
        public int GetNextParcelId()
        {
            XElement t = XElement.Load(configPath);
            int id = Convert.ToInt32(t.Element("NewParcelId").Value);
            return id;
        }
        #endregion

        #region UPDATE
        public void UpdateCustomer(int customerId, string newName, string newPhone)
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
                throw new DO.ExistIdException($"Missing Customer Id {customerId}");
        }
        public void UpdateDroneModel(int droneId, string newName)
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
                throw new DO.ExistIdException($"Missing Drone Id {droneId}");

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
                newBS.FreeChargeSlots =  slotsCount - dronesInChargeList.Where(x => x.StationId == baseStationId).Count(); ;
                baseStationsList.Add(newBS);
            }
            else
                throw new DO.ExistIdException($"Missing Customer Base-Station {baseStationId}");

            XMLTools.SaveListToXMLSerializer(baseStationsList, baseStationsPath);
        }
        #endregion

        public int GetDroneChargeBaseStationId(int droneId) =>
            (XMLTools.LoadListFromXMLSerializer<DroneCharge?>(dronesInChargePath).Find(dc => (int)dc?.DroneId == droneId)
                ?? throw new ExistIdException("Drone is not being charged", droneId)).StationId;
    }
}
