using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Core.Services.Payment.Robokassa;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("Robokassa")]
    public class RobokassaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string MerchantLogin
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.MerchantLogin); }
            set { Parameters.TryAddValue(RobokassaTemplate.MerchantLogin, value.DefaultOrEmpty()); }
        }

        public string Password
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.Password); }
            set { Parameters.TryAddValue(RobokassaTemplate.Password, value.DefaultOrEmpty()); }
        }

        public string CurrencyLabel
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.CurrencyLabel); }
            set { Parameters.TryAddValue(RobokassaTemplate.CurrencyLabel, value.DefaultOrEmpty()); }
        }

        public List<SelectListItem> CurrencyLabels
        {
            get
            {
                var listCurrencyLabels = new List<SelectListItem>()
                    {new SelectListItem() {Text = "Способ по умолчанию от Robokassa", Value = ""}};

                var merchantLogin = MerchantLogin;
                if (!string.IsNullOrEmpty(merchantLogin))
                {
                    var currencies = RobokassaHelper.GetCurrencies(merchantLogin);
                    if (currencies != null && currencies.Result != null && currencies.Result.Code == 0)
                    {
                        foreach (var group in currencies.GroupsContainer.Groups)
                        {
                            var selectListGroup = new SelectListGroup { Name = group.Description };
                            listCurrencyLabels.AddRange(group.CurrencyContainer.Currencies.Select(x => new SelectListItem() { Text = x.Name, Value = x.Alias, Group = selectListGroup }));
                        }
                    }
                }

                //var selectingListItem = listCurrencyLabels.Find(x => x.Value == CurrencyLabel);
                //if (selectingListItem != null)
                //    selectingListItem.Selected = true;

                return listCurrencyLabels;
            }
        }

        public string PasswordNotify
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.PasswordNotify); }
            set { Parameters.TryAddValue(RobokassaTemplate.PasswordNotify, value.DefaultOrEmpty()); }
        }

        public bool SendReceiptData
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.SendReceiptData).TryParseBool(); }
            set { Parameters.TryAddValue(RobokassaTemplate.SendReceiptData, value.ToString()); }
        }

        public bool IsTest
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.IsTest).TryParseBool(); }
            set { Parameters.TryAddValue(RobokassaTemplate.IsTest, value.ToString()); }
        }

        public float Fee
        {
            get { return Parameters.ElementOrDefault(RobokassaTemplate.Fee).TryParseFloat(); }
            set { Parameters.TryAddValue(RobokassaTemplate.Fee, value.ToInvariantString()); }
        }
                

        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-robokassa", "Инструкция. Подключение к системе Robokassa"); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(MerchantLogin) ||
                string.IsNullOrWhiteSpace(PasswordNotify) ||
                string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
