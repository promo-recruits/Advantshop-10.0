//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("CyberPlat")]
    public class CyberPlat : PaymentMethod
    {
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var orderSumStr = order.Sum
                .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                .ToString("F2", CultureInfo.InvariantCulture);
            string message = "Orderid=" + order.OrderID + "&Amount=" + orderSumStr + "&Currency=" + "&PaymentDetails=" + "&Email=" + "&FirstName=" +
                             "&LastName=" + "&MiddleName=none&Phone=" + "&Address=" + "&Language=" + "&return_url=";

            return new PaymentForm
            {
                Url = "https://card.cyberplat.ru/cgi-bin/getform.cgi",
                InputValues = new NameValueCollection
                {
                    {"version", "2.0"},
                    {"message", message}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;
            var paymentNumber = req["pg_order_id"];
            if (int.TryParse(paymentNumber, out var orderId) && OrderService.GetOrder(orderId) != null)
            {
                OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                return NotificationMessahges.SuccessfullPayment(paymentNumber);
            }
            return NotificationMessahges.Fail;
        }


        private static bool CheckData(HttpRequest req)
        {
            return true;
        }
    }
}