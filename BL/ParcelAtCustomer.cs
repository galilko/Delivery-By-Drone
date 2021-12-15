namespace BO
{
    public class ParcelAtCustomer
    {
        public int Id { get; set; }
        public WeightCategories Weight { get; set; }
        public Priorities Priority { get; set; }
        public ParcelStatus Status { get; set; }
        public CustomerAtParcel CustomerAtParcel { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}
