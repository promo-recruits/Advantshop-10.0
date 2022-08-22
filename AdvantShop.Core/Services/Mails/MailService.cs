using AdvantShop.Configuration;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Core.Services.Loging.Emails;
using AdvantShop.Diagnostics;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AdvantShop.Core.Services.Customers.AdminInformers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Mails;

namespace AdvantShop.Core.Services.Mails
{
    public class MailService
    {
        private static IEmailLoger Loger { get { return LogingManager.GetEmailLoger(); } }

        /// <summary>
        /// run in task
        /// </summary>
        public static bool SendMailNow(Guid customerIdTo, string strTo, string strSubject, string strText, bool isBodyHtml, string replyTo = null, int lettercount = 1)
        {
            return SendMailNow(customerIdTo, strTo, strSubject, strText, isBodyHtml, emailingId: null, replyTo: replyTo, lettercount: lettercount);
        }

        /// <summary>
        /// directly executed
        /// </summary>
        public static bool SendMail(Guid customerIdTo, string strTo, string strSubject, string strText, bool isBodyHtml, string replyTo = null, bool needretry = true, int lettercount = 1)
        {
            return SendMail(customerIdTo, strTo, strSubject, strText, isBodyHtml, null, null, replyTo, needretry, lettercount);
        }

        /// <summary>
        /// run in task
        /// </summary>
        public static bool SendMailNow(Guid customerIdTo, string strTo, string strSubject, string strText, bool isBodyHtml, Guid? emailingId, string replyTo = null, int lettercount = 1)
        {
            Task.Factory.StartNew(() => SendMail(customerIdTo, strTo, strSubject, strText, isBodyHtml, emailingId, null, replyTo, lettercount: lettercount));
            return true;
        }

        public static bool SendMailNow(Guid customerIdTo, string strTo, string strSubject, string strText, bool isBodyHtml, int? formatId, string replyTo = null, int lettercount = 1)
        {
            Task.Factory.StartNew(() => SendMail(customerIdTo, strTo, strSubject, strText, isBodyHtml, null, formatId, replyTo, lettercount: lettercount));
            return true;
        }

        public static bool SendMail(Guid customerIdTo, string strTo, MailTemplate template, Guid? emailingId = null, string replyTo = null, bool needretry = true, int lettercount = 1)
        {
            template.BuildMail();
            return SendMail(customerIdTo, strTo, template.Subject, template.Body, true, emailingId, (int)template.Type, replyTo, needretry, lettercount);
        }

        public static bool SendMailNow(string strTo, MailTemplate template, Guid? emailingId = null, string replyTo = null, int lettercount = 1)
        {
            SendMailNow(Guid.Empty, strTo, template, emailingId, replyTo,  lettercount);
            return true;
        }

        public static bool SendMailNow(Guid customerIdTo, string strTo, MailTemplate template, Guid? emailingId = null, string replyTo = null, int lettercount = 1)
        {
            template.BuildMail();
            Task.Factory.StartNew(() => SendMail(customerIdTo, strTo, template.Subject, template.Body, true, emailingId, (int)template.Type, replyTo, lettercount: lettercount));
            return true;
        }

        /// <summary>
        /// directly executed
        /// </summary>
        public static bool SendMail(Guid customerIdTo, string strTo, string strSubject, string strText, bool isBodyHtml, Guid? emailingId, int? formatId, string replyTo = null, bool needretry = true, int lettercount = 1)
        {
            if (string.IsNullOrEmpty(strSubject) || string.IsNullOrEmpty(strText) || string.IsNullOrEmpty(strTo))
                return false;

            strText = PrepareMessage(strText);

            var status = EmailStatus.Sent;
            try
            {
                if (SettingsMail.UseAdvantshopMail)
                {
                    AdvantShopMailService.Send(customerIdTo, strTo, strSubject, strText, isBodyHtml, replyTo, lettercount, emailingId, formatId);
                }
                else
                {
                    SmtpMailService.Send(strTo, strSubject, strText, isBodyHtml, replyTo, needretry);
                }
            }
            catch (Exception e)
            {
                status = EmailStatus.Error;

                if (SettingsMail.UseAdvantshopMail)
                {
                    if (e.Message.ToLower().Contains("ограничение отправки для триала в день"))
                    {
                        AdminInformerService.Add(new AdminInformer
                        {
                            Body = "Ошибка при отправке писем: ограничение писем в день для триалального режима. Подключите свою почту в Настройках почты.",
                            Type = AdminInformerType.Error
                        });
                    }
                }

                if (needretry)
                {
                    Debug.Log.Warn(e.Message + " " + strTo, e);
                }
                else
                {
                    throw new BlException(e.Message + " " + strTo);
                }
            }
            finally
            {
                if (!SettingsMail.UseAdvantshopMail)
                {
                    Loger.LogEmail(new EmailLogItem()
                    {
                        CreateOn = DateTime.Now,
                        CustomerId = customerIdTo,
                        Body = strText,
                        EmailAddress = strTo,
                        Subject = strSubject,
                        Status = status
                    });
                }
            }

            return true;
        }

        private static string PrepareMessage(string message)
        {
            return message.Replace("\"userfiles/", string.Format("\"{0}/userfiles/", SettingsMain.SiteUrl.Trim('/')));
        }

        public static void Save(string fromEmail, string fromName)
        {
            if (SettingsMail.UseAdvantshopMail)
            {
                AdvantShopMailService.SaveSender(fromEmail, fromName);
                CapSettingProvider.Reset();
            }
        }

        public static bool SendValidate()
        {
            if (SettingsMail.UseAdvantshopMail)
            {
                var res = AdvantShopMailService.SendValidate();
                CapSettingProvider.Reset();
                Track.TrackService.TrackEvent(Track.ETrackEvent.Core_Settings_BindAdvantshopMailService);
                return res;
            }
            throw new BlException("validation only for UseAdvantshopMail");
        }

        public static void Reset()
        {
            if (SettingsMail.UseAdvantshopMail)
                CapSettingProvider.Reset();
        }

        public static List<EmailLogItem> GetLast(Guid customerId)
        {
            if (SettingsMail.UseAdvantshopMail)
                return AdvantShopMailService.GetLast(customerId);

            return null;
        }

        public static string ValidateMailSettingsBeforeSending()
        {
            string error = null;

            if (SettingsMail.UseAdvantshopMail &&
                !string.IsNullOrEmpty(CapShopSettings.FromEmail) && CapShopSettings.FromEmail.Contains("adv-mail"))
            {
                error =
                    LocalizationService.GetResourceFormat("Core.Services.Mails.MailService.NeedActivateYourEmailError",
                        UrlService.GetUrl("adminv2/settingsmail#?notifyTab=emailsettings"));
            }

            return error;
        }
    }
}
