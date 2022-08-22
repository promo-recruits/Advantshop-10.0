using AdvantShop.Core.Common.Extensions;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.IPTelephony.CallBack;

namespace AdvantShop.Core.Services.IPTelephony.Yandex
{
    public class YandexCallBack : CallBack.CallBack
    {
        private readonly string _callbackUserKey;
        private readonly string _businessNumber;

        public YandexCallBack()
        {
            _callbackUserKey = SettingsTelephony.YandexCallbackUserKey;
            _businessNumber = SettingsTelephony.YandexCallbackBusinessNumber;
        }

        public override bool Enabled
        {
            get { return base.Enabled && SettingsTelephony.CurrentIPTelephonyOperatorType == EOperatorType.Yandex; }
        }

        public override CallBackAnswer CreateCallBack(string phone)
        {
            if (_callbackUserKey.IsNullOrEmpty() || _businessNumber.IsNullOrEmpty())
                return new CallBackAnswer(false, "Callback not configured");
            if (phone.IsNotEmpty() && phone.Length == 10)
                phone = "+7" + phone;
            var telephony = new YandexTelephony();
            var result = telephony.MakeCall(_callbackUserKey, _businessNumber, phone);
            if (result != null)
                return new CallBackAnswer(result.Success, !result.Success && result.Message.IsNotEmpty() ? result.Message : string.Empty);
            return new CallBackAnswer(false, "service error");
        }
    }
}
