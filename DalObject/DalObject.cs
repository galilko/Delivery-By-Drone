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
        public double[] GetBatteryUsages()
        {
            return new double[5] { DataSource.Config.BatteryUseFREE, DataSource.Config.BatteryUseLight,
                DataSource.Config.BatteryUseMedium, DataSource.Config.BatteryUseHeavy, DataSource.Config.BatteryChargeRate};
        }
        #region thread-safe DalObject singleton
        private DalObject() { DataSource.Initialize(this);
            
            XmlSerializer serialiser1 = new XmlSerializer(typeof(List<Drone?>));//modify to the name of your object
            XmlSerializer serialiser2 = new XmlSerializer(typeof(List<BaseStation?>));//modify to the name of your object
            XmlSerializer serialiser3 = new XmlSerializer(typeof(List<Customer?>));//modify to the name of your object
            XmlSerializer serialiser4 = new XmlSerializer(typeof(List<Parcel?>));//modify to the name of your object
            TextWriter Filestream1 = new StreamWriter(@"XmlData\DronesXml.xml"); //here you enter the path in your computer where you want your xml file 
            TextWriter Filestream2 = new StreamWriter(@"XmlData\BaseStationsXml.xml"); //here you enter the path in your computer where you want your xml file 
            TextWriter Filestream3 = new StreamWriter(@"XmlData\CustomersXml.xml"); //here you enter the path in your computer where you want your xml file 
            TextWriter Filestream4 = new StreamWriter(@"XmlData\ParcelsXml.xml"); //here you enter the path in your computer where you want your xml file 
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

        #region ADD
        public void AddBaseStation(BaseStation newBaseStation)
        {
            if (DataSource.BaseStationsList.Exists(item => item?.Id == newBaseStation.Id))
                throw new ExistIdException($"Base Station {newBaseStation.Id} is already exist");
            DataSource.BaseStationsList.Add(newBaseStation);
        }
        public void AddDrone(Drone newDrone)
        {
            if (DataSource.DronesList.Exists(item => item?.Id == newDrone.Id))
                throw new ExistIdException($"Drone {newDrone.Id} is already exist");
            DataSource.DronesList.Add(newDrone);
        }
        public void AddCustomer(Customer newCustomer)
        {
            if (DataSource.CustomersList.Exists(item => item?.Id == newCustomer.Id))
                throw new ExistIdException($"Customer {newCustomer.Id} is already exist");
            DataSource.CustomersList.Add(newCustomer);
        }
        public void AddParcel(Parcel newParcel)
        {
            if (DataSource.ParcelsList.Exists(item => item?.Id == newParcel.Id))
                throw new ExistIdException($"Parcel {newParcel.Id} is already exist");
            newParcel.Id = DataSource.Config.NewParcelId++;
            DataSource.ParcelsList.Add(newParcel);
        }
        #endregion

        #region Request (Get) single objects
        public BaseStation GetBaseStation(int id) =>
            DataSource.BaseStationsList.Find(d => (int)d?.Id == id && (bool)d?.IsActive)
                ?? throw new ExistIdException("Base Station does not exists", id);
        public Drone GetDrone(int id) =>
            DataSource.DronesList.Find(d => (int)d?.Id == id)
                ?? throw new ExistIdException("Drone does not exists", id);
        public Customer GetCustomer(int id)=>
            DataSource.CustomersList.Find(c => (int)c?.Id == id)
                ?? throw new ExistIdException("Customer does not exists", id);
        public Parcel GetParcel(int id) =>
            DataSource.ParcelsList.Find(d => (int)d?.Id == id)
                ?? throw new ExistIdException("Parcel does not exists", id);
        public int GetDroneChargeBaseStationId(int droneId) =>
            (DataSource.DroneChargeList.Find(dc => (int)dc?.DroneId == droneId)
                ?? throw new ExistIdException("Drone is not being charged", droneId)).StationId;
        #endregion

        #region Request (Get) collections
        public IEnumerable<BaseStation?> GetBaseStations(Func<BaseStation?, bool> predicate = null) =>
            DataSource.BaseStationsList.Where(predicate ?? (b => true)).Where(b => b?.IsActive == true);
        public IEnumerable<Drone?> GetDrones(Func<Drone?, bool> predicate = null) =>
            DataSource.DronesList.Where(predicate ?? (p => true));
        public IEnumerable<Customer?> GetCustomers(Func<Customer?, bool> predicate = null) =>
            DataSource.CustomersList.Where(predicate ?? (c => true)).Where(c => c?.IsActive == true);
           /* if (predicate == null)
                return DataSource.CustomersList.Where(x => x.IsActive == true).ToList();

            return (from item in DataSource.CustomersList
                    where item.IsActive is true
                    where predicate(item)
                    select item);*/

        public IEnumerable<Parcel?> GetParcels(Func<Parcel?, bool> predicate = null) =>
            DataSource.ParcelsList.Where(predicate ?? (p => true));
        public IEnumerable<DroneCharge?> GetDronesInCharge(Func<DroneCharge?, bool> predicate = null) =>
            DataSource.DroneChargeList.Where(predicate ?? (dc => true));
        #endregion

        #region DELETE
        public void DeleteBaseStation(int id)
        {
            if (!DataSource.BaseStationsList.Exists(x => x?.Id == id)) //if base station doesn't exist
                throw new ExistIdException($"Base Station {id} doesn't exists");
            for (int i = 0; i < DataSource.BaseStationsList.Count; i++)
            {
                if (DataSource.BaseStationsList[i]?.Id == id)
                {
                    BaseStation myBaseStation = (BaseStation)DataSource.BaseStationsList[i];
                    myBaseStation.IsActive = false;
                    DataSource.BaseStationsList[i] = myBaseStation;
                    return;
                }
            }
        }
        public void DeleteDrone(int id)
        {
            if (!DataSource.DronesList.Exists(x => x?.Id == id))
                throw new ExistIdException($"Base Station {id} doesn't exists");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
            {
                if (DataSource.DronesList[i]?.Id == id)
                {
                    DataSource.DronesList.Remove(DataSource.DronesList[i]);
                    return;
                }
            }
        }
        public void DeleteCustomer(int id)
        {
            if (!DataSource.CustomersList.Exists(x => x?.Id == id)) //if base station doesn't exist
                throw new ExistIdException($"Customer {id} doesn't exists");
            for (int i = 0; i < DataSource.CustomersList.Count; i++)
            {
                if (DataSource.CustomersList[i]?.Id == id)
                {
                    Customer myCustomer = (Customer)DataSource.CustomersList[i];
                    myCustomer.IsActive = false;
                    DataSource.CustomersList[i] = myCustomer;
                    return;
                }
            }
        }
        public void DeleteParcel(int id)
        {
            try
            {
                    DataSource.ParcelsList.Remove(GetParcel(id));
            }
            catch
            {
            throw new ExistIdException($"cannot delete Parcel {id}");
            }
        }
        #endregion

        #region UPDATE
        public void UpdateBaseStation(int baseStationId, string newName, int slotsCount)
        {
            if (!DataSource.BaseStationsList.Exists(x => x?.Id == baseStationId)) //if base station doesn't exist
                throw new ExistIdException($"Base Station {baseStationId} doesn't exists");
            for (int i = 0; i < DataSource.BaseStationsList.Count; i++)
            {
                if (DataSource.BaseStationsList[i]?.Id == baseStationId)
                {
                    BaseStation myBaseStation = (BaseStation)DataSource.BaseStationsList[i]; 
                    myBaseStation.FreeChargeSlots = slotsCount == 0 ? myBaseStation.FreeChargeSlots : slotsCount - DataSource.DroneChargeList.Where(x => x?.StationId == baseStationId).Count();
                    myBaseStation.Name = string.IsNullOrEmpty(newName) ? myBaseStation.Name : newName;
                    DataSource.BaseStationsList[i] = myBaseStation;
                    return;
                }
            }
        }
        public void UpdateDroneModel(int droneId, string newName)
        {
            if (!DataSource.DronesList.Exists(x => x?.Id == droneId)) //if drone doesn't exist
                throw new ExistIdException($"Drone {droneId} doesn't exists");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
            {
                if (DataSource.DronesList[i]?.Id == droneId)
                {
                    Drone myDrone = (Drone)DataSource.DronesList[i];
                    myDrone.Model = string.IsNullOrEmpty(newName) ? myDrone.Model : newName;
                    DataSource.DronesList[i] = myDrone;
                    return;
                }
            }
        }
        public void UpdateCustomer(int customerId, string newName, string newPhone)
        {
            if (!DataSource.CustomersList.Exists(x => x?.Id == customerId))
                throw new ExistIdException($"Customer {customerId} doesn't exist");
            for (int i = 0; i < DataSource.CustomersList.Count; i++)
            {
                if (DataSource.CustomersList[i]?.Id == customerId)
                {
                    Customer myCustomer = (Customer)DataSource.CustomersList[i];
                    myCustomer.Name = string.IsNullOrEmpty(newName) ? myCustomer.Name : newName;
                    myCustomer.Phone = string.IsNullOrEmpty(newPhone) ? myCustomer.Phone : newPhone;
                    DataSource.CustomersList[i] = myCustomer;
                    return;
                }
            }
        }
        #region Parcel Delivery Handling
        public void ScheduleDroneForParcel(int parcelId, int droneId)
        {
            if (!DataSource.ParcelsList.Exists(item => item?.Id == parcelId))
                throw new ExistIdException($"Parcel {parcelId} doesn't exist");
            if (!DataSource.DronesList.Exists(item => item?.Id == droneId))
                throw new ExistIdException($"Drone {droneId} doesn't exist");
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i]?.Id == parcelId)
                {
                    for (int j = 0; j < DataSource.DronesList.Count; j++)
                    {
                        if (DataSource.DronesList[j]?.Id == droneId)
                        {
                            Parcel my_parcel = (Parcel)DataSource.ParcelsList[i];
                            my_parcel.DroneId = droneId;
                            my_parcel.Scheduled = DateTime.Now;
                            DataSource.ParcelsList[i] = my_parcel;
                            return;
                        }
                    }
                }
        }
        public void PickingUpAParcel(int parcelId)
        {
            if (!DataSource.ParcelsList.Exists(item => item?.Id == parcelId))
                throw new ExistIdException($"Parcel {parcelId} doesn't exist");
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i]?.Id == parcelId)
                {
                    Parcel myParcel = (Parcel)DataSource.ParcelsList[i];
                    myParcel.PickedUp = DateTime.Now;
                    for (int j = 0; j < DataSource.DronesList.Count; j++)
                        if (DataSource.DronesList[j]?.Id == DataSource.ParcelsList[i]?.DroneId)
                        {
                            Drone myDrone = (Drone)DataSource.DronesList[j];
                            DataSource.ParcelsList[i] = myParcel;
                            DataSource.DronesList[j] = myDrone;
                            return;
                        }
                }
        }
        public void DeliverAParcel(int parcelId)
        {
            if (!DataSource.ParcelsList.Exists(item => item?.Id == parcelId))
                throw new ExistIdException($"Parcel {parcelId} doesn't exist");
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i]?.Id == parcelId)
                {
                    Parcel myParcel = (Parcel)DataSource.ParcelsList[i];
                    myParcel.Delivered = DateTime.Now;
                    for (int j = 0; j < DataSource.DronesList.Count; j++)
                        if (DataSource.DronesList[j]?.Id == DataSource.ParcelsList[i]?.DroneId)
                        {
                            Drone? myDrone = DataSource.DronesList[j];
                            DataSource.ParcelsList[i] = myParcel;
                            DataSource.DronesList[j] = myDrone;
                            return;
                        }
                }
        }
        public int GetNextParcelId()
        {
            return DataSource.Config.NewParcelId;
        }
        #endregion
        #region Handling charging 
        public void ChargeDrone(int droneId, int baseStationId)
        {
            if (!DataSource.DronesList.Exists(item => item?.Id == droneId))
                throw new ExistIdException($"Drone {droneId} doesn't exist");
            if (!DataSource.BaseStationsList.Exists(item => item?.Id == baseStationId))
                throw new ExistIdException($"Base Station {baseStationId} doesn't exist");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
                if (DataSource.DronesList[i]?.Id == droneId)
                    for (int j = 0; j < DataSource.BaseStationsList.Count; j++)
                        if (DataSource.BaseStationsList[j]?.Id == baseStationId)
                        {
                            if (DataSource.BaseStationsList[j]?.IsActive == false)
                                throw new ExistIdException($"Base Station {baseStationId} isn't active");
                            DataSource.DroneChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
                            BaseStation myBaseStation = (BaseStation)DataSource.BaseStationsList[j];
                            myBaseStation.FreeChargeSlots--;
                            DataSource.BaseStationsList[j] = myBaseStation;
                            return;
                        }
        }
        public void ReleaseDroneFromCharge(int droneId)
        {
            if (!DataSource.DronesList.Exists(item => item?.Id == droneId))
                throw new ExistIdException($"Drone {droneId} doesn't exist");
            for (int i = 0; i < DataSource.DronesList.Count; i++)
                if (DataSource.DronesList[i]?.Id == droneId)
                {
                    Drone myDrone = (Drone)DataSource.DronesList[i];
                    DroneCharge droneCharge = (DroneCharge)DataSource.DroneChargeList.Find(dc => dc?.DroneId == droneId);
                    for (int j = 0; j < DataSource.BaseStationsList.Count; j++)
                        if (DataSource.BaseStationsList[j]?.Id == droneCharge.StationId)
                        {
                            BaseStation myBaseStation = (BaseStation)DataSource.BaseStationsList[j];
                            myBaseStation.FreeChargeSlots++;
                            DataSource.BaseStationsList[j] = myBaseStation;
                            DataSource.DroneChargeList.Remove(droneCharge);
                            break;
                        }
                }
        }
        #endregion
        #endregion
    }
}
