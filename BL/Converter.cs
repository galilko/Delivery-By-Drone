using BO;
using System.Collections.Generic;

namespace BlApi
{
    public class Converter
    {
        internal static DO.Drone ConvertBlDroneToDalDrone(DroneToList myDrone)
        {
            return new DO.Drone()
            {
                Id = myDrone.Id,
                MaxWeight = (DO.WeightCategories)myDrone.Weight,
                Model = myDrone.Model
            };
        }

        internal static DO.BaseStation ConvertBlBsToDalBs(BaseStation myBaseStation)
        {
            return new DO.BaseStation()
            {
                Id = (int)myBaseStation.Id,
                Name = myBaseStation.Name,
                Lattitude = (double)myBaseStation.BSLocation.Latitude,
                Longitude = (double)myBaseStation.BSLocation.Longitude,
                FreeChargeSlots = (int)myBaseStation.FreeChargeSlots,
                IsActive = true
            };
        }

        internal static DO.Customer ConvertBlCustomerToDalCustomer(Customer myCustomer)
        {
            return new DO.Customer()
            {
                Id = myCustomer.Id,
                Name = myCustomer.Name,
                Phone = myCustomer.PhoneNumber,
                Lattitude = (double)myCustomer.CustomerLocation.Latitude,
                Longitude = (double)myCustomer.CustomerLocation.Longitude,
                IsActive = true
            };
        }

        internal static DO.Parcel ConvertBlParcelToDalParcel(Parcel myParcel)
        {
            return new DO.Parcel()
            {
                SenderId = myParcel.Sender.Id,
                TargetId = myParcel.Target.Id,
                DroneId = myParcel.DroneAtParcel.Id,
                Priority = (DO.Priorities)myParcel.Priority,
                Weight = (DO.WeightCategories)myParcel.Weight,
                Requested = myParcel.Requested,
                Scheduled = myParcel.Scheduled,
                PickedUp = myParcel.PickedUp,
                Delivered = myParcel.Delivered,
            };
        }

        internal static ParcelInTransfer ConvertDalParcelToBlParcelInTranfer(DO.Parcel parcel, List<DO.Customer> customerList)
        {
            ParcelInTransfer pit = new() { Id = parcel.Id };
            DO.Customer sender = customerList.Find(x => x.Id == parcel.SenderId);
            pit.Sender = new() { Id = sender.Id, Name = sender.Name };
            DO.Customer target = customerList.Find(x => x.Id == parcel.TargetId);
            pit.Reciever = new() { Id = target.Id, Name = target.Name };
            pit.Weight = (WeightCategories)parcel.Weight;
            pit.Priority = (Priorities)parcel.Priority;
            if (parcel.PickedUp == null)
                pit.Status = false;
            else
                pit.Status = true;
            pit.CollectionLocation = new() { Latitude = sender.Lattitude, Longitude = sender.Longitude };
            pit.DeliveryDestinationLocation = new() { Latitude = target.Lattitude, Longitude = target.Longitude };
            pit.TransportDistance = LocationFuncs.DistanceBetweenTwoLocations(pit.CollectionLocation, pit.DeliveryDestinationLocation);
            return pit;
        }

        internal static ParcelStatus CalculateParcelStatus(DO.Parcel parcel)
        {
            if (parcel.Delivered != null)
                return ParcelStatus.Delivered;
            else if (parcel.PickedUp != null)
                return ParcelStatus.PickedUp;
            else if (parcel.Scheduled != null)
                return ParcelStatus.Scheduled;
            else
                return ParcelStatus.Defined;
        }
    }
}
