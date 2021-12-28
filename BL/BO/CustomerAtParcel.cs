namespace BO
{
    public class CustomerAtParcel
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public override string ToString() => $"Id: {Id}\tName: {Name}";
    }
}
