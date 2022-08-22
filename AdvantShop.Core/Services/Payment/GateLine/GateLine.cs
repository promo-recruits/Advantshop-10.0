using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Orders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Payment
{
    [PaymentKey("GateLine")]
    public class GateLine : PaymentMethod
    {
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
            get { return UrlStatus.NotificationUrl | UrlStatus.FailUrl | UrlStatus.ReturnUrl; }
        }
        public string Site { get; set; }
        public string Password { get; set; }
        public bool TestMode { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                           {
                               {GateLineTemplate.Site, Site},
                               {GateLineTemplate.Password, Password},
                               {GateLineTemplate.TestMode, TestMode.ToString()}
                           };
            }
            set
            {
                Site = value.ElementOrDefault(GateLineTemplate.Site);
                Password = value.ElementOrDefault(GateLineTemplate.Password);
                TestMode = value.ElementOrDefault(GateLineTemplate.TestMode).TryParseBool();
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var values = new NameValueCollection
            {
                {
                    "amount",
                    order.Sum
                        .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                        .ToString("F2", CultureInfo.InvariantCulture)
                },
                {"description", GetOrderDescription(order.Number)},
                {"site", Site},
                //{"email", Email},
                {"merchant_order_id", order.OrderID.ToString()}
            };
            values.Add("checksum", GetCheckSum(values));
            
            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                Url = string.Format("https://{0}/pay",
                    TestMode ? "simpleapi.sandbox.gateline.net:18610" : "simpleapi.gateline.net"),
                InputValues = values
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            HttpRequest req = context.Request;

            if (req["operation"].IsNotEmpty() && req["operation"] == "test")
            {
                context.Response.Write("SUCCESS");
                context.Response.End();
                return string.Empty;
            }

            if (CheckFields(req) && int.TryParse(req["merchant_order_id"], out var orderId))
            {
                Order order = OrderService.GetOrder(orderId);
                if (order != null)
                {
                    OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                    return NotificationMessahges.SuccessfullPayment(orderId.ToString());
                }
            }
            return NotificationMessahges.InvalidRequestData;
        }

        private string GetCheckSum(NameValueCollection values)
        {
            var parameters = new List<string>();
            foreach (var key in values.AllKeys.OrderBy(key => key))
            foreach (var value in values.GetValues(key) ?? new []{string.Empty})
            {
                parameters.Add($"{key}={value}");
            }

            var input = parameters.AggregateString(';');
            var privateKey = Password;

            var encoding = new UTF8Encoding();

            var hmac = new HMACSHA1(encoding.GetBytes(privateKey));
            var hash = hmac.ComputeHash(encoding.GetBytes(input));

            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private bool CheckFields(HttpRequest req)
        {
            // message - Описание результата
            // status - Статус операции
            // order_id* - ID ордера
            // merchant_order_id* - Идентификатор заказа в системе клиента
            // code** - Код ошибки
            // checksum - Контрольная сумма
            // * - поле может не передаваться, если установлен статус error
            // ** - поле передается только для статуса error

            if (new[] { "message", "status", "checksum" }.Any(val => string.IsNullOrEmpty(req[val])))
                return false;

            // status - Статус операции:
            // success - Операция проведена успешно
            // failed - Операция была инициирована, но не завершилась удачно по какой-либо причине.
            // error - Возникла проблема, которая не позволяет запустить проведение операции.
            if (req["status"] != "success")
                return false;

            var values = new NameValueCollection
                {
                    {"message", req["message"]},
                    {"status", req["status"]}
                };
            if (!string.IsNullOrEmpty(req["order_id"]))
                values.Add("order_id", req["order_id"]);
            if (!string.IsNullOrEmpty(req["merchant_order_id"]))
                values.Add("merchant_order_id", req["merchant_order_id"]);
            if (!string.IsNullOrEmpty(req["code"]))
                values.Add("code", req["code"]);

            return GetCheckSum(values).ToLower() == req["checksum"].ToLower();
        }
    }
}