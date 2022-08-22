//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;
using System.Linq;
using System.Reflection;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Taxes;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Core.Services.Payment.Tinkoff
{
    // Документация: https://oplata.tinkoff.ru/landing/develop/documentation/termins_and_operations

    public class TinkoffService
    {
        private const string BaseUrl = "https://securepay.tinkoff.ru/v2/";

        private readonly string _terminalKey;
        private readonly string _secretKey;
        private readonly bool _sendReceiptData;

        public TinkoffService(string terminalKey, string secretKey, bool sendReceiptData)
        {
            _terminalKey = terminalKey;
            _secretKey = secretKey;
            _sendReceiptData = sendReceiptData;
        }

        /// <summary>
        /// Запрос создание заказа
        /// </summary>
        public TinkoffInitResponse Init(Order order, string description, string taxation, Currency paymentCurrency, TaxElement tax)
        {
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;
            var receipt = _sendReceiptData
                ? new Receipt()
                {
                    Email = !string.IsNullOrEmpty(order.OrderCustomer.Email) ? order.OrderCustomer.Email : null,
                    Phone = order.OrderCustomer.StandardPhone.HasValue && order.OrderCustomer.StandardPhone.ToString().Length <= 15 
                        ? ("+" + order.OrderCustomer.StandardPhone.ToString()) 
                        : null,
                    Taxation = taxation,
                    Items =
                        order
                            .GetOrderItemsForFiscal(paymentCurrency)
                            .Select(item => new Item()
                            {
                                Name = (item.Name + (item.Color.IsNotEmpty() || item.Size.IsNotEmpty()
                                    ? string.Format(" ({0}{1}{2})",
                                        item.Color.IsNotEmpty()
                                            ? $"{SettingsCatalog.ColorsHeader}: {item.Color}"
                                            : null,
                                        item.Color.IsNotEmpty() && item.Size.IsNotEmpty()
                                            ? ", "
                                            : null,
                                        item.Size.IsNotEmpty()
                                            ? $"{SettingsCatalog.SizesHeader}: {item.Size}"
                                            : null)
                                    : string.Empty)).Reduce(128),
                                ShopCode = item.ArtNo,
                                Amount = (int)Math.Round(item.Price * item.Amount * 100, 0),
                                Price = (int)Math.Round(item.Price * 100, 0),
                                Quantity = item.Amount,
                                Tax = GetTaxType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType),
                                PaymentMethod = GetPaymentMethodType(item.PaymentMethodType),
                                PaymentObject = item.PaymentSubjectType.ToString()
                            })
                            .ToList()
                }
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.Items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        Name = "Подарочный сертификат",
                        ShopCode = x.CertificateCode,
                        Amount = (int)(Math.Round(x.Sum * 100, 0)),
                        Price = (int)(Math.Round(x.Sum * 100, 0)),
                        Quantity = 1,
                        Tax = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentMethod = GetPaymentMethodType(AdvantShop.Configuration.SettingsCertificates.PaymentMethodType),
                        PaymentObject = AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType.ToString()
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.Items.Add(new Item()
                {
                    Name = "Доставка",
                    ShopCode = "Доставка",
                    Amount = (int)(Math.Round(orderShippingCostWithDiscount * 100, 0)),
                    Price = (int)(Math.Round(orderShippingCostWithDiscount * 100, 0)),
                    Quantity = 1,
                    Tax = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType),
                    PaymentMethod = GetPaymentMethodType(order.ShippingPaymentMethodType),
                    PaymentObject = order.ShippingPaymentSubjectType.ToString()
                });
            }
            
            long sum = 0;

            if (receipt != null && receipt.Items != null)
            {
                foreach (var item in receipt.Items)
                    sum += item.Amount;
            }
            else
            {
                sum = (long)Math.Round(Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2) * 100);
            }

            TinkoffInitResponse response;
            
            var orderStrId = string.Format("{0}_{1}", order.OrderID, DateTime.Now.ToUnixTime());

            var data = new TinkoffInitData
            {
                TerminalKey = _terminalKey,
                Amount = sum, // Сумма платежа в копейках
                OrderId = orderStrId,
                Description = description,
                Receipt = receipt
            };

            response = MakeRequest<TinkoffInitResponse, TinkoffInitData>("Init", data);

            if (response == null)
                return null;

            if (!response.Success)
            {
                Debug.Log.Warn(string.Format("TinkoffService Init. code: {0} error: {1}, details: {2}, response: {3}",
                    response.ErrorCode, response.Message, response.Details, JsonConvert.SerializeObject(response)));
            }
            
            return response;
        }

        #region Private methods

        private string GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return "none";

            if (taxType.Value == TaxType.Vat0)
                return "vat0";

            if (taxType.Value == TaxType.Vat10)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat110";
                else
                    return "vat10";
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat118";
                else
                    return "vat18";
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return "vat120";
                else
                    return "vat20";
            }


            return "none";
        }

        private string GetPaymentMethodType(ePaymentMethodType paymentMethodType)
        {
            switch (paymentMethodType)
            {
                case ePaymentMethodType.full_prepayment:
                    return "full_prepayment";
                case ePaymentMethodType.partial_prepayment:
                    return "prepayment"; // из-за этого значения нельзя enum.Tostring();
                case ePaymentMethodType.advance:
                    return "advance";
                case ePaymentMethodType.full_payment:
                    return "full_payment";
                case ePaymentMethodType.partial_payment:
                    return "partial_payment";
                case ePaymentMethodType.credit:
                    return "credit";
                case ePaymentMethodType.credit_payment:
                    return "credit_payment";
                default:
                    throw new NotImplementedException(paymentMethodType.ToString() + " not implemented in Tinkoff");
            }
        }


        private T MakeRequest<T, TData>(string url, TData data = null) 
            where T : class
            where TData : TinkoffBaseData
        {
            try
            {
                var request = WebRequest.Create(BaseUrl + url) as HttpWebRequest;
                request.Timeout = 5000;
                request.Method = "POST";
                request.ContentType = "application/json";

                if (data != null)
                {
                    if (string.IsNullOrEmpty(data.TerminalKey))
                        data.TerminalKey = _terminalKey;

                    data.Token = GenerateToken(AsDictionary(data));

                    string dataPost = JsonConvert.SerializeObject(data);

                    byte[] bytes = Encoding.UTF8.GetBytes(dataPost);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }

                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }

                var dataAnswer = JsonConvert.DeserializeObject<T>(responseContent);

                return dataAnswer;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Error(error, ex);
                                }
                            else
                                Debug.Log.Error(ex);
                    }
                    else
                        Debug.Log.Error(ex);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }
        #endregion

        #region Help

        public string GenerateToken(Dictionary<string, string> data)
        {
            if (data.ContainsKey("Token"))
                data.Remove("Token");

            if (data.ContainsKey("Password"))
                data.Remove("Password");
            data.Add("Password", _secretKey);

            var stringBuilder = new StringBuilder();
            foreach (var key in data.Keys.OrderBy(x => x))
            {
                var value = data[key];

                if (string.IsNullOrEmpty(value))
                    continue;

                stringBuilder.Append(value);
            }
            var token = stringBuilder.ToString().Sha256();

            data.Remove("Password");

            return token;
        }

        public Dictionary<string, string> AsDictionary(object source, BindingFlags bindingAttr = BindingFlags.Public | BindingFlags.Instance)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            foreach (var property in source.GetType().GetProperties(bindingAttr))
            {
                var val = property.GetValue(source, BindingFlags.GetProperty, null, null, CultureInfo.InvariantCulture);
                if (!property.PropertyType.IsGenericType && property.PropertyType != typeof(string) && val != null)
                {
                    // dictionary.Add(property.Name, JsonConvert.SerializeObject(val));
                }
                else
                    dictionary.Add(property.Name, val != null ? val.ToString() : null);
            }
            return dictionary;
        }

        public TinkoffNotifyData ReadNotifyData(string postPayload)
        {
            return JsonConvert.DeserializeObject<TinkoffNotifyData>(postPayload);
        }

        #endregion
    }
}
