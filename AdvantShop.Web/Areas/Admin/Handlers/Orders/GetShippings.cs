using System.Linq;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using AdvantShop.Web.Admin.Models.Orders;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class GetShippings
    {
        private readonly int _orderId;
        private string _country;
        private string _region;
        private string _district;
        private string _city;
        private string _zip;
        private readonly BaseShippingOption _shipping;
        private readonly bool _getAll;
        private readonly bool _applyPay;

        public GetShippings(int orderId, string country, string city, string district, string region, string zip, BaseShippingOption shipping = null, bool getAll = true, bool applyPay = true)
        {
            _orderId = orderId;
            _country = country;
            _region = region;
            _district = district;
            _city = city;
            _zip = zip;
            _shipping = shipping;
            _getAll = getAll;
            _applyPay = applyPay;
        }

        public OrderShippingsModel Execute()
        {
            var order = OrderService.GetOrder(_orderId);
            if (order == null)
                return null;

            var model = new OrderShippingsModel();

            if (order.OrderItems == null || order.OrderItems.Count == 0)
                return model;

            if (!order.IsDraft && order.OrderCustomer != null)
            {
                _country = order.OrderCustomer.Country;
                _region = order.OrderCustomer.Region;
                _district = order.OrderCustomer.District;
                _city = order.OrderCustomer.City;
                _zip = order.OrderCustomer.Zip;
            }

            var preOrder = PreOrder.CreateFromOrder(order);
            preOrder.CountryDest = _country ?? "";
            preOrder.RegionDest = _region ?? "";
            preOrder.DistrictDest = _district ?? "";
            preOrder.CityDest = _city ?? "";
            preOrder.ZipDest = _zip ?? "";
            preOrder.ShippingOption = _shipping;
            preOrder.IsFromAdminArea = true;

            var items = order.OrderItems.Select(x => new PreOrderItem(x)).ToList();

            var manager = new ShippingManager(preOrder, items, order.Sum - order.ShippingCost - order.PaymentCost);
#if !DEBUG
            manager.TimeLimitMilliseconds = 20_000; // 20 seconds
#endif
            model.Shippings = manager.GetOptions(_getAll);

            if (_applyPay && model.Shippings != null && model.Shippings.Count > 0)
            {
                if (order.PaymentMethod != null)
                {
                    var preCoast = items.Sum(x => x.Amount * x.Price) - preOrder.TotalDiscount;
                    foreach (var shippingOption in model.Shippings)
                    {
                        var paymentOption = order.PaymentMethod.GetOption(shippingOption, preCoast + shippingOption.FinalRate);
                        if (paymentOption != null)
                            shippingOption.ApplyPay(paymentOption);
                    }
                }
                else if (preOrder.PaymentOption != null)
                    foreach (var shippingOption in model.Shippings)
                        shippingOption.ApplyPay(preOrder.PaymentOption);

            }

            if (model.Shippings != null)
                foreach (var shipping in model.Shippings) {
                    // *** Warrning!!! First change currency
                    shipping.CurrentCurrency = order.OrderCurrency;
                    shipping.ManualRate = shipping.FinalRate;
                    // ***
                }

            model.CustomShipping = new BaseShippingOption()
            {
                Name = "",
                Rate = _shipping != null ? _shipping.ManualRate : 0,
                ManualRate = _shipping != null ? _shipping.ManualRate : 0,
                IsCustom = true
            };

            return model;
        }
    }
}
