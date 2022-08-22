//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment.Alfabank;
using AdvantShop.Orders;
using AdvantShop.Taxes;

namespace AdvantShop.Payment
{
    [PaymentKey("Alfabank")]
    public class Alfabank : PaymentMethod
    {
        public const string AlfabankOrderId = "alfabankorderId";

        public string UserName { get; set; }
        public string Password { get; set; }
        public string MerchantLogin { get; set; }
        public string UseTestMode { get; set; }
        public bool SendReceiptData { get; set; }
        public string Taxation { get; set; }
        public string GatewayCountry { get; set; }

        public override ProcessType ProcessType
        {
            get { return ProcessType.ServerRequest; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.ReturnUrl; }
        }

        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.FailUrl | UrlStatus.ReturnUrl; }
        }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {AlfabankTemplate.UserName, UserName},
                               {AlfabankTemplate.Password, Password},
                               {AlfabankTemplate.MerchantLogin, MerchantLogin},
                               {AlfabankTemplate.UseTestMode, UseTestMode},
                               {AlfabankTemplate.SendReceiptData, SendReceiptData.ToString()},
                               {AlfabankTemplate.Taxation, Taxation},
                               {AlfabankTemplate.GatewayCountry, GatewayCountry},
                           };
            }
            set
            {
                UserName = value.ElementOrDefault(AlfabankTemplate.UserName);
                Password = value.ElementOrDefault(AlfabankTemplate.Password);
                MerchantLogin = value.ElementOrDefault(AlfabankTemplate.MerchantLogin);
                UseTestMode = value.ElementOrDefault(AlfabankTemplate.UseTestMode);
                SendReceiptData = value.ElementOrDefault(AlfabankTemplate.SendReceiptData).TryParseBool();
                Taxation = value.ElementOrDefault(AlfabankTemplate.Taxation);
                GatewayCountry = value.ElementOrDefault(AlfabankTemplate.GatewayCountry);
            }
        }

        public override string ProcessServerRequest(Order order)
        {
            var tax = TaxId.HasValue ? TaxService.GetTax(TaxId.Value) : null;
            var service = new AlfabankService(GatewayCountry, UserName, Password, MerchantLogin, UseTestMode);
            var response = service.Register(order, GetOrderDescription(order.Number), SendReceiptData, Taxation, PaymentCurrency, SuccessUrl, FailUrl, tax);

            if (response != null)
                return response.FormUrl;

            return "";
        }

        public override string ProcessResponse(HttpContext context)
        {
            if (context.Request[AlfabankService.ReturnUrlParamNameMerchantOrder].IsNullOrEmpty())
            {
                return NotificationMessahges.InvalidRequestData;
            }

            var orderNumber = context.Request[AlfabankService.ReturnUrlParamNameMerchantOrder];

            var order = orderNumber.IsNotEmpty() ? OrderService.GetOrderByNumber(orderNumber) : null;

            if (order == null)
                return NotificationMessahges.InvalidRequestData;

            var service = new AlfabankService(GatewayCountry, UserName, Password, MerchantLogin, UseTestMode);
            var response = service.GetOrderStatus(context.Request[AlfabankService.ReturnUrlParamNameAlfaOrder], context.Request[AlfabankService.ReturnUrlParamNameMerchantOrder]);

            if (response == null || response.ErrorCode != 0 || response.OrderStatus != "2")
                return NotificationMessahges.InvalidRequestData;

            OrderService.PayOrder(order.OrderID, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
            return NotificationMessahges.SuccessfullPayment(order.Number);
        }

    }
}
