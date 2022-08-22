using System;
using System.Collections.Generic;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Web.Admin.Models.Orders
{
    public partial class OrderItemsModel
    {
        public int OrderId { get; set; }
        public string Number { get; set; }

        public string StatusName { get; set; }
        public int OrderStatusId { get; set; }

        public string PaymentDate { get; set; }
        public bool IsPaid { get; set; }

        public string PaymentMethodName { get; set; }
        public string ShippingMethodName { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingMethod { get; set; }

        public float Sum { get; set; }

        public string SumFormatted
        {
            get
            {
                return PriceFormatService.FormatPrice(Sum, CurrencyValue, CurrencySymbol, CurrencyCode, IsCodeBefore);
            }
        }

        public string CurrencyCode { get; set; }
        public float CurrencyValue { get; set; }
        public string CurrencySymbol { get; set; }
        public bool IsCodeBefore { get; set; }
        public bool EnablePriceRounding { get; set; }
        public float RoundNumbers { get; set; }
        public int CurrencyNumCode { get; set; }

        public DateTime OrderDate { get; set; }

        public string OrderDateFormatted
        {
            get { return Culture.ConvertDate(OrderDate); }
        }

        public Guid CustomerId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Organization { get; set; }

        public string BuyerName
        {
            get
            {
                var name = StringHelper.AggregateStrings(" ", FirstName, LastName);

                if (!string.IsNullOrEmpty(Organization))
                    name += !string.IsNullOrEmpty(name) ? " (" + Organization + ")" : Organization;

                return name;
            }
        }

        public string ManagerId { get; set; }

        public string Color { get; set; }
        public float Rating { get; set; }

        public bool ManagerConfirmed { get; set; }
        public Guid ManagerCustomerId { get; set; }
        public string ManagerName { get; set; }

        public string AdminOrderComment { get; set; }

        private List<string> _orderItems;
        public List<string> OrderItems
        {
            get
            {
                if (_orderItems != null)
                    return _orderItems;

                _orderItems = new List<string>();

                var orderItems = OrderService.GetOrderItems(OrderId);
                var currency = new Currency()
                {
                    Iso3 = CurrencyCode,
                    NumIso3 = CurrencyNumCode,
                    Rate = CurrencyValue,
                    Symbol = CurrencySymbol,
                    IsCodeBefore = IsCodeBefore,
                    EnablePriceRounding = EnablePriceRounding,
                    RoundNumbers = RoundNumbers
                };

                foreach (var item in orderItems)
                {
                    _orderItems.Add(
                        string.Format("{0} {1} - {2}, {3} шт.", item.Name, item.ArtNo,
                            PriceService.SimpleRoundPrice(item.Price, currency).FormatPrice(currency),
                            item.Amount));
                }

                if (orderItems.Count == 0)
                {
                    var certificates = GiftCertificateService.GetOrderCertificates(OrderId);
                    if (certificates != null && certificates.Count > 0)
                    {
                        foreach (var certificate in certificates)
                        {
                            _orderItems.Add(string.Format("{0} {1}",
                                LocalizationService.GetResource("Admin.Orders.OrderItemsModel.Certificate"),
                                certificate.CertificateCode));
                        }
                    }
                }

                return _orderItems;
            }
        }

        public string City { get; set; }
    }
}
