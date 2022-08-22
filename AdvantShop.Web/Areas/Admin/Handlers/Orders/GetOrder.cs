using System.Linq;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Core.Services.Crm;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Loging.TrafficSource;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Web.Admin.Handlers.Tasks;
using AdvantShop.Web.Admin.Models.Orders.OrdersEdit;
using AdvantShop.Web.Admin.Models.Tasks;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetOrder
    {
        private readonly string _customerId;
        private readonly string _phone;
        private readonly bool _isEditMode;
        private readonly int _orderId;

        public GetOrder(string customerId, string phone)
        {
            _customerId = customerId;
            _phone = phone;
        }

        public GetOrder(bool isEditMode, int orderId)
        {
            _isEditMode = isEditMode;
            _orderId = orderId;
        }

        public OrderModel Execute()
        {
            var currentManager = ManagerService.GetManager(CustomerContext.CustomerId);

            var model = new OrderModel()
            {
                IsEditMode = _isEditMode,
                OrderId = _orderId,
                Order = new Order
                {
                    ManagerId = currentManager != null ? currentManager.ManagerId : (int?)null,
                    IsDraft = true
                }
            };

            if (!string.IsNullOrEmpty(_customerId) || !string.IsNullOrWhiteSpace(_phone))
                GetByCustomer(model);

            if (!_isEditMode)
                return model;

            var order = OrderService.GetOrder(_orderId);
            if (order == null || !OrderService.CheckAccess(order))
                return null;
            
            model.Order = order;

            model.PrevOrderId = OrderService.GetPrevOrder(order.OrderID, !CustomerContext.CurrentCustomer.IsAdmin ? currentManager : null);
            model.NextOrderId = OrderService.GetNextOrder(order.OrderID, !CustomerContext.CurrentCustomer.IsAdmin ? currentManager : null);

            if (order.OrderCurrency != null)
            {
                model.OrderCurrency = order.OrderCurrency;
                var currency = CurrencyService.GetAllCurrencies(true).FirstOrDefault(x => x.Iso3 == model.OrderCurrency.Iso3);
                if (currency != null)
                    model.OrderCurrency.Name = currency.Name;
            }

            if (order.OrderCustomer != null)
            {
                model.Customer = CustomerService.GetCustomer(order.OrderCustomer.CustomerID);
            }

            if (BonusSystem.IsActive)
                model.BonusCard = order.GetOrderBonusCard();
            
            model.OrderTrafficSource = OrderTrafficSourceService.Get(order.OrderID, TrafficSourceType.Order);

            if (order.LpId != null)
                model.LpLink = new LpService().GetLpLink(order.LpId.Value);
            
            return model;
        }

        private void GetByCustomer(OrderModel model)
        {
            var customer = CustomerService.GetCustomer(_customerId.TryParseGuid());
            if (customer == null)
            {
                if (!string.IsNullOrEmpty(_phone))
                    customer = CustomerService.GetCustomersByPhone(_phone).FirstOrDefault();
            }

            if (customer == null)
            {
                model.Order.OrderCustomer = new OrderCustomer()
                {
                    Phone = _phone,
                    StandardPhone = StringHelper.ConvertToStandardPhone(_phone, true, true)
                };
                return;
            }

            model.Order.OrderCustomer = (OrderCustomer) customer;
            
            model.Customer = customer;
        }
    }
}
