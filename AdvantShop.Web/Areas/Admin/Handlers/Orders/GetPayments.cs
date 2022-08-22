using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Orders;
using AdvantShop.Payment;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetPayments
    {
        private readonly int _orderId;
        private string _country;
        private string _city;
        private string _region;
        private string _district;
        private readonly Order _order;

        public GetPayments(int orderId, string country, string city, string region, string district)
        {
            _orderId = orderId;
            _country = country;
            _city = city;
            _region = region;
            _district = district;
        }

        public GetPayments(Order order)
        {
            _order = order;
        }

        public List<BasePaymentOption> Execute()
        {
            var order = _order ?? OrderService.GetOrder(_orderId);
            if (order == null || order.OrderItems == null || order.OrderItems.Count == 0)
                return null;

            if (!order.IsDraft && order.OrderCustomer != null)
            {
                _country = order.OrderCustomer.Country;
                _region = order.OrderCustomer.Region;
                _district = order.OrderCustomer.District;
                _city = order.OrderCustomer.City;
            }

            var preOrder = PreOrder.CreateFromOrder(order, actualizeShippingAndPayment: true);
            preOrder.CountryDest = _country ?? "";
            preOrder.RegionDest = _region ?? "";
            preOrder.DistrictDest = _district ?? "";
            preOrder.CityDest = _city ?? "";
            preOrder.IsFromAdminArea = true;

            var items = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();

            var manager = new PaymentManager(preOrder, items, null);
            return manager.GetOptions();
        }
    }
}
