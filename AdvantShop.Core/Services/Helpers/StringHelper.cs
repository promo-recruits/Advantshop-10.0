//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Repository;

namespace AdvantShop.Helpers
{
    public class StringHelper
    {
        /// <summary>
        /// The method create a Base64 encoded string from a normal string.
        /// </summary>
        /// <param name="toEncode">The String containing the characters to encode.</param>
        /// <returns>The Base64 encoded string.</returns>
        public static string EncodeTo64(string toEncode)
        {
            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(toEncode);
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        /// <summary>
        /// The method to Decode your Base64 strings.
        /// </summary>
        /// <param name="encodedData">The String containing the characters to decode.</param>
        /// <returns>A String containing the results of decoding the specified sequence of bytes.</returns>
        public static string DecodeFrom64(string encodedData)
        {
            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData);
            string returnValue = Encoding.UTF8.GetString(encodedDataAsBytes);
            return returnValue;
        }

        public static String UTF8ByteArrayToString(Byte[] characters)
        {
            var encoding = new UTF8Encoding();
            String constructedString = encoding.GetString(characters);
            return (constructedString);
        }

        public static Byte[] StringToUTF8ByteArray(String pXmlString)
        {
            var encoding = new UTF8Encoding();
            Byte[] byteArray = encoding.GetBytes(pXmlString);
            return byteArray;
        }

        public static string GetWindows1251(string utfText)
        {
            var win = Encoding.GetEncoding("windows-1251");
            var utf = Encoding.GetEncoding("UTF-8");

            var utfBytes = utf.GetBytes(utfText);
            var winBytes = Encoding.Convert(utf, win, utfBytes, 0, utfBytes.Length);

            var winStr = utf.GetString(winBytes, 0, winBytes.Length);

            return winStr;
        }

        public static string GetPlainFieldName(string fieldName)
        {
            return !fieldName.ToLower().Contains("as") ? fieldName : fieldName.Split(new[] { "as" }, StringSplitOptions.RemoveEmptyEntries).First();
        }

        public static string ReplaceCharInStringByIndex(string strSource, int intIndex, Char chrNewSymb)
        {
            var sb = new StringBuilder(strSource);
            sb[intIndex] = chrNewSymb;
            return sb.ToString();
        }

        public static string Translit(string str)
        {
            if (str.IsNullOrEmpty())
                return String.Empty;

            var dic = new Dictionary<char, string>
            {
                              {'а', "a"},
                              {'б', "b"},
                              {'в', "v"},
                              {'г', "g"},
                              {'д', "d"},
                              {'е', "e"},
                              {'ё', "e"},
                              {'ж', "zh"},
                              {'з', "z"},
                              {'и', "i"},
                              {'й', "i"},
                              {'к', "k"},
                              {'л', "l"},
                              {'м', "m"},
                              {'н', "n"},
                              {'о', "o"},
                              {'п', "p"},
                              {'р', "r"},
                              {'с', "s"},
                              {'т', "t"},
                              {'у', "u"},
                              {'ф', "f"},
                              {'х', "kh"},
                              {'ц', "ts"},
                              {'ч', "ch"},
                              {'ш', "sh"},
                              {'щ', "sch"},
                              {'ъ', ""},
                              {'ы', "y"},
                              {'ь', ""},
                              {'э', "e"},
                              {'ю', "iu"},
                              {'я', "ya"},
                          };
            var sb = new StringBuilder();
            foreach (char c in str.ToLower())
            {
                sb.Append(dic.ContainsKey(c) ? dic[c] : c.ToString());
            }
            return sb.ToString();
        }


        public static string TranslitToRus(string str)
        {
            if (str.IsNullOrEmpty())
                return String.Empty;

            var dic = new Dictionary<char, string>
            {
                              {'a', "а"},
                              {'b', "и"},
                              {'c', "к"},
                              {'d', "д"},
                              {'e', "е"},
                              {'f', "ф"},
                              {'g', "г"},
                              {'h', "х"},
                              {'i', "и"},
                              {'j', "й"},
                              {'k', "к"},
                              {'l', "л"},
                              {'m', "м"},
                              {'n', "н"},
                              {'o', "о"},
                              {'p', "п"},
                              {'q', "к"},
                              {'r', "р"},
                              {'s', "с"},
                              {'t', "т"},
                              {'u', "у"},
                              {'v', "в"},
                              {'w', "в"},
                              {'x', "х"},
                              {'y', "й"},
                              {'z', "з"},
                          };
            var sb = new StringBuilder();
            foreach (char c in str.ToLower())
            {
                sb.Append(dic.ContainsKey(c) ? dic[c] : c.ToString());
            }
            return sb.ToString();
        }

        public static string TranslitToRusKeyboard(string str)
        {
            if (str.IsNullOrEmpty())
                return String.Empty;

            var dic = new Dictionary<char, string>
            {
                              {'`', "ё"},
                              {'q', "й"},
                              {'w', "ц"},
                              {'e', "у"},
                              {'r', "к"},
                              {'t', "е"},
                              {'y', "н"},
                              {'u', "г"},
                              {'i', "ш"},
                              {'o', "щ"},
                              {'p', "з"},
                              {'[', "х"},
                              {']', "ъ"},
                              {'a', "ф"},
                              {'s', "ы"},
                              {'d', "в"},
                              {'f', "а"},
                              {'g', "п"},
                              {'h', "р"},
                              {'j', "о"},
                              {'k', "л"},
                              {'l', "д"},
                              {';', "ж"},
                              {'\'', "э"},
                              {'z', "я"},
                              {'x', "ч"},
                              {'c', "с"},
                              {'v', "м"},
                              {'b', "и"},
                              {'n', "т"},
                              {'m', "ь"},
                              {',', "б"},
                              {'.', "ю"},

                              {'й', "q"},
                              {'ц', "w"},
                              {'у', "e"},
                              {'к', "r"},
                              {'е', "t"},
                              {'н', "y"},
                              {'г', "u"},
                              {'ш', "i"},
                              {'щ', "o"},
                              {'з', "p"},
                              {'ф', "a"},
                              {'ы', "s"},
                              {'в', "d"},
                              {'а', "f"},
                              {'п', "g"},
                              {'р', "h"},
                              {'о', "j"},
                              {'л', "k"},
                              {'д', "l"},
                              {'я', "z"},
                              {'ч', "x"},
                              {'с', "c"},
                              {'м', "v"},
                              {'и', "b"},
                              {'т', "n"},
                              {'ь', "m"},
                          };
            var sb = new StringBuilder();
            foreach (char c in str.ToLower())
            {
                sb.Append(dic.ContainsKey(c) ? dic[c] : c.ToString());
            }
            return sb.ToString();
        }


        public static string TransformUrl(string url)
        {
            var pattern = !SettingsMain.EnableCyrillicUrl ? "[^a-zA-Z0-9_-]+" : "[^a-zA-Zа-яА-Я0-9_-]+";
            var rg = new Regex(pattern, RegexOptions.Singleline);
            var temp = rg.Replace(url, "-");
            return Regex.Replace(temp, "-+", "-").Trim('-');
        }

        public static string GetReSpacedString(string strSource)
        {
            return GetReSpacedString(strSource, 19); // By default
        }

        public static string GetReSpacedString(string strSource, int intCountCharsBeforeSplit)
        {

            if (String.IsNullOrEmpty(strSource))
                return String.Empty;

            var sbResult = new StringBuilder();
            int j = 0;

            foreach (char t in strSource)
            {
                j += 1;

                if (t == ' ')
                    j = 0;

                if (j >= intCountCharsBeforeSplit)
                {
                    // Добавляем пробле в строку и сбрасываем счетчик.
                    sbResult.Append(t);
                    sbResult.Append(' ');
                    j = 0;
                }
                else
                {
                    // Продолжаем формировать строку.
                    sbResult.Append(t);
                }
            }

            return (sbResult.ToString().Replace(" /", "/ ").Replace(" .", ". ").Replace(" ,", ", ")); // IE Fix with " x" space.

        }

        public static string ToPuny(string value)
        {
            if (string.IsNullOrEmpty(value) || !IsAbsoluteUrl(value))
                return value;

            var url = value;
            var isHost = false;

            if (!url.StartsWith("http"))
            {
                url = "http://" + url;
                isHost = true;
            }

            Uri uri;
            if (Uri.TryCreate(url, UriKind.Absolute, out uri))
            {
                var idn = new IdnMapping();

                var punyUri = idn.GetAscii(uri.Host);
                if (isHost)
                    return value.Replace(uri.Host, punyUri);

                uri = ReplaceHost(uri, punyUri);
                return uri.ToString();
            }

            return value;
        }

        private static bool IsAbsoluteUrl(string url)
        {
            if (url.StartsWith("http"))
                return true;

            var arr = url.Replace("//", "").Split('/');
            if (arr.Length > 0 && arr[0].Contains('.'))
                return true;

            return false;
        }

        private static Uri ReplaceHost(Uri original, string newHostName)
        {
            var builder = new UriBuilder(original) { Host = newHostName };
            return builder.Uri;
        }

        public static string FromPuny(string value)
        {
            Uri uri;
            IdnMapping idn = new IdnMapping();

            if (Uri.TryCreate(value, UriKind.Absolute, out uri))
            {
                uri = ReplaceHost(uri, idn.GetUnicode(uri.Host));
                return uri.ToString();
            }
            else
            {
                return value;
            }
        }

        public static bool GetMoneyFromString(string stringMoney, out float decimalMoney)
        {
            return Single.TryParse(stringMoney.Replace(" ", "").Replace(((char)160).ToString(), "").Replace(",", "."), NumberStyles.Any, CultureInfo.InvariantCulture, out decimalMoney);
        }

        public static string RemoveHTML(string inputString)
        {
            if (string.IsNullOrEmpty(inputString))
                return string.Empty;
            const string HTML_TAG_PATTERN = "<.*?>";
            return HttpUtility.HtmlDecode(Regex.Replace(inputString, HTML_TAG_PATTERN, String.Empty).Replace("&nbsp;", " "));

        }

        //public static long? ConvertToStandardPhone(string phone, bool force = false, bool forceTrimEight= false)
        //{
        //    var country = IpZoneContext.CurrentZone;
        //    return ConvertToStandardPhone(phone, force, country.DialCode, forceTrimEight);
        //}

        public static string ReplaceCirilikSymbol(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;
            value = value.Replace('ё', 'е');
            value = value.Replace('Ё', 'Е');
            return value;
        }

        public static long? ConvertToStandardPhone(string phone, bool force = false, bool forceTrimEight = false, int? dcode = null)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var str = Regex.Replace(phone, @"[^\d]", "");

            if (string.IsNullOrWhiteSpace(str))
                return null;

            // <dialCode, length>
            var presets = new Dictionary<string, int>
            {
                { "7", 11 },    // Россия
                { "380", 12 },  // Украина
                { "375", 12 },  // Беларусь
                { "996", 12},   // Киргизия
            };

            if (presets.Keys.Any(dialCode => str.StartsWith(dialCode) && str.Length == presets[dialCode]))
                return str.TryParseLong(true);

            if (str.StartsWith("8") && (str.Length == 11 || forceTrimEight))
            {
                str = "7" + str.Remove(0, 1);
            }
            else
            {
                var dialCode = dcode ?? IpZoneContext.CurrentZone.DialCode;

                if (dialCode.HasValue && !str.StartsWith(dialCode.Value.ToString()) && !force)
                    str = dialCode.Value.ToString() + str;
            }


            return str.TryParseLong(true);
        }

        public static string ConvertToMobileStandardPhone(string phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return null;

            var str = Regex.Replace(phone, @"[^\d]", "");

            if (string.IsNullOrWhiteSpace(str))
                return null;

            var standardPhone = str.TryParseLong(true);

            return standardPhone.HasValue && standardPhone.Value != 0
                ? (phone.StartsWith("+") ? "+" : "") + standardPhone.Value
                : null;
        }

        public static string GeneratePassword(int length)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            var res = new StringBuilder();
            var rnd = new Random();

            while (0 < length--)
                res.Append(valid[rnd.Next(valid.Length)]);

            return res.ToString();
        }

        public static string AggregateStrings(string separator, params string[] args)
        {
            return String.Join(separator, args.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public static string GenerateDiffHtml(string oldValue, string newValue)
        {
            HtmlDiff.HtmlDiff diffHelper = new HtmlDiff.HtmlDiff(oldValue, newValue, "span", "span", null, null,
                "background-color:#ddfade;white-space:pre;", "background-color:#ffe7e7;text-decoration:line-through;white-space:pre;");

            return diffHelper.Build();
        }

        public static string FormatDateTimeInterval(DateTime date)
        {
            TimeInterval ti;
            var datesRange = (date - DateTime.Now).Duration();
            if (datesRange.TotalDays > 1)
                ti = new TimeInterval() { Interval = (int)Math.Floor(datesRange.TotalDays), IntervalType = TimeIntervalType.Days };
            else if (datesRange.TotalHours > 1)
                ti = new TimeInterval() { Interval = (int)Math.Floor(datesRange.TotalHours), IntervalType = TimeIntervalType.Hours };
            else if (datesRange.TotalMinutes > 1)
                ti = new TimeInterval() { Interval = (int)Math.Floor(datesRange.TotalMinutes), IntervalType = TimeIntervalType.Minutes };
            else
                return "Только что";

            return string.Format("{0} {1}", ti.Interval, ti.Numeral("минут"));
        }

        
        /// <summary>
        /// Определяет кодировку по тексту файла
        /// </summary>
        /// <returns></returns>
        public static Encoding DetectFileTextEncoding(string filename, int taster = 1000)
        {
            // https://stackoverflow.com/questions/1025332/determine-a-strings-encoding-in-c-sharp

            // Function to detect the encoding for UTF-7, UTF-8/16/32 (bom, no bom, little
            // & big endian), and local default codepage, and potentially other codepages.
            // 'taster' = number of bytes to check of the file (to save processing). Higher
            // value is slower, but more reliable (especially UTF-8 with special characters
            // later on may appear to be ASCII initially). If taster = 0, then taster
            // becomes the length of the file (for maximum reliability). 'text' is simply
            // the string with the discovered encoding applied to the file.

            byte[] b = File.ReadAllBytes(filename);

            //////////////// First check the low hanging fruit by checking if a
            //////////////// BOM/signature exists (sourced from http://www.unicode.org/faq/utf_bom.html#bom4)
            if (b.Length >= 4 && b[0] == 0x00 && b[1] == 0x00 && b[2] == 0xFE && b[3] == 0xFF) { return Encoding.GetEncoding("utf-32BE"); }  // UTF-32, big-endian 
            else if (b.Length >= 4 && b[0] == 0xFF && b[1] == 0xFE && b[2] == 0x00 && b[3] == 0x00) { return Encoding.UTF32; }    // UTF-32, little-endian
            else if (b.Length >= 2 && b[0] == 0xFE && b[1] == 0xFF) { return Encoding.BigEndianUnicode; }     // UTF-16, big-endian
            else if (b.Length >= 2 && b[0] == 0xFF && b[1] == 0xFE) { return Encoding.Unicode; }              // UTF-16, little-endian
            else if (b.Length >= 3 && b[0] == 0xEF && b[1] == 0xBB && b[2] == 0xBF) { return Encoding.UTF8; } // UTF-8
            else if (b.Length >= 3 && b[0] == 0x2b && b[1] == 0x2f && b[2] == 0x76) { return Encoding.UTF7; } // UTF-7


            //////////// If the code reaches here, no BOM/signature was found, so now
            //////////// we need to 'taste' the file to see if can manually discover
            //////////// the encoding. A high taster value is desired for UTF-8
            if (taster == 0 || taster > b.Length) taster = b.Length;    // Taster size can't be bigger than the filesize obviously.


            // Some text files are encoded in UTF8, but have no BOM/signature. Hence
            // the below manually checks for a UTF8 pattern. This code is based off
            // the top answer at: https://stackoverflow.com/questions/6555015/check-for-invalid-utf8
            // For our purposes, an unnecessarily strict (and terser/slower)
            // implementation is shown at: https://stackoverflow.com/questions/1031645/how-to-detect-utf-8-in-plain-c
            // For the below, false positives should be exceedingly rare (and would
            // be either slightly malformed UTF-8 (which would suit our purposes
            // anyway) or 8-bit extended ASCII/UTF-16/32 at a vanishingly long shot).
            int i = 0;
            bool utf8 = false;
            while (i < taster - 4)
            {
                if (b[i] <= 0x7F) { i += 1; continue; }     // If all characters are below 0x80, then it is valid UTF8, but UTF8 is not 'required' (and therefore the text is more desirable to be treated as the default codepage of the computer). Hence, there's no "utf8 = true;" code unlike the next three checks.
                if (b[i] >= 0xC2 && b[i] <= 0xDF && b[i + 1] >= 0x80 && b[i + 1] < 0xC0) { i += 2; utf8 = true; continue; }
                if (b[i] >= 0xE0 && b[i] <= 0xF0 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0) { i += 3; utf8 = true; continue; }
                if (b[i] >= 0xF0 && b[i] <= 0xF4 && b[i + 1] >= 0x80 && b[i + 1] < 0xC0 && b[i + 2] >= 0x80 && b[i + 2] < 0xC0 && b[i + 3] >= 0x80 && b[i + 3] < 0xC0) { i += 4; utf8 = true; continue; }
                utf8 = false; break;
            }
            if (utf8 == true)
            {
                return Encoding.UTF8;
            }


            // The next check is a heuristic attempt to detect UTF-16 without a BOM.
            // We simply look for zeroes in odd or even byte places, and if a certain
            // threshold is reached, the code is 'probably' UF-16.          
            double threshold = 0.1; // proportion of chars step 2 which must be zeroed to be diagnosed as utf-16. 0.1 = 10%
            int count = 0;
            for (int n = 0; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { return Encoding.BigEndianUnicode; }
            count = 0;
            for (int n = 1; n < taster; n += 2) if (b[n] == 0) count++;
            if (((double)count) / taster > threshold) { return Encoding.Unicode; } // (little-endian)

            
            for (int n = 0; n < taster - 9; n++)
            {
                if (
                    ((b[n + 0] == 'c' || b[n + 0] == 'C') && (b[n + 1] == 'h' || b[n + 1] == 'H') && (b[n + 2] == 'a' || b[n + 2] == 'A') && (b[n + 3] == 'r' || b[n + 3] == 'R') && (b[n + 4] == 's' || b[n + 4] == 'S') && (b[n + 5] == 'e' || b[n + 5] == 'E') && (b[n + 6] == 't' || b[n + 6] == 'T') && (b[n + 7] == '=')) ||
                    ((b[n + 0] == 'e' || b[n + 0] == 'E') && (b[n + 1] == 'n' || b[n + 1] == 'N') && (b[n + 2] == 'c' || b[n + 2] == 'C') && (b[n + 3] == 'o' || b[n + 3] == 'O') && (b[n + 4] == 'd' || b[n + 4] == 'D') && (b[n + 5] == 'i' || b[n + 5] == 'I') && (b[n + 6] == 'n' || b[n + 6] == 'N') && (b[n + 7] == 'g' || b[n + 7] == 'G') && (b[n + 8] == '='))
                    )
                {
                    if (b[n + 0] == 'c' || b[n + 0] == 'C') n += 8; else n += 9;
                    if (b[n] == '"' || b[n] == '\'') n++;
                    int oldn = n;
                    while (n < taster && (b[n] == '_' || b[n] == '-' || (b[n] >= '0' && b[n] <= '9') || (b[n] >= 'a' && b[n] <= 'z') || (b[n] >= 'A' && b[n] <= 'Z')))
                    { n++; }
                    byte[] nb = new byte[n - oldn];
                    Array.Copy(b, oldn, nb, 0, n - oldn);
                    try
                    {
                        string internalEnc = Encoding.ASCII.GetString(nb);
                        return Encoding.GetEncoding(internalEnc);
                    }
                    catch { break; }    // If C# doesn't recognize the name of the encoding, break.
                }
            }
            return Encoding.Default;
        }
    }
}
