using AdvantShop.Configuration;
using AdvantShop.Core.Services.Bonuses;
using AdvantShop.Orders;
using AdvantShop.Shipping;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Services.Localization;
using System;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    public class PaymentManager
    {
        private readonly List<PaymentMethod> _listMethod;
        private readonly PreOrder _preOrder;
        private readonly List<PreOrderItem> _items;
        private readonly BaseShippingOption _shipping;
        private readonly ShoppingCart _cart;

        private float productsPrice;
        private float bonusCost;

        public PaymentManager(PreOrder preOrder, List<PreOrderItem> items, ShoppingCart cart)
        {
            _listMethod = PaymentService.GetAllPaymentMethods(true);
            _preOrder = preOrder;
            _items = items;
            _shipping = preOrder.ShippingOption;
            _cart = cart;
        }

        public List<BasePaymentOption> GetOptions(bool getPreOrderPayment = false)
        {
            var options = new List<BasePaymentOption>();

            if (_shipping == null)
                return options;
            var currentItems = _listMethod;

            if (getPreOrderPayment && this._preOrder.PaymentOption != null)
            {
                currentItems = currentItems.Where(x => x.PaymentMethodId == _preOrder.PaymentOption.Id).ToList();
            }

            productsPrice = _cart != null
                ? _cart.TotalPrice - _cart.TotalDiscount
                : _items.Sum(x => x.Amount * x.Price) - _preOrder.TotalDiscount;

            var displayCertificateMetod = SettingsCheckout.EnableGiftCertificateService &&
                                          _cart != null && _cart.Certificate != null && (SettingsCertificates.ShowCertificatePaymentMetodOnlyCoversSum == false || productsPrice + _shipping.FinalRate <= 0);
            if (displayCertificateMetod)
            {
                var certificateMethod = currentItems.FirstOrDefault(x => x is PaymentGiftCertificate);
                if (certificateMethod == null)
                {
                    certificateMethod = PaymentGiftCertificate.Factory();
                    PaymentService.AddPaymentMethod(certificateMethod);
                }
                options.Add(certificateMethod.GetOption(null, 0));
                return options;
            }

            var availableMethod = PaymentService.UseGeoMapping(currentItems, _preOrder.CountryDest, _preOrder.CityDest);
            var notAvailableByShipping = ShippingMethodService.NotAvailablePayments(_shipping.MethodId);

            availableMethod = notAvailableByShipping.Any()
                ? availableMethod.Where(x => !notAvailableByShipping.Contains(x.PaymentMethodId)).ToList()
                : availableMethod;

            if (!availableMethod.Any() && getPreOrderPayment)
                return GetOptions();

            bonusCost = 0F;

            if (_preOrder.BonusUseIt && BonusSystem.IsActive)
            {
                var bonusCard = BonusSystemService.GetCard(_preOrder.BonusCardId);
                if (bonusCard != null && !bonusCard.Blocked && bonusCard.BonusesTotalAmount > 0)
                {
                    bonusCost = BonusSystemService.GetBonusCost(bonusCard, _cart, _shipping.FinalRate, _preOrder.BonusUseIt).BonusPrice;
                }
            }
            
            var preCoast = productsPrice + _shipping.FinalRate - bonusCost;

            foreach (var paymentMethod in availableMethod)
            {
                if (paymentMethod is PaymentGiftCertificate) continue;

                var preCoastInPaymentCurrency =
                    preCoast.ConvertCurrency(CurrencyService.CurrentCurrency,
                        paymentMethod.PaymentCurrency ?? CurrencyService.CurrentCurrency);

                if (paymentMethod is ICreditPaymentMethod creditPaymentMethod 
                    && creditPaymentMethod.ActiveCreditPayment 
                    && (creditPaymentMethod.MinimumPrice > preCoastInPaymentCurrency
                        || creditPaymentMethod.MaximumPrice < preCoastInPaymentCurrency)) continue;

                if (ShippingMethodService.IsPaymentNotUsed(_shipping.MethodId, paymentMethod.PaymentMethodId))
                    continue;

                options.Add(paymentMethod.GetOption(_shipping, preCoast));
            }
            options = options.Where(x => x != null && _shipping.AvailablePayment(x)).ToList();

            return options;
        }

        public BasePaymentOption UpdatePaymentByNewShipping(BasePaymentOption paymentOption, BaseShippingOption shippingOption)
        {
            var preCoast = productsPrice + shippingOption.FinalRate - bonusCost;
            return (BasePaymentOption)Activator.CreateInstance(paymentOption.GetType(), PaymentService.GetPaymentMethod(paymentOption.Id), preCoast);
        }
    }
}
