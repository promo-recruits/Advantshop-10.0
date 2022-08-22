//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("ChronoPay")]
    public class ChronoPay : PaymentMethod
    {
        private const string Url = "https://payments.chronopay.com/"; //"https://secure.chronopay.com/index_shop.cgi";

        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string SharedSecret { get; set; }

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
            get { return UrlStatus.ReturnUrl; }
        }
        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {ChronoPayTemplate.ProductId, ProductId},
                               {ChronoPayTemplate.ProductName, ProductName},
                               {ChronoPayTemplate.SharedSecret, SharedSecret}
                           };
            }
            set
            {
                ProductId = value.ElementOrDefault(ChronoPayTemplate.ProductId);
                ProductName = value.ElementOrDefault(ChronoPayTemplate.ProductName);
                SharedSecret = value.ElementOrDefault(ChronoPayTemplate.SharedSecret);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr =
                String.Format(
                    CultureInfo.InvariantCulture,
                    "{0:0.00}",
                    order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency));
            
            return new PaymentForm
            {
                Url = Url,
                InputValues = new NameValueCollection
                {
                    {"product_id", ProductId},
                    {"product_name", ProductName},
                    {"product_price", orderSumStr},
                    {"product_price_currency", paymentCurrency.Iso3},
                    {"cb_url", NotificationUrl},
                    {"decline_url", CancelUrl},
                    {"success_url", SuccessUrl},
                    {"cb_type", "P"},
                    {"cs1", order.OrderID.ToString()},
                    {"sign", $"{ProductId}-{orderSumStr}-{SharedSecret}".Md5()}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (!ValidateResponseSign(req.Form))
            {
                return NotificationMessahges.InvalidRequestData;
            }

            if (Int32.TryParse(req["OrderID"], out var orderId))
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(order.OrderID, true);
                    return NotificationMessahges.SuccessfullPayment(order.OrderID.ToString());
                }
            }
            return NotificationMessahges.Fail;
        }

        private bool ValidateResponseSign(NameValueCollection rspParams)
        {
            string rspSign = rspParams["sign"];
            if (String.IsNullOrEmpty(rspSign))
            {
                return false;
            }
            return rspSign.Equals(String.Format("{0}{1}{2}{3}{4}", SharedSecret, rspParams["customer_id"], rspParams["transaction_id"], rspParams["transaction_type"], rspParams["total"]).Md5());
        }
    }
}