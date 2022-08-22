using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Design;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Settings;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Settings.System
{
    public class GetSystemSettingsHandler
    {
        public SystemSettingsModel Execute()
        {
            var model = new SystemSettingsModel
            {
                //AdditionalHeadMetaTag = SettingsSEO.CustomMetaString,

                LogoImageAlt = SettingsMain.LogoImageAlt,
                AdminDateFormat = SettingsMain.AdminDateFormat,
                ShortDateFormat = SettingsMain.ShortDateFormat,
                EnablePhoneMask = SettingsMain.EnablePhoneMask,
                //IsStoreClosed = SettingsMain.IsStoreClosed,
                //EnableInplace = SettingsMain.EnableInplace,
                //DisplayToolBarBottom = SettingsDesign.DisplayToolBarBottom,
                //DisplayCityInTopPanel = SettingsDesign.DisplayCityInTopPanel,
                //ShowCopyright = SettingsDesign.ShowCopyright,

                EnableCaptcha = SettingsMain.EnableCaptcha,
                EnableCaptchaInCheckout = SettingsMain.EnableCaptchaInCheckout,
                EnableCaptchaInRegistration = SettingsMain.EnableCaptchaInRegistration,
                EnableCaptchaInFeedback = SettingsMain.EnableCaptchaInFeedback,
                EnableCaptchaInProductReview = SettingsMain.EnableCaptchaInProductReview,
                EnableCaptchaInPreOrder = SettingsMain.EnableCaptchaInPreOrder,
                EnableCaptchaInGiftCerticate = SettingsMain.EnableCaptchaInGiftCerticate,
                EnableCaptchaInBuyInOneClick = SettingsMain.EnableCaptchaInBuyInOneClick,
                CaptchaMode = (int)SettingsMain.CaptchaMode,
                CaptchaLength = SettingsMain.CaptchaLength,

                GoogleActive = SettingsOAuth.GoogleActive,
                MailActive = SettingsOAuth.MailActive,
                YandexActive = SettingsOAuth.YandexActive,
                VkontakteActive = SettingsOAuth.VkontakteActive,
                FacebookActive = SettingsOAuth.FacebookActive,
                OdnoklassnikiActive = SettingsOAuth.OdnoklassnikiActive,
                GoogleClientId = SettingsOAuth.GoogleClientId,
                GoogleClientSecret = SettingsOAuth.GoogleClientSecret,
                VkontakeClientId = SettingsOAuth.VkontakeClientId,
                VkontakeSecret = SettingsOAuth.VkontakeSecret,
                OdnoklassnikiClientId = SettingsOAuth.OdnoklassnikiClientId,
                OdnoklassnikiSecret = SettingsOAuth.OdnoklassnikiSecret,
                OdnoklassnikiPublicApiKey = SettingsOAuth.OdnoklassnikiPublicApiKey,
                FacebookClientId = SettingsOAuth.FacebookClientId,
                FacebookApplicationSecret = SettingsOAuth.FacebookApplicationSecret,
                MailAppId = SettingsOAuth.MailAppId,
                MailClientSecret = SettingsOAuth.MailClientSecret,
                YandexClientId = SettingsOAuth.YandexClientId,
                YandexClientSecret = SettingsOAuth.YandexClientSecret,

                CallbackUrl = StringHelper.ToPuny(SettingsMain.SiteUrl) + "login",

                IsSaas = SaasDataService.IsSaasEnabled,
                IsTrial = Trial.TrialService.IsTrialEnabled,
                LicKey = SettingsLic.LicKey,
                ActiveLic = SettingsLic.ActiveLic,

                FilesSize = SettingsMain.CurrentFilesStorageSize, //AttachmentService.GetAllAttachmentsSize(),

                //ShowUserAgreementText = SettingsCheckout.IsShowUserAgreementTextValue,
                //UserAgreementText = SettingsCheckout.UserAgreementText,
                //DisplayCityBubble = SettingsDesign.DisplayCityBubble,
                //ShowCookiesPolicyMessage = SettingsNotifications.ShowCookiesPolicyMessage,
                //CookiesPolicyMessage = SettingsNotifications.CookiesPolicyMessage,

                //StoreActive = SettingsMain.StoreActive,
                //LandingActive = SettingsLandingPage.ActiveLandingPage,
                //BonusActive = SettingsMain.BonusAppActive,
                //CrmActive = SettingsCrm.CrmActive,
                //TasksActive = SettingsTasks.TasksActive,
                //BookingActive = SettingsMain.BookingActive,
                //TriggersActive = SettingsMain.TriggersActive,
                //PartnersActive = SettingsMain.PartnersActive,

                ShowImageSearchEnabled = SettingsCatalog.ShowImageSearchEnabled,

                TrackProductChanges = SettingsMain.TrackProductChanges,

                //SiteLanguage = SettingsMain.Language,
                AdminStartPage = SettingsMain.AdminStartPage,

                ImageQuality = SettingsMain.ImageQuality,
                SiteLanguage = SettingsMain.Language,
            };

            model.Languages = LanguageService.GetList().Select(x => new SelectListItem { Text = x.Name, Value = x.LanguageCode }).ToList();
            if (model.Languages.Count == 0)
                model.Languages.Add(new SelectListItem { Text = "Нет языков" });

            model.CaptchaModes = new List<SelectListItem>();
            foreach (CaptchaMode value in Enum.GetValues(typeof(CaptchaMode)))
            {
                model.CaptchaModes.Add(new SelectListItem()
                {
                    Text = value.Localize(),
                    Value = ((int)value).ToString()
                });
            }

            model.EnableExperimentalFeatures = SettingsFeatures.EnableExperimentalFeatures;
            model.Features = Enum.GetValues(typeof(EFeature)).Cast<EFeature>()
                .ToDictionary(feature => feature.ToString(), feature => SettingsFeatures.IsFeatureEnabled(feature));

            //model.Languages = new List<SelectListItem>();

            //var languagesList = LanguageService.GetList();
            //if (languagesList != null)
            //{
            //    model.Languages.AddRange(languagesList.Select(item => new SelectListItem { Text = item.Name, Value = item.LanguageCode }));
            //}
            //else
            //{
            //    model.Languages.Add(new SelectListItem { Text = "Нет языков" });
            //}


            var sitemapFilePathXml = SettingsGeneral.AbsolutePath + "sitemap.xml";
            var sitemapFilePathHtml = SettingsGeneral.AbsolutePath + "sitemap.html";

            if (File.Exists(sitemapFilePathXml))
            {
                model.SiteMapFileXmlDate = Localization.Culture.ConvertDate(new FileInfo(sitemapFilePathXml).LastWriteTime);
                model.SiteMapFileXmlLinkText = SettingsMain.SiteUrl + "/sitemap.xml";
                model.SiteMapFileXmlLink = model.SiteMapFileXmlLinkText + "?rnd=" + (new Random().Next(10000));
            }

            if (File.Exists(sitemapFilePathHtml))
            {
                model.SiteMapFileHtmlDate = Localization.Culture.ConvertDate(new FileInfo(sitemapFilePathHtml).LastWriteTime);
                model.SiteMapFileHtmlLinkText = SettingsMain.SiteUrl + "/sitemap.html";
                model.SiteMapFileHtmlLink = model.SiteMapFileHtmlLinkText + "?rnd=" + (new Random().Next(10000));
            }

            model.AdminAreaColorScheme = SettingsDesign.AdminAreaColorScheme;
            var colorSchemes = new List<AdminAreaColorScheme>();

            try
            {
                foreach (var dir in Directory.GetDirectories(HostingEnvironment.MapPath("~/areas/admin/templates/adminv3/content/src/color-schemes/")))
                {
                    var configPath = dir + "\\config.json";
                    var cssPath = dir + "\\styles.css";

                    if (!File.Exists(configPath) || !File.Exists(cssPath))
                        continue;

                    var key = dir.Split('\\').LastOrDefault();
                    var colorScheme = JsonConvert.DeserializeObject<AdminAreaColorScheme>(File.ReadAllText(configPath));
                    colorScheme.Key = key;

                    colorSchemes.Add(colorScheme);
                }
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            model.AdminAreaColorSchemes = colorSchemes.OrderBy(x => x.SortOrder).Select(x => new SelectListItem() {Text = x.Name, Value = x.Key }).ToList();

            model.AdminAreaTemplate = AdminAreaTemplate.Template;
            model.AdminAreaTemplates = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Новая версия", Value = "adminv3"},
                new SelectListItem() {Text = "Старая версия", Value = "adminv2"},
            };

            model.AdminStartPages = new List<SelectListItem>()
            {
                new SelectListItem() {Text = "Рабочий стол", Value = "desktop"},
                new SelectListItem() {Text = "Заказы", Value = "orders"},
            };

            if (SettingsCrm.CrmActive)
                model.AdminStartPages.Add(new SelectListItem() { Text = "Лиды", Value = "leads" });

            var dashboard = SalesChannelService.GetByType(ESalesChannelType.Dashboard);
            if (dashboard.Enabled)
                model.AdminStartPages.Add(new SelectListItem() { Text = "Мои сайты", Value = "dashboard" });
            else if (model.AdminStartPage == "dashboard")
                model.AdminStartPage = SettingsMain.AdminStartPage = "orders";

            if (SettingsTasks.TasksActive)
                model.AdminStartPages.Add(new SelectListItem() {Text = "Задачи", Value = "tasks"});
            else if (model.AdminStartPage == "tasks")
                model.AdminStartPage = SettingsMain.AdminStartPage = "orders";
            
            return model;
        }
    }
}
