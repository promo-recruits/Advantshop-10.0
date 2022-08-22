using System;
using System.Collections.Generic;
using AdvantShop.Customers;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Models.Shared.Smses
{
    public class SendSmsModel
    {
        public Guid? CustomerId { get; set; }
        public string Phone { get; set; }
        public int? OrderId { get; set; }
        public int? LeadId { get; set; }

        public List<Guid> CustomerIds { get; set; }
        public List<int> SubscriptionIds { get; set; }

        public string Text { get; set; }

        public string PageType { get; set; }
    }


    public class SendSmsModelItem
    {
        public Customer Customer { get; set; }
        public long? Phone { get; set; }

        public SendSmsModelItem() { }

        public SendSmsModelItem(Customer c)
        {
            Customer = c;
            Phone = c.StandardPhone ?? StringHelper.ConvertToStandardPhone(c.Phone, true, true);
        }

        public SendSmsModelItem(AdvantShop.Customers.Subscription s)
        {
            Phone = s.StandardPhone;
            Customer = s.CustomerId.HasValue ? new Customer
            {
                FirstName = s.FirstName,
                LastName = s.LastName
            } : null;
        }
    }

    public class SendSmsModelItemComparer : IEqualityComparer<SendSmsModelItem>
    {
        public bool Equals(SendSmsModelItem x, SendSmsModelItem y)
        {
            return x.Phone == y.Phone;
        }

        public int GetHashCode(SendSmsModelItem obj)
        {
            return (obj.Phone ?? 0).GetHashCode();
        }
    }

}
