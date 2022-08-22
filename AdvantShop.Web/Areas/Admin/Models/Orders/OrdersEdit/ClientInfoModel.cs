using AdvantShop.Customers;
using AdvantShop.Orders;
using System.Collections.Generic;

namespace AdvantShop.Web.Admin.Models.Orders.OrdersEdit
{
    public class ClientInfoModel
    {
        public Customer Customer { get; set; }
        public Order Order { get; set; }
        public List<string> InterestingCategories { get; set; }
        public string CustomerGroup { get; set; }
        public string CustomerId { get; set; }
        public int OrderId { get; set; }
        public string Segment { get; set; }
        public ClientStatistic Statistic { get; set; }
    }

    public class ClientStatistic
    {
        public string AdminCommentAboutCustomer { get; set; }
        public string CustomerId { get; set; }
        public int OrdersCount { get; set; }
        public int OrderId { get; set; }
        public string RegistrationDuration { get; internal set; }
        public string OrdersSum { get; set; }
        public string AverageCheck { get; set; }
        public string RegistrationDate { get; set; }
    }

}
