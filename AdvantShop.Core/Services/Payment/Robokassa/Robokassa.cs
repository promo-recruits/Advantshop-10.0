//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Core.Services.Payment.Robokassa;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Localization;
using AdvantShop.Orders;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for Robokassa
    /// </summary>
    [PaymentKey("Robokassa")]
    public class Robokassa : PaymentMethod
    {
        #region Receipt

        private class Receipt
        {
            public List<Item> items { get; set; }
        }

        private class Item
        {
            public string name { get; set; }
            public float quantity { get; set; }
            public float sum { get; set; }
            public string tax { get; set; }
            public string payment_method { get; set; }
            public string payment_object { get; set; }
        }

        /*
        none – без НДС;
        vat0 – НДС по ставке 0%;
        vat10 – НДС чека по ставке 10%;
        vat18 – НДС чека по ставке 18%;
        vat110 – НДС чека по расчетной ставке 10/110;
        vat118 – НДС чека по расчетной ставке 18/118.
        */
        private string GetVatType(TaxType? taxType, float? taxRate, ePaymentMethodType paymentMethodType)
        {
            if (!taxType.HasValue || taxType.Value == TaxType.VatWithout)
                return "none";

            if (taxType.Value == TaxType.Vat0)
                return "vat0";

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat110";
                else
                    return "vat10";
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat118";
                else
                    return "vat18";
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat120";
                else
                    return "vat20";
            }

            if (taxType.Value == TaxType.Other &&
                taxRate.HasValue)
                return "vat" + taxRate;

            return "none";
        }

        private string GetPaymentMethodType(ePaymentMethodType paymentMethodType)
        {
            switch (paymentMethodType)
            {
                case ePaymentMethodType.full_prepayment:
                    return "full_prepayment";
                case ePaymentMethodType.partial_prepayment:
                    return "prepayment"; // из-за этого значения нельзя enum.Tostring();
                case ePaymentMethodType.advance:
                    return "advance";
                case ePaymentMethodType.full_payment:
                    return "full_payment";
                case ePaymentMethodType.partial_payment:
                    return "partial_payment";
                case ePaymentMethodType.credit:
                    return "credit";
                case ePaymentMethodType.credit_payment:
                    return "credit_payment";
                default:
                    throw new NotImplementedException(paymentMethodType.ToString() + " not implemented in Robokassa");
            }
        }

        #endregion

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl | NotificationType.Handler; }
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
        public float Fee { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {RobokassaTemplate.MerchantLogin, MerchantLogin},
                               {RobokassaTemplate.CurrencyLabel, CurrencyLabel},
                               {RobokassaTemplate.Password, Password},
                               {RobokassaTemplate.PasswordNotify, PasswordNotify},
                               {RobokassaTemplate.SendReceiptData, SendReceiptData.ToString()},
                               {RobokassaTemplate.IsTest, IsTest.ToString()},
                               {RobokassaTemplate.Fee, Fee.ToInvariantString()},
                           };
            }
            set
            {
                if (value.ContainsKey(RobokassaTemplate.MerchantLogin))
                    MerchantLogin = value[RobokassaTemplate.MerchantLogin];
                Password = value.ElementOrDefault(RobokassaTemplate.Password);
                PasswordNotify = value.ElementOrDefault(RobokassaTemplate.PasswordNotify);
                CurrencyLabel = value.ContainsKey(RobokassaTemplate.CurrencyLabel)
                                    ? value[RobokassaTemplate.CurrencyLabel]
                                    : null;
                SendReceiptData = value.ElementOrDefault(RobokassaTemplate.SendReceiptData).TryParseBool();
                IsTest = value.ElementOrDefault(RobokassaTemplate.IsTest).TryParseBool();
                Fee = value.ElementOrDefault(RobokassaTemplate.Fee).TryParseFloat();
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var receipt = SendReceiptData
                ? new Receipt
                {
                    items = 
                        order
                           .GetOrderItemsForFiscal(paymentCurrency)
                           .Select(item => new Item()
                           {
                               name = item.Name.Reduce(64),
                               sum = (float)Math.Round(item.Price * item.Amount, 2),
                               quantity = item.Amount,
                               tax = GetVatType(tax?.TaxType ?? item.TaxType, tax?.Rate ?? item.TaxRate, item.PaymentMethodType),
                               payment_method = GetPaymentMethodType(item.PaymentMethodType),
                               payment_object = item.PaymentSubjectType.ToString()
                           })
                           .ToList()
                }
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        name = "Подарочный сертификат " + x.CertificateCode,
                        sum = x.Sum,
                        quantity = 1,
                        tax = GetVatType(tax?.TaxType ?? certTax?.TaxType, tax?.Rate ?? certTax?.Rate ?? 0f, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        payment_method = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        payment_object = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString()
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0)
            {
                receipt?.items.Add(new Item
                {
                    name = "Доставка",
                    sum = orderShippingCostWithDiscount,
                    quantity = 1,
                    tax = GetVatType(tax?.TaxType ?? order.ShippingTaxType, tax?.Rate, order.ShippingPaymentMethodType),
                    payment_method = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    payment_object = order.ShippingPaymentSubjectType.ToString()
                });
            }

            var receiptString = receipt != null ? HttpUtility.UrlEncode(Newtonsoft.Json.JsonConvert.SerializeObject(receipt)) : null;
            var sum = order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency).SubtractFee(Fee);

            var handler = new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://auth.robokassa.ru/Merchant/Index.aspx",
                InputValues = new NameValueCollection
                {
                    {"MrchLogin", MerchantLogin},
                    {"OutSum", sum.ToInvariantString()},
                    {"InvId", order.OrderID.ToString()},
                    {"Desc", GetOrderDescription(order.Number)},
                    {"IncCurrLabel", CurrencyLabel.IsNotEmpty() ? CurrencyLabel : null},
                    {"IsTest", IsTest ? "1" : "0"},
                    {"Culture", Culture.Language == Culture.SupportLanguage.Russian ? "ru" : "en"},
                    {"shp_partner", "API_Advantshop"},


                    {
                        "SignatureValue",
                        (MerchantLogin + ":" + sum.ToInvariantString() + ":" + order.OrderID + ":" + (receiptString.IsNotEmpty() ? receiptString + ":" : string.Empty) + Password  + ":" + "shp_partner=API_Advantshop").Md5()
                    },
                    {"receipt", receiptString }
                }
            };

            return handler;
        }

        public override string ProcessResponse(HttpContext context)
        {
            if (context.Request.Url.AbsolutePath.Contains("paymentnotification"))
                return ProcessResponseNotify(context);
            return ProcessResponseReturn(context);
        }

        private string ProcessResponseReturn(HttpContext context)
        {
            var req = context.Request;
            int orderId = 0;

            if (int.TryParse(req["InvId"], out orderId))
            {
                if (CheckFields(req))
                {

                    Order order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                        return NotificationMessahges.SuccessfullPayment(orderId.ToString());
                    }
                }
                return NotificationMessahges.InvalidRequestData;
            }
            return string.Empty;
        }

        private bool CheckFields(HttpRequest req)
        {
            if (string.IsNullOrEmpty(req["OutSum"]) || string.IsNullOrEmpty(req["InvId"]) || string.IsNullOrEmpty(req["Culture"]) ||
                string.IsNullOrEmpty(req["SignatureValue"]))
                return false;
            if (req["SignatureValue"].ToLower() !=
                (req["OutSum"].Trim() + ":" + req["InvId"] + ":" + Password).Md5(false))
                return false;
            return true;
        }

        private string ProcessResponseNotify(HttpContext context)
        {
            var req = context.Request;
            int orderId = 0;
            if (CheckFieldsExt(req) && int.TryParse(req["InvId"], out orderId))
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true);
                    return string.Format("OK{0}", req["InvId"]);
                }
            }
            return NotificationMessahges.InvalidRequestData;
        }

        private bool CheckFieldsExt(HttpRequest req)
        {
            if (string.IsNullOrEmpty(req["OutSum"]) || string.IsNullOrEmpty(req["InvId"]) || string.IsNullOrEmpty(req["SignatureValue"]))
                return false;
            if (req["SignatureValue"].ToLower() !=
                (req["OutSum"].Trim() + ":" + req["InvId"] + ":" + PasswordNotify + (string.IsNullOrEmpty(req["shp_partner"]) ? "" : ":" + "shp_partner=API_Advantshop")).Md5(false))
                return false;
            return true;
        }
    }
}