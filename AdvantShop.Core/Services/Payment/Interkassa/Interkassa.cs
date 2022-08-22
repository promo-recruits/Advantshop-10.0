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
    [PaymentKey("Interkassa")]
    public class Interkassa : PaymentMethod
    {
        public string ShopId { get; set; }
        
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

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {InterkassaTemplate.ShopId, ShopId}
                           };
            }
            set
            {
                ShopId = value.ElementOrDefault(InterkassaTemplate.ShopId);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            return new PaymentForm
            {
                Url = "http://www.interkassa.com/lib/payment.php",
                InputValues = new NameValueCollection
                {
                    {"ik_shop_id", ShopId},
                    {
                        "ik_payment_amount",
                        order.Sum
                            .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                            .ToString("F2", CultureInfo.InvariantCulture)
                    },
                    {"ik_payment_id", order.OrderID.ToString()},
                    {"ik_payment_desc", GetOrderDescription(order.Number)},
                    {"ik_paysystem_alias", ""}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            var orderID = 0;
            if (CheckFields(context) && req["ik_payment_state"] == "success" && int.TryParse(req["ik_payment_id"], out orderID))
            {
                Order order = OrderService.GetOrder(orderID);
                if (order != null)
                {
                    OrderService.PayOrder(orderID, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                    return NotificationMessahges.SuccessfullPayment(orderID.ToString());
                }
            }
            return NotificationMessahges.InvalidRequestData;
        }

        private bool CheckFields(HttpContext context)
        {
            // check summ
            return true;
        }
    }
}