using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.IPTelephony.Zadarma
{
    public class Zadarma : IPTelephonyOperator
    {
        private const string ServiceUrl = "https://api.zadarma.com";

        private string _key;
        private string _secret;

        public override EOperatorType Type
        {
            get { return EOperatorType.Zadarma; }
        }

        public Zadarma()
        {
            _key = SettingsTelephony.ZadarmaKey;
            _secret = SettingsTelephony.ZadarmaSecret;
        }

        public override CallBack.CallBack CallBack
        {
            get { return new ZadarmaCallBack(); }
        }

        public override string GetRecordLink(int callId)
        {
            var call = CallService.GetCall(callId);
            if (call == null || call.RecordLink.IsNullOrEmpty())
                return string.Empty;

            var @params = new Dictionary<string, string>();
            if (call.RecordLink.IsNotEmpty())
                @params.Add("call_id", call.RecordLink);
            else
                @params.Add("pbx_call_id", call.CallId);

            var result = MakeRequest<ZadarmaRecordResponse>("/v1/pbx/record/request/", @params);

            if (result == null)
                return string.Empty;
            if (result.Link.IsNullOrEmpty())
                return result.Links.FirstOrDefault();
            return result.Link;
        }

        public ZadarmaCallbackResponse CreateCallBack(string from, string to)
        {
            var @params = new Dictionary<string, string>()
            {
                { "from", from },
                { "to", to }
            };
            return MakeRequest<ZadarmaCallbackResponse>("/v1/request/callback/", @params);
        }

        private T MakeRequest<T>(string queryUrl, Dictionary<string, string> @params) where T : ZadarmaResponse
        {
            if (_key.IsNullOrEmpty() || _secret.IsNullOrEmpty())
                return null;

            var queryParams = @params.OrderBy(key => key.Key).Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).AggregateString("&");
            var queryParamsMd5 = BitConverter.ToString(MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(queryParams))).Replace("-", "").ToLower();
            var sign = StringHelper.EncodeTo64(
                SecurityHelper.EncodeWithHmacSha1(StringHelper.AggregateStrings("", queryUrl, queryParams, queryParamsMd5), _secret));

            var url = ServiceUrl + queryUrl;

            try
            {
                return RequestHelper.MakeRequest<T>(url,
                    data: queryParams,
                    headers: new Dictionary<string, string>
                    {
                        { HttpRequestHeader.Authorization.ToString(), _key + ":" + sign }
                    },
                    contentType: ERequestContentType.FormUrlencoded,
                    method: ERequestMethod.GET);
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex.Message + " URL: " + url, ex);
            }
            return null;
        }
    }
}
