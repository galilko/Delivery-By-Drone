using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Converter
    {
        internal static IDAL.DO.Drone ConvertBlDroneToDalDrone(DroneToList myDrone)
        {
            return new IDAL.DO.Drone()
            {
                Id = myDrone.Id,
                MaxWeight = (IDAL.DO.WeightCategories)myDrone.Weight,
                Model = myDrone.Model
            };
        }

        internal static IDAL.DO.BaseStation ConvertBlBsToDalBs(BaseStation myBaseStation)
        {
            return new IDAL.DO.BaseStation()
            {
                Id = myBaseStation.Id,
                Name = myBaseStation.Name,
                Lattitude = myBaseStation.BSLocation.Latitude,
                Longitude = myBaseStation.BSLocation.Longitude,
                FreeChargeSlots = myBaseStation.FreeChargeSlots
            };
        }

        internal static IDAL.DO.Customer ConvertBlCustomerToDalCustomer(Customer myCustomer)
        {
            return new IDAL.DO.Customer()
            {
                Id = myCustomer.Id,
                Name = myCustomer.Name,
                Phone = myCustomer.PhoneNumber,
                Lattitude = myCustomer.CustomerLocation.Latitude,
                Longitude = myCustomer.CustomerLocation.Longitude
            };
        }

        internal static IDAL.DO.Parcel ConvertBlParcelToDalParcel(Parcel myParcel)
        {
            return new IDAL.DO.Parcel()
            {
                SenderId = myParcel.Sender.Id,
                TargetId = myParcel.Target.Id,
                DroneId = myParcel.DroneAtParcel.Id,
                Priority = (IDAL.DO.Priorities)myParcel.Priority,
                Weight = (IDAL.DO.WeightCategories)myParcel.Weight,
                Requested = myParcel.Requested,
                Scheduled = myParcel.Scheduled,
                PickedUp = myParcel.PickedUp,
                Delivered = myParcel.Delivered,
            };
        }

        internal static ParcelInTransfer ConvertDalParcelToBlParcelInTranfer(IDAL.DO.Parcel parcel, List<IDAL.DO.Customer> customerList)
        {
            ParcelInTransfer pit = new() { Id = parcel.Id };
            IDAL.DO.Customer sender = customerList.Find(x => x.Id == parcel.SenderId);
            pit.Sender = new() { Id = sender.Id, Name = sender.Name }; 
            IDAL.DO.Customer target = customerList.Find(x => x.Id == parcel.TargetId);
            pit.Reciever = new() { Id = target.Id, Name = target.Name };
            pit.Weight = (WeightCategories)parcel.Weight;
            pit.Priority = (Priorities)parcel.Priority;
            if (parcel.PickedUp == DateTime.MinValue)
                pit.Status = false;
            else
                pit.Status = true;
            pit.CollectionLocation = new() { Latitude = sender.Lattitude, Longitude = sender.Longitude };
            pit.DeliveryDestinationLocation = new() { Latitude = target.Lattitude, Longitude = target.Longitude };
            pit.TransportDistance = LocationFuncs.DistanceBetweenTwoLocations(pit.CollectionLocation, pit.DeliveryDestinationLocation);
            return pit;
        }

        internal static ParcelStatus CalculateParcelStatus(IDAL.DO.Parcel parcel)
        {
            if (parcel.Delivered != DateTime.MinValue)
                return ParcelStatus.Delivered;
            else if (parcel.PickedUp != DateTime.MinValue)
                return ParcelStatus.PickedUp;
            else if (parcel.Scheduled != DateTime.MinValue)
                return ParcelStatus.Scheduled;
            else
                return ParcelStatus.Defined;
        }
    }
}
