using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Dal
{
    internal sealed class DalObject : DalApi.IDal
    {
        public double[] GetBatteryUse()
        {
            return new double[5] { DataSource.Config.BatteryUseFREE, DataSource.Config.BatteryUseLight,
                DataSource.Config.BatteryUseMedium, DataSource.Config.BatteryUseHeavy, DataSource.Config.BatteryChargeRate};
        }
        #region thread-safe DalObject singleton
        private DalObject() { DataSource.Initialize();
            
            XmlSerializer serialiser1 = new XmlSerializer(typeof(List<Drone>));//modify to the name of your object
            XmlSerializer serialiser2 = new XmlSerializer(typeof(List<BaseStation>));//modify to the name of your object
            XmlSerializer serialiser3 = new XmlSerializer(typeof(List<Customer>));//modify to the name of your object
            XmlSerializer serialiser4 = new XmlSerializer(typeof(List<Parcel>));//modify to the name of your object
            TextWriter Filestream1 = new StreamWriter(@"C:\Users\Gal Gabay\source\repos\galilko\dotNet5782_6024_2498\XmlData\DronesXml.xml"); //here you enter the path in your computer where you want your xml file 
            TextWriter Filestream2 = new StreamWriter(@"C:\Users\Gal Gabay\source\repos\galilko\dotNet5782_6024_2498\XmlData\BaseStationsXml.xml"); //here you enter the path in your computer where you want your xml file 
            TextWriter Filestream3 = new StreamWriter(@"C:\Users\Gal Gabay\source\repos\galilko\dotNet5782_6024_2498\XmlData\CustomersXml.xml"); //here you enter the path in your computer where you want your xml file 
            TextWriter Filestream4 = new StreamWriter(@"C:\Users\Gal Gabay\source\repos\galilko\dotNet5782_6024_2498\XmlData\ParcelsXml.xml"); //here you enter the path in your computer where you want your xml file 
            serialiser1.Serialize(Filestream1, DataSource.DronesList);//here you enter the name of your list
            serialiser2.Serialize(Filestream2, DataSource.BaseStationsList);//here you enter the name of your list
            serialiser3.Serialize(Filestream3, DataSource.CustomersList);//here you enter the name of your list
            serialiser4.Serialize(Filestream4, DataSource.ParcelsList);//here you enter the name of your list
            Filestream1.Close();
            Filestream2.Close();
            Filestream3.Close();
            Filestream4.Close();
        }
        static readonly object padlock = new object ();  
        static DalObject instance = null;
        internal static DalObject Instance
        {
            get
            {
                lock (padlock)
                {
                        if (instance == null)
                        {
                            instance = new DalObject();
                        }
                        return instance;
                    }
            }
        }
        #endregion


        #region  Base Station
        /// <summary>
        /// adding a new base atation into list
        /// </summary>
        /// <param name="name"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="freeSlots"></param> 
        public void AddBaseStation(BaseStation newBaseStation)
        {
            if (DataSource.BaseStationsList.Exists(item => item.Id == newBaseStation.Id))
                throw new BaseStationException($"Base Station {newBaseStation.Id} is already exist");
            DataSource.BaseStationsList.Add(newBaseStation);
        }
        /// <summary>
        /// find a base-station by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BaseStation FindBaseStation(int id)
        {
            foreach (var item in DataSource.BaseStationsList)
                if (item.Id == id)
                    return item;
            throw new BaseStationException($"base station {id} doesn't exist");
        }
        /// <summary>
        /// find a base-station by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void DeleteBaseStation(int id)
        {
            if (!DataSource.BaseStationsList.Exists(x => x.Id == id)) //if base station doesn't exist
                throw new BaseStationException($"Base Station {id} doesn't exists");
            for (int i = 0; i < DataSource.BaseStationsList.Count; i++)
            {
                if (DataSource.BaseStationsList[i].Id == id)
                {
                    BaseStation myBaseStation = DataSource.BaseStationsList[i];
                    myBaseStation.IsActive = false;
                    DataSource.BaseStationsList[i] = myBaseStation;
                    return;
                }
            }
        }
        /// <summary>
        /// change the name and the number of slots in base station
        /// </summary>
        /// <param name="baseStationId"></param>
        /// <param name="newName"></param>
        /// <param name="slotsCount"></param>
        public void UpdateBaseStation(int baseStationId, string newName, int slotsCount)
        {
            if (!DataSource.BaseStationsList.Exists(x => x.Id == baseStationId)) //if base station doesn't exist
                throw new BaseStationException($"Base Station {baseStationId} doesn't exists");
            for (int i = 0; i < DataSource.BaseStationsList.Count; i++)
            {
                if (DataSource.BaseStationsList[i].Id == baseStationId)
                {
                    BaseStation myBaseStation = DataSource.BaseStationsList[i]; 
                    myBaseStation.FreeChargeSlots = slotsCount == 0 ? myBaseStation.FreeChargeSlots : slotsCount - DataSource.DroneChargeList.Where(x => x.StationId == baseStationId).Count();
                    myBaseStation.Name = string.IsNullOrEmpty(newName) ? myBaseStation.Name : newName;
                    DataSource.BaseStationsList[i] = myBaseStation;
                    return;
                }
            }
        }
        /// <summary>
        /// return List of base-stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseStation> AllBaseStations(Func<BaseStation, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.BaseStationsList.Where(x=>x.IsActive == true).ToList();
            else
                return DataSource.BaseStationsList.Where(predicate).Where(x=>x.IsActive==true).ToList();
        }
        #endregion


        #region Drone
        /// <summary>
        /// adding a new drone into list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <param name="weight"></param>
        /// <param name="status"></param>
        /// <param name="battery"></param>
        public void AddDrone(Drone newDrone)
        {
            if (DataSource.DronesList.Exists(item => item.Id == newDrone.Id))
                throw new DroneException($"Drone {newDrone.Id} is already exist");
            DataSource.DronesList.Add(newDrone);
        }
        /// <summary>
        /// find a drone by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Drone FindDrone(int? id)
        {
            foreach (var item in DataSource.DronesList)
                if (item.Id == id)
                    return item;
            throw new DroneException($"Drone {id} doesn't exist");
        }
        /// <summary>
        /// connect a drone to charging at base-station
        /// </summary>
        /// <param name="droneId">drone's id to charge</param>
        /// <param name="baseStationId">requested base-station</param>
        /// <summary>
        /// change the drone model
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="newName"></param>
        public void DeleteDrone(int? id)
        {
            if (!DataSource.DronesList.Exists(x => x.Id == id))
                throw new BaseStationException($"Base Station {id} doesn't exists");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
            {
                if (DataSource.DronesList[i].Id == id)
                {
                    DataSource.DronesList.Remove(DataSource.DronesList[i]);
                    return;
                }
            }
        }
        public void ChargeDrone(int? droneId, int baseStationId)
        {
            if (!DataSource.DronesList.Exists(item => item.Id == droneId))
                throw new DroneException($"Drone {droneId} doesn't exist");
            if (!DataSource.BaseStationsList.Exists(item => item.Id == baseStationId))
                throw new BaseStationException($"Base Station {baseStationId} doesn't exist");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
                if (DataSource.DronesList[i].Id == droneId)
                    for (int j = 0; j < DataSource.BaseStationsList.Count; j++)
                        if (DataSource.BaseStationsList[j].Id == baseStationId)
                        {
                            if (DataSource.BaseStationsList[j].IsActive == false)
                                throw new BaseStationException($"Base Station {baseStationId} isn't active");
                            DataSource.DroneChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
                            BaseStation myBaseStation = DataSource.BaseStationsList[j];
                            myBaseStation.FreeChargeSlots--;
                            DataSource.BaseStationsList[j] = myBaseStation;
                            return;
                        }
        }
        /// <summary>
        /// release a drone from charging
        /// </summary>
        /// <param name="droneId">drone's id to release</param>
        public void ReleaseDroneFromCharge(int? droneId)
        {
            if (!DataSource.DronesList.Exists(item => item.Id == droneId))
                throw new DroneException($"Drone {droneId} doesn't exist");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
                if (DataSource.DronesList[i].Id == droneId)
                {
                    Drone myDrone = DataSource.DronesList[i];
                    DroneCharge droneCharge = DataSource.DroneChargeList.Find(myDroneCharge => myDroneCharge.DroneId == droneId);
                    for (int j = 0; j < DataSource.BaseStationsList.Count; j++)
                        if (DataSource.BaseStationsList[j].Id == droneCharge.StationId)
                        {
                            BaseStation myBaseStation = DataSource.BaseStationsList[j];
                            myBaseStation.FreeChargeSlots++;
                            DataSource.BaseStationsList[j] = myBaseStation;
                            DataSource.DroneChargeList.Remove(droneCharge);
                            break;
                        }
                }
        }
        #endregion


        #region Customer
        /// <summary>
        /// adding a new customer into list
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="phone"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void AddCustomer(Customer newCustomer)
        {
            if (DataSource.CustomersList.Exists(item => item.Id == newCustomer.Id))
                throw new CustomerException($"Customer {newCustomer.Id} is already exist");
            DataSource.CustomersList.Add(newCustomer);
        }
        /// <summary>
        /// find a customer by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer FindCustomer(int? id)
        {
            foreach (var item in DataSource.CustomersList)
                if (item.Id == id && item.IsActive)
                    return item;
            throw new CustomerException($"Customer {id} doesn't exist");
        }

        public void DeleteCustomer(int id)
        {
            if (!DataSource.CustomersList.Exists(x => x.Id == id)) //if base station doesn't exist
                throw new CustomerException($"Customer {id} doesn't exists");
            for (int i = 0; i < DataSource.CustomersList.Count; i++)
            {
                if (DataSource.CustomersList[i].Id == id)
                {
                    Customer myCustomer = DataSource.CustomersList[i];
                    myCustomer.IsActive = false;
                    DataSource.CustomersList[i] = myCustomer;
                    return;
                }
            }
        }

        /// <summary>
        /// change the name and the phon number
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="newName"></param>
        /// <param name="newPhone"></param>
        public void UpdateCustomer(int? customerId, string newName, string newPhone)
        {
            if (!DataSource.CustomersList.Exists(x => x.Id == customerId))
                throw new CustomerException($"Customer {customerId} doesn't exist");
            for (int i = 0; i < DataSource.CustomersList.Count; i++)
            {
                if (DataSource.CustomersList[i].Id == customerId)
                {
                    Customer myCustomer = DataSource.CustomersList[i];
                    myCustomer.Name = string.IsNullOrEmpty(newName) ? myCustomer.Name : newName;
                    myCustomer.Phone = string.IsNullOrEmpty(newPhone) ? myCustomer.Phone : newPhone;
                    DataSource.CustomersList[i] = myCustomer;
                    return;
                }
            }
        }
        #endregion


        #region Parcel
        /// <summary>
        /// adding a new parcel into list
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="targetId"></param>
        /// <param name="weight"></param>
        /// <param name="priority"></param>
        public void AddParcel(Parcel newParcel)
        {
            if (DataSource.ParcelsList.Exists(item => item.Id == newParcel.Id))
                throw new ParcelException($"Parcel {newParcel.Id} is already exist");
            newParcel.Id = DataSource.Config.NewParcelId++;
            DataSource.ParcelsList.Add(newParcel);
        }
        /// <summary>
        /// find a parcel by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Parcel FindParcel(int id)
        {
            foreach (var item in DataSource.ParcelsList)
                if (item.Id == id)
                    return item;
            throw new ParcelException($"Parcel {id} doesn't exist");
        }
        /// <summary>
        /// delete a parcel by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public void DeleteParcel(int id)
        {
            try
            {
                    DataSource.ParcelsList.Remove(FindParcel(id));
            }
            catch
            {
            throw new ParcelException($"cannot delete Parcel {id}");
            }
        }
        /// <summary>
        /// schedule a drone for parcel delivery
        /// </summary>
        /// <param name="parcelId">parcel id for deliver</param>
        /// <param name="droneId">drone id for schedule</param>
        public void ScheduleDroneForParcel(int parcelId, int? droneId)
        {
            if (!DataSource.ParcelsList.Exists(item => item.Id == parcelId))
                throw new ParcelException($"Parcel {parcelId} doesn't exist");
            if (!DataSource.DronesList.Exists(item => item.Id == droneId))
                throw new DroneException($"Drone {droneId} doesn't exist");
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i].Id == parcelId)
                {
                    for (int j = 0; j < DataSource.DronesList.Count; j++)
                    {
                        if (DataSource.DronesList[j].Id == droneId)
                        {
                            Parcel my_parcel = DataSource.ParcelsList[i];
                            my_parcel.DroneId = droneId;
                            my_parcel.Scheduled = DateTime.Now;
                            DataSource.ParcelsList[i] = my_parcel;
                            return;
                        }
                    }
                }
        }
        /// <summary>
        /// picking up a parcel by its drone
        /// </summary>
        /// <param name="parcelId">parcel id to picking up</param>
        public void PickingUpAParcel(int parcelId)
        {
            if (!DataSource.ParcelsList.Exists(item => item.Id == parcelId))
                throw new ParcelException($"Parcel {parcelId} doesn't exist");
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i].Id == parcelId)
                {
                    Parcel myParcel = DataSource.ParcelsList[i];
                    myParcel.PickedUp = DateTime.Now;
                    for (int j = 0; j < DataSource.DronesList.Count; j++)
                        if (DataSource.DronesList[j].Id == DataSource.ParcelsList[i].DroneId)
                        {
                            Drone myDrone = DataSource.DronesList[j];
                            DataSource.ParcelsList[i] = myParcel;
                            DataSource.DronesList[j] = myDrone;
                            return;
                        }
                }
        }
        /// <summary>
        /// finish delivery of a parcel
        /// </summary>
        /// <param name="parcelId">parcel id to deliver</param>
        public void DeliverAParcel(int parcelId)
        {
            if (!DataSource.ParcelsList.Exists(item => item.Id == parcelId))
                throw new ParcelException($"Parcel {parcelId} doesn't exist");
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i].Id == parcelId)
                {
                    Parcel myParcel = DataSource.ParcelsList[i];
                    myParcel.Delivered = DateTime.Now;
                    for (int j = 0; j < DataSource.DronesList.Count; j++)
                        if (DataSource.DronesList[j].Id == DataSource.ParcelsList[i].DroneId)
                        {
                            Drone myDrone = DataSource.DronesList[j];
                            DataSource.ParcelsList[i] = myParcel;
                            DataSource.DronesList[j] = myDrone;
                            return;
                        }
                }
        }
        #endregion



        #region All lists
        /// <summary>
        /// return List of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> AllCustomers(Func<Customer, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.CustomersList.Where(x => x.IsActive == true).ToList();

            return (from item in DataSource.CustomersList
                    where item.IsActive is true
                    where predicate(item)
                    select item);
        }
        /// <summary>
        /// return List of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> AllDrones() => DataSource.DronesList;
        /// <summary>
        /// return List of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> AllParcels(Func<Parcel, bool> predicate = null)
        {
            if (predicate == null)
                return DataSource.ParcelsList.ToList();
            else
                return DataSource.ParcelsList.Where(predicate);
        }
        #endregion


        #region others

        /// <summary>
        /// return List of none-scheduled parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> NoneScheduledParcels()
        {
            List<Parcel> tmpList = new();
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i].Scheduled == null)
                    tmpList.Add(DataSource.ParcelsList[i]);
            return tmpList;
        }
        /// <summary>
        /// return List of base-stations with free-slots
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseStation> FreeSlotsBaseStations()
        {
            List<BaseStation> tmpList = new();
            for (int i = 0; i < DataSource.BaseStationsList.Count; i++)
                if (DataSource.BaseStationsList[i].FreeChargeSlots > 0)
                    tmpList.Add(DataSource.BaseStationsList[i]);
            return tmpList;
        }

        #endregion

        public IEnumerable<DroneCharge> GetListOfInChargeDrones()
        {
            return DataSource.DroneChargeList;
        }
        void IDal.UpdateDroneModel(int? droneId, string newName)
        {
            throw new NotImplementedException();
        }

    }
}
