//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Web;
using AdvantShop.Design;
using AdvantShop.Core.UrlRewriter;
using System.IO;
using System.Web.Hosting;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Caching;
using AdvantShop.Core;
using AdvantShop.Core.Services.Screenshot;

namespace AdvantShop.Configuration
{
    public class SettingsDesign
    {
        private const string TemplateContextKey = "CurrentTemplate";
        private const string PreviewTemplateContextKey = "PreviewCurrentTemplate";

        private const string IsMobileTemplateContextKey = "IsMobileCurrentTemplate";
        private const string IsSocialTemplateContextKey = "IsSocialCurrentTemplate";

        public enum eSearchBlockLocation
        {
            None = 0,
            TopMenu = 1,
            CatalogMenu = 2
        }

        public enum eMenuStyle
        {
            Classic,
            Modern,
            Accordion,
            Treeview,
        }


        public enum eMainPageMode
        {
            Default = 0,
            TwoColumns = 1,
            ThreeColumns = 2
        }


        public enum eShowShippingsInDetails
        {
            [Localize("Core.Settings.SettingsCatalog.ShowShippingsInDetails.Never")]
            Never = 0,
            [Localize("Core.Settings.SettingsCatalog.ShowShippingsInDetails.ByClick")]
            ByClick = 1,
            [Localize("Core.Settings.SettingsCatalog.ShowShippingsInDetails.Always")]
            Always = 2
        }

        public enum eRelatedProductSourceType
        {
            [Localize("Core.Settings.SettingsCatalog.RelatedProductSourceType.Default")]
            Default = 0,
            [Localize("Core.Settings.SettingsCatalog.RelatedProductSourceType.FromCategory")]
            FromCategory = 1,
            [Localize("Core.Settings.SettingsCatalog.RelatedProductSourceType.FromModule")]
            FromModule = 2
        }

        #region Design settings in db

        public static string Template
        {
            get
            {
                if (AppServiceStartAction.state != PingDbState.NoError)
                    return TemplateService.DefaultTemplateId;

                if (HttpContext.Current != null)
                {
                    var currentTemplate = HttpContext.Current.Items[TemplateContextKey] as string;
                    if (currentTemplate != null)
                        return currentTemplate;

                    var tpl = Customers.CustomerContext.CurrentCustomer.IsAdmin && PreviewTemplate != null ? PreviewTemplate : SettingProvider.Items["Template"];

                    if (tpl != TemplateService.DefaultTemplateId && !Directory.Exists(HostingEnvironment.MapPath("~/templates/" + tpl)))
                    {
                        tpl = TemplateService.DefaultTemplateId;
                    }

                    HttpContext.Current.Items[TemplateContextKey] = tpl;
                    return tpl;
                }
                else
                {
                    var tpl = SettingProvider.Items["Template"];
                    if (tpl != TemplateService.DefaultTemplateId && !Directory.Exists(HostingEnvironment.MapPath("~/templates/" + tpl)))
                    {
                        tpl = TemplateService.DefaultTemplateId;
                    }
                    return tpl;
                }
            }
            private set => HttpContext.Current.Items[TemplateContextKey] = value;
        }

        public static string TemplateInDb
        {
            get
            {
                if (SettingProvider.Items["Template"] != TemplateService.DefaultTemplateId && !Directory.Exists(HostingEnvironment.MapPath("~/templates/" + SettingProvider.Items["Template"])))
                {
                    SettingProvider.Items["Template"] = TemplateService.DefaultTemplateId;
                }

                return SettingProvider.Items["Template"];
            }
        }



        public static string PreviewTemplate
        {
            get
            {
                var tpl = SettingProvider.Items["PreviewTemplate"];
                if (tpl != null && tpl != TemplateService.DefaultTemplateId && !Directory.Exists(HostingEnvironment.MapPath("~/templates/" + tpl)))
                {
                    tpl = null;
                    SettingProvider.Items["PreviewTemplate"] = null;
                }
                return tpl;
            }
            set
            {
                SettingProvider.Items["PreviewTemplate"] = value;

                if (value != null)
                    TemplateSettingsProvider.SetTemplateSettings(value);
            }
        }


        public static void ChangeTemplate(string template)
        {
            PreviewTemplate = null;

            SettingProvider.Items["Template"] = template;
            Template = template;

            TemplateSettingsProvider.SetTemplateSettings(template);
            CacheManager.Clean();

            var screenService = new ScreenshotService();

            var screen = template ==  TemplateService.DefaultTemplateId
                ? "../images/design/preview.jpg"
                : string.Format("../Templates/{0}/preview.jpg", template);

            screenService.SetStoreScreenShots(screen);
            screenService.UpdateStoreScreenShotInBackground();
        }

        public static bool ShoppingCartVisibility
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowShoppingCartOnMainPage"]);
            set => SettingProvider.Items["ShowShoppingCartOnMainPage"] = value.ToString();
        }

        public static bool EnableZoom
        {
            get => Convert.ToBoolean(SettingProvider.Items["EnabledZoom"]);
            set => SettingProvider.Items["EnabledZoom"] = value.ToString();
        }

        public static bool DisplayToolBarBottom
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayToolBarBottom"]);
            set => SettingProvider.Items["DisplayToolBarBottom"] = value.ToString();
        }

        public static eShowShippingsInDetails ShowShippingsMethodsInDetails
        {
            get => (eShowShippingsInDetails)int.Parse(SettingProvider.Items["ShowShippingsMethodsInDetails"]);
            set => SettingProvider.Items["ShowShippingsMethodsInDetails"] = ((int)value).ToString();
        }

        public static int ShippingsMethodsInDetailsCount
        {
            get => int.Parse(SettingProvider.Items["ShippingsMethodsInDetailsCount"]);
            set => SettingProvider.Items["ShippingsMethodsInDetailsCount"] = value.ToString();
        }

        /// <summary>
        /// Определять город автоматически
        /// </summary>
        public static bool DisplayCityInTopPanel
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayCityInTopPanel"]);
            set => SettingProvider.Items["DisplayCityInTopPanel"] = value.ToString();
        }

        public static string DefaultCityIfNotAutodetect
        {
            get => SettingProvider.Items["DefaultCityIfNotAutodetect"];
            set => SettingProvider.Items["DefaultCityIfNotAutodetect"] = value;
        }

        public static bool DisplayCityBubble
        {
            get => Convert.ToBoolean(SettingProvider.Items["DisplayCityBubble"]);
            set => SettingProvider.Items["DisplayCityBubble"] = value.ToString();
        }

        public static eRelatedProductSourceType RelatedProductSourceType
        {
            get => (eRelatedProductSourceType)int.Parse(SettingProvider.Items["RelatedProductSourceType"]);
            set => SettingProvider.Items["RelatedProductSourceType"] = ((int)value).ToString();
        }

        public static string MainPageListName
        {
            get => SettingProvider.Items["MainPageListName"];
            set => SettingProvider.Items["MainPageListName"] = value;
        }

        public static bool ShowCopyright
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowCopyright"]);
            set => SettingProvider.Items["ShowCopyright"] = value.ToString();
        }

        public static string CopyrightText
        {
            get => SettingProvider.Items["CopyrightText"];
            set => SettingProvider.Items["CopyrightText"] = value;
        }

        public static string CopyrightTextFormatted =>
            !string.IsNullOrEmpty(CopyrightText)
                ? CopyrightText.Replace("#CURRENT_YEAR#", DateTime.Now.Year.ToString())
                : "";

        public static bool ShowClientId
        {
            get => Convert.ToBoolean(SettingProvider.Items["ShowClientId"]);
            set => SettingProvider.Items["ShowClientId"] = value.ToString();
        }
        
        public static bool FilterVisibility
        {
            get => Convert.ToBoolean(SettingProvider.Items["FilterVisibility"]);
            set => SettingProvider.Items["FilterVisibility"] = value.ToString();
        }

        public static string AdminAreaColorScheme
        {
            get
            {
                var scheme = SettingProvider.Items["AdminAreaColorScheme"];

                if (string.IsNullOrEmpty(scheme))
                    scheme = SettingProvider.Items["AdminAreaColorScheme"] = "default";

                return scheme;
            }
            set => SettingProvider.Items["AdminAreaColorScheme"] = value;
        }

        #endregion

        #region Mobile template 

        public static bool IsMobileTemplate
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var currentTemplate = HttpContext.Current.Items[IsMobileTemplateContextKey];
                    if (currentTemplate != null)
                        return Convert.ToBoolean(currentTemplate);
                }
                return false;
            }
            set => HttpContext.Current.Items[IsMobileTemplateContextKey] = value.ToString();
        }
        
        public static string MobileTemplate
        {
            get => SettingProvider.Items["MobileTemplate"];
            set => SettingProvider.Items["MobileTemplate"] = value;
        }

        #endregion

        #region Current template settings in template.config

        public static bool IsSocialTemplate
        {
            get
            {
                if (HttpContext.Current != null)
                {
                    var currentTemplate = HttpContext.Current.Items[IsSocialTemplateContextKey];
                    if (currentTemplate != null)
                        return Convert.ToBoolean(currentTemplate);

                    var socialType = UrlService.IsSocialUrl(HttpContext.Current.Request.Url.AbsoluteUri);
                    var isSocial = socialType != UrlService.ESocialType.none;

                    if (isSocial &&
                        ((socialType == UrlService.ESocialType.vk && !IsVkTemplateActive) ||
                         (socialType == UrlService.ESocialType.fb && !IsFbTemplateActive)))
                    {
                        isSocial = false;
                    }

                    HttpContext.Current.Items[IsSocialTemplateContextKey] = isSocial.ToString();

                    return isSocial;
                }
                return false;
            }
            set => HttpContext.Current.Items[IsSocialTemplateContextKey] = value.ToString();
        }

        public static bool IsVkTemplateActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsVkTemplateActive"]);
            set => SettingProvider.Items["IsVkTemplateActive"] = value.ToString();
        }

        public static bool IsFbTemplateActive
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsFbTemplateActive"]);
            set => SettingProvider.Items["IsFbTemplateActive"] = value.ToString();
        }

        public static string Theme
        {
            get => TemplateSettingsProvider.Items["Theme"];
            set => TemplateSettingsProvider.Items["Theme"] = value;
        }

        public static string ColorScheme
        {
            get => TemplateSettingsProvider.Items["ColorScheme"];
            set => TemplateSettingsProvider.Items["ColorScheme"] = value;
        }

        public static string Background
        {
            get => TemplateSettingsProvider.Items["Background"];
            set => TemplateSettingsProvider.Items["Background"] = value;
        }


        public static string ThemeInDb
        {
            get => TemplateSettingsProvider.GetSettingValue("Theme", TemplateInDb);
            set => TemplateSettingsProvider.SetSettingValue("Theme", value, TemplateInDb);
        }

        public static string ColorSchemeInDb
        {
            get => TemplateSettingsProvider.GetSettingValue("ColorScheme", TemplateInDb);
            set => TemplateSettingsProvider.SetSettingValue("ColorScheme", value, TemplateInDb);
        }

        public static string BackgroundInDb
        {
            get => TemplateSettingsProvider.GetSettingValue("Background", TemplateInDb);
            set => TemplateSettingsProvider.SetSettingValue("Background", value, TemplateInDb);
        }


        public static eSearchBlockLocation SearchBlockLocation
        {
            get
            {
                var result = eSearchBlockLocation.CatalogMenu;
                Enum.TryParse<eSearchBlockLocation>(TemplateSettingsProvider.Items["SearchBlockLocation"], out result);
                return result;
            }
            set => TemplateSettingsProvider.Items["SearchBlockLocation"] = value.ToString();
        }

        public static eMainPageMode MainPageMode
        {
            get
            {
                var mainPageMode = eMainPageMode.Default;
                Enum.TryParse<eMainPageMode>(TemplateSettingsProvider.Items["MainPageMode"], out mainPageMode);
                return mainPageMode;
            }
            set => TemplateSettingsProvider.Items["MainPageMode"] = value.ToString();
        }

        public static eMenuStyle MenuStyle
        {
            get
            {
                var mainPageMode = eMenuStyle.Modern;
                Enum.TryParse<eMenuStyle>(TemplateSettingsProvider.Items["MenuStyle"], out mainPageMode);
                return mainPageMode;
            }
            set => TemplateSettingsProvider.Items["MenuStyle"] = value.ToString();
        }


        public static string CarouselAnimation
        {
            get => TemplateSettingsProvider.Items["CarouselAnimation"];
            set => TemplateSettingsProvider.Items["CarouselAnimation"] = value;
        }

        public static int CarouselAnimationSpeed
        {
            get
            {
                int intTempResult = -1;
                Int32.TryParse(TemplateSettingsProvider.Items["CarouselAnimationSpeed"], out intTempResult);
                return intTempResult;
            }
            set => TemplateSettingsProvider.Items["CarouselAnimationSpeed"] = value.ToString();
        }

        public static int CarouselAnimationDelay
        {
            get
            {
                int intTempResult = -1;
                Int32.TryParse(TemplateSettingsProvider.Items["CarouselAnimationDelay"], out intTempResult);
                return intTempResult;
            }
            set => TemplateSettingsProvider.Items["CarouselAnimationDelay"] = value.ToString();
        }

        public const int CarouselOneColumnWidth = 1160;
        public const int CarouselOneColumnHeight = 400;

        public const int CarouselTwoColumnWidth = 865;
        public const int CarouselTwoColumnHeight = 400;

        public const int CarouselMobileWidth = 360;
        public const int CarouselMobileHeight = 170;

        /// <summary>
        /// ShowSeeProductOnMainPage
        /// </summary>
        public static bool RecentlyViewVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["RecentlyViewVisibility"]);
            set => TemplateSettingsProvider.Items["RecentlyViewVisibility"] = value.ToString();
        }

        /// <summary>
        /// ShowNewsOnMainPage
        /// </summary>
        public static bool NewsVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["NewsVisibility"]);
            set => TemplateSettingsProvider.Items["NewsVisibility"] = value.ToString();
        }

        /// <summary>
        /// ShowNewsSubscriptionOnMainPage
        /// </summary>
        public static bool NewsSubscriptionVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["NewsSubscriptionVisibility"]);
            set => TemplateSettingsProvider.Items["NewsSubscriptionVisibility"] = value.ToString();
        }

        /// <summary>
        /// ShowStatusCommentOnMainPage
        /// </summary>
        public static bool CheckOrderVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["CheckOrderVisibility"]);
            set => TemplateSettingsProvider.Items["CheckOrderVisibility"] = value.ToString();
        }

        /// <summary>
        /// ShowMainPageProductsOnMainPage
        /// </summary>
        public static bool MainPageProductsVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["MainPageProductsVisibility"]);
            set => TemplateSettingsProvider.Items["MainPageProductsVisibility"] = value.ToString();
        }

        /// <summary>
        /// GiftSertificateBlock
        /// </summary>
        public static bool GiftSertificateVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["GiftSertificateVisibility"]);
            set => TemplateSettingsProvider.Items["GiftSertificateVisibility"] = value.ToString();
        }

        /// <summary>
        /// WishList
        /// </summary>
        public static bool WishListVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["WishListVisibility"]);
            set => TemplateSettingsProvider.Items["WishListVisibility"] = value.ToString();
        }

        /// <summary>
        /// CarouseltVisibility
        /// </summary>
        public static bool CarouselVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["CarouselVisibility"]);
            set => TemplateSettingsProvider.Items["CarouselVisibility"] = value.ToString();
        }

        public static int CountMainPageProductInLine
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["CountMainPageProductInLine"]);
            set => TemplateSettingsProvider.Items["CountMainPageProductInLine"] = value.ToString();
        }

        public static int CountMainPageProductInSection
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["CountMainPageProductInSection"]);
            set => TemplateSettingsProvider.Items["CountMainPageProductInSection"] = value.ToString();
        }

        public static int CountCatalogProductInLine
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["CountCatalogProductInLine"]);
            set => TemplateSettingsProvider.Items["CountCatalogProductInLine"] = value.ToString();
        }

        public static int CountCategoriesInLine
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["CountCategoriesInLine"]);
            set => TemplateSettingsProvider.Items["CountCategoriesInLine"] = value.ToString();
        }

        public static bool BrandCarouselVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["BrandCarouselVisibility"]);
            set => TemplateSettingsProvider.Items["BrandCarouselVisibility"] = value.ToString();
        }


        public static string DefaultLogo
        {
            get => TemplateSettingsProvider.GetSettingValue("DefaultLogo");
            set => TemplateSettingsProvider.SetSettingValue("DefaultLogo", value);
        }

        public static string DefaultSlides
        {
            get => TemplateSettingsProvider.GetSettingValue("DefaultSlides");
            set => TemplateSettingsProvider.SetSettingValue("DefaultSlides", value);
        }

        public static bool IsDefaultSlides
        {
            get => Convert.ToBoolean(SettingProvider.Items["IsDefaultSlides"]);
            set => SettingProvider.Items["IsDefaultSlides"] = value.ToString();
        }

        public static bool MainPageCategoriesVisibility
        {
            get => Convert.ToBoolean(TemplateSettingsProvider.Items["MainPageCategoriesVisibility"]);
            set => TemplateSettingsProvider.Items["MainPageCategoriesVisibility"] = value.ToString();
        }

        public static int CountMainPageCategoriesInLine
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["CountMainPageCategoriesInLine"]);
            set => TemplateSettingsProvider.Items["CountMainPageCategoriesInLine"] = value.ToString();
        }

        public static int CountMainPageCategoriesInSection
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["CountMainPageCategoriesInSection"]);
            set => TemplateSettingsProvider.Items["CountMainPageCategoriesInSection"] = value.ToString();
        }

        public static bool TopMenuVisibility
        {
            get => Convert.ToBoolean(SettingProvider.Items["TopMenu"]);
            set => TemplateSettingsProvider.Items["TopMenu"] = value.ToString();
        }

        public static int LogoImageWidth
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["LogoImageWidth"]);
            set => TemplateSettingsProvider.Items["LogoImageWidth"] = value.ToString();
        }
        public static int LogoImageHeight
        {
            get => Convert.ToInt32(TemplateSettingsProvider.Items["LogoImageHeight"]);
            set => TemplateSettingsProvider.Items["LogoImageHeight"] = value.ToString();
        }
        #endregion;
    }
}