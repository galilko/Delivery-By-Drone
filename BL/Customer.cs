using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IBL.BO
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Location CustomerLocation { get; set; }
        public List<ParcelAtCustomer> ParcelFromCustomerList { get; set; }
        public List<ParcelAtCustomer> ParcelToCustomerList { get; set; }
    }
}
