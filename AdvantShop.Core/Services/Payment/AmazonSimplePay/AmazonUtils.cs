//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace App_Code.AdvantShop.Payment.AmazonSimplePay
{
    public class AmazonUtils
    {
        public static string SignParameters(NameValueCollection parameters, String key, String httpMethod, String host, String requestUri) //throws Exception
        {
            string stringToSign = CalculateSignV2(parameters, httpMethod, host, requestUri);
            return Sign(stringToSign, key, "HmacSHA256");
        }

        private static string CalculateSignV2(NameValueCollection parameters, String httpMethod, String hostHeader, String requestUri)// throws SignatureException
        {
            var stringToSign = new StringBuilder();
            if (httpMethod == null) throw new Exception("HttpMethod cannot be null");
            stringToSign.Append(httpMethod);
            stringToSign.Append("\n");

            stringToSign.Append(hostHeader == null ? string.Empty : hostHeader.ToLower());
            stringToSign.Append("\n");

            stringToSign.Append(String.IsNullOrEmpty(requestUri) ? "/" : UrlEncode(requestUri, true));
            stringToSign.Append("\n");


            foreach (var key in parameters.AllKeys.OrderBy(x => x, StringComparer.Ordinal).Where(key => String.Compare(key, "signature", true) != 0))
                foreach (var value in parameters.GetValues(key) ?? new []{string.Empty})
                {
                    stringToSign.Append(UrlEncode(key, false));
                    stringToSign.Append("=");
                    stringToSign.Append(UrlEncode(value, false));
                    stringToSign.Append("&");
                }

            return stringToSign.Remove(stringToSign.Length - 1, 1).ToString();
        }

        private static String Sign(String data, String key, String signatureMethod)
        {
            try
            {
                var encoding = new ASCIIEncoding();
                HMAC hmac = HMAC.Create(signatureMethod);
                hmac.Key = encoding.GetBytes(key);
                hmac.Initialize();
                var cs = new CryptoStream(Stream.Null, hmac, CryptoStreamMode.Write);
                var bytes = encoding.GetBytes(data);
                cs.Write(bytes, 0, bytes.Length);
                cs.Close();
                byte[] rawResult = hmac.Hash;
                String sig = Convert.ToBase64String(rawResult, 0, rawResult.Length);
                return sig;
            }
            catch (Exception e)
            {
                throw new Exception("Failed to generate signature: " + e.Message);
            }
        }
        private static String UrlEncode(String data, bool path)
        {
            var encoded = new StringBuilder();
            String unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~" + (path ? "/" : "");

            foreach (char symbol in Encoding.UTF8.GetBytes(data))
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    encoded.Append(symbol);
                }
                else
                {
                    encoded.Append("%" + String.Format("{0:X2}", (int)symbol));
                }
            }

            return encoded.ToString();

        }

    }
}