﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("OnPay")]
    public class OnPayPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string FormPay
        {
            get { return Parameters.ElementOrDefault(OnPayTemplate.FormPay); }
            set { Parameters.TryAddValue(OnPayTemplate.FormPay, value.DefaultOrEmpty()); }
        }

        public string SendMethod
        {
            get { return Parameters.ElementOrDefault(OnPayTemplate.SendMethod, "GET") ?? "GET"; }
            set { Parameters.TryAddValue(OnPayTemplate.SendMethod, value.DefaultOrEmpty()); }
        }

        public bool CheckMd5
        {
            get { return Parameters.ElementOrDefault(OnPayTemplate.CheckMd5).TryParseBool(); }
            set { Parameters.TryAddValue(OnPayTemplate.CheckMd5, value.ToString()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(OnPayTemplate.SecretKey); }
            set { Parameters.TryAddValue(OnPayTemplate.SecretKey, value.DefaultOrEmpty()); }
        }
        
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-onpay", "Инструкция. Подключение платежного модуля OnPay.ru"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(FormPay) ||
                string.IsNullOrWhiteSpace(SendMethod) ||
                string.IsNullOrWhiteSpace(SecretKey))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
