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
        IEnumerable<BaseStation> AllBaseStations(Func<BaseStation, bool> predicate = null);
        IEnumerable<Customer> AllCustomers(Func<Customer, bool> predicate = null);
        IEnumerable<Drone> AllDrones();
        IEnumerable<Parcel> AllParcels(Func<Parcel, bool> predicate = null);
        void ChargeDrone(int? droneId, int baseStationId);
        void DeliverAParcel(int parcelId);
        BaseStation FindBaseStation(int id);
        Customer FindCustomer(int? id);
        Drone FindDrone(int? id);
        Parcel FindParcel(int id);
        void DeleteBaseStation(int id);
        IEnumerable<BaseStation> FreeSlotsBaseStations();
        IEnumerable<Parcel> NoneScheduledParcels();
        void PickingUpAParcel(int parcelId);
        void ReleaseDroneFromCharge(int? droneId);
        void ScheduleDroneForParcel(int parcelId, int? droneId);
        double[] GetBatteryUse();
        void UpdateCustomer(int? customerId, string newName, string newPhone);
        IEnumerable<DroneCharge> GetListOfInChargeDrones();
        void UpdateDroneModel(int? droneId, string newName);
        void UpdateBaseStation(int baseStationId, string newName, int slotsCount);
        void DeleteDrone(int? id);
        void DeleteCustomer(int id);
        void DeleteParcel(int id);
    }
}