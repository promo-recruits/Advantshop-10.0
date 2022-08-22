using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System.Text;
using AdvantShop.Core.Services.Catalog;
using Newtonsoft.Json;

namespace AdvantShop.Payment
{
    [PaymentKey("Qppi")]
    public class Qppi: PaymentMethod
    {
        public string MerchantXid { get; set; }
        public string PrivateSecurityKey { get; set; }
        public bool Sandbox { get; set; }
        public string ExternalProjectName { get; set; }

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
                               {QppiTemplate.MerchantXid, MerchantXid},
                               {QppiTemplate.PrivateSecurityKey, PrivateSecurityKey},
                               {QppiTemplate.Sandbox, Sandbox.ToString()},
                               {QppiTemplate.ExternalProjectName, ExternalProjectName}
                           };
            }
            set
            {
                MerchantXid = value.ElementOrDefault(QppiTemplate.MerchantXid);
                PrivateSecurityKey = value.ElementOrDefault(QppiTemplate.PrivateSecurityKey);
                Sandbox = value.ElementOrDefault(QppiTemplate.Sandbox).TryParseBool();
                ExternalProjectName = value.ElementOrDefault(QppiTemplate.ExternalProjectName);
            }
        }
        
        /*
         * MerchantXid - Идентификатор Партнера в системе Куппи.ру (выдается Куппи.ру)
         * ExternalProjectID - Идентификатор Проекта, для которого Клиент запрашивает оплату услуг
         * ExternalProjectName - Наименование проекта, для которого Клиент запрашивает оплату услуг
         * ExternalUserID - Идентификатор Клиента в Проекте (username, игровой ник и т.п.)
         * CurrencySid - Тип валюты (USD, RUR)
         * MerchantOrderNo - Уникальный номер Заказа в систему Партнера
         * Amount - Сумма заказа
         * NotifyIfFailed - Флаг, указывающий, требуется ли уведомлять (вызовом PayNotification) о неудачной операции или нет (default=False) 
         * IsTestMode - Флаг, указывающий, используется ли тестовый режим
         * SecurityKey - md5 в ASCII кодировке от {MerchantOrderNo}+{NowUtc}+{PrivateSecurityKey}+{Amount}+{ExternalUserId}+{IsTestMode}
         * ExtraParams- Дополнительные параметры (не обяз.)
         */
        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var orderSumStr = order.Sum
                .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                .ToInvariantString();
            var paymentCurrencyIso3 = string.Equals(paymentCurrency.Iso3, "RUB", StringComparison.OrdinalIgnoreCase) ? "RUR" : paymentCurrency.Iso3;
            return new PaymentForm
            {
                Url = "http://www.qppi.ru/Pay/Listener",
                InputValues = new NameValueCollection
                {
                  {"MerchantXid", MerchantXid},
                  {"PrivateSecurityKey", PrivateSecurityKey},
                  {"ExternalProjectID", order.OrderID.ToString()},
                  {"ExternalProjectName", ExternalProjectName},
                  {"ExternalUserID", order.OrderCustomer.Email},
                  {
                      "CurrencySid",
                      paymentCurrencyIso3},
                  {"MerchantOrderNo", order.OrderID.ToString()},
                  {"Amount", orderSumStr},
                  {"IsTestMode", Sandbox.ToString()},
                  {"SecurityKey", string.Format("{0}{1}{2}{3}{4}{5}", order.OrderID, DateTime.Now.ToString("yyyyMMddHHmmss"), PrivateSecurityKey, orderSumStr, order.OrderCustomer.Email, Sandbox).Md5(false, Encoding.ASCII)},
                  {"NowUtc", DateTime.Now.ToString("yyyyMMddHHmmss")}
                }
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (int.TryParse(req["orderId"], out var orderId))
            {
                var order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));

                    context.Response.Write(JsonConvert.SerializeObject(new { Code = "200", Comment = "Ok" }));
                    context.Response.End();
                    return "";
                }
            }

            context.Response.Write(JsonConvert.SerializeObject(new { Code = "-405", Comment = "BadOrderId" }));
            context.Response.End();
            return "";
        }
    }
}