using System;
using System.Linq;
using System.Web;
using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Handlers.Orders
{
    public class ChangeOrderItemCustomOptions
    {
        private readonly OrderItem _orderItem;
        private readonly Currency _currency;
        private readonly string _customOptionsXml;
        private readonly string _artno;

        public ChangeOrderItemCustomOptions(OrderItem orderItem, Currency currency, string customOptionsXml, string artno)
        {
            _orderItem = orderItem;
            _currency = currency;
            _customOptionsXml = customOptionsXml;
            _artno = artno;
        }

        public bool Execute()
        {
            if (_orderItem.ProductID == null)
                throw new BlException("Товар не найден");

            var product = ProductService.GetProduct(_orderItem.ProductID.Value);
            if (product == null)
                throw new BlException("Товар не найден");

            var offer = product.Offers.FirstOrDefault(x => x.ArtNo == _artno) ??
                        product.Offers.FirstOrDefault(x => x.Main);

            var xml = !String.IsNullOrWhiteSpace(_customOptionsXml) && _customOptionsXml != "null"
                        ? HttpUtility.UrlDecode(_customOptionsXml)
                        : null;

            _orderItem.SelectedOptions = CustomOptionsService.DeserializeFromXml(xml, product.Currency.Rate);

            if (offer != null)
            {
                var customOptionsPrice = CustomOptionsService.GetCustomOptionPrice(offer.RoundedPrice, xml, product.Currency.Rate);
                var price = offer.RoundedPrice + customOptionsPrice;
                
                var discount = PriceService.GetFinalDiscount(price, product.Discount, _currency.Rate);
                _orderItem.Price = PriceService.GetFinalPrice(price, discount);
            }

            OrderService.DeleteOrderItemCustomOptions(_orderItem.OrderItemID);
            
            foreach (var option in _orderItem.SelectedOptions)
            {
                OrderService.AddOrderItemCustomOption(option, _orderItem.OrderItemID);
            }

            return true;
        }
    }
}
