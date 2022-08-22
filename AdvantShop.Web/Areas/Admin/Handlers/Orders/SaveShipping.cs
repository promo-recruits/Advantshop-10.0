﻿using System.Linq;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Customers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;
using AdvantShop.Shipping;
using AdvantShop.Taxes;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class SaveShipping
    {
        private readonly Order _order;
        private readonly string _country;
        private readonly string _city;
        private readonly string _district;
        private readonly string _region;
        private readonly BaseShippingOption _shipping;

        public SaveShipping(Order order, string country, string city, string district, string region, BaseShippingOption shipping)
        {
            _order = order;
            _country = country;
            _city = city;
            _district = district;
            _region = region;
            _shipping = shipping;
        }

        public void Execute()
        {
            var trackChanges = !_order.IsDraft;
            var changedBy = new OrderChangedBy(CustomerContext.CurrentCustomer);

            var customer = _order.OrderCustomer;

            if (_order.IsDraft && customer != null && (customer.Country != _country || customer.Region != _region || customer.City != _city || customer.District != _district))
            {
                customer.Country = _country ?? "";
                customer.Region = _region ?? "";
                customer.District = _district ?? "";
                customer.City = _city ?? "";

                OrderService.UpdateOrderCustomer(customer, changedBy, trackChanges);
            }
            
            _order.ShippingMethodId = _shipping.MethodId;
            _order.ArchivedShippingName = _shipping.Name ?? _shipping.NameRate;
            _order.ShippingCost = _shipping.ManualRate;
            var shippingTax = _shipping.TaxId.HasValue ? TaxService.GetTax(_shipping.TaxId.Value) : null;
            _order.ShippingTaxType = shippingTax == null ? TaxType.None : shippingTax.TaxType;
            var orderPickPoint = _shipping.GetOrderPickPoint();
            _order.AvailablePaymentCashOnDelivery = _shipping.IsAvailablePaymentCashOnDelivery;
            _order.AvailablePaymentPickPoint = _shipping.IsAvailablePaymentPickPoint;

            if (orderPickPoint != null)
                OrderService.AddUpdateOrderPickPoint(_order.OrderID, orderPickPoint);
            else
                OrderService.DeleteOrderPickPoint(_order.OrderID);

            new UpdateOrderTotal(_order).Execute();
        }
    }
}
