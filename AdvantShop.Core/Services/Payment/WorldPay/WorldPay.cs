//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    [PaymentKey("WorldPay")]
    public class WorldPay : PaymentMethod
    {
        private string Url
        {
            get
            {
                return Sandbox
                           ? "https://test.sagepay.com/gateway/service/vspform-register.vsp"
                           : "https://live.sagepay.com/gateway/service/vspform-register.vsp";
            }
        }
       
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
            get
            {
                return UrlStatus.NotificationUrl;
            }
        }
   
        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[] {
            "USD","ARS","AUD","BRL","CAD",
            "CHF","CLP","CNY","COP","CZK",
            "DKK","EUR","GBP","HKD","HUF",
            "IDR","JPY","KES","KRW","MXP",
            "MYR","NOK","NZD","PHP","PLN",
            "PTE","SEK","SGD","SKK","THB",
            "TWD","USD","VND","ZAR"};

        public bool Sandbox { get; set; }
        public int InstID { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {WorldPayTemplate.InstID, InstID.ToString()},
                               {WorldPayTemplate.Sandbox, Sandbox.ToString()},
                           };
            }
            set
            {
                int intval;
                if (int.TryParse(value.ElementOrDefault(WorldPayTemplate.InstID), out intval))
                    InstID = intval;

                bool boolval;
                Sandbox = !bool.TryParse(value.ElementOrDefault(WorldPayTemplate.Sandbox), out boolval) || boolval;
            }
        }
        
        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentNo = order.OrderID.ToString();
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr = string.Format("{0:0.00}", order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency));
            var description = string.Format("Order #{0} payment", order.Number);

            var currencyCode = paymentCurrency.Iso3;
            return new PaymentForm
            {
                Url = Url,
                InputValues = new NameValueCollection
                {
                    {"instId", InstID.ToString()},
                    {"cartId", paymentNo},
                    {"Amount", orderSumStr},
                    {"currency", currencyCode},
                    {"desc", description},
                    {"SuccessURL", SuccessUrl},
                    {"FailureURL", FailUrl},
                    //FIELDS TO ENCRYPTION instId:amount:currency:cartId
                    {"signature", (InstID.ToString() + orderSumStr + currencyCode + paymentNo).Md5()},
                    {"MC_code", (orderSumStr + currencyCode + paymentNo).Md5()}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            //!!!!!!!!!! UNSAFE !!!!!!!!!!!!!!
            var req = context.Request;
            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;
            var paymentNumber = req["cartId"];
            int orderID = 0;
            if (int.TryParse(paymentNumber, out orderID) &&
                OrderService.GetOrder(orderID) != null)
            {
                var order = OrderService.GetOrder(orderID);
                if (order != null && req["amount"] == string.Format("{0:0.00}", order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)))
                {
                    OrderService.PayOrder(orderID, true);
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }
            }
            return NotificationMessahges.Fail;
        }

        private static bool CheckData(HttpRequest req)
        {
            return !new[]
                        {
                            "cartId",
                            "instId",
                            "amount",
                            "transId",
                            "currency",
                            "MC_code"
                        }.Any(item => string.IsNullOrEmpty(req[item]))
                        && (req["amount"] + req["currency"] + req["cartId"]).Md5() == req["MC_code"];

        }
    }
}