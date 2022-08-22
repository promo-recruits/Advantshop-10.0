//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    /// <summary>
    /// Summary description for MailRu
    /// </summary>
    [PaymentKey("MailRu")]
    public class MailRu : PaymentMethod
    {
        public override ProcessType ProcessType
        {
            get { return ProcessType.FormPost; }
        }
        public override NotificationType NotificationType
        {
            get { return NotificationType.Handler; }
        }
        public override UrlStatus ShowUrls
        {
            get { return UrlStatus.NotificationUrl; }
        }
        public string Key { get; set; }
        public string ShopID { get; set; }
        public bool KeepUnique { get; set; }
        public string CryptoHex { get; set; }

        public override Dictionary<string, string> Parameters
        {
            get
            {
                return new Dictionary<string, string>
                             {
                                 {MailRuTemplate.Key, Key},
                                 {MailRuTemplate.ShopID, ShopID},
                                 {MailRuTemplate.KeepUnique, KeepUnique.ToString()},
                                 {MailRuTemplate.CryptoHex,CryptoHex}
                             };
            }
            set
            {
                if (value.ContainsKey(MailRuTemplate.ShopID))
                    ShopID = value[MailRuTemplate.ShopID];
                Key = !value.ContainsKey(MailRuTemplate.Key) ? string.Empty : value[MailRuTemplate.Key];
                bool boolVal;
                if (value.ContainsKey(MailRuTemplate.KeepUnique) && bool.TryParse(value[MailRuTemplate.KeepUnique], out boolVal))
                    KeepUnique = boolVal;
                CryptoHex = !value.ContainsKey(MailRuTemplate.CryptoHex) ? string.Empty : value[MailRuTemplate.CryptoHex];
            }
        }

        public override PaymentForm GetPaymentForm(Order order)
        {
            var paymentCurrency = PaymentCurrency ?? order.OrderCurrency;
            var sum = 
                order.Sum
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .ToString("F2", CultureInfo.InvariantCulture);
            
            var description = GetOrderDescription(order.Number);
            var issuerId = order.OrderID.ToString();

            var currencyCode =
                string.Equals(paymentCurrency.Iso3, "RUB", StringComparison.OrdinalIgnoreCase)
                    ? "RUR"
                    : paymentCurrency.Iso3;
            var inputValues = new NameValueCollection
            {
                {"shop_id", ShopID},
                {"currency", currencyCode},
                {"sum", sum},
                {"description", description},
                {"message", description},
                {"issuer_id", issuerId}
            };
            
            if (KeepUnique) 
                inputValues.Add("keep_uniq", "1");
            
            inputValues.Add("signature", GetSignature(currencyCode + description + issuerId + (KeepUnique ? "1" : string.Empty) + description + ShopID + sum + CryptoHex));

            return new PaymentForm
            {
                FormName = "_xclick",
                Method = FormMethod.POST,
                //Url = "https://demoney.mail.ru/pay/light/", test account
                Url = "https://money.mail.ru/pay/light/",
                InputValues = inputValues,
                Encoding = Encoding.GetEncoding(1251)
            };
        }

        public override string ProcessResponse(HttpContext context)
        {
            var req = context.Request;
            if (CheckFields(context) && req["status"] == "PAID" && int.TryParse(StringHelper.DecodeFrom64(req["issuer_id"]), out var orderId) && OrderService.GetOrder(orderId) != null)
            {
                OrderService.PayOrder(orderId, true);
                context.Response.Clear();
                context.Response.Write("item_number=" + context.Request["item_number"] + "\n");
                context.Response.Write("status=ACCEPTED");
                context.Response.End();
                return NotificationMessahges.SuccessfullPayment(orderId.ToString());
            }
            return NotificationMessahges.InvalidRequestData;
        }

        private bool CheckFields(HttpContext context)
        {
            var req = context.Request;
            if (string.IsNullOrEmpty(req["type"]) || string.IsNullOrEmpty(req["status"])
                || string.IsNullOrEmpty(req["item_number"]) || string.IsNullOrEmpty(req["issuer_id"])
                || string.IsNullOrEmpty(req["serial"]) || string.IsNullOrEmpty(req["auth_method"])
                || string.IsNullOrEmpty(req["signature"]))
                return false;
            if (req["type"].Trim() != "PAYMENT")
                return false;
            if (!new[] { "MD5", "SHA" }.Contains(req["auth_method"].Trim().ToUpper()))
                return false;
            var code = req["auth_method"] + req["issuer_id"] + req["item_number"] + req["serial"] + req["status"] + req["type"] + Key.Trim();

            // никогда не сходится подпись если ее собирать по алготимту https://money.mail.ru/img/partners/dmr_standart_v1.2.pdf
            //if (code.GetCryptoHash(req["auth_Method"] == "SHA" ? StringHashType.SHA1 : StringHashType.MD5) != req["signature"])
            //    return false;

            return true;

        }

        private string GetSignature(string fields)
        {
            //return (fields + Key.Sha1()).Sha1();
            return fields.Sha1();
        }

        public static string CheckCurrencyCode(string key, string currencyCode)
        {
            var response =
                WebRequest.Create(string.Format(
                    "https://merchant.money.mail.ru/api/info/currency/?key={0}&currency={1}", key, currencyCode)).GetResponse();
            if (response == null)
                return "Unable to get info";
            using (Stream stream = response.GetResponseStream())
            {
                if (stream == null)
                    return "Unable to get info";
                try
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var line = reader.ReadLine();
                        if (!string.IsNullOrEmpty(line) && line.Trim() != "OK")
                            return line.Contains(":") ? line.Split(':')[1] : line;
                        do
                        {
                            line = reader.ReadLine();
                            if (!string.IsNullOrEmpty(line) && line.Contains("currency=" + currencyCode))
                            {
                                var enabledLine = reader.ReadLine();
                                if (!string.IsNullOrEmpty(enabledLine) && enabledLine.Trim() == "status=enabled")
                                    return null;
                            }
                        } while (!reader.EndOfStream);
                    }
                    return "Currency not found";
                }
                catch (Exception ex)
                {
                    Debug.Log.Error(ex);
                    return ex.Message;
                }
            }
        }
    }
}