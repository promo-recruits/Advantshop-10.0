using System;
using AdvantShop.Customers;

namespace AdvantShop.Core.Services.ChangeHistories
{
    public class ChangedBy
    {
        public string Name { get; set; }
        public Guid? CustomerId { get; set; }
        public DateTime ModificationTime { get; set; }


        public ChangedBy(string name)
        {
            Name = name;
            ModificationTime = DateTime.Now;
        }

        public ChangedBy(Customer customer)
        {
            if (customer != null)
            {
                Name = customer.FirstName + " " + customer.LastName;
                CustomerId = customer.Id;
            }
            ModificationTime = DateTime.Now;
        }
    }
}
