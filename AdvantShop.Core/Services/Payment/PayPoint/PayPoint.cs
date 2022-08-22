//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    [PaymentKey("PayPoint")]
    public class PayPoint : PaymentMethod
    {
        private static string Url
        {
            get
            {
                return "https://www.secpay.com/java-bin/ValCard";
            }
        }
       
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[]
        {
            "AUD", "CAD", "EUR", "GBP", "HKD", "JPY", "USD"
        };
 
        public string Merchant { get; set; }
        public string Password { get; set; }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PayPointTemplate.Merchant, Merchant},
                               {PayPointTemplate.Password, Password},
                           };
            }
            set
            {
                Merchant = value.ElementOrDefault(PayPointTemplate.Merchant);
                Password = value.ElementOrDefault(PayPointTemplate.Password);
            }

        }
        
        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentNo = order.OrderID.ToString();
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var sum = string.Format("{0:0.00}", order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency));
            return new PaymentForm
            {
                Url = Url,
                InputValues = new NameValueCollection
                {
                    {"merchant", Merchant},
                    {"trans_id", paymentNo},
                    {"amount", sum},
                    {"callback", SuccessUrl},
                    {"currency", paymentCurrency.Iso3},
                    {"digest", (paymentNo + sum + Password).Md5()}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;
            var paymentNumber = req["trans_id"];
            if (int.TryParse(paymentNumber, out var orderId) &&
                OrderService.GetOrder(orderId) != null)
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null && req["amount"] == string.Format("{0:0.00}", order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)))
                {
                    OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }

            }
            return NotificationMessahges.Fail;
        }

        private static bool CheckData(HttpRequest req)
        {
            Func<string, string> getParams = query => QueryHelper.ChangeQueryParam(req.Url.PathAndQuery, "hash", null);
            Func<string> postParams = () =>
                                      req.Form.Cast<KeyValuePair<string, string>>().Where(
                                          item => req["md_flds"].Contains(item.Key))
                                          .Aggregate(new StringBuilder(),
                                                     (curr, item) =>
                                                     curr.AppendFormat("{0}={1}&", item.Key, item.Value),
                                                     curr => curr.ToString().TrimEnd('&'));
            return !new[]
                        {
                            "valid",
                            "trans_id",
                            "hash",
                            "amount"
                        }.Any(param => string.IsNullOrEmpty(req[param]))
                   && req["valid"].ToLower() == "true"
                   && req["code"] == "A"
                   && (req.HttpMethod == "GET"
                           ? getParams(req.Url.PathAndQuery).Md5() == req["hash"]
                           : !string.IsNullOrEmpty(req["md_flds"]) && postParams().Md5() == req["hash"]);
        }
    }
}