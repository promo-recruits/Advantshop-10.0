using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AdvantShop.Diagnostics;
using System.IO;

namespace AdvantShop.Core.Services.Messages
{
	public class MessageServices
	{
        private static readonly Regex PhoneValid = new Regex("^([0-9]{10,15})$", RegexOptions.Compiled | RegexOptions.Singleline);

        public static void SendMessage(long phone, string text)
		{
			Task.Run(() => SendMessageNow(phone, text));
		}

		public static ISendMessage GetActiveModule()
		{
			var moduleType = AttachedModules.GetModuleById("Wazzup", true);
            if (moduleType != null)
            {
                var module = (ISendMessage)Activator.CreateInstance(moduleType, null);
                return module;
            }
            return null;
		}

		private static string SendMessageNow(long phone, string text, bool? throwException = false)
		{
			string result = null;
            var throwError = throwException != null && throwException.Value;

            if (string.IsNullOrWhiteSpace(text) || phone == 0 || !IsValidPhone(phone))
            {
                if (throwError)
                    throw new BlException("Укажите валидный телефон и текст");
                return result;
            }

            try
            {
                var module = GetActiveModule();
                if (module == null)
                {
                    if (throwError)
                        throw new BlException("Не подключен модуль Wazzup");
                    return result;
                }

                result = module.SendMessage(phone, text);

            }
            catch (WebException ex)
            {
                using (var eResponse = ex.Response)
                    if (eResponse != null)
                    {
                        using (var eStream = eResponse.GetResponseStream())
                            if (eStream != null)
                                using (var reader = new StreamReader(eStream))
                                {
                                    var error = reader.ReadToEnd();
                                    Debug.Log.Error(error);

                                    if (throwError) throw;
                                }
                    }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);

                if (throwError) throw;
            }
			return result;
		}

        private static bool IsValidPhone(long phone)
        {
            return PhoneValid.IsMatch(phone.ToString());
        }
    }
}
