using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using AdvantShop.Core.Services.Payment.SberBankAcquiring;
using AdvantShop.Diagnostics;
using AdvantShop.Orders;
using Newtonsoft.Json;
using HttpUtility = System.Web.HttpUtility;
using System.Linq;
using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Helpers;
using AdvantShop.Taxes;
using Tax = AdvantShop.Core.Services.Payment.SberBankAcquiring.Tax;
using AdvantShop.Core.Services.Taxes;
using AdvantShop.Core.Services.Orders;
using AdvantShop.Repository.Currencies;

namespace AdvantShop.Payment
{
    // Документация: https://developer.sberbank.ru/acquiring-api-rest-requests1pay

    public class SberBankAcquiringService
    {
        private const string BaseUrl = "https://securepayments.sberbank.ru/payment/rest/";
        private const string BaseTestUrl = "https://3dsec.sberbank.ru/payment/rest/";

        private readonly string _userName;
        private readonly string _password;
        private readonly string _merchantLogin;
        private readonly bool _testMode;
        private readonly bool _sendReceiptData;

        public SberBankAcquiringService(string userName, string password, string merchantLogin, bool testMode, bool sendReceiptData)
        {
            _userName = userName;
            _password = password;
            _merchantLogin = merchantLogin;
            _testMode = testMode;
            _sendReceiptData = sendReceiptData;
        }

        /// <summary>
        /// Запрос  регистрации заказа
        /// </summary>
        public SberbankAcquiringRegisterResponse Register(Order order, string description, Currency paymentCurrency, string returnUrl, string failUrl, TaxElement tax)
        {
            int index = 1;
            paymentCurrency = paymentCurrency ?? order.OrderCurrency;

            Receipt receipt = _sendReceiptData
                ? new Receipt()
                {
                    customerDetails = ValidationHelper.IsValidEmail(order.OrderCustomer.Email)
                        ? new CustomerDetails { email = order.OrderCustomer.Email }
                        : null,

                    cartItems = new CartItems
                    {
                        items =
                            order
                                .GetOrderItemsForFiscal(paymentCurrency)
                                .Select(item => new Item()
                                {
                                    positionId = (index++).ToString(),
                                    name = item.Name.Reduce(100),
                                    itemCode = item.ArtNo.Reduce(100),
                                    itemAmount = (int)Math.Round(item.Price * item.Amount * 100),
                                    itemPrice = (int)Math.Round(item.Price * 100, 0),
                                    quantity = new Quantity()
                                    {
                                        value = item.Amount,
                                        measure = item.Unit.IsNotEmpty()
                                            ? item.Unit
                                            : "штук"
                                    },
                                    tax = new Tax() { taxType = GetTaxType(tax?.TaxType ?? item.TaxType, item.PaymentMethodType) },
                                    itemAttributes = new ItemAttributes()
                                    {
                                        attributes = new List<Core.Services.Payment.SberBankAcquiring.Attribute>()
                                        {
                                            new Core.Services.Payment.SberBankAcquiring.Attribute() { name = "paymentMethod", value = ((int)item.PaymentMethodType).ToString()},
                                            new Core.Services.Payment.SberBankAcquiring.Attribute() { name = "paymentObject", value = ((int)item.PaymentSubjectType).ToString()},
                                        }
                                    }
                                }).ToList()
                    }
                }
                : null;

            if (receipt != null && order.OrderCertificates != null && order.OrderCertificates.Count > 0)
            {
                var certTax = TaxService.GetCertificateTax();
                receipt.cartItems.items.AddRange(order.OrderCertificates
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency)
                    .Select(x =>
                    new Item
                    {
                        name = "Подарочный сертификат",
                        itemCode = x.CertificateCode,
                        positionId = (index++).ToString(),
                        itemAmount = (int)(Math.Round(x.Sum * 100)),
                        itemPrice = (int)(Math.Round(x.Sum * 100)),
                        quantity = new Quantity { value = 1, measure = "штук" },
                        tax = new Tax() { taxType = GetTaxType(tax?.TaxType ?? certTax?.TaxType, AdvantShop.Configuration.SettingsCertificates.PaymentMethodType) },

                        itemAttributes = new ItemAttributes()
                        {
                            attributes = new List<Core.Services.Payment.SberBankAcquiring.Attribute>()
                            {
                                new Core.Services.Payment.SberBankAcquiring.Attribute() { name = "paymentMethod", value = ((int)AdvantShop.Configuration.SettingsCertificates.PaymentMethodType).ToString() },
                                new Core.Services.Payment.SberBankAcquiring.Attribute() { name = "paymentObject", value = ((int)AdvantShop.Configuration.SettingsCertificates.PaymentSubjectType).ToString() },
                            }
                        }
                    }));
            }

            var orderShippingCostWithDiscount = 
                order.ShippingCostWithDiscount
                    .ConvertCurrency(order.OrderCurrency, paymentCurrency);
            if (orderShippingCostWithDiscount > 0 && receipt != null)
            {
                receipt.cartItems.items.Add(new Item()
                {
                    name = "Доставка",
                    itemCode = "Доставка",
                    positionId = (index++).ToString(),
                    itemAmount = (int)(Math.Round(orderShippingCostWithDiscount * 100)),
                    itemPrice = (int)(Math.Round(orderShippingCostWithDiscount * 100)),
                    quantity = new Quantity { value = 1, measure = "штук" },
                    tax = new Tax() { taxType = GetTaxType(tax?.TaxType ?? order.ShippingTaxType, order.ShippingPaymentMethodType) },

                    itemAttributes = new ItemAttributes()
                    {
                        attributes = new List<Core.Services.Payment.SberBankAcquiring.Attribute>()
                                        {
                                            new Core.Services.Payment.SberBankAcquiring.Attribute() { name = "paymentMethod", value = ((int)order.ShippingPaymentMethodType).ToString() },
                                            new Core.Services.Payment.SberBankAcquiring.Attribute() { name = "paymentObject", value = ((int)order.ShippingPaymentSubjectType).ToString() },
                                        }
                    }
                });
            }

            long sum = 0;

            if (receipt != null && receipt.cartItems != null && receipt.cartItems.items != null)
            {
                foreach (var item in receipt.cartItems.items)
                    sum += item.itemAmount;
            }
            else
            {
                sum = (long)Math.Round(Math.Round(order.Sum.ConvertCurrency(order.OrderCurrency, paymentCurrency), 2) * 100);
            }

            int retriesNum = 0;
            string orderStrId;
            bool success = false;
            SberbankAcquiringRegisterResponse response;

            do
            {
                // если заказ уже есть в сбербанке, но был изменен на стороне магазина, подменяем id на id_номерпопытки
                orderStrId = string.Format("{0}_{1}", order.OrderID, DateTime.Now.ToUnixTime());

                var data = new Dictionary<string, object>()
                {
                    {"userName", _userName},
                    {"password", _password},
                    {"orderNumber", orderStrId},
                    {"amount", sum},    // Сумма платежа в копейках (или центах)
                    {"currency", paymentCurrency.NumIso3}, // ISO 4217
                    {"returnUrl", returnUrl},
                    {"failUrl", failUrl},
                    //{"pageView", "DESKTOP"}, // "MOBILE"
                    {"clientId", order.OrderCustomer.CustomerID},
                    {"merchantLogin", _merchantLogin},  // Чтобы зарегистрировать заказ от имени дочернего мерчанта, укажите его логин в этом параметре.
                    //{"bindingId", "" }                // Идентификатор связки, созданной ранее. Может использоваться, только если у магазина есть разрешение на работу со связками. Если этот параметр передаётся в данном запросе, то это означает: 1. Данный заказ может быть оплачен только с помощью связки; 2. Плательщик будет перенаправлен на платёжную страницу, где требуется только ввод CVC.
                };

                if (receipt != null)
                {
                    data.Add("orderBundle", JsonConvert.SerializeObject(receipt));
                }

                response = MakeRequest<SberbankAcquiringRegisterResponse>("register.do", data);

                if (response == null)
                    response = new SberbankAcquiringRegisterResponse { ErrorCode = "1" };
                //return null;

                string error;
                success = !HasRegisterError(response, out error);

                if (!success)
                {
                    Debug.Log.Info(string.Format("SberBankAcquiringService Register. code: {0} error: {1}, obj: {2}",
                                                    error, response.ErrorMessage, JsonConvert.SerializeObject(response)));

                }
                retriesNum++;
            } while (response.ErrorCode == "1" && retriesNum < 3);

            return success ? response : null;
        }

