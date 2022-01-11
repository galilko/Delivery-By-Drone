using DO;
using System;
using System.Collections.Generic;

namespace DalApi
{
    public interface IDal
    {
        void AddBaseStation(BaseStation newBaseStation);
        void AddCustomer(Customer newCustomer);
        void AddDrone(Drone newDrone);
        void AddParcel(Parcel newParcel);
        IEnumerable<BaseStation> GetBaseStations(Func<BaseStation, bool> predicate = null);
        IEnumerable<Customer> GetCustomers(Func<Customer, bool> predicate = null);
        IEnumerable<Drone> GetDrones();
        IEnumerable<Parcel> GetParcels(Func<Parcel, bool> predicate = null);
        void ChargeDrone(int? droneId, int baseStationId);
        void DeliverAParcel(int parcelId);
        BaseStation GetBaseStation(int id);
        Customer GetCustomer(int? id);
        Drone GetDrone(int? id);
        Parcel GetParcel(int id);
        void DeleteBaseStation(int id);
        IEnumerable<BaseStation> FreeSlotsBaseStations();
        IEnumerable<Parcel> NoneScheduledParcels();
        void PickingUpAParcel(int parcelId);
        void ReleaseDroneFromCharge(int? droneId);
        void ScheduleDroneForParcel(int parcelId, int? droneId);
        double[] GetBatteryUse();
        void UpdateCustomer(int? customerId, string newName, string newPhone);
        IEnumerable<DroneCharge> GetListOfInChargeDrones(Func<DroneCharge, bool> predicate = null);
        void UpdateDroneModel(int? droneId, string newName);
        void UpdateBaseStation(int? baseStationId, string newName, int slotsCount);
        void DeleteDrone(int? id);
        void DeleteCustomer(int id);
        void DeleteParcel(int id);
        int GetNextParcelId();
    }
}