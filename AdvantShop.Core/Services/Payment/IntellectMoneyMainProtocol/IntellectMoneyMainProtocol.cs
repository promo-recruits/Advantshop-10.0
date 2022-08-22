﻿//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using AdvantShop.Taxes;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    // docs: https://wiki.intellectmoney.ru/pages/viewpage.action?pageId=4849803#id-%D0%9F%D1%80%D0%BE%D1%82%D0%BE%D0%BA%D0%BE%D0%BB%D0%BF%D1%80%D0%B8%D0%B5%D0%BC%D0%B0%D0%BF%D0%BB%D0%B0%D1%82%D0%B5%D0%B6%D0%B5%D0%B9Intellectmoney-4.2.1.%D0%A4%D0%BE%D1%80%D0%BC%D0%B0%D0%B7%D0%B0%D0%BF%D1%80%D0%BE%D1%81%D0%B0%D0%BF%D0%BB%D0%B0%D1%82%D0%B5%D0%B6%D0%B0
    
    [PaymentKey("IntellectMoneyMainProtocol")]
    public class IntellectMoneyMainProtocol : PaymentMethod
    {
        private const string Separator = "::";

        public string EshopId { get; set; }
        public string Preference { get; set; }
        public string SecretKey { get; set; }
        public bool SendReceiptData { get; set; }
        public string ReceiptDataInn { get; set; }

        public override ProcessType ProcessType => ProcessType.FormPost;
        
        public override NotificationType NotificationType => NotificationType.Handler;
        
        public override UrlStatus ShowUrls => UrlStatus.NotificationUrl | UrlStatus.CancelUrl;

        public override Dictionary<string, string> Parameters
        {
            get =>
                new Dictionary<string, string>
                {
                    {IntellectMoneyMainProtocolTemplate.EshopId , EshopId},
                    {IntellectMoneyMainProtocolTemplate.Preference , Preference},
                    {IntellectMoneyMainProtocolTemplate.SecretKey , SecretKey},
                    {IntellectMoneyMainProtocolTemplate.SendReceiptData , SendReceiptData.ToString()},
                    {IntellectMoneyMainProtocolTemplate.ReceiptDataInn , ReceiptDataInn},
                };
            set
            {
                EshopId = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.EshopId);
                Preference = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.Preference);
                SecretKey = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.SecretKey);
                SendReceiptData = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.SendReceiptData).TryParseBool();
                ReceiptDataInn = value.ElementOrDefault(IntellectMoneyMainProtocolTemplate.ReceiptDataInn);
            }
        }

        public static Dictionary<string, string> GetPaymentSystems()
        {
            return PaymentSystems;
        }

        public static readonly Dictionary<string, string> PaymentSystems = new Dictionary<string, string>
        {
            {"", "Не выбран"},
            {"inner", "Оплата с кошелька Rbk Money"},
            {"bankCard", "Банковская карта Visa/MasterCard"},
            {"exchangers", "Электронные платежные системы"},
            {"prepaidcard", "Предоплаченная карта RBK Money"},
            {"transfers", "Системы денежных переводов"},
            {"terminals", "Платёжные терминалы"},
            {"iFree", "SMS"},
            {"bank", "Банковский платёж"},
            {"postRus", "Почта России"},
            {"atm", "Банкоматы"},
            {"yandex", "Яндекс"},
            {"ibank", "Интернет банкинг"},
            {"euroset", "Евросеть"}
        };

        public override PaymentForm GetPaymentForm(Order order)
        {
            return new PaymentForm
            {
                Url = "https://merchant.intellectmoney.ru/ru/",
                InputValues = GetParams(order)
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            var s = req.InputStream;
            s.Seek(0, System.IO.SeekOrigin.Begin);
            var payload = new System.IO.StreamReader(s).ReadToEnd();
            var valCollection = HttpUtility.ParseQueryString(payload, System.Text.Encoding.GetEncoding(1251));
            
            if (!CheckData(valCollection) && !CheckData(req.Form))
            {
                Debug.Log.Error(req.ServerVariables["ALL_RAW"]);
                return NotificationMessahges.InvalidRequestData;
            }

            var paymentNumber = valCollection["orderId"];
            int.TryParse(valCollection["paymentStatus"], out var payStatus);
            if (payStatus != 5)
                return "OK";

            if (int.TryParse(paymentNumber, out var orderId) && OrderService.GetOrder(orderId) != null)
            {
                OrderService.PayOrder(orderId, true);
                return "OK";
                //return NotificationMessahges.SuccessfullPayment(paymentNumber);
            }
            return NotificationMessahges.Fail;
        }

        private NameValueCollection GetParams(Order order)
        {
            var dict = new NameValueCollection
            {
                {"eshopId", EshopId},
                {"orderId", order.OrderID.ToString()},
                {"serviceName", "Order #" + order.Number},
                {
                    "recipientAmount", order.Sum
                        .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                        .ToString("F2", CultureInfo.InvariantCulture)
                },
                {"recipientCurrency", PaymentCurrency?.Iso3 ?? order.OrderCurrency.CurrencyCode},
                //{ "userName", order.OrderCustomer.FirstName + " " + order.OrderCustomer.LastName},
                {"user_email", order.OrderCustomer.Email},
                {"preference", Preference}
            };

            if (SendReceiptData && !string.IsNullOrEmpty(ReceiptDataInn))
            {
                var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
                var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
                var orderItems = order.GetOrderItemsForFiscal(paymentCurrency, toIntegerAmount: true).ToList();

                var merchantReceipt = new IntellectMoneyMainProtocolMerchantReceipt()
                {
                    Inn = ReceiptDataInn,
                    SkipAmountCheck = 1,
                    Group = "Main",
                    Content = new IntellectMoneyMainProtocolMerchantReceiptContent()
                    {
                        Type = 1,
                        CustomerContact = order.OrderCustomer.Email ?? "+" + order.OrderCustomer.StandardPhone,
                        Positions = orderItems.Select(x =>
                            new IntellectMoneyMainProtocolMerchantReceiptContentPosition(tax?.TaxType ?? x.TaxType)
                            {
                                Text = HttpUtility.HtmlEncode(x.Name),
                                Quantity = x.Amount.ToString("F3", CultureInfo.InvariantCulture),
                                Price = x.Price.ToString("F3", CultureInfo.InvariantCulture),
                            }).ToList()
                    }
                };

                dict.Add("merchantReceipt", JsonConvert.SerializeObject(merchantReceipt));
            }

            return dict;
        }

        private bool CheckData(NameValueCollection req)
        {
            return
               (req["eshopId"] + Separator +
                req["orderId"] + Separator +
                req["serviceName"] + Separator +
                req["eshopAccount"] + Separator +
                req["recipientAmount"] + Separator +
                req["recipientCurrency"] + Separator +
                req["paymentStatus"] + Separator +
                req["userName"] + Separator +
                req["userEmail"] + Separator +
                req["paymentData"] + Separator +
                SecretKey).Md5(false) == req["hash"];
        }
    }
}