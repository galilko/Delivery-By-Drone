namespace BO
{
    public class CustomerToList
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public int SentAndSuppliedParcels { get; set; }
        public int SentAndNotSuppliedParcels { get; set; }
        public int RecievedParcels { get; set; }
        public int InProcessParcelsToCustomer { get; set; }
        public override string ToString() => ToolStringClass.ToStringProperty(this);
    }
}
