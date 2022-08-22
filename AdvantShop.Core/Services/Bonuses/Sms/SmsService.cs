using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AdvantShop.Core.Services.Bonuses.Model;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Sms.Template;
using AdvantShop.Core.Services.Smses;
using AdvantShop.Diagnostics;
using RazorEngine;
using RazorEngine.Templating;
using Encoding = System.Text.Encoding;

namespace AdvantShop.Core.Services.Bonuses.Sms
{

    public class SmsService
    {
        private static string NormalizeRazor(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;
            var pattern = string.Format( @"({0})(\w+)({0})", BaseSmsTemplate.ModelTag);
            var replaced = Regex.Replace(text, pattern, "@Raw(Model.$2)");
            return replaced;
        }

        public static bool Valid(string text, ESmsType type)
        {
            try
            {
                var viewModel = BaseSmsTemplate.Factory(type);
                Engine.Razor.RunCompile(NormalizeRazor(text), GetMd5Hash(text), null, viewModel.Prepare());
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return false;
        }

        public static void Process(long phone, ESmsType smsType, BaseSmsTemplate model)
        {
            if (!BonusSystem.SmsEnabled)
                return;
            
            var smsNotification = SmsTemplateService.Get(smsType);
            if (smsNotification == null)
                return;

            var smsText = "";
            try
            {
                smsText = Engine.Razor.RunCompile(NormalizeRazor(smsNotification.SmsBody), GetMd5Hash(smsNotification.SmsBody),  null, model.Prepare());
                
                var state = SmsNotifier.SendSmsNowWithResult(phone, smsText);

                SmsTemplateService.AddSmsLog(new SmsLog { Body = smsText, State = state, Phone = phone, Created = DateTime.Now });
            }
            catch (Exception ex)
            {
                SmsTemplateService.AddSmsLog(new SmsLog { Body = smsText, State = "error " + ex.Message, Phone = phone, Created = DateTime.Now });                
            }
        }

        private static string GetMd5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            foreach (byte t in hash)
            {
                sb.Append(t.ToString("X2"));
            }
            return sb.ToString();
        }
    }
}
