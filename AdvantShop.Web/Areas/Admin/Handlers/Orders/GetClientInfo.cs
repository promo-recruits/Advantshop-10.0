using System;
using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetClientInfo
    {
        private readonly OrderModel _orderModel;

        public GetClientInfo(OrderModel orderModel)
        {
            _orderModel = orderModel;
        }

        public ClientInfoModel Execute()
        {
            var order = _orderModel.Order;

            var model = new ClientInfoModel()
            {
                CustomerGroup = order.GroupDiscountString,
            };

            if (_orderModel.Customer != null)
            {
                model.CustomerId = _orderModel.Customer.Id.ToString();
                model.Customer = _orderModel.Customer;
                model.OrderId = _orderModel.OrderId;
                model.Order = _orderModel.Order;
                model.InterestingCategories = StatisticService.GetCustomerInterestingCategories(_orderModel.Customer.Id).Take(8).ToList();

                model.Statistic = new ClientStatistic();
                model.Statistic.RegistrationDate = Culture.ConvertDate(_orderModel.Customer.RegistrationDateTime);
                model.Statistic.RegistrationDuration = _orderModel.Customer.RegistrationDateTime.GetDurationString(DateTime.Now);
                model.Statistic.AdminCommentAboutCustomer = _orderModel.Customer.AdminComment;
                model.Statistic.OrdersCount = OrderService.GetOrdersCountByCustomer(_orderModel.Customer.Id);
                var count = StatisticService.GetCustomerOrdersCount(_orderModel.Customer.Id);
                var sum = count > 0 ? StatisticService.GetCustomerOrdersSum(_orderModel.Customer.Id) : 0;

                model.Statistic.OrdersSum = sum.FormatPrice();
                model.Statistic.AverageCheck = (count > 0 ? (sum / count) : 0).FormatPrice();

            }

            return model;
        }
    }
}
