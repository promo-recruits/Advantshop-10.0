//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

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
    [PaymentKey("SagePay")]
    public class SagePay : PaymentMethod
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
            get { return NotificationType.ReturnUrl; }
        }

        public override bool CurrencyAllAvailable => false;

        public override string[] CurrencyIso3Available => new[] {"EUR", "GBP", "USD"};

        public string Vendor { get; set; }
        public bool Sandbox { get; set; }
        public string Password { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                {
                    {SagePayTemplate.Vendor, Vendor},
                    {SagePayTemplate.Sandbox, Sandbox.ToString()},
                    {SagePayTemplate.Password, Password},
                };
            }
            set
            {
                Vendor = value.ElementOrDefault(SagePayTemplate.Vendor);
                Sandbox = value.ElementOrDefault(SagePayTemplate.Sandbox).TryParseBool();
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

                    {"VPSProtocol", "2.23"},
                    {"TxType", "PAYMENT"},
                    {"Vendor", Vendor},
                    {"VendorTxCode", paymentNo},
                    {"Amount", orderSumStr},
                    {"Currency", currencyCode},
                    {"Description", description},
                    {"SuccessURL", SuccessUrl},
                    {"FailureURL", FailUrl},
                    {"Crypt", GetCrypt(paymentNo, orderSumStr, currencyCode, description, SuccessUrl, FailUrl)}
                }
            };
        }

        private string GetCrypt(string vendorTxCode, string amount, string currency, string description,
            string successUrl, string failUrl)
        {
            return
                SagePayUtils.EncryptAndEncode(
                    string.Format("VendorTxCode={0}Amount={1}Currency={2}Description={3}SuccessURL={4}FailureURL={5}",
                        vendorTxCode, amount, currency, description, successUrl, failUrl), Password);

        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (GetData(req) == null)
                return NotificationMessahges.InvalidRequestData;
            var paymentNumber = req["VendorTxCode"];
            if (int.TryParse(paymentNumber, out var orderId) &&
                OrderService.GetOrder(orderId) != null)
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null && 
                    req["Amount"] == string.Format("{0:0.00}", order.Sum.ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)))
                {
                    OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                    return NotificationMessahges.SuccessfullPayment(order.Number);
                }

            }
            return NotificationMessahges.Fail;
        }

        private NameValueCollection GetData(HttpRequest req)
        {
            if (string.IsNullOrEmpty(req["Crypt"]))
                return null;
            var queryParams = HttpUtility.ParseQueryString(SagePayUtils.DecodeAndDecrypt(req["Crypt"], Password));
            return new[]
            {
                "Status",
                "StatusDetail",
                "VendorTxCode",
                "VPSTxId",
                "Amount",
            }.Any(item => string.IsNullOrEmpty(queryParams[item]))
                ? null
                : queryParams;
        }
    }
}