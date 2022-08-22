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
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("eWAY")]
    public class eWAY : PaymentMethod
    {
        public string CustomerID { get; set; }
        public bool Sandbox { get; set; }
        //public string ReturnUrl { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.CancelUrl | UrlStatus.ReturnUrl | UrlStatus.NotificationUrl; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               //{eWAYTemplate.ReturnUrl, ReturnUrl},
                               {eWAYTemplate.Sandbox, Sandbox.ToString()},
                               {eWAYTemplate.CustomerID, CustomerID},
                           };
            }
            set
            {
                //ReturnUrl = value.ContainsKey(eWAYTemplate.ReturnUrl) ? value[eWAYTemplate.ReturnUrl] : "";
                CustomerID = value.ElementOrDefault(eWAYTemplate.CustomerID);
                Sandbox = value.ElementOrDefault(eWAYTemplate.Sandbox).TryParseBool();
            }
        }
        
        public override PaymentForm GetPaymentForm(Order order)
        {
            var orderSum = order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency);
            return new PaymentForm
            {
                Url = Sandbox ? "https://au.ewaygateway.com/Request/" : "https://www.eway.com.au/gateway/payment.asp",
                InputValues = new NameValueCollection
                {
                    {"ewayCustomerID", CustomerID},
                    {"eWAYURL", SuccessUrl},
                    {"ewayTotalAmount", (orderSum * 100).ToString("F0", CultureInfo.InvariantCulture)},
                    {"ewayCustomerInvoiceDescription", GetOrderDescription(order.Number)},
                    {"ewayTrxnNumber", order.Number}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            //TODO some other response processing
            if (Sandbox)
                return NotificationMessahges.TestMode;

            try
            {
                if (context.Request.GetUrlReferrer() == null || context.Request.GetUrlReferrer().Host != "www.eway.com.au")
                    return NotificationMessahges.InvalidRequestData;
                var form = context.Request.Form;
                var orderNumber = form["ewayTrxnNumber"];
                var status = form["ewayTrxnStatus"].TryParseBool();
                var sum = form["eWAYReturnAmount"].Replace(",", "").Replace("$", "").Replace(".", "").TryParseFloat();
                var responseText = form["eWAYresponseText"];


                var order = OrderService.GetOrderByNumber(orderNumber);
                if (status && Math.Abs(order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency) - sum / 100) > 1f)
                    return NotificationMessahges.InvalidRequestData;

                OrderService.PayOrder(order.OrderID, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                return NotificationMessahges.SuccessfullPayment(orderNumber);
            }
            catch (Exception ex)
            {
                return NotificationMessahges.LogError(ex);
            }
        }
    }
}