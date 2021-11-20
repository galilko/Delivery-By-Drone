using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    class Converter
    {
        public static List<DroneToList> ConvertDroneTolist(List<IDAL.DO.Drone> Drones , List<IDAL.DO.Parcel> Parcels, List<IDAL.DO.Customer> customers, List<IDAL.DO.BaseStation> BaseStations)
        {
            List<DroneToList> temp;
            foreach (var item in Drones)
            {
                Drone a = new Drone();
                a.Id = item.Id;
                a.Model = item.Model;
                a.Weight = (WeightCategories)item.MaxWeight;
                var temp1 = Parcels.Find(temp => temp.DroneId == a.Id);
                if (!temp1.Equals(default(IDAL.DO.Parcel)))
                {
                    a.Status = DroneStatusCategories.Delivery;
                    if(!temp1.PickedUp.IsDaylightSavingTime())
                    {
                        var temp2 = customers.Find(temp => temp.Id == temp1.SenderId);
                        var temp3 = BaseStations.

                    }

                    

                  
                }
            }
    }
}
