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
    [PaymentAdminModel("Alfabank")]
    public class AlfabankPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string UserName
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.UserName); }
            set { Parameters.TryAddValue(AlfabankTemplate.UserName, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.Password); }
            set { Parameters.TryAddValue(AlfabankTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string MerchantLogin
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.MerchantLogin); }
            set { Parameters.TryAddValue(AlfabankTemplate.MerchantLogin, value.DefaultOrEmpty()); }
        }

        public bool UseTestMode
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.UseTestMode).TryParseBool(true) ?? true; }
            set { Parameters.TryAddValue(AlfabankTemplate.UseTestMode, value.ToString().DefaultOrEmpty()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(AlfabankTemplate.SendReceiptData, value.ToString()); }
        }

        public string Taxation
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.Taxation); }
            set { Parameters.TryAddValue(AlfabankTemplate.Taxation, value.DefaultOrEmpty()); }
        }

        public string GatewayCountry
        {
            get { return Parameters.ElementOrDefault(AlfabankTemplate.GatewayCountry); }
            set { Parameters.TryAddValue(AlfabankTemplate.GatewayCountry, value.DefaultOrEmpty()); }
        }
        
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/alfa-bank", "Инструкция. Подключение платежного модуля Альфа-Банк"); }
        }

        public List<SelectListItem> Taxations
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Общая", Value = "0"},
                    new SelectListItem() {Text = "Упрощенная (доходы)", Value = "1"},
                    new SelectListItem() {Text = "Упрощенная (доходы минус расходы)", Value = "2"},
                    new SelectListItem() {Text = "Единый налог на вмененный доход", Value = "3"},
                    new SelectListItem() {Text = "Единый сельскохозяйственный налог", Value = "4"},
                    new SelectListItem() {Text = "Патентная система налогообложения", Value = "5"},
                };

                var type = types.Find(x => x.Value == Taxation);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public List<SelectListItem> GatewayCountries
        {
            get
            {
                var types = new List<SelectListItem>()
                {
                    new SelectListItem() {Text = "Россия", Value = ""},
                    new SelectListItem() {Text = "Казахстан", Value = "kz"},
                };

                var type = types.Find(x => x.Value == Taxation);
                if (type != null)
                    type.Selected = true;

                return types;
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
