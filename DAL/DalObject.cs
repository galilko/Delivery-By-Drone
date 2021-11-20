using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IDAL.DO;

namespace DalObject
{
    public class DalObject : IDAL.IDal
    {
        public double[] GetBatteryUse()
        {
            return new double[5] { DataSource.Config.BatteryUseFREE, DataSource.Config.BatteryUseLight,
                DataSource.Config.BatteryUseMedium, DataSource.Config.BatteryUseHeavy, DataSource.Config.BatteryChargeRate};
        }

        #region ctor
        public DalObject()
        {
            DataSource.Initialize();
        }
        #endregion
        #region Update methods
        #region Schedule Drone For Parcel

        /// <summary>
        /// schedule a drone for parcel delivery
        /// </summary>
        /// <param name="parcelId">parcel id for deliver</param>
        /// <param name="droneId">drone id for schedule</param>
        public void ScheduleDroneForParcel(int parcelId, int droneId)
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
        #endregion
        #region PickingUpAParcel
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
                            myDrone.Status = DroneStatusCategories.Delivery;
                            DataSource.ParcelsList[i] = myParcel;
                            DataSource.DronesList[j] = myDrone;
                            return;
                        }
                }
        }
        #endregion
        #region DeliverAParcel

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
                            myDrone.Status = DroneStatusCategories.Free;
                            DataSource.ParcelsList[i] = myParcel;
                            DataSource.DronesList[j] = myDrone;
                            return;
                        }
                }
        }
        #endregion
        #region ChargeDrone
        /// <summary>
        /// connect a drone to charging at base-station
        /// </summary>
        /// <param name="droneId">drone's id to charge</param>
        /// <param name="baseStationId">requested base-station</param>
        public void ChargeDrone(int droneId, int baseStationId)
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
                            DataSource.DroneChargeList.Add(new() { DroneId = droneId, StationId = baseStationId });
                            Drone myDrone = DataSource.DronesList[i];
                            BaseStation myBaseStation = DataSource.BaseStationsList[j];
                            myBaseStation.FreeChargeSlots--;
                            DataSource.DronesList[i] = myDrone;
                            DataSource.BaseStationsList[j] = myBaseStation;
                            return;
                        }
        }
        #endregion
        #region Release Drone From Charge
        /// <summary>
        /// release a drone from charging
        /// </summary>
        /// <param name="droneId">drone's id to release</param>
        public void ReleaseDroneFromCharge(int droneId)
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

        #endregion
        #region Add methods
        #region  Add Base Station
        /// <summary>
        /// adding a new base atation into array
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
        #endregion
        #region Add Drone
        /// <summary>
        /// adding a new drone into array
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
        #endregion
        #region Add Customer
        /// <summary>
        /// adding a new customer into array
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
        #endregion
        #region Add Parcel
        /// <summary>
        /// adding a new parcel into array
        /// </summary>
        /// <param name="senderId"></param>
        /// <param name="targetId"></param>
        /// <param name="weight"></param>
        /// <param name="priority"></param>
        public void AddParcel(Parcel newParcel)
        {
            if (DataSource.ParcelsList.Exists(item => item.Id == newParcel.Id))
                throw new ParcelException($"Parcel {newParcel.Id} is already exist");
            DataSource.ParcelsList.Add(newParcel);
        }
        #endregion
        #endregion
        #region Find methods
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
        /// find a drone by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Drone FindDrone(int id)
        {
            foreach (var item in DataSource.DronesList)
                if (item.Id == id)
                    return item;
            throw new DroneException($"Drone {id} doesn't exist");
        }
        /// <summary>
        /// find a customer by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Customer FindCustomer(int id)
        {
            foreach (var item in DataSource.CustomersList)
                if (item.Id == id)
                    return item;
            throw new CustomerException($"Customer {id} doesn't exist");
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
        #endregion
        #region All lists
        /// <summary>
        /// return array of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> AllCustomers() => DataSource.CustomersList;
        /// <summary>
        /// return array of drones
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Drone> AllDrones() => DataSource.DronesList;
        /// <summary>
        /// return array of base-stations
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BaseStation> AllBaseStations() => DataSource.BaseStationsList;
        /// <summary>
        /// return array of parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> AllParcels() => DataSource.ParcelsList;
        #endregion
        #region others
        /// <summary>
        /// return array of none-scheduled parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> NoneScheduledParcels()
        {
            List<Parcel> tmpList = new();
            for (int i = 0; i < DataSource.ParcelsList.Count; i++)
                if (DataSource.ParcelsList[i].Scheduled == DateTime.MinValue)
                    tmpList.Add(DataSource.ParcelsList[i]);
            return tmpList;
        }
        /// <summary>
        /// return array of base-stations with free-slots
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
    }
}
