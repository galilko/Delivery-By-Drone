using BO;
using System;
using System.Collections.Generic;


namespace BlApi
{
    public interface IBL
    {
        void AddBaseStation(BaseStation myBaseStation);
        void AddCustomer(Customer myCustomer);
        void AddDrone(DroneToList myDrone, int baseStationId);
        void AddParcel(Parcel myParcel);
        IEnumerable<BaseStationToList> AllBlBaseStations();
        IEnumerable<CustomerToList> AllBlCustomers();
        IEnumerable<DroneToList> AllBlDrones(Func<DroneToList, bool> predicate = null);
        IEnumerable<ParcelToList> AllBlParcels();
        IEnumerable<ParcelToList> ParcelsByStatus(ParcelStatus myStatus);
        void ChargeDrone(int? droneId);
        void DeliverAParcel(int? droneId);
        BaseStation FindBaseStation(int baseStationId);
        Customer FindCustomer(int? customerId);
        Drone FindDrone(int? droneId);
        Parcel FindParcel(int parcelId);
        void DeleteBaseStation(int id);
        IEnumerable<BaseStationToList> FreeSlotsBaseStations();
        IEnumerable<ParcelToList> NoneScheduledParcels();
        void PickingUpAParcel(int? droneId);
        void releaseDrone(int? droneId, TimeSpan tSpanInCharge);
        void ScheduleDroneForParcel(int? droneId);
        void UpdateBaseStation(int baseStationId, string newName, int slotsCount);
        void UpdateCustomer(int? customerId, string newName, string newPhone);
        void UpdateDroneModel(int? droneId, string newName);
    }
}