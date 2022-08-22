using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Admin.Models.Settings
{
    public class SendTestMessageModel : IValidatableObject
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }


        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(To))
                yield return new ValidationResult("Пустой e-mail получателя");

            foreach (var mail in To.Split(new[] { ';', ',' }))
            {
                if (!ValidationHelper.IsValidEmail(mail))
                {
                    yield return new ValidationResult("Невалидный один из e-mail получателей");
                    break;
                }
            }
            

        }
    }
}


