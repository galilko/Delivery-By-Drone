using IDAL.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IDAL
{
    namespace DO
    {
        public struct Parcel
        {
            public int Id { get; set; }
            public int SenderId { get; set; }
            public int TargetId { get; set; }
            public WeightCategories Weight { get; set; }
            public Priorities Priority { get; set; }
            public DateTime Requested { get; set; }
            public int DroneId { get; set; }
            public DateTime Scheduled { get; set; }
            public DateTime PickedUp { get; set; }
            public DateTime Delivered { get; set; }

            public override string ToString()
            {
                string result = "";
                result += $"Id: {Id} \n ";
                result += $"Sender Id: {SenderId} \n ";
                result += $"Target Id: {TargetId} \n ";
                result += $"Weight: {Weight} \n ";
                result += $"Priority: {Priority} \n ";
                result += $"Requested: {Requested} \n ";
                result += $"Drone Id: {DroneId} \n ";
                result += $"Scheduled: {Scheduled} \n ";
                result += $"Picked Up: {PickedUp} \n ";
                result += $"Delivered: {Delivered} \n ";
                return result;
            }
        }
    }
}
