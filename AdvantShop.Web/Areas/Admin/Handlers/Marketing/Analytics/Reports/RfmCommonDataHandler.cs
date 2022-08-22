using System;
using System.Linq;
using AdvantShop.Orders;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class RfmCommonDataHandler
    {
        private readonly DateTime _from;
        private readonly DateTime _to;

        public RfmCommonDataHandler(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        public object GetData()
        {
            var customers = AdvantShop.Customers.CustomerService.GetCustomers(_from, _to);
            var customersByOrderDate = AdvantShop.Customers.CustomerService.GetCustomersCountByOrderDate(_from, _to);
            var moreOneOrderCount = OrderStatisticsService.GetCustomersCountWithMoreOneOrder(_from, _to);
            var blackListCount = customers != null ? customers.Count(i => i.ClientStatus == AdvantShop.Customers.CustomerClientStatus.Bad) : 0;
            var vipCount = customers != null ? customers.Count(i => i.ClientStatus == AdvantShop.Customers.CustomerClientStatus.Vip) : 0;
            var lifetimeValue = OrderStatisticsService.GetLifetimeValue(_from, _to);

            return new
            {
                allCount = customers.Count,
                vipCount,
                blackListCount,
                moreOneOrderCount = (moreOneOrderCount > 0 && customersByOrderDate / 100 > 0 ? (moreOneOrderCount / (customersByOrderDate / 100)) : 0).ToString("0.0#"),
                lifetimeValue = lifetimeValue.ToString("0.0#")
            };
        }
    }
}