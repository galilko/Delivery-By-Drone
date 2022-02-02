using BO;
using System;
using System.Collections;
using System.Collections.Generic;


namespace BlApi
{
    public interface IBL
    {
        /// <summary>
        /// adding a new base atation into list through BL
        /// </summary>
        /// <param name="myBaseStation">myBasestion to add</param>
        void AddBaseStation(BaseStation myBaseStation);
        /// <summary>
        /// adding a new customer into list through BL
        /// </summary>
        /// <param name="myCustomer">myCustomer to add</param>
        void AddCustomer(Customer myCustomer);
        /// <summary>
        /// adding a new drone into list through BL
        /// </summary>
        /// <param name="myDrone">myDrone to add</param>
        /// <param name="baseStationId">baseStationId to set the myDrone there</param>
        void AddDrone(DroneToList myDrone, int baseStationId);
        /// <summary>
        /// adding a new parcel into list through BL
        /// </summary>
        /// <param name="myParcel">Parcel to add</param>
        void AddParcel(Parcel myParcel);


        /// <summary>
        /// get IEnumerable of Base Stations through BL
        /// </summary>
        /// <returns>IEnumerable of all Base Stations</returns>
        IEnumerable<BaseStationToList> GetBaseStations();
        /// <summary>
        /// get IEnumerable of Base Stations that have free slots through BL
        /// </summary>
        /// <returns>IEnumerable of Base Stations that have free slots</returns>
        IEnumerable<BaseStationToList> FreeSlotsBaseStations();
        /// <summary>
        /// get IEnumerable of Customers through BL
        /// </summary>
        /// <returns>IEnumerable of all Customers</returns>
        IEnumerable<CustomerToList> GetCustomers();
        /// <summary>
        /// get IEnumerable of Drones through BL
        /// </summary>
        /// <param name="predicate">for sort the drones list. if null returns all.</param>
        /// <returns>IEnumerable of all/Sorted Drones</returns>
        IEnumerable<DroneToList> GetDrones(Func<DroneToList, bool> predicate = null);
        /// <summary>
        /// get IEnumerable of parcels through BL
        /// </summary>
        /// <returns>IEnumerable of all parcels</returns>
        IEnumerable<ParcelToList> GetParcels();
        /// <summary>
        /// get IEnumerable of parcels by status through BL
        /// </summary>
        /// <param name="myStatus">status for sort</param>
        /// <returns>IEnumerable of parcels by status</returns>
        IEnumerable<ParcelToList> ParcelsByStatus(ParcelStatus myStatus);
        /// <summary>
        /// return all parcels that some customer sent
        /// </summary>
        /// <param name="id">customer Id</param>
        /// <returns>IEnumerable of all parcels that some customer sent</returns>
        IEnumerable<ParcelToList> GetSentParcels(int id);
        /// <summary>
        /// return all parcels that some customer recieved
        /// </summary>
        /// <param name="id">customer Id</param>
        /// <returns>IEnumerable of all parcels that some customer sent</returns>
        IEnumerable<ParcelToList> GetRecievedParcels(int id);
        /// <summary>
        /// get all parcels that didn't scheduled
        /// </summary>
        /// <returns>return IEnumerable with all the parcels that didn't scheduled</returns>
        IEnumerable<ParcelToList> NoneScheduledParcels();
        /// <summary>
        /// get parcels that requested between two dates
        /// </summary>
        /// <param name="fromDate">from</param>
        /// <param name="toDate">to</param>
        /// <returns>return IEnumerable with all the parcels that requested between two dates</returns>
        IEnumerable<ParcelToList> ParcelsByDates(DateTime? fromDate, DateTime? toDate);

        /// <summary>
        /// find a base-station by id and return it through BL
        /// </summary>
        /// <param name="baseStationId"> base-stationId to find</param>
        /// <returns></returns>
        BaseStation GetBaseStation(int baseStationId);
        /// <summary>
        /// find a customer by id and return it through BL
        /// </summary>
        /// <param name="customerId">customerId to find</param>
        /// <returns>return if he find one that matching</returns>
        Customer GetCustomer(int customerId);
        /// <summary>
        /// find a drone by id and return it from BL
        /// </summary>
        /// <param name="droneId">droneId to find</param>
        /// <returns>returns if he find one</returns>
        Drone GetDrone(int droneId);
        /// <summary>
        /// find a parcel by id and return it through BL
        /// </summary>
        /// <param name="parcelId">parcelId to find</param>
        /// <returns>returns the parcel that asked</returns>
        Parcel GetParcel(int parcelId);

        /// <summary>
        /// get base-station to list by id and return it through BL
        /// </summary>
        /// <param name="id"> base-station Id to find</param>
        /// <returns>base-station to list</returns>
        BaseStationToList GetBSToList(int id);
        /// <summary>
        /// get customer to list by id and return it through BL
        /// </summary>
        /// <param name="id"> customer Id to find</param>
        /// <returns>customer to list</returns>
        CustomerToList GetCustomerToList(int id);
        /// <summary>
        /// get drone to list by id and return it through BL
        /// </summary>
        /// <param name="id"> drone Id to find</param>
        /// <returns>drone to list</returns>
        DroneToList GetDroneToList(int id);
        /// <summary>
        /// get parcel to list by id and return it through BL
        /// </summary>
        /// <param name="id"> parcel Id to find</param>
        /// <returns>parcel to list</returns>
        ParcelToList GetParcelToList(int id);
        
        void UpdateBaseStation(int baseStationId, string newName, int slotsCount);
        void UpdateCustomer(int customerId, string newName, string newPhone);
        void UpdateDroneModel(int droneId, string newName);

        /// <summary>
        /// delete (inactivate) a base station by Id 
        /// </summary>
        /// <param name="id">base station Id</param>
        void DeleteBaseStation(int id);
        /// <summary>
        /// delete (inactivate) a drone by Id 
        /// </summary>
        /// <param name="id">drone Id</param>
        void DeleteDrone(int id);
        /// <summary>
        /// delete (inactivate) a customer by Id 
        /// </summary>
        /// <param name="id">customer Id</param>
        void DeleteCustomer(int id);
        /// <summary>
        /// delete a parcel by Id 
        /// </summary>
        /// <param name="id">base station Id</param>
        void DeleteParcel(int id);

        /// <summary>
        /// schedule a drone for parcel delivery through BL
        /// </summary>
        /// <param name="parcelId">parcel id for deliver</param>
        /// <param name="droneId">drone id for schedule</param>
        void ScheduleDroneForParcel(int droneId);
        /// <summary>
        /// picking up a parcel by its drone through BL
        /// </summary>
        /// <param name="droneId">droneId to Recognize </param>
        void PickingUpAParcel(int droneId);
        /// <summary>
        /// finish delivery of a parcel through BL
        /// </summary>
        /// <param name="droneId">droneId to Recognize</param>
        void DeliverAParcel(int droneId);
        /// <summary>
        /// get the next parcel id
        /// </summary>
        /// <returns>the next parcel id</returns>
        int GetNextParcelId();

        /// <summary>
        /// connect a drone to charging at base-station throu BL
        /// </summary>
        /// <param name="droneId"> to connect</param>
        void ChargeDrone(int droneId);
        /// <summary>
        /// release a drone from charging through BL
        /// </summary>
        /// <param name="droneId">droneId to release</param>
        /// <param name="tSpanInCharge">tSpanInCharge to calculate the amount of charge</param>
        void releaseDrone(int droneId);

        void StartDroneSimulator(int id, Action update, Func<bool> checkStop);


    }
}