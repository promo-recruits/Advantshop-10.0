using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.IPTelephony.Yandex
{
    public class YandexTelephony : IPTelephonyOperator
    {
        private const string ServiceUrl = "https://api.yandex.mightycall.ru/api/v2/";

        private readonly string _apiKey;
        private readonly string _mainUserKey;

        public override EOperatorType Type
        {
            get { return EOperatorType.Yandex; }
        }

        public YandexTelephony()
        {
            _apiKey = SettingsTelephony.YandexApiKey;
            _mainUserKey = SettingsTelephony.YandexMainUserKey;
        }

        public override CallBack.CallBack CallBack
        {
            get { return new YandexCallBack(); }
        }

        public override string GetRecordLink(int callId)
        {
            var call = CallService.GetCall(callId);
            if (call == null)
                return null;
            var callInfo = GetCallInfo(call.CallId);
            if (callInfo != null && callInfo.Call != null && callInfo.Call.CallRecord != null)
                return callInfo.Call.CallRecord.Uri;

            return null;
        }

        private YandexCallInfoResponse GetCallInfo(string callId)
        {
            if (callId.IsNullOrEmpty())
                return null;
            return MakeAuthRequest<YandexCallInfoResponse>(_mainUserKey, "calls/" + callId, method: "GET");
        }

        public YandexResponse MakeCall(string callbackUserKey, string businessNumber, string externalNumber)
        {
            if (businessNumber.IsNullOrEmpty() || externalNumber.IsNullOrEmpty() || callbackUserKey.IsNullOrEmpty())
                return null;
            var @params = new Dictionary<string, string>()
            {
                { "from", businessNumber },
                { "to", externalNumber }
            };
            return MakeAuthRequest<YandexResponse>(callbackUserKey, "calls/makecall", @params);
        }

        #region Token

        private string GetToken(string userKey)
        {
            if (_apiKey.IsNullOrEmpty() || userKey.IsNullOrEmpty())
                return string.Empty;

            var result = GetTokenFromDB(userKey);
            if (result == null || result.AccessToken.IsNullOrEmpty() || result.Expires < DateTime.Now)
            {
                result = GetTokenRemote(userKey);
            }

            return result != null ? result.AccessToken : string.Empty;
        }

        private YandexTokenResponse GetTokenRemote(string userKey)
        {
            var @params = new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" },
                { "client_id", _apiKey },
                { "client_secret", userKey }
            };
            var result = MakeRequest<YandexTokenResponse>("auth/token", @params);
            SaveTokenToDB(userKey, result);

            return result;
        }

        private YandexTokenResponse GetTokenFromDB(string userKey)
        {
            try
            {
                var accessTokens = JsonConvert.DeserializeObject<Dictionary<string, YandexTokenResponse>>(SettingsTelephony.YandexAccessTokens ?? string.Empty);
                if (accessTokens.ContainsKey(userKey))
                    return accessTokens[userKey];
            }
            catch (Exception e)
            {
                Debug.Log.Error(e);
            }
            return null;
        }

        private bool SaveTokenToDB(string userKey, YandexTokenResponse token)
        {
            if (token == null)
                return false;
            token.Expires = DateTime.Now.AddSeconds(token.ExpiresIn);
            var accessTokens = new Dictionary<string, YandexTokenResponse>();
            try
            {
                accessTokens = JsonConvert.DeserializeObject<Dictionary<string, YandexTokenResponse>>(SettingsTelephony.YandexAccessTokens ?? string.Empty)
                    ?? new Dictionary<string, YandexTokenResponse>();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            // remove expired access tokens
            var result = accessTokens.Where(pair => pair.Value.Expires > DateTime.Now).ToDictionary(pair => pair.Key, pair => pair.Value);
            result.TryAddValue(userKey, token);
            SettingsTelephony.YandexAccessTokens = JsonConvert.SerializeObject(result);

            return true;
        }

        #endregion

        private T MakeAuthRequest<T>(string userKey, string queryUrl, Dictionary<string, string> @params = null, string method = "POST", string contentType = "application/x-www-form-urlencoded", bool retry = true) where T : YandexResponse
        {
            var token = GetToken(userKey);
            if (token.IsNullOrEmpty())
            {
                Debug.Log.Warn("Can't get Yandex auth token");
                return null;
            }

            string queryParams = null;
            if (@params != null && @params.Any())
                queryParams = @params.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).AggregateString("&");
            var url = ServiceUrl + queryUrl;
            //AdvantShop.Statistic.CommonStatistic.WriteLog(
            //    string.Format("REQ [{0}]: \r\n url: {1} \r\n data: {2}", DateTime.Now.ToString("dd.MM HH:mm:ss"), url, queryParams));
            if (method == "GET" && queryParams.IsNotEmpty())
                url += "?" + queryParams;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = contentType;
                request.Headers[HttpRequestHeader.Authorization] = "bearer " + token;
                request.Headers["x-api-key"] = _apiKey;

                if (method != "GET" && queryParams.IsNotEmpty())
                {
                    byte[] byteArray = Encoding.GetEncoding("windows-1251").GetBytes(queryParams);
                    request.ContentLength = byteArray.Length;
                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                    }
                }
                string responseFromServer = "";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                            using (var reader = new StreamReader(dataStream))
                            {
                                responseFromServer = reader.ReadToEnd();
                            }
                    }
                }
                //AdvantShop.Statistic.CommonStatistic.WriteLog(
                //    string.Format("RESP {2} [{0}]: \r\n data: {1} \r\n", DateTime.Now.ToString("dd.MM HH:mm:ss"), responseFromServer, url));
                var result = JsonConvert.DeserializeObject<T>(responseFromServer);
                if (result != null && result.Message.IsNotEmpty() && result.Message.ToLower().Contains("authorization has been denied") && retry)
                {
                    GetTokenRemote(userKey);
                    return MakeAuthRequest<T>(userKey, queryUrl, @params, method, contentType, false);
                }
                return result;
            }
            //catch (WebException ex)
            //{
            //    if (ex.Status == WebExceptionStatus.ProtocolError)
            //    {
            //        // todo: 
            //        var response = ex.Response as HttpWebResponse;
            //        if (response != null && response.StatusCode == HttpStatusCode.Unauthorized && retry)
            //        {
            //            GetTokenRemote(userKey);
            //            return MakeAuthRequest<T>(userKey, queryUrl, @params, method, contentType, false);
            //        }
            //    }
            //    //AdvantShop.Statistic.CommonStatistic.WriteLog(
            //    //    string.Format("ERROR WebException {2} [{0}]: \r\n data: {1} \r\n", DateTime.Now.ToString("dd.MM HH:mm:ss"), ex.Message, url));
            //    Debug.Log.Error(ex + " URL: " + url, ex);
            //}
            catch (Exception ex)
            {
                //AdvantShop.Statistic.CommonStatistic.WriteLog(
                //    string.Format("ERROR Exception {2} [{0}]: \r\n data: {1} \r\n", DateTime.Now.ToString("dd.MM HH:mm:ss"), ex.Message, url));
                Debug.Log.Error(ex + " URL: " + url, ex);
            }
            return null;
        }

        private T MakeRequest<T>(string queryUrl, Dictionary<string, string> @params, string method = "POST", string contentType = "application/x-www-form-urlencoded") where T : YandexResponse
        {
            var queryParams = @params.Select(pair => string.Format("{0}={1}", pair.Key, pair.Value)).AggregateString("&");
            var url = ServiceUrl + queryUrl;
            //AdvantShop.Statistic.CommonStatistic.WriteLog(
            //    string.Format("REQ [{0}]: \r\n url: {1} \r\n data: {2}", DateTime.Now.ToString("dd.MM HH:mm:ss"), url, queryParams));
            if (method == "GET" && queryParams.IsNotEmpty())
                url += "?" + queryParams;

            try
            {
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = method;
                request.ContentType = contentType;
                request.Headers["x-api-key"] = _apiKey;

                if (method != "GET" && queryParams.IsNotEmpty())
                {
                    byte[] byteArray = Encoding.GetEncoding("windows-1251").GetBytes(queryParams);
                    request.ContentLength = byteArray.Length;
                    using (var dataStream = request.GetRequestStream())
                    {
                        dataStream.Write(byteArray, 0, byteArray.Length);
                        dataStream.Close();
                    }
                }
                string responseFromServer = "";
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using (var dataStream = response.GetResponseStream())
                    {
                        if (dataStream != null)
                            using (var reader = new StreamReader(dataStream))
                            {
                                responseFromServer = reader.ReadToEnd();
                            }
                    }
                }
                //AdvantShop.Statistic.CommonStatistic.WriteLog(
                //    string.Format("RESP [{0}]: \r\n data: {1} \r\n", DateTime.Now.ToString("dd.MM HH:mm:ss"), responseFromServer));
                return JsonConvert.DeserializeObject<T>(responseFromServer);
            }
            catch (Exception ex)
            {
                //AdvantShop.Statistic.CommonStatistic.WriteLog(
                //    string.Format("ERROR [{0}]: \r\n data: {1} \r\n", DateTime.Now.ToString("dd.MM HH:mm:ss"), ex.Message));
                Debug.Log.Error(ex.Message + " URL: " + url, ex);
            }
            return null;
        }
    }
}
