﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System.Text;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;

namespace AdvantShop.Payment
{
    [PaymentKey("BitPay")]
    public class BitPay: PaymentMethod
    {
        /*
         * Documantation - https://bitpay.com/downloads/bitpayApi.pdf
         */
        public string ApiKey { get; set; }


        public override ProcessType ProcessType
        {
            get { return ProcessType.Javascript; } //.FormPost; }
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
                               {BitPayTemplate.ApiKey, ApiKey},
                           };
            }
            set
            {
                ApiKey = value.ElementOrDefault(BitPayTemplate.ApiKey);
            }
        }

        private string GetPosData(IList<OrderItem> items)
        {
            string res = "";

            for (int i = 0; i < items.Count; i++)
            {
                res += string.Format("\"artNo_{0}\": \"{1}\"", (i + 1), items[i].ArtNo);
            }

            return "'{ " + res + " }'";
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSum = 
                order.Sum
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            return new PaymentForm
            {
                Url = "https://bitpay.com/api/invoice/",
                InputValues = new NameValueCollection
                {
                    //{"posData", GetPosData(order.OrderItems)},
                    {"price", orderSum.ToString("F2", CultureInfo.InvariantCulture)},
                    {"currency", paymentCurrency.Iso3},
                    {"orderID", order.OrderID.ToString()},
                    {"itemDesc", GetOrderDescription(order.Number)},
                    {"physical", "true"}
                }
            };
        }

        public override string ProcessJavascript(Order order)
        {
            var sb = new StringBuilder();

            sb.AppendLine("<script type=\"text/javascript\"> ");
            sb.AppendLine("function bitpay() { ");
            sb.AppendLine("$.ajax({ dataType: \"json\", type: \"POST\",  url: \"httphandlers/orderconfirmation/bitpayservice.ashx\",\n");
            sb.AppendFormat("  data: {{ orderId: \"{0}\" }}, ",  order.OrderID);
            sb.AppendLine("  success: function (data) { if (data != null && data.error == \"\") { window.location = data.url; } },");
            sb.AppendLine("});");

            sb.AppendLine("} ");
            sb.AppendLine("</script>");

            return sb.ToString();
        }

        public override string ProcessJavascriptButton(Order order)
        {
            return "bitpay();";
        }



        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            try
            {
                Debug.Log.Error(string.Format("{0}{1}{2}{3}", req["id"], req["url"], req["posData"], req["status"]));
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return NotificationMessahges.InvalidRequestData;
        }        
    }
}