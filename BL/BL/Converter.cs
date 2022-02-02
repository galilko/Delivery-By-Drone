using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    internal class Converter
    {
        internal static IDal dal = DalFactory.GetDal();
        /// <summary>
        /// Convert DroneToList To Dal Drone
        /// </summary>
        /// <param name="myDrone"></param>
        /// <returns>Dal Drone</returns>
        internal static DO.Drone GetDalDrone(DroneToList myDrone)
        {
            return new DO.Drone()
            {
                Id = (int)myDrone.Id,
                MaxWeight = (DO.WeightCategories)myDrone.Weight,
                Model = myDrone.Model
            };
        }
        /// <summary>
        /// Convert BL Customer to Dal Customer
        /// </summary>
        /// <param name="myCustomer"></param>
        /// <returns>Dal Customer</returns>
        internal static DO.Customer GetDalCustomer(Customer myCustomer)
        {
                return new DO.Customer()
            {
                Id = (int)myCustomer.Id,
                Name = myCustomer.Name,
                Phone = myCustomer.PhoneNumber,
                Latitude = (double)myCustomer.Location.Latitude,
                Longitude = (double)myCustomer.Location.Longitude,
                IsActive = true
            };
        }
        /// <summary>
        /// Convert BL Parcel to Dal Parcel
        /// </summary>
        /// <param name="myParcel"></param>
        /// <returns>Dal Parcel</returns>
        internal static DO.Parcel GetDalParcel(Parcel myParcel)
        {
            myParcel.Requested = DateTime.Now;
            myParcel.Scheduled = myParcel.PickedUp = myParcel.Delivered = null;
            myParcel.DroneAtParcel = new DroneAtParcel();
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
        /// <summary>
        /// Calculate BL Parcel status according to Dal Parcel times
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns>Parcel status</returns>
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
        /// <summary>
        /// Convert Dal Parcel to BL Parcel
        /// </summary>
        /// <param name="dalParcel"></param>
        /// <param name="blDrones"></param>
        /// <returns>BL Parcel</returns>
        internal static Parcel GetBlParcel(DO.Parcel dalParcel, List<DroneToList> blDrones)
        {
            Parcel blParcel = new()
            {
                Id = dalParcel.Id,
                Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = dal.GetCustomer(dalParcel.SenderId).Name },
                Target = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = dal.GetCustomer(dalParcel.TargetId).Name },
                Weight = (WeightCategories)dalParcel.Weight,
                Priority = (Priorities)dalParcel.Priority,
                Requested = dalParcel.Requested,
                Scheduled = dalParcel.Scheduled,
                PickedUp = dalParcel.PickedUp,
                Delivered = dalParcel.Delivered
            };

            if (blParcel.Scheduled != null)
            {
                blParcel.DroneAtParcel = new DroneAtParcel()
                {
                    Id = dalParcel.DroneId,
                    BatteryStatus = blDrones.Find(x => x.Id == dalParcel.DroneId).BatteryStatus,
                    Location = blDrones.Find(x => x.Id == dalParcel.DroneId).Location
                };
            }
            return blParcel;
        }
        /// <summary>
        /// Convert Dal Parcel to ParcelToList
        /// </summary>
        /// <param name="parcel"></param>
        /// <returns>ParcelToList</returns>
        internal static ParcelToList GetParcelToList(DO.Parcel parcel)
        {
            return new ParcelToList()
            {
                Id = parcel.Id,
                SenderName = dal.GetCustomer(parcel.SenderId).Name,
                TargetName = dal.GetCustomer(parcel.TargetId).Name,
                Weight = (WeightCategories)parcel.Weight,
                Priority = (Priorities)parcel.Priority,
                Status = Converter.CalculateParcelStatus(parcel)
            };
        }
        /// <summary>
        /// Convert BL Base-Station To Dal Base-Station
        /// </summary>
        /// <param name="myBaseStation"></param>
        /// <returns>Dal Base-Station</returns>
        internal static DO.BaseStation GetDalBS(BaseStation myBaseStation)
        {
            return new DO.BaseStation()
            {
                Id = (int)myBaseStation.Id,
                Name = myBaseStation.Name,
                Latitude = (double)myBaseStation.Location.Latitude,
                Longitude = (double)myBaseStation.Location.Longitude,
                FreeChargeSlots = (int)myBaseStation.FreeChargeSlots,
                IsActive = true
            };// use with AddBaseStation from mydal to set basestation from the user to datasorce        }
        }
        /// <summary>
        /// Convert Dal Base-Station To BL Base-Station
        /// </summary>
        /// <param name="dalBs"></param>
        /// <param name="blDrones"></param>
        /// <returns></returns>
        internal static BaseStation GetBlBS(DO.BaseStation dalBs, List<DroneToList> blDrones)
        {
            BaseStation blBS = new BaseStation()
            {
                Id = dalBs.Id,                                                                // find the wanted BaseStation and fit him to BL BaseStation
                Name = dalBs.Name,
                Location = new Location() { Latitude = dalBs.Latitude, Longitude = dalBs.Longitude },
                FreeChargeSlots = dalBs.FreeChargeSlots,
                DronesInCharge = new()
            };

            List<DO.DroneCharge?> dronesInCharge = dal.GetDronesInCharge(x => x?.StationId == blBS.Id).ToList(); //update the blBs.DronesInCharge with all the drones that chargeing on the BaseStation that found 
            foreach (DO.DroneCharge droneCharge in dronesInCharge)
            {
                blBS.DronesInCharge.Add(new DroneInCharge()
                {
                    Id = droneCharge.DroneId,
                    BatteryStatus = blDrones.Find(x => x.Id == droneCharge.DroneId).BatteryStatus
                });
            }
            return blBS;
        }
        /// <summary>
        /// Convert Dal Base-Station To BaseStationToList
        /// </summary>
        /// <param name="bs"></param>
        /// <returns>BaseStationToList</returns>
        internal static BaseStationToList GetBSToList(DO.BaseStation bs)
        {
            return new BaseStationToList()
            {
                Id = bs.Id,
                Name = bs.Name,
                FreeChargeSlots = bs.FreeChargeSlots,
                BusyChargeSlots = dal.GetDronesInCharge(x => x?.StationId == bs.Id).Count()
            };
        }
        /// <summary>
        /// Convert Dal Customer To BL Customer
        /// </summary>
        /// <param name="dalCustomer"></param>
        /// <returns>BL Customer</returns>
        internal static Customer GetBlCustomer(DO.Customer dalCustomer)
        {
            Customer blCustomer = new()
            {
                Id = dalCustomer.Id,
                Name = dalCustomer.Name,
                Location = new Location() { Latitude = dalCustomer.Latitude, Longitude = dalCustomer.Longitude },
                PhoneNumber = dalCustomer.Phone
            };
            blCustomer.ParcelFromCustomerList = new List<ParcelAtCustomer>();
            foreach (DO.Parcel item in dal.GetParcels().Where(x => x?.SenderId == blCustomer.Id)) // creat a list into  blCustomer of all the parcel that he hever send and all the details of hevry parcel
            {
                blCustomer.ParcelFromCustomerList.Add(new ParcelAtCustomer()
                {
                    Id = item.Id,
                    Weight = (WeightCategories)item.Weight,
                    Priority = (Priorities)item.Priority,
                    Status = Converter.CalculateParcelStatus(item),
                    CustomerAtParcel = new CustomerAtParcel()
                    {
                        Id = item.TargetId,
                        Name = dal.GetCustomer(item.TargetId).Name
                    }
                });
            }
            blCustomer.ParcelToCustomerList = new List<ParcelAtCustomer>();
            foreach (DO.Parcel item in dal.GetParcels(x => x?.TargetId == blCustomer.Id))//creat a list into  blCustomer of all the parcel that he hever have received and all the details of hevry parcel
            {
                blCustomer.ParcelToCustomerList.Add(new ParcelAtCustomer()
                {
                    Id = item.Id,
                    Weight = (WeightCategories)item.Weight,
                    Priority = (Priorities)item.Priority,
                    Status = Converter.CalculateParcelStatus(item),
                    CustomerAtParcel = new CustomerAtParcel()
                    {
                        Id = item.SenderId,
                        Name = dal.GetCustomer(item.SenderId).Name
                    }
                });
            }
            return blCustomer;
        }
        /// <summary>
        /// Convert Dal Customer To BL CustomerToList
        /// </summary>
        /// <param name="customer"></param>
        /// <returns>CustomerToList</returns>
        internal static CustomerToList GetCustomerToList(DO.Customer customer)
        {
            return new CustomerToList()
            {
                Id = customer.Id,
                Name = customer.Name,
                PhoneNumber = customer.Phone,
                SentAndSuppliedParcels = dal.GetParcels(x => x?.SenderId == customer.Id && x?.Delivered != null).Count(),
                SentAndNotSuppliedParcels = dal.GetParcels(x => x?.SenderId == customer.Id && x?.Delivered == null).Count(),
                RecievedParcels = dal.GetParcels(x => x?.TargetId == customer.Id && x?.Delivered != null).Count(),
                InProcessParcelsToCustomer = dal.GetParcels().Where(x => x?.TargetId == customer.Id && x?.Delivered == null).Count(),
            };
        }
        /// <summary>
        /// Convert Dal Parcel to ParcelInTransfer
        /// </summary>
        /// <param name="dalParcel"></param>
        /// <returns>ParcelInTransfer</returns>
        internal static ParcelInTransfer GetParcelInTransfer(DO.Parcel dalParcel)
        {
            ParcelInTransfer parcelInTransfer = new()
            {
                Id = dalParcel.Id,                                                                                                 // update the details in  ParcelInTransfer 
                Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = dal.GetCustomer(dalParcel.SenderId).Name },
                Reciever = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = dal.GetCustomer(dalParcel.TargetId).Name },
                Weight = (WeightCategories)dalParcel.Weight,
                CollectionLocation = new() { Latitude = dal.GetCustomer(dalParcel.SenderId).Latitude, Longitude = dal.GetCustomer(dalParcel.SenderId).Longitude },
                DeliveryDestinationLocation = new() { Latitude = dal.GetCustomer(dalParcel.TargetId).Latitude, Longitude = dal.GetCustomer(dalParcel.TargetId).Longitude },
                Priority = (Priorities)dalParcel.Priority,
            };
            parcelInTransfer.TransportDistance = new Customer() { Location = parcelInTransfer.CollectionLocation }.Distance(new Customer() { Location = parcelInTransfer.DeliveryDestinationLocation }); // use in Distance calculation function that creat for calculat distance between two locations
            if (dalParcel.PickedUp == null) // update the status of  parcelInTransfer to be false if the parcel not pickup or true if are pickup                                                                                                                      
                parcelInTransfer.Status = false;
            else
                parcelInTransfer.Status = true;
            return parcelInTransfer;
        }
        /// <summary>
        /// Convert DroneToList to BL Drone
        /// </summary>
        /// <param name="dtl"></param>
        /// <returns>BL Drone</returns>
        internal static Drone GetBlDrone(DroneToList dtl)
        {
            Drone drone = new()
            {
                Id = dtl.Id,
                Model = dtl.Model,
                Weight = dtl.Weight,
                BatteryStatus = dtl.BatteryStatus,
                Status = dtl.Status,
                Location = dtl.Location
            };
            if (drone.Status == DroneStatusCategories.Delivery) 
            {
                DO.Parcel dalParcel = (DO.Parcel)dal.GetParcels().First(x => x?.DroneId == drone.Id && x?.Delivered == null);
                ParcelInTransfer parcelInTransfer = Converter.GetParcelInTransfer(dalParcel);
                drone.CurrentParcel = parcelInTransfer;
            }
            return drone;
        }
    }
}
