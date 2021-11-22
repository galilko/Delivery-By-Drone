using IDAL.DO;
using System.Collections.Generic;

namespace IDAL
{
    public interface IDal
    {
        void AddBaseStation(BaseStation newBaseStation);
        void AddCustomer(Customer newCustomer);
        void AddDrone(Drone newDrone);
        void AddParcel(Parcel newParcel);
        IEnumerable<BaseStation> AllBaseStations();
        IEnumerable<Customer> AllCustomers();
        IEnumerable<Drone> AllDrones();
        IEnumerable<Parcel> AllParcels();
        void ChargeDrone(int droneId, int baseStationId);
        void DeliverAParcel(int parcelId);
        BaseStation FindBaseStation(int id);
        Customer FindCustomer(int id);
        Drone FindDrone(int id);
        Parcel FindParcel(int id);
        IEnumerable<BaseStation> FreeSlotsBaseStations();
        IEnumerable<Parcel> NoneScheduledParcels();
        void PickingUpAParcel(int parcelId);
        void ReleaseDroneFromCharge(int droneId);
        void ScheduleDroneForParcel(int parcelId, int droneId);
        double[] GetBatteryUse();
        List<DroneCharge> GetListOfInChargeDrones();
        void UpdateDroneModel(int droneId, string newName);
        void UpdateBaseStation(int baseStationId, string newName, int slotsCount);
        void UpdateCustomer(int customerId, string newName, string newPhone);
    }
}