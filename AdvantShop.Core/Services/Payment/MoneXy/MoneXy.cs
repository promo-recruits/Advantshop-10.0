using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Web;
using System.Linq;
using System.Text;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Orders;

namespace AdvantShop.Payment
{
    [PaymentKey("MoneXy")]
    public class MoneXy : PaymentMethod
    {
        public string MerchantId { get; set; }
        public string MerchantCurrency { get; set; }
        public string ShopName { get; set; }
        public bool IsCheckHash { get; set; }
        public string SecretKey { get; set; }
        //public float CurrencyValue { get; set; }


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
                    {MoneXyTemplate.MerchantId, MerchantId},
                    {MoneXyTemplate.MerchantCurrency, MerchantCurrency},
                    {MoneXyTemplate.MerchantShopName, ShopName},
                    {MoneXyTemplate.IsCheckHash, IsCheckHash.ToString()},
                    {MoneXyTemplate.SecretKey, SecretKey},
                    //{MoneXyTemplate.MerchantCurrencyValue, CurrencyValue.ToString()}
                };
            }
            set
            {
                MerchantId = value.ElementOrDefault(MoneXyTemplate.MerchantId);
                MerchantCurrency = value.ElementOrDefault(MoneXyTemplate.MerchantCurrency);
                ShopName = value.ElementOrDefault(MoneXyTemplate.MerchantShopName);
                IsCheckHash = value.ElementOrDefault(MoneXyTemplate.IsCheckHash).TryParseBool();
                SecretKey = value.ElementOrDefault(MoneXyTemplate.SecretKey);

                //float decVal = 0;
                //CurrencyValue = value.ContainsKey(MoneXyTemplate.MerchantCurrencyValue) &&
                //                float.TryParse(value[MoneXyTemplate.MerchantCurrencyValue], out decVal)
                //                    ? decVal
                //                    : 1;
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var orderSumStr =
                order.Sum
                    .ConvertCurrency(order.OrderCurrency, PaymentCurrency ?? order.OrderCurrency)
                    .ToString("F2", CultureInfo.InvariantCulture);
            
            var formHandler = new PaymentForm
            {
                Url = "https://www.monexy.ua/merchant/merchant.php",
                InputValues = new NameValueCollection
                {
                    {"myMonexyMerchantID", MerchantId},
                    {"myMonexyMerchantShopName", ShopName},
                    {"myMonexyMerchantSum", orderSumStr},
                    {"myMonexyMerchantCurrency", MerchantCurrency},
                    {"myMonexyMerchantOrderId", order.OrderID.ToString()},
                    {"myMonexyMerchantOrderDesc", ""},
                    {"myMonexyMerchantResultUrl", this.SuccessUrl},
                    {"myMonexyMerchantSuccessUrl", this.SuccessUrl},
                    {"myMonexyMerchantFailUrl", this.FailUrl},
                },
                Encoding = Encoding.GetEncoding(1251)
            };

            var strBuild = new StringBuilder();
            foreach (var key in formHandler.InputValues.AllKeys.OrderBy(x => x))
            foreach (var value in formHandler.InputValues.GetValues(key) ?? new []{string.Empty})
                strBuild.Append(key + "=" + value);

            var paramStr = strBuild.ToString();

            var hash = IsCheckHash ? (paramStr + SecretKey).Md5(false) : paramStr.Md5(false);

            formHandler.InputValues.Add("myMonexyMerchantHash", hash);

            return formHandler;
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;

            if (req["MerchantId"].IsNotEmpty() && req["OrderId"].IsNotEmpty())
            {
                if (CheckFields(context))
                {
                    var orderId = req["OrderId"].TryParseInt();
                    var order = OrderService.GetOrder(orderId);
                    if (order != null)
                    {
                        OrderService.PayOrder(orderId, true, changedBy: new OrderChangedBy("Подтверждение оплаты платежной системой"));
                        return "OK";
                    }
                }
            }
            return string.Empty;
        }

        private bool CheckFields(HttpContext context)
        {
            // check hash
            return true;
        }
    }
}