using AdvantShop.Core;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Localization;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Taxes;

namespace AdvantShop.Payment
{
    [PaymentKey("UniversalPayGate")]
    public class UniversalPayGate : PaymentMethod
    {
        #region Receipt

        private class Receipt
        {
            public List<ReceiptItem> Items { get; set; }
        }

        private class ReceiptItem
        {
            public string Name { get; set; }
            public float Quantity { get; set; }
            public float Sum { get; set; }
            public string Tax { get; set; }
        }

        #endregion

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }
        public string MerchantLogin { get; set; }
        public string Password { get; set; }
        public string PasswordNotify { get; set; }
        public string CurrencyLabel { get; set; }
        public bool SendReceiptData { get; set; }
        public bool IsTest { get; set; }
        public string Url { get; set; }
        public string UrlTest { get; set; }
        public string Code { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {UniversalPayGateTemplate.MerchantLogin, MerchantLogin},
                               {UniversalPayGateTemplate.Password, Password},
                               {UniversalPayGateTemplate.PasswordNotify, PasswordNotify},
                               {UniversalPayGateTemplate.SendReceiptData, SendReceiptData.ToString()},
                               {UniversalPayGateTemplate.IsTest, IsTest.ToString()},
                               {UniversalPayGateTemplate.Url, Url},
                               {UniversalPayGateTemplate.UrlTest, UrlTest},
                               {UniversalPayGateTemplate.Code, Code},
                           };
            }
            set
            {
                if (value.ContainsKey(UniversalPayGateTemplate.MerchantLogin))
                    MerchantLogin = value[UniversalPayGateTemplate.MerchantLogin];
                Password = value.ElementOrDefault(UniversalPayGateTemplate.Password);
                PasswordNotify = value.ElementOrDefault(UniversalPayGateTemplate.PasswordNotify);
                SendReceiptData = value.ElementOrDefault(UniversalPayGateTemplate.SendReceiptData).TryParseBool();
                IsTest = value.ElementOrDefault(UniversalPayGateTemplate.IsTest).TryParseBool();
                Url = value.ElementOrDefault(UniversalPayGateTemplate.Url);
                UrlTest = value.ElementOrDefault(UniversalPayGateTemplate.UrlTest);
                Code = value.ElementOrDefault(UniversalPayGateTemplate.Code);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;

            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var receipt = SendReceiptData
               ? new Receipt
                   {
                    Items = 
                        order
                            .GetOrderItemsForFiscal(paymentCurrency)
                            .Select(item => new ReceiptItem
                            {
                                Name = item.Name.Reduce(64),
                                Sum = (float)Math.Round(item.Price * item.Amount, 2),
                                Quantity = item.Amount,
                                Tax = GetTax(tax?.TaxType ?? item.TaxType, item.PaymentMethodType)
                            })
                            .ToList()
                   }
               : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new ReceiptItem
                    {
                        Name = "Подарочный сертификат",
                        Sum = (float)Math.Round(x.Sum, 2),
                        Quantity = 1,
                        Tax = GetTax(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType)
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.Items.Add(new ReceiptItem
                {
                    Name = "Доставка",
                    Sum = (float)Math.Round(orderShippingCostWithDiscount, 2),
                    Quantity = 1,
                    Tax = GetTax(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType)
                });
            }

            var receiptString = receipt != null ? HttpUtility.UrlEncode(JsonConvert.SerializeObject(receipt)) : null;

            var sum =
                (receipt != null
                    ? receipt.Items.Sum(x => x.Sum)
                    : (float) Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2))
                .ToInvariantString();

            var inputValues = new NameValueCollection
                {
                    {"Login", MerchantLogin},
                    {"Sum", sum},
                    {"OrderId", order.OrderID.ToString()},
                    {"Desc", GetOrderDescription(order.Number)},
                    {
                        "Signature",
                        (MerchantLogin + ":" + sum + ":" + order.OrderID + ":" + (receiptString.IsNotEmpty() ? receiptString + ":" : string.Empty) + Password).Md5()
                    },
                    {"Receipt", receiptString },
                    {"Email", order.OrderCustomer.Email},
                    {"Phone", order.OrderCustomer.Phone},
                    {"SuccessUrl", SuccessUrl},
                    {"FailUrl", FailUrl},
                    {"NotificationUrl", NotificationUrl}
                };

            if (IsTest)
                inputValues.Add("IsTest", "1");

            var handler = new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = IsTest && !string.IsNullOrWhiteSpace(UrlTest) ? UrlTest : Url,
                InputValues = inputValues
            };

            return handler;
        }

        private static string GetTax(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (!taxType.HasValue)
                return String.Empty;
            
            if (taxType.Value == TaxType.VatWithout)
                return "VatWithout";

            if (taxType.Value == TaxType.Vat0)
                return "Vat0";

            if (taxType.Value == TaxType.Vat10)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "Vat110";
                else
                    return "Vat10";
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "Vat118";
                else
                    return "Vat18";
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (AdvantShop.Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "Vat120";
                else
                    return "Vat20";
            }

            return "None";
        }

        public override string ProcessResponse(HttpContext context)
        {
            return ProcessResponseNotify(context);
        }

        private string ProcessResponseNotify(HttpContext context)
        {
            var req = context.Request;
            var error = CheckFieldsExt(req);
            if (string.IsNullOrEmpty(error) && int.TryParse(req["OrderId"], out var orderId))
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true);
                    return string.Format("OK_{0}", req["OrderId"]);
                }
            }
            return string.Format("FAIL_{0}_{1}", req["OrderId"], error);
        }

        private string CheckFieldsExt(HttpRequest req)
        {
            if (string.IsNullOrEmpty(req["Sum"]) ||
                string.IsNullOrEmpty(req["OrderId"]) ||
                string.IsNullOrEmpty(req["Signature"]))
                return "Sum or OrderId or Signature is empty";

            if (req["Signature"].ToLower() != (req["Sum"].Trim() + ":" + req["OrderId"] + ":" + PasswordNotify).Md5(false))
                return "Signature conflict";

            return string.Empty; ;
        }
    }
}
