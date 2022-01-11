using BO;
using DalApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BL
{
    public class Converter
    {
        internal static DO.Drone GetDalDrone(DroneToList myDrone)
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
                Latitude = (double)myBaseStation.Location.Latitude,
                Longitude = (double)myBaseStation.Location.Longitude,
                FreeChargeSlots = (int)myBaseStation.FreeChargeSlots,
                IsActive = true
            };
        }

        internal static DO.Customer GetDalCustomer(Customer myCustomer)
        {
            return new DO.Customer()
            {
                Id = myCustomer.Id,
                Name = myCustomer.Name,
                Phone = myCustomer.PhoneNumber,
                Latitude = (double)myCustomer.Location.Latitude,
                Longitude = (double)myCustomer.Location.Longitude,
                IsActive = true
            };
        }

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
            pit.CollectionLocation = new() { Latitude = sender.Latitude, Longitude = sender.Longitude };
            pit.DeliveryDestinationLocation = new() { Latitude = target.Latitude, Longitude = target.Longitude };
            pit.TransportDistance = new Customer() { Location = pit.CollectionLocation }.Distance(new Customer() { Location = pit.DeliveryDestinationLocation });
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

        internal static Parcel GetBlParcel(DO.Parcel dalParcel, IDal myDal, List<DroneToList> blDrones)
        {
            Parcel blParcel = new()
            {
                Id = dalParcel.Id,
                Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = myDal.GetCustomer(dalParcel.SenderId).Name },
                Target = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = myDal.GetCustomer(dalParcel.TargetId).Name },
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

        internal static ParcelToList GetParcelToList(DO.Parcel parcel, IDal myDal)
        {
            return new ParcelToList()
            {
                Id = parcel.Id,
                SenderName = myDal.GetCustomer(parcel.SenderId).Name,
                TargetName = myDal.GetCustomer(parcel.TargetId).Name,
                Weight = (WeightCategories)parcel.Weight,
                Priority = (Priorities)parcel.Priority,
                Status = Converter.CalculateParcelStatus(parcel)
            };
        }

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

        internal static BaseStation GetBlBS(DO.BaseStation dalBs, IDal myDal, List<DroneToList> blDrones)
        {
            BaseStation blBS = new BaseStation()
            {
                Id = dalBs.Id,                                                                // find the wanted BaseStation and fit him to BL BaseStation
                Name = dalBs.Name,
                Location = new Location() { Latitude = dalBs.Latitude, Longitude = dalBs.Longitude },
                FreeChargeSlots = dalBs.FreeChargeSlots,
                DronesInCharge = new()
            };
            List<DO.DroneCharge> dronesInCharge = myDal.GetListOfInChargeDrones().Where(x => x.StationId == blBS.Id).ToList(); //update the blBs.DronesInCharge with all the drones that chargeing on the BaseStation that found 
            foreach (var droneCharge in dronesInCharge)
            {
                blBS.DronesInCharge.Add(new DroneInCharge()
                {
                    Id = droneCharge.DroneId,
                    BatteryStatus = blDrones.Find(x => x.Id == droneCharge.DroneId).BatteryStatus
                });
            }
            return blBS;
        }

        internal static BaseStationToList GetBSToList(DO.BaseStation bs, IDal myDal)
        {
            return new BaseStationToList()
            {
                Id = bs.Id,
                Name = bs.Name,
                FreeChargeSlots = bs.FreeChargeSlots,
                BusyChargeSlots = myDal.GetListOfInChargeDrones().Where(x => x.StationId == bs.Id).Count()
            };
        }

        internal static Customer GetBlCustomer(DO.Customer dalCustomer, IDal myDal)
        {
            Customer blCustomer = new()
            {
                Id = dalCustomer.Id,
                Name = dalCustomer.Name,
                Location = new Location() { Latitude = dalCustomer.Latitude, Longitude = dalCustomer.Longitude },
                PhoneNumber = dalCustomer.Phone
            };
            blCustomer.ParcelFromCustomerList = new List<ParcelAtCustomer>();
            foreach (var item in myDal.GetParcels().Where(x => x.SenderId == blCustomer.Id)) // creat a list into  blCustomer of all the parcel that he hever send and all the details of hevry parcel
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
                        Name = myDal.GetCustomer(item.TargetId).Name
                    }
                });
            }
            blCustomer.ParcelToCustomerList = new List<ParcelAtCustomer>();
            foreach (var item in myDal.GetParcels().Where(x => x.TargetId == blCustomer.Id))//creat a list into  blCustomer of all the parcel that he hever have received and all the details of hevry parcel
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
                        Name = myDal.GetCustomer(item.SenderId).Name
                    }
                });
            }
            return blCustomer;
        }

        internal static CustomerToList GetCustomerToList(DO.Customer customer, IDal myDal)
        {
            return new CustomerToList()
            {
                Id = customer.Id,
                Name = customer.Name,
                PhoneNumber = customer.Phone,
                SentAndSuppliedParcels = myDal.GetParcels().Where(x => x.SenderId == customer.Id && x.Delivered != null).Count(),
                SentAndNotSuppliedParcels = myDal.GetParcels().Where(x => x.SenderId == customer.Id && x.Delivered == null).Count(),
                RecievedParcels = myDal.GetParcels().Where(x => x.TargetId == customer.Id && x.Delivered != null).Count(),
                InProcessParcelsToCustomer = myDal.GetParcels().Where(x => x.TargetId == customer.Id && x.Delivered == null).Count(),
            };
        }

        internal static ParcelInTransfer GetParcelInTransfer(DO.Parcel dalParcel, IDal myDal)
        {
            ParcelInTransfer parcelInTransfer = new()
            {
                Id = dalParcel.Id,                                                                                                 // update the details in  ParcelInTransfer 
                Sender = new CustomerAtParcel() { Id = dalParcel.SenderId, Name = myDal.GetCustomer(dalParcel.SenderId).Name },
                Reciever = new CustomerAtParcel() { Id = dalParcel.TargetId, Name = myDal.GetCustomer(dalParcel.TargetId).Name },
                Weight = (WeightCategories)dalParcel.Weight,
                CollectionLocation = new() { Latitude = myDal.GetCustomer(dalParcel.SenderId).Latitude, Longitude = myDal.GetCustomer(dalParcel.SenderId).Longitude },
                DeliveryDestinationLocation = new() { Latitude = myDal.GetCustomer(dalParcel.TargetId).Latitude, Longitude = myDal.GetCustomer(dalParcel.TargetId).Longitude },
                Priority = (Priorities)dalParcel.Priority,
            };
            parcelInTransfer.TransportDistance = new Customer() { Location = parcelInTransfer.CollectionLocation }.Distance(new Customer() { Location = parcelInTransfer.DeliveryDestinationLocation }); // use in Distance calculation function that creat for calculat distance between two locations
            if (dalParcel.PickedUp == null) // update the status of  parcelInTransfer to be false if the parcel not pickup or true if are pickup                                                                                                                      
                parcelInTransfer.Status = false;
            else
                parcelInTransfer.Status = true;
            return parcelInTransfer;
        }

        internal static Drone GetBlDrone(DroneToList dtl, IDal myDal)
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
                DO.Parcel dalParcel = myDal.GetParcels().First(x => x.DroneId == drone.Id && x.Delivered == null);
                ParcelInTransfer parcelInTransfer = Converter.GetParcelInTransfer(dalParcel, myDal);
                drone.CurrentParcel = parcelInTransfer;
            }
            return drone;
        }
    }
}
