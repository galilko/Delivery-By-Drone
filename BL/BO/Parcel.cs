using System;

namespace BO
{
    public class Parcel
    {
        public int Id { get; set; }
        public CustomerAtParcel Sender { get; set; }
        public CustomerAtParcel Target { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public DroneAtParcel DroneAtParcel { get; set; }
        public DateTime? Requested { get; set; }
        public DateTime? Scheduled { get; set; }
        public DateTime? PickedUp { get; set; }
        public DateTime? Delivered { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}
