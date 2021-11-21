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
    }
}
