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
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for PSIGate
    /// </summary>
    [PaymentKey("PSIGate")]
    public class PSIGate : PaymentMethod
    {
        private string Url
        {
            get
            {
                return Sandbox
                           ? "https://devcheckout.psigate.com/HTMLPost/HTMLMessenger"
                           : "https://checkout.psigate.com/HTMLPost/HTMLMessenger";
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

        public string StoreKey { get; set; }
        public bool Sandbox { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {PSIGateTemplate.StoreKey, StoreKey},
                               {PSIGateTemplate.Sandbox, Sandbox.ToString()},
                           };
            }
            set
            {
                StoreKey = value.ElementOrDefault(PSIGateTemplate.StoreKey);

                bool boolval;
                if (bool.TryParse(value.ElementOrDefault(PSIGateTemplate.Sandbox), out boolval))
                    Sandbox = boolval;
            }

        }
 
        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentNo = order.OrderID.ToString();
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr = order.Sum
                .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                .ToString("F2",CultureInfo.InvariantCulture);
            
            var inputVals = new NameValueCollection
            {
                {"StoreKey", StoreKey},
                {"ThanksURL", SuccessUrl},
                {"NoThanksURL", CancelUrl},
                {"ResponseFormat", "HTML1"},
                {"OrderID", paymentNo},
                {"FullTotal", orderSumStr}
            };
            if (Sandbox)
                inputVals.Add("TestResult", "R"); // Random test results
            
            return new PaymentForm
            {
                Url = Url,
                InputValues = inputVals
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (!CheckData(req))
                return NotificationMessahges.InvalidRequestData;
            var paymentNumber = req["OrderID"];
            if (int.TryParse(paymentNumber, out var orderId) &&
                OrderService.GetOrder(orderId) != null &&
                (req["Approved"].ToUpper() == "APPROVED" && req["ReturnCode"][0] == 'Y'))
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null &&
                    req["FullTotal"] == order.Sum
                        .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                        .ToString("F2", CultureInfo.InvariantCulture))
                {
                    OrderService.PayOrder(orderId, true,
                        changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }

            }
            return NotificationMessahges.Fail;
        }

        private bool CheckData(HttpRequest req)
        {
            return
            !new[]
                 {
                     "OrderID",
                     "FullTotal",
                     "Approved",
                     "ReturnCode"
                 }.Any(param => string.IsNullOrEmpty(req[param]));
        }
    }

}