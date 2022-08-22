//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("IntellectMoney")]
    public class IntellectMoney : PaymentMethod
    {
        public string MerchantId { get; set; }
        public string SecretKey { get; set; }
        

        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }

        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler | NotificationType.ReturnUrl; }
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
                                 {IntellectMoneyTemplate.MerchantId, MerchantId},
                                 {IntellectMoneyTemplate.SecretKey, SecretKey}
                             };

            }
            set
            {
                MerchantId = value.ElementOrDefault(IntellectMoneyTemplate.MerchantId);
                SecretKey = value.ElementOrDefault(IntellectMoneyTemplate.SecretKey);
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = "https://Merchant.IntellectMoney.ru/",
                InputValues = new NameValueCollection
                {
                    {"lmi_payee_purse", MerchantId},
                    {
                        "LMI_PAYMENT_AMOUNT",
                        order.Sum
                            .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                            .ToString("F2", CultureInfo.InvariantCulture)
                    },
                    {"LMI_PAYMENT_DESC", GetOrderDescription(order.Number)},
                    {"LMI_PAYMENT_NO", order.OrderID.ToString()},
                    {"lmi_result_url", NotificationUrl},
                    {"lmi_success_url", SuccessUrl},
                    {"lmi_success_method", "1"}, // POST
                    {"lmi_fail_url", FailUrl},
                    {"lmi_fail_method", "1"}, // POST
                    //{"lmi_expire_date", System.DateTime.Now.AddDays(10).ToString("yyyyMMdd hh:MM:ss")}, // Срок действия СКО
                    //{"lmi_sim_mode", Sandbox ? "1" : "0"},
                    //{"EMAIL", order.OrderCustomer.Email},
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;
 
            // Форма предварительного запроса
            if (req["LMI_PREREQUEST"].IsNullOrEmpty() && req["LMI_PREREQUEST"] == "1")
            {
                context.Response.Write("YES");
                context.Response.End();
                return string.Empty;
            }

            if (CheckFields(req) && int.TryParse(req["LMI_PAYMENT_NO"], out var orderId))
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));

                    context.Response.Write("YES");
                    context.Response.End();
                    return string.Empty;
                    //return NotificationMessahges.SuccessfullPayment(orderID.ToString());
                }
            }

            if (context.Request.Url.AbsolutePath.EndsWith(".aspx"))
                return "Заказ успешно оплачен.";

            return NotificationMessahges.InvalidRequestData;
        }

        private bool CheckFields(HttpRequest req)
        {
            return new[]
                        {
                            "LMI_PAYEE_PURSE",
                            "LMI_PAYMENT_AMOUNT",
                            "LMI_PAYMENT_NO",
                            "LMI_MODE",
                            "LMI_SYS_INVS_NO",
                            "LMI_SYS_TRANS_NO",
                            "LMI_SYS_TRANS_DATE",
                            "LMI_PAYER_PURSE",
                            "LMI_PAYER_WM"
                        }.All(field => !string.IsNullOrEmpty(req[field]))
                       &&
                       (req["LMI_PAYEE_PURSE"] +
                        req["LMI_PAYMENT_AMOUNT"] +
                        req["LMI_PAYMENT_NO"] +
                        req["LMI_MODE"] +
                        req["LMI_SYS_INVS_NO"] +
                        req["LMI_SYS_TRANS_NO"] +
                        req["LMI_SYS_TRANS_DATE"] +
                        SecretKey +
                        req["LMI_PAYER_PURSE"] +
                        req["LMI_PAYER_WM"])
                           .Md5(true) == req["LMI_HASH"];
        }
    }
}