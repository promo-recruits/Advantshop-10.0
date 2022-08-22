using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Diagnostics;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace AdvantShop.Core.Services.Helpers
{
    public enum ERequestMethod
    {
        [StringName("POST")]
        POST,
        [StringName("GET")]
        GET,
        [StringName("PUT")]
        PUT,
    }

    public enum ERequestContentType
    {
        [StringName("application/json")]
        TextJson,

        [StringName("application/json; charset=utf-8")]
        TextJsonUtf8,

        [StringName("application/x-www-form-urlencoded")]
        FormUrlencoded
    }

    public class RequestHelper
    {
        public static T MakeRequest<T>(string urlAction, object data = null,
            Dictionary<string, string> headers = null,
            ERequestMethod method = ERequestMethod.POST,
            ERequestContentType contentType = ERequestContentType.TextJson,
            int timeoutSeconds = 60) //where T : class
        {
            try
            {
                if (data != null && method == ERequestMethod.GET)
                {
                    urlAction += (urlAction.Contains("?") ? "&" : "?") + data.ToString();
                }

                var request = WebRequest.Create(urlAction) as HttpWebRequest;
                request.Timeout = timeoutSeconds * 1000;
                request.Method = method.StrName();
                if (headers != null)
                {
                    if (headers.ContainsKey("Accept"))
                    {
                        request.Accept = headers["Accept"];
                        headers.Remove("Accept");
                    }

                    foreach (var key in headers.Keys)
                    {
                        request.Headers[key] = headers[key];
                    }
                }
                request.ContentType = contentType.StrName();

                if (data != null && method == ERequestMethod.POST)
                {
                    var stringData = contentType == ERequestContentType.FormUrlencoded && data.GetType() == typeof(string)
                        ? data.ToString()
                        : JsonConvert.SerializeObject(data);

                    byte[] bytes = Encoding.UTF8.GetBytes(stringData);
                    request.ContentLength = bytes.Length;

                    using (var requestStream = request.GetRequestStream())
                    {
                        requestStream.Write(bytes, 0, bytes.Length);
                        requestStream.Close();
                    }
                }


                var responseContent = "";
                using (var response = request.GetResponse())
                {
                    using (var stream = response.GetResponseStream())
                    {
                        if (stream != null)
                            using (var reader = new StreamReader(stream))
                            {
                                responseContent = reader.ReadToEnd();
                            }
                    }
                }

                var dataAnswer = typeof(T) != typeof(string) ? JsonConvert.DeserializeObject<T>(responseContent) : (T)Convert.ChangeType(responseContent, typeof(T));
                return dataAnswer;
            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                {
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                        {
                            if (eStream == null) throw ex;

                            using (var reader = new StreamReader(eStream))
                            {
                                var error = reader.ReadToEnd();
                                var wRespStatusCode = ((HttpWebResponse)ex.Response).StatusCode;
                                if (wRespStatusCode == HttpStatusCode.BadRequest)
                                {
                                    throw new BlException(error);
                                }
                                throw new Exception(error);
                            }
                        }
                    }
                    throw ex;
                }
            }
        }

        #region MultipartFormPost
        private static readonly Encoding encoding = Encoding.UTF8;

        public static T MultipartFormPost<T>(string urlAction, string userAgent, Dictionary<string, object> postParameters)
        {
            string formDataBoundary = String.Format("------------{0:N}", Guid.NewGuid());
            string contentType = "multipart/form-data; boundary=" + formDataBoundary;
            byte[] formData = GetMultipartFormData(postParameters, formDataBoundary);
            var webResponse = PostForm(urlAction, userAgent, contentType, formData);
            StreamReader responseReader = new StreamReader(webResponse.GetResponseStream());
            var responseContent = responseReader.ReadToEnd();
            webResponse.Close();
            return JsonConvert.DeserializeObject<T>(responseContent);
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new MemoryStream();
            bool needsCLRF = false;

            foreach (var param in postParameters)
            {
                if (needsCLRF)
                    formDataStream.Write(encoding.GetBytes("\r\n"), 0, encoding.GetByteCount("\r\n"));

                needsCLRF = true;

                string postData = string.Format("--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                    boundary,
                    param.Key,
                    param.Value);
                formDataStream.Write(encoding.GetBytes(postData), 0, encoding.GetByteCount(postData));
            }
            string footer = "\r\n--" + boundary + "--\r\n";
            formDataStream.Write(encoding.GetBytes(footer), 0, encoding.GetByteCount(footer));

            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        private static HttpWebResponse PostForm(string postUrl, string userAgent, string contentType, byte[] formData)
        {
            HttpWebRequest request = WebRequest.Create(postUrl) as HttpWebRequest;

            if (request == null)
            {
                throw new NullReferenceException("request is not a http request");
            }

            request.Method = "POST";
            request.ContentType = contentType;
            request.UserAgent = userAgent;
            request.CookieContainer = new CookieContainer();
            request.ContentLength = formData.Length;

            using (Stream requestStream = request.GetRequestStream())
            {
                requestStream.Write(formData, 0, formData.Length);
                requestStream.Close();
            }

            return request.GetResponse() as HttpWebResponse;
        }
        #endregion
    }
}
