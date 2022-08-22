using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("YandexKassa")]
    public class YandexKassaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string ProtocolApi { get { return YandexKassa.ProtocolApi; } }
        public string Protocol
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.Protocol); }
            set { Parameters.TryAddValue(YandexKassaTemplate.Protocol, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> Protocols
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Платежный модуль", Value = ""},
                    new SelectListItem() {Text = "Виджет", Value = YandexKassa.ProtocolWidget},
                    new SelectListItem() {Text = "API", Value = YandexKassa.ProtocolApi},
                };

                var type = types.Find(x => x.Value == Protocol);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public string ShopId
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.ShopID); }
            set { Parameters.TryAddValue(YandexKassaTemplate.ShopID, value.DefaultOrEmpty()); }
        }

        public string ScId
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.ScID); }
            set { Parameters.TryAddValue(YandexKassaTemplate.ScID, value.DefaultOrEmpty()); }
        }

        public string YaPaymentType
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.YaPaymentType); }
            set { Parameters.TryAddValue(YandexKassaTemplate.YaPaymentType, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> YaPaymentTypes
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Умный платеж (все доступные методы)", Value = ""},
                    new SelectListItem() {Text = "Со счета в ЮMoney", Value = "PC"},
                    new SelectListItem() {Text = "С банковской карты", Value = "AC"},
                    new SelectListItem() {Text = "Со счета мобильного телефона", Value = "MC"},
                    new SelectListItem() {Text = "По коду через терминал", Value = "GP"},
                    new SelectListItem() {Text = "Оплата через Сбербанк: оплата по SMS или Сбербанк Онлайн", Value = "SB"},
                    new SelectListItem() {Text = "Оплата через мобильный терминал (mPOS)", Value = "WM"},
                    new SelectListItem() {Text = "Оплата через Альфа-Клик", Value = "AB"},
                    new SelectListItem() {Text = "Оплата через MasterPass", Value = "МА"},
                    new SelectListItem() {Text = "Оплата через Промсвязьбанк", Value = "PB"},
                    new SelectListItem() {Text = "Оплата через QIWI Wallet", Value = "QW"},
                    //new SelectListItem() {Text = "Оплата через КупиВкредит (Тинькофф Банк)", Value = "KV"},
                    //new SelectListItem() {Text = "Оплата через Доверительный платеж на Куппи.ру", Value = "QP"},
                    new SelectListItem() {Text = "Заплатить по частям (кредит)", Value = "CR"}
                };

                var type = types.Find(x => x.Value == YaPaymentType);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public List<SelectListItem> YaPaymentTypesNew
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Умный платеж (все доступные методы)", Value = ""},
                    new SelectListItem() {Text = "Банковские карты", Value = "bank_card"},
                    new SelectListItem() {Text = "ЮMoney", Value = "yoo_money"},
                    new SelectListItem() {Text = "Сбербанк Онлайн", Value = "sberbank"},
                    new SelectListItem() {Text = "QIWI Wallet", Value = "qiwi"},
                    new SelectListItem() {Text = "Webmoney", Value = "webmoney"},
                    new SelectListItem() {Text = "Наличные через терминалы", Value = "cash"},
                    //new SelectListItem() {Text = "Баланс мобильного", Value = "mobile_balance"},
                    new SelectListItem() {Text = "Альфа-Клик", Value = "alfabank"},
                    new SelectListItem() {Text = "Тинькофф", Value = "tinkoff_bank"},
                    new SelectListItem() {Text = "Заплатить по частям (кредит)", Value = "installments"},
                };

                var type = types.Find(x => x.Value == YaPaymentType);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.Password); }
            set { Parameters.TryAddValue(YandexKassaTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.Password); }
            set { Parameters.TryAddValue(YandexKassaTemplate.Password, value.DefaultOrEmpty()); }
        }

        //public bool DemoMode
        //{
        //    get { return Parameters.ElementOrDefault(YandexKassaTemplate.DemoMode).TryParseBool(); }
        //    set { Parameters.TryAddValue(YandexKassaTemplate.DemoMode, value.ToString()); }
        //}

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(YandexKassaTemplate.SendReceiptData, value.ToString()); }
        }

        public float MinimumPrice
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.MinimumPrice).TryParseFloat(); }
            set { Parameters.TryAddValue(YandexKassaTemplate.MinimumPrice, value.ToInvariantString()); }
        }

        public string MaximumPrice
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.MaximumPrice); }
            set { Parameters.TryAddValue(YandexKassaTemplate.MaximumPrice, value.TryParseFloat(true)?.ToInvariantString() ?? string.Empty); }
        }

        public float FirstPayment
        {
            get { return Parameters.ElementOrDefault(YandexKassaTemplate.FirstPayment).TryParseFloat(); }
            set { Parameters.TryAddValue(YandexKassaTemplate.FirstPayment, value.ToInvariantString()); }
        }
        

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-yandex-kassa", "Инструкция. Подключение платежного модуля \"Касса от ЮMoney\""); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Protocol == YandexKassa.ProtocolApi || Protocol == YandexKassa.ProtocolWidget)
            {
                if (string.IsNullOrWhiteSpace(ShopId) ||
                    string.IsNullOrWhiteSpace(SecretKey))
                {
                    yield return new ValidationResult("Заполните обязательные поля");
                }
            }
            else if (string.IsNullOrWhiteSpace(ShopId) ||
                string.IsNullOrWhiteSpace(ScId) ||
                string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
