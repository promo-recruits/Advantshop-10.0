using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Orders;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Models.Shared.Common;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.SQL;

namespace AdvantShop.Web.Admin.Handlers.Marketing.Analytics.Reports
{
    public class GetCustomersGroupHandler : AnalyticsBaseHandler
    {
        private readonly DateTime _dateFrom;
        private readonly DateTime _dateTo;

        private EGroupDateBy _groupBy;
        private readonly string _groupFormatString;


        public GetCustomersGroupHandler(DateTime dateFrom, DateTime dateTo, string groupFormatString)
        {
            _dateFrom = dateFrom;
            _dateTo = dateTo;
            _groupFormatString = groupFormatString ?? "dd";
            _groupBy = Filter(_groupFormatString);
        }

        public ChartDataJsonModel GetGroups()
        {
            //var payments =
            //    OrderStatisticsService.GetPayments(_dateFrom, _dateTo, false, 0)
            //        .OrderByDescending(x => x.Sum)
            //        .ToList();

            var groups = CustomerGroupService.GetCustomerGroupsWithCount(_dateFrom, _dateTo);

            var groupsStat = groups.Take(9).ToList();

            if (groups.Count >= 10)
                groupsStat.Add(new CustomerGroup()
                {
                    GroupName = "Другие",
                    CustomersCount = groups.Skip(9).Sum(x => x.CustomersCount)                    
                });

            return new ChartDataJsonModel()
            {
                Data = new List<object>() { groupsStat.Select(x => x.CustomersCount) },
                Labels = groupsStat.Select(x => !string.IsNullOrWhiteSpace(x.GroupName) ? x.GroupName.Reduce(30) : "n/a").ToList(),
                Series = new List<string>() { "Группы покупателей" }
            };
        }
    }
}