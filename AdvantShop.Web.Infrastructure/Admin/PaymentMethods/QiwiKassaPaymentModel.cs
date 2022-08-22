using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Payment;
using AdvantShop.Payment;

namespace AdvantShop.Web.Infrastructure.Admin.PaymentMethods
{
    [PaymentAdminModel("QiwiKassa")]
    public class QiwiKassaPaymentModel : PaymentMethodAdminModel, IValidatableObject
    {
        public string SecrectKey
        {
            get { return Parameters.ElementOrDefault(QiwiKassaTemplate.SecrectKey); }
            set { Parameters.TryAddValue(QiwiKassaTemplate.SecrectKey, value.DefaultOrEmpty()); }
        }
        
        public override Tuple<string, string> Instruction
        {
            get { return new Tuple<string, string>("http://www.advantshop.net/help/pages/connect-qiwi-kassa", "Инструкция. Подключение платежного модуля Qiwi касса"); }
        }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(SecrectKey))
            {
                yield return new ValidationResult("Заполните обязательные поля");
            }
        }
    }
}
