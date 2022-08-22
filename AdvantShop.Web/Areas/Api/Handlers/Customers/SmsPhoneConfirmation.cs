using System;
using System.Web;
using AdvantShop.Areas.Api.Models.Customers;
using AdvantShop.Core;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Helpers;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Areas.Api.Handlers.Customers
{
    public class SmsPhoneConfirmation : AbstractCommandHandler<SmsPhoneConfirmationResponse>
    {
        private readonly string _phone;
        private long? _standardPhone;

        public SmsPhoneConfirmation(string phone)
        {
            _phone = phone;
        }

        protected override void Validate()
        {
            _standardPhone = StringHelper.ConvertToStandardPhone(HttpUtility.HtmlEncode(_phone), true, true);

            if (_standardPhone == null || _standardPhone == 0)
                throw new BlException("Не валидный телефон");
            
        }

        protected override SmsPhoneConfirmationResponse Handle()
        {
            var code = new Random().Next(1001, 9999).ToString();

            SmsNotifier.SendSmsNowWithResult(_standardPhone.Value, code, throwException: true);

            return new SmsPhoneConfirmationResponse() { Code = code };
        }
    }
}