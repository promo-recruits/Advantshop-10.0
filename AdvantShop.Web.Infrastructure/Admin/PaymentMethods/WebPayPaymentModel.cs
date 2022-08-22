﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("WebPay")]
    public class WebPayPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string StoreId
        {
            get { return Parameters.ElementOrDefault(WebPayTemplate.StoreId); }
            set { Parameters.TryAddValue(WebPayTemplate.StoreId, value.DefaultOrEmpty()); }
        }

        public string SecretKey
        {
            get { return Parameters.ElementOrDefault(WebPayTemplate.SecretKey); }
            set { Parameters.TryAddValue(WebPayTemplate.SecretKey, value.DefaultOrEmpty()); }
        }

        public bool TestMode
        {
            get { return Parameters.ElementOrDefault(WebPayTemplate.TestMode).TryParseBool(); }
            set { Parameters.TryAddValue(WebPayTemplate.TestMode, value.ToString()); }
        }
        
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("https://www.advantshop.net/help/pages/webpay", "Инструкция. Подключение платежного модуля Webpay"); }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(StoreId) ||
                string.IsNullOrWhiteSpace(SecretKey))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