        /// <summary>
        /// Запрос состояния заказа
        /// </summary>
        public SberbankAcquiringOrderStatusResponse GetOrderStatus(string orderId)
        {
            var data = new Dictionary<string, object>()
            {
                {"userName", _userName},
                {"password", _password},
                {"orderNumber", orderId},
            };

            var response = MakeRequest<SberbankAcquiringOrderStatusResponse>("getOrderStatusExtended.do", data);

            if (response == null)
                return null;

            Debug.Log.Info(string.Format("SberBankAcquiringService GetOrderStatus. code: {0} error: {1}, errorcode: {2}, obj: {3}",
                                            GetOrderStatusName(response.OrderStatus ?? ""), response.ErrorMessage, response.ErrorCode, JsonConvert.SerializeObject(response)));

            return response;
        }



        #region Private methods

        /*
         0 – без НДС;
         1 – НДС по ставке 0%;
         2 – НДС чека по ставке 10%;
         3 – НДС чека по ставке 18%;
         4 – НДС чека по расчётной ставке 10/110;
         5 – НДС чека по расчётной ставке 18/118.
         6 - НДС чека по ставке 20%;
         7 - НДС чека по расчётной ставке 20/120.
         */
        private int GetTaxType(TaxType? taxType, ePaymentMethodType paymentMethodType)
        {
            if (taxType == null || taxType.Value == TaxType.VatWithout)
                return 0;

            if (taxType.Value == TaxType.Vat0)
                return 1;

            if (taxType.Value == TaxType.Vat10)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                     paymentMethodType == ePaymentMethodType.partial_prepayment ||
                     paymentMethodType == ePaymentMethodType.advance))
                    return 4;
                else
                    return 2;
            }

            if (taxType.Value == TaxType.Vat18)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                    paymentMethodType == ePaymentMethodType.partial_prepayment ||
                    paymentMethodType == ePaymentMethodType.advance))
                    return 5;
                else
                    return 3;
            }

            if (taxType.Value == TaxType.Vat20)
            {
                if (Configuration.SettingsCheckout.TaxTypeByPaymentMethodType &&
                    (paymentMethodType == ePaymentMethodType.full_prepayment ||
                    paymentMethodType == ePaymentMethodType.partial_prepayment ||
                    paymentMethodType == ePaymentMethodType.advance))
                    return 7;
                else
                    return 6;
            }

            return 0;
        }

        private bool HasRegisterError(SberbankAcquiringRegisterResponse response, out string error)
        {
            error = null;
            var code = response.ErrorCode ?? "";
            switch (code)
            {
                case "0":
                    error = "Обработка запроса прошла без системных ошибок";
                    break;

                case "1":
                    error = "Заказ с таким номером уже зарегистрирован в системе";
                    break;

                case "2":
                    error = "Обработка запроса прошла без системных ошибок";
                    break;

                case "3":
                    error = "Неизвестная (запрещенная) валюта";
                    break;

                case "4":
                    error = "Отсутствует обязательный параметр запроса";
                    break;

                case "5":
                    error = "Ошибка значение параметра запроса";
                    break;

                case "7":
                    error = "Системная ошибка";
                    break;
            }

            return code != "0" && code.IsNotEmpty();
        }

        private string GetOrderStatusName(string code)
        {
            switch (code)
            {
                case "0":
                    return "Заказ зарегистрирован, но не оплачен";

                case "1":
                    return "Предавторизованная сумма захолдирована (для двухстадийных платежей)";

                case "2":
                    return "Проведена полная авторизация суммы заказа";

                case "3":
                    return "Авторизация отменена";

                case "4":
                    return "По транзакции была проведена операция возврата";

                case "5":
                    return "Инициирована авторизация через ACS банка-эмитента";

                case "6":
                    return "Авторизация отклонена";
            }

            return string.Empty;
        }

        private T MakeRequest<T>(string url, Dictionary<string, object> data = null) where T : class
        {
            try
            {
                var request = WebRequest.Create((_testMode ? BaseTestUrl : BaseUrl) + url) as HttpWebRequest;
                request.Timeout = 50000;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";

                if (data != null)
                {
                    string dataPost = "";
                    foreach (var key in data.Keys)
                    {
                        var value = data[key];

                        if (value == null || value.ToString() == "")
                            continue;

                        if (dataPost != "")
                            dataPost += "&";

                        dataPost += key + "=" + HttpUtility.UrlEncode(value.ToString());
                    }

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
                                    Debug.Log.Error(error);
                                }
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
    }
}