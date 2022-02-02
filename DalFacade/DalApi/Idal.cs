using DO;
using System;
using System.Collections.Generic;

namespace DalApi
{
    public interface IDal
    {
        /// <summary>
        /// adding a new base atation into list
        /// </summary>
        /// <param name="newBaseStation"></param>
        void AddBaseStation(BaseStation newBaseStation);
        /// <summary>
        /// adding a new customer into list
        /// </summary>
        /// <param name="newCustomer"></param>
        void AddCustomer(Customer newCustomer);
        /// <summary>
        /// adding a new drone into list
        /// </summary>
        /// <param name="newDrone"></param>
        void AddDrone(Drone newDrone);
        /// <summary>
        /// adding a new parcel into list
        /// </summary>
        /// <param name="newParcel"></param>
        void AddParcel(Parcel newParcel);
        
        
        /// <summary>
        /// return IEnumerable of base-stations
        /// </summary>
        /// <returns></returns>
        IEnumerable<BaseStation?> GetBaseStations(Func<BaseStation?, bool> predicate = null);
        /// <summary>
        /// return IEnumerable of customers
        /// </summary>
        /// <returns></returns>
        IEnumerable<Customer?> GetCustomers(Func<Customer?, bool> predicate = null);
        /// <summary>
        /// return IEnumerable of drones
        /// </summary>
        /// <returns></returns>
        IEnumerable<Drone?> GetDrones(Func<Drone?, bool> predicate = null);
        /// <summary>
        /// return IEnumerable of parcels
        /// </summary>
        /// <returns>IEnumerable of parcels</returns>
        IEnumerable<Parcel?> GetParcels(Func<Parcel?, bool> predicate = null);
        /// <summary>
        /// return IEnumerable of drones-charge
        /// </summary>
        /// <returns></returns>
        IEnumerable<DroneCharge?> GetDronesInCharge(Func<DroneCharge?, bool> predicate = null);
       
        
        /// <summary>
        /// find a base-station by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        BaseStation GetBaseStation(int id);
        /// <summary>
        /// find a customer by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Customer GetCustomer(int id);
        /// <summary>
        /// find a drone by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Drone GetDrone(int id);
        /// <summary>
        /// find a parcel by id and return it
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Parcel GetParcel(int id);
       
        
        /// <summary>
        /// finish delivery of a parcel
        /// </summary>
        /// <param name="parcelId">parcel id to deliver</param>
        void DeliverAParcel(int parcelId);
        /// <summary>
        /// schedule a drone for parcel delivery
        /// </summary>
        /// <param name="parcelId">parcel id for deliver</param>
        /// <param name="droneId">drone id for schedule</param>
        void ScheduleDroneForParcel(int parcelId, int droneId);
        /// <summary>
        /// picking up a parcel by its drone
        /// </summary>
        /// <param name="parcelId">parcel id to picking up</param>
        void PickingUpAParcel(int parcelId);
        /// <summary>
        /// connect a drone to charging at base-station
        /// </summary>
        /// <param name="droneId">drone's id to charge</param>
        /// <param name="baseStationId">requested base-station</param>        
        void ChargeDrone(int droneId, int baseStationId);
        /// <summary>
        /// release a drone from charging
        /// </summary>
        /// <param name="droneId">drone's id to release</param>
        void ReleaseDroneFromCharge(int droneId);


        /// <summary>
        /// change the name and the phon number
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="newName"></param>
        /// <param name="newPhone"></param>
        void UpdateCustomer(int customerId, string newName, string newPhone);
        /// <summary>
        /// change the drone model
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="newName"></param>
        void UpdateDroneModel(int droneId, string newName);
        /// <summary>
        /// change the name and the number of slots in base station
        /// </summary>
        /// <param name="baseStationId"></param>
        /// <param name="newName"></param>
        /// <param name="slotsCount"></param>
        void UpdateBaseStation(int baseStationId, string newName, int slotsCount);
       
        
        /// <summary>
        /// delete a base-station by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void DeleteBaseStation(int id);
        /// <summary>
        /// delete a drone by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void DeleteDrone(int id);
        /// <summary>
        /// delete a customer by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void DeleteCustomer(int id);
        /// <summary>
        /// delete a parcel by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        void DeleteParcel(int id);
        
        double[] GetBatteryUsages();
        int GetDroneChargeBaseStationId(int droneId);
        //void BaseStationDroneIn(int baseStationId);
        int GetNextParcelId();
       
    }
}