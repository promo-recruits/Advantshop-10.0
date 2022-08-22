//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.Services.SalesChannels;
using Newtonsoft.Json;

namespace AdvantShop.Configuration
{
    public enum CaptchaMode
    {
        [Localize("Core.Configuration.CaptchaMode.Numeric")]
        Numeric = 0,

        [Localize("Core.Configuration.CaptchaMode.AlphaNumericEn")]
        AlphaNumericEn = 1,

        [Localize("Core.Configuration.CaptchaMode.AlphaNumericRu")]
        AlphaNumericRu = 2,
    }

    public class SettingsMain
    {

        private static readonly List<string> TechDomains = new List<string> { "on-advantshop.net", "on-promo-z.ru", "advant.cloud", "advant.me", "advantme.", "advant.one", "advant.space", "advant.pw" };

        public static bool IsTechDomain(Uri uri)
        {
            return uri.IsAbsoluteUri && TechDomains.Any(domain => uri.Host.Contains(domain));
        }

        public static bool EnableInplace
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableInplace"]);
            set => SettingProvider.Items["EnableInplace"] = value.ToString();
        }

        public static bool EnablePhoneMask
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnablePhoneMask"]);
            set => SettingProvider.Items["EnablePhoneMask"] = value.ToString();
        }

        public static bool EnableCaptcha
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCheckOrderConfirmCode"]);
            set => SettingProvider.Items["EnableCheckOrderConfirmCode"] = value.ToString();
        }

        public static bool EnableCaptchaInCheckout
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInCheckout"]);
            set => SettingProvider.Items["EnableCaptchaInCheckout"] = value.ToString();
        }

        public static bool EnableCaptchaInRegistration
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInRegistration"]);
            set => SettingProvider.Items["EnableCaptchaInRegistration"] = value.ToString();
        }

        public static bool EnableCaptchaInPreOrder
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInPreOrder"]);
            set => SettingProvider.Items["EnableCaptchaInPreOrder"] = value.ToString();
        }

        public static bool EnableCaptchaInGiftCerticate
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInGiftCerticate"]);
            set => SettingProvider.Items["EnableCaptchaInGiftCerticate"] = value.ToString();
        }

        public static bool EnableCaptchaInFeedback
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInFeedback"]);
            set => SettingProvider.Items["EnableCaptchaInFeedback"] = value.ToString();
        }

        public static bool EnableCaptchaInProductReview
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInProductReview"]);
            set => SettingProvider.Items["EnableCaptchaInProductReview"] = value.ToString();
        }

        public static bool EnableCaptchaInBuyInOneClick
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCaptchaInBuyInOneClick"]);
            set => SettingProvider.Items["EnableCaptchaInBuyInOneClick"] = value.ToString();
        }

        public static CaptchaMode CaptchaMode
        {
            get => SettingProvider.Items["CaptchaMode"] != null ? (CaptchaMode)Convert.ToInt32(SettingProvider.Items["CaptchaMode"]) : CaptchaMode.AlphaNumericRu;
            set => SettingProvider.Items["CaptchaMode"] = ((int)value).ToString();
        }

        public static int CaptchaLength
        {
            get => Convert.ToInt32(SettingProvider.Items["CaptchaLength"]);
            set => SettingProvider.Items["CaptchaLength"] = value.ToString();
        }



        public static bool EnableAutoUpdateCurrencies
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableAutoUpdateCurrencies"]);
            set
            {
                SettingProvider.Items["EnableAutoUpdateCurrencies"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static string LogoImageName
        {
            get => SettingProvider.Items["MainPageLogoFileName"];
            set
            {
                SettingProvider.Items["MainPageLogoFileName"] = value;
                IsDefaultLogo = false;
            }
        }

        public static int LogoImageWidth
        {
            get => Convert.ToInt32(SettingProvider.Items["LogoImageWidth"]);
            set => SettingProvider.Items["LogoImageWidth"] = value.ToString();
        }

        public static int LogoImageHeight
        {
            get => Convert.ToInt32(SettingProvider.Items["LogoImageHeight"]);
            set => SettingProvider.Items["LogoImageHeight"] = value.ToString();
        }

        public static string PreviewLogoImageName
        {
            get => SettingProvider.Items["PreviewLogoImageName"];
            set => SettingProvider.Items["PreviewLogoImageName"] = value;
        }

        public static bool IsDefaultLogo
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsDefaultLogo"]);
            set => SettingProvider.Items["IsDefaultLogo"] = value.ToString();
        }

        public static string FaviconImageName
        {
            get => SettingProvider.Items["MainFaviconFileName"];
            set => SettingProvider.Items["MainFaviconFileName"] = value;
        }

        public static string SiteUrl
        {
            get => SettingProvider.Items["ShopURL"].Trim('/');
            set => SettingProvider.Items["ShopURL"] = value.Trim('/');
        }

        public static bool IsStoreClosed
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsStoreClosed"]);
            set => SettingProvider.Items["IsStoreClosed"] = value.ToString();
        }

        public static bool StoreActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["StoreActive"]);
            set => SettingProvider.Items["StoreActive"] = value.ToString();
        }

        public static string SiteUrlPlain => SiteUrl.Replace("http://", "").Replace("https://", "").Replace("www.", "");


        public static string ShopName
        {
            get => SettingProvider.Items["ShopName"];
            set => SettingProvider.Items["ShopName"] = value;
        }

        public static string AdminShopName
        {
            get => SettingProvider.Items["AdminShopName"];
            set => SettingProvider.Items["AdminShopName"] = value;
        }

        public static string LogoImageAlt
        {
            get => SettingProvider.Items["ImageAltText"];
            set => SettingProvider.Items["ImageAltText"] = value;
        }

        public static string Language
        {
            get => SettingProvider.Items["Language"] ?? "ru-RU";
            set => SettingProvider.Items["Language"] = value;
        }

        public static string AdminDateFormat
        {
            get => SettingProvider.Items["AdminDateFormat"];
            set => SettingProvider.Items["AdminDateFormat"] = value;
        }

        public static string ShortDateFormat
        {
            get => SettingProvider.Items["ShortDateFormat"];
            set => SettingProvider.Items["ShortDateFormat"] = value;
        }

        public static int SellerCountryId
        {
            get => int.Parse(SettingProvider.Items["SellerCountryId"]);
            set => SettingProvider.Items["SellerCountryId"] = value.ToString();
        }

        public static int SellerRegionId
        {
            get => int.Parse(SettingProvider.Items["SellerRegionId"]);
            set => SettingProvider.Items["SellerRegionId"] = value.ToString();
        }

        public static string Phone
        {
            get => SettingProvider.Items["Phone"];
            set => SettingProvider.Items["Phone"] = value;
        }

        public static string MobilePhone
        {
            get => SettingProvider.Items["MobilePhone"];
            set => SettingProvider.Items["MobilePhone"] = value;
        }

        public static string PhoneDescription
        {
            get => SettingProvider.Items["PhoneDescription"];
            set => SettingProvider.Items["PhoneDescription"] = value;
        }

        public static string City
        {
            get => SettingProvider.Items["City"];
            set => SettingProvider.Items["City"] = value;
        }

        public static string SearchPage
        {
            get => SettingProvider.Items["SearchPage"];
            set => SettingProvider.Items["SearchPage"] = value;
        }

        public static string SearchArea
        {
            get => SettingProvider.Items["SearchArea"];
            set => SettingProvider.Items["SearchArea"] = value;
        }

        public static string Achievements
        {
            get => SettingProvider.Items["Achievements"];
            set => SettingProvider.Items["Achievements"] = value;
        }

        public static string AchievementsPoints
        {
            get => SettingProvider.Items["AchievementsPoints"];
            set => SettingProvider.Items["AchievementsPoints"] = value;
        }

        public static string AchievementsDescription
        {
            get => SettingProvider.Items["AchievementsDescription"];
            set => SettingProvider.Items["AchievementsDescription"] = value;
        }

        public static string AchievementsPopUp
        {
            get => SettingProvider.Items["AchievementsPopUp"];
            set => SettingProvider.Items["AchievementsPopUp"] = value;
        }

        public static bool EnableCyrillicUrl
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnableCyrillicUrl"]);
            set => SettingProvider.Items["EnableCyrillicUrl"] = value.ToString();
        }

        public static bool UseMultiThreads
        {
            get
            {
                if (SettingProvider.Items["UseMultiThreads"] == null)
                {
                    UseMultiThreads = true;
                }
                return Convert.ToBoolean(SettingProvider.Items["UseMultiThreads"]);
            }
            set => SettingProvider.Items["UseMultiThreads"] = value.ToString();
        }

        /// <summary>
        /// Текущий размер файлов хранилища
        /// </summary>
        public static long CurrentFilesStorageSize
        {
            get => Convert.ToInt64(SettingProvider.Items["CurrentFilesStorageSize"]);
            set => SettingProvider.Items["CurrentFilesStorageSize"] = value.ToString();
        }

        public static double CurrentFilesStorageSwTime
        {
            get => Convert.ToDouble(SettingProvider.Items["CurrentFilesStorageSwTime"]);
            set => SettingProvider.Items["CurrentFilesStorageSwTime"] = value.ToString();
        }

        public static DateTime CurrentFilesStorageLastUpdateTime
        {
            get => SettingProvider.Items["CurrentFilesStorageLastUpdateTime"].TryParseDateTime();
            set => SettingProvider.Items["CurrentFilesStorageLastUpdateTime"] = value.ToString();
        }
        
        public static bool BonusAppActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["BonusAppActive"]);
            set => SettingProvider.Items["BonusAppActive"] = value.ToString();
        }

        public static bool BookingActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["BookingActive"]);
            set
            {
                SettingProvider.Items["BookingActive"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static bool TriggersActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["TriggersActive"]);
            set
            {
                SettingProvider.Items["TriggersActive"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static bool PartnersActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["PartnersActive"]);
            set => SettingProvider.Items["PartnersActive"] = value.ToString();
        }

        public static bool VkChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["VkChannelActive"]);
            set
            {
                SettingProvider.Items["VkChannelActive"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static bool InstagramChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["InstagramChannelActive"]);
            set
            {
                SettingProvider.Items["InstagramChannelActive"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static bool FacebookChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["FacebookChannelActive"]);
            set => SettingProvider.Items["FacebookChannelActive"] = value.ToString();
        }

        public static bool YandexChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["YandexChannelActive"]);
            set => SettingProvider.Items["YandexChannelActive"] = value.ToString();
        }

        public static bool AvitoChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["AvitoChannelActive"]);
            set => SettingProvider.Items["AvitoChannelActive"] = value.ToString();
        }

        public static bool GoogleChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["GoogleChannelActive"]);
            set => SettingProvider.Items["GoogleChannelActive"] = value.ToString();
        }

        public static bool ResellerChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["ResellerChannelActive"]);
            set => SettingProvider.Items["ResellerChannelActive"] = value.ToString();
        }

        public static bool OkChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["OkChannelActive"]);
            set
            {
                SettingProvider.Items["OkChannelActive"] = value.ToString();
                JobActivationManager.SettingUpdated();
            }
        }

        public static bool TelegramChannelActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["TelegramChannelActive"]);
            set => SettingProvider.Items["TelegramChannelActive"] = value.ToString();
        }

        public static string AdminHomeForceRedirectUrl
        {
            get => SettingProvider.Items["AdminHomeForceRedirectUrl"];
            set => SettingProvider.Items["AdminHomeForceRedirectUrl"] = value;
        }

        public static string AdminStartPage
        {
            get 
            {
                var dashboard = SalesChannelService.GetByType(ESalesChannelType.Dashboard);
                return SettingProvider.Items["AdminStartPage"] ?? (dashboard != null && dashboard.Enabled ? "dashboard" : "orders"); 
            }
            set => SettingProvider.Items["AdminStartPage"] = value;
        }

        public static string StoreScreenShot
        {
            get => SettingProvider.Items["StoreScreenShot"];
            set => SettingProvider.Items["StoreScreenShot"] = value;
        }

        public static string StoreScreenShotMiddle
        {
            get => SettingProvider.Items["StoreScreenShotMiddle"];
            set => SettingProvider.Items["StoreScreenShotMiddle"] = value;
        }

        public static bool TrackProductChanges
        {
            get => Convert.ToBoolean(SettingProvider.Items["TrackProductChanges"]);
            set => SettingProvider.Items["TrackProductChanges"] = value.ToString();
        }

        public static long ImageQuality
        {
            get => Convert.ToInt64(SettingProvider.Items["ImageQuality"]);
            set => SettingProvider.Items["ImageQuality"] = value.ToString();
        }

        public static List<AdditionalPhone> AdditionalPhones
        {
            get
            {
                try
                {
                    var phones = SettingProvider.Items["AdditionalPhones"];
                    
                    return string.IsNullOrEmpty(phones)
                        ? new List<AdditionalPhone>()
                        : JsonConvert.DeserializeObject<List<AdditionalPhone>>(phones);
                }
                catch {}

                return new List<AdditionalPhone>();
            }
            set => SettingProvider.Items["AdditionalPhones"] = JsonConvert.SerializeObject(value ?? new List<AdditionalPhone>());
        }
    }
}