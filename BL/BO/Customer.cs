using System.Collections.Generic;

namespace BO
{
    public class Customer
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Location CustomerLocation { get; set; }
        public List<ParcelAtCustomer> ParcelFromCustomerList { get; set; }
        public List<ParcelAtCustomer> ParcelToCustomerList { get; set; }
        public override string ToString()
        {
            string result = "";
            result += $"Id:\t\t\t {Id}\n";
            result += $"Name:\t\t\t {Name}\n";
            result += $"Phone Number:\t\t {PhoneNumber}\n";
            result += $"Location:\t\t {CustomerLocation}\n";
            result += "******Parcels from customer******\n";
            if (ParcelFromCustomerList.Count > 0)
                foreach (var item in ParcelFromCustomerList)
                    result += $"{item.ToString()}\n";
            else
                result += "There aren't parcels from this customer\n";
            result += "******Parcels to customer******\n";
            if (ParcelToCustomerList.Count > 0)
                foreach (var item in ParcelToCustomerList)
                    result += $"{item.ToString()}\n";
            else
                result += "There aren't parcels to this customer\n";
            return result;
        }
    }
}
