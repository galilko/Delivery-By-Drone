namespace BO
{
    public class ParcelInTransfer
    {
        public int Id { get; set; }
        public CustomerAtParcel Sender { get; set; }
        public CustomerAtParcel Reciever { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public bool Status { get; set; }
        public Location CollectionLocation { get; set; }
        public Location DeliveryDestinationLocation { get; set; }
        public double TransportDistance { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}
