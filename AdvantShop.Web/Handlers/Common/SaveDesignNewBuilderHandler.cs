using System;
using System.Collections.Generic;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Services.Repository;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Customers;
using AdvantShop.Design;
using AdvantShop.Helpers;
using AdvantShop.Models.Common;
using AdvantShop.Track;
using AdvantShop.Trial;

namespace AdvantShop.Handlers.Common
{
    public class SaveDesignNewBuilderHandler
    {
        private DesignNewBuilderModel _model;

        public SaveDesignNewBuilderHandler() { }
        public SaveDesignNewBuilderHandler(DesignNewBuilderModel model)
        {
            _model = model;
        }

        public void Execute()
        {
            if (Demo.IsDemoEnabled && !CustomerContext.CurrentCustomer.IsAdmin)
            {
                CommonHelper.SetCookie("theme", _model.CurrentTheme);
                CommonHelper.SetCookie("background", _model.CurrentBackGround);
                CommonHelper.SetCookie("colorscheme", _model.CurrentColorScheme);
                CommonHelper.SetCookie("structure", _model.MainPageMode);

                return;
            }

            SettingsDesign.Theme = _model.CurrentTheme;
            SettingsDesign.Background = _model.CurrentBackGround;
            SettingsDesign.ColorScheme = _model.CurrentColorScheme;
            SettingsDesign.MainPageMode = (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), _model.MainPageMode);
            

            TrialService.TrackEvent(TrialEvents.ChangeColorScheme, _model.CurrentColorScheme);
            TrialService.TrackEvent(TrialEvents.ChangeBackGround, _model.CurrentBackGround);
            TrialService.TrackEvent(TrialEvents.ChangeTheme, _model.CurrentTheme);
            TrialService.TrackEvent(TrialEvents.ChangeMainPageMode, _model.MainPageMode);

            TrackService.TrackEvent(ETrackEvent.Trial_ChangeDesignTransformer);

            var mobileBrowserColorVariantsSelected = SettingsMobile.BrowserColorVariantsSelected.TryParseEnum(SettingsMobile.eBrowserColorVariants.ColorScheme);
            if (mobileBrowserColorVariantsSelected == SettingsMobile.eBrowserColorVariants.ColorScheme)
            {
                var curColorScheme = DesignService.GetCurrenDesign(eDesign.Color);
                SettingsMobile.BrowserColor = curColorScheme.Color;
            }

            #region Common

            SaveSetting("SearchBlockLocation", _model.SearchBlockLocation);
            SaveSetting("RecentlyViewVisibility", _model.RecentlyViewVisibility);
            SaveSetting("WishListVisibility", _model.WishListVisibility);
            SaveSetting("MenuStyle", _model.MenuStyle);

            SaveSetting("TopPanel", _model.TopPanel);
            SaveSetting("Header", _model.Header);
            SaveSetting("TopMenu", _model.TopMenu);
            SaveSetting("TopMenuVisibility", _model.TopMenuVisibility);


            SettingsMain.IsStoreClosed = _model.IsStoreClosed;
            SettingsMain.EnableInplace = _model.EnableInplace;
            SettingsDesign.DisplayToolBarBottom = _model.DisplayToolBarBottom;
            SettingsDesign.DisplayCityInTopPanel = _model.DisplayCityInTopPanel;
            //SettingsDesign.ShowCopyright = _model.ShowCopyright;
            //SettingsDesign.CopyrightText = _model.CopyrightText;

            SettingsSEO.CustomMetaString = _model.AdditionalHeadMetaTag;
            SettingsCheckout.IsShowUserAgreementTextValue = _model.ShowUserAgreementText;
            SettingsCheckout.AgreementDefaultChecked = _model.AgreementDefaultChecked;
            SettingsCheckout.UserAgreementText = _model.UserAgreementText;
            SettingsDesign.DisplayCityBubble = _model.DisplayCityBubble;
            SettingsNotifications.ShowCookiesPolicyMessage = _model.ShowCookiesPolicyMessage;
            SettingsNotifications.CookiesPolicyMessage = _model.CookiesPolicyMessage;

            var langChanged = SettingsMain.Language != _model.SiteLanguage;
            SettingsMain.Language = _model.SiteLanguage;

            if (langChanged)
            {
                CacheManager.Clean();
                LocalizationService.GenerateJsResourcesFile();
            }


            if (_model.AdditionalPhones != null && _model.AdditionalPhones.Count > 0)
            {
                var i = 0;
                var phones = new List<AdditionalPhone>();
                
                foreach (var phone in _model.AdditionalPhones)
                {
                    i++;

                    if (i != 1 && (string.IsNullOrWhiteSpace(phone.Phone) || string.IsNullOrWhiteSpace(phone.StandardPhone)) && phone.Type != EAdditionalPhoneType.Telegram)
                        continue;

                    phone.StandardPhone = StringHelper.ConvertToMobileStandardPhone(phone.StandardPhone);

                    if (i == 1)
                    {
                        SettingsMain.Phone = phone.Phone;
                        SettingsMain.MobilePhone = phone.StandardPhone;
                        SettingsMain.PhoneDescription = phone.Description;
                    }
                    else
                    {
                        phones.Add(phone);
                    }
                }
                SettingsMain.AdditionalPhones = phones;
            }

            #endregion

            #region Main page

            SaveSetting("CarouselVisibility", _model.CarouselVisibility);
            SaveSetting("CarouselAnimationSpeed", _model.CarouselAnimationSpeed);
            SaveSetting("CarouselAnimationDelay", _model.CarouselAnimationDelay);

            SaveSetting("MainPageProductsVisibility", _model.MainPageProductsVisibility);
            SaveSetting("CountMainPageProductInSection", _model.CountMainPageProductInSection);
            SaveSetting("CountMainPageProductInLine", _model.CountMainPageProductInLine);

            SaveSetting("NewsVisibility", _model.NewsVisibility);
            SaveSetting("NewsSubscriptionVisibility", _model.NewsSubscriptionVisibility);
            SaveSetting("CheckOrderVisibility", _model.CheckOrderVisibility);
            SaveSetting("GiftSertificateVisibility", _model.GiftSertificateVisibility);
            SaveSetting("BrandCarouselVisibility", _model.BrandCarouselVisibility);

            SaveSetting("MainPageCategoriesVisibility", _model.MainPageCategoriesVisibility);
            SaveSetting("CountMainPageCategoriesInSection", _model.CountMainPageCategoriesInSection);
            SaveSetting("CountMainPageCategoriesInLine", _model.CountMainPageCategoriesInLine);

            #endregion

            #region Catalog

            SaveSetting("CountCategoriesInLine", _model.CountCategoriesInLine);

            SettingsCatalog.ShowQuickView = _model.ShowQuickView;
            SettingsCatalog.ProductsPerPage = _model.ProductsPerPage;
            SaveSetting("CountCatalogProductInLine", _model.CountCatalogProductInLine);
            SettingsCatalog.ShowProductsCount = _model.ShowProductsCount;
            SettingsCatalog.DisplayCategoriesInBottomMenu = _model.DisplayCategoriesInBottomMenu;
            SettingsCatalog.ShowProductArtNo = _model.ShowProductArtNo;
            SettingsCatalog.EnableProductRating = _model.EnableProductRating;
            SettingsCatalog.EnableCompareProducts = _model.EnableCompareProducts;
            SettingsCatalog.EnablePhotoPreviews = _model.EnablePhotoPreviews;
            SettingsCatalog.ShowCountPhoto = _model.ShowCountPhoto;
            SettingsCatalog.ShowOnlyAvalible = _model.ShowOnlyAvalible;
            SettingsCatalog.MoveNotAvaliableToEnd = _model.MoveNotAvaliableToEnd;
            SettingsCatalog.ShowNotAvaliableLable = _model.ShowNotAvaliableLable;

            SettingsDesign.FilterVisibility = _model.FilterVisibility;
            SettingsCatalog.ShowPriceFilter = _model.ShowPriceFilter;
            SettingsCatalog.ShowProducerFilter = _model.ShowProducerFilter;
            SettingsCatalog.ShowSizeFilter = _model.ShowSizeFilter;
            SettingsCatalog.ShowColorFilter = _model.ShowColorFilter;
            SettingsCatalog.ShowPropertiesFilterInProductList = _model.ShowPropertiesFilterInProductList;
            SettingsCatalog.ExcludingFilters = _model.ExcludingFilters;

            SettingsCatalog.SizesHeader = _model.SizesHeader;
            SettingsCatalog.ColorsHeader = _model.ColorsHeader;
            SettingsCatalog.ColorsViewMode = _model.ColorsViewMode.TryParseEnum<ColorsViewMode>();
            SettingsPictureSize.ColorIconWidthCatalog = _model.ColorIconWidthCatalog;
            SettingsPictureSize.ColorIconHeightCatalog = _model.ColorIconHeightCatalog;
            SettingsPictureSize.ColorIconWidthDetails = _model.ColorIconWidthDetails;
            SettingsPictureSize.ColorIconHeightDetails = _model.ColorIconHeightDetails;
            SettingsCatalog.ComplexFilter = _model.ComplexFilter;

            SettingsCatalog.BuyButtonText = _model.BuyButtonText;
            SettingsCatalog.DisplayBuyButton = _model.DisplayBuyButton;
            SettingsCatalog.PreOrderButtonText = _model.PreOrderButtonText;
            SettingsCatalog.DisplayPreOrderButton = _model.DisplayPreOrderButton;

            SettingsCatalog.DefaultCatalogView = _model.DefaultCatalogView.TryParseEnum<ProductViewMode>();
            SettingsCatalog.EnabledCatalogViewChange = _model.EnableCatalogViewChange;
            SettingsCatalog.DefaultSearchView = _model.DefaultSearchView.TryParseEnum<ProductViewMode>();
            SettingsCatalog.EnabledSearchViewChange = _model.EnableSearchViewChange;

            SaveSetting("BigProductImageWidth", _model.BigProductImageWidth);
            SaveSetting("BigProductImageHeight", _model.BigProductImageHeight);
            SaveSetting("MiddleProductImageWidth", _model.MiddleProductImageWidth);
            SaveSetting("MiddleProductImageHeight", _model.MiddleProductImageHeight);
            SaveSetting("SmallProductImageWidth", _model.SmallProductImageWidth);
            SaveSetting("SmallProductImageHeight", _model.SmallProductImageHeight);
            SaveSetting("XSmallProductImageWidth", _model.XSmallProductImageWidth);
            SaveSetting("XSmallProductImageHeight", _model.XSmallProductImageHeight);
            SaveSetting("BigCategoryImageWidth", _model.BigCategoryImageWidth);
            SaveSetting("BigCategoryImageHeight", _model.BigCategoryImageHeight);
            SaveSetting("SmallCategoryImageWidth", _model.SmallCategoryImageWidth);
            SaveSetting("SmallCategoryImageHeight", _model.SmallCategoryImageHeight);

            #endregion

            #region Product

            SettingsCatalog.DisplayWeight = _model.DisplayWeight;
            SettingsCatalog.DisplayDimensions = _model.DisplayDimensions;
            SettingsCatalog.ShowStockAvailability = _model.ShowStockAvailability;

            //SettingsCatalog.CompressBigImage = _model.CompressBigImage;
            SettingsDesign.EnableZoom = _model.EnableZoom;

            SettingsCatalog.AllowReviews = _model.AllowReviews;
            SettingsCatalog.ModerateReviews = _model.ModerateReviews;
            SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers = _model.ReviewsVoiteOnlyRegisteredUsers;
            SettingsCatalog.DisplayReviewsImage = _model.DisplayReviewsImage;
            SettingsCatalog.AllowReviewsImageUploading = _model.AllowReviewsImageUploading;
            SettingsPictureSize.ReviewImageWidth = _model.ReviewImageWidth;
            SettingsPictureSize.ReviewImageHeight = _model.ReviewImageHeight;

            SettingsDesign.ShowShippingsMethodsInDetails = _model.ShowShippingsMethodsInDetails.TryParseEnum<SettingsDesign.eShowShippingsInDetails>();
            SettingsDesign.ShippingsMethodsInDetailsCount = _model.ShippingsMethodsInDetailsCount;

            SettingsCatalog.RelatedProductName = _model.RelatedProductName;
            SettingsCatalog.AlternativeProductName = _model.AlternativeProductName;
            SettingsDesign.RelatedProductSourceType = _model.RelatedProductSourceType.TryParseEnum<SettingsDesign.eRelatedProductSourceType>();
            SettingsCatalog.RelatedProductsMaxCount = _model.RelatedProductsMaxCount;

            #endregion

            #region Checkout

            SaveSetting("PaymentIconWidth", _model.PaymentIconWidth);
            SaveSetting("PaymentIconHeight", _model.PaymentIconHeight);
            SaveSetting("ShippingIconWidth", _model.ShippingIconWidth);
            SaveSetting("ShippingIconHeight", _model.ShippingIconHeight);

            #endregion

            #region Brands

            SaveSetting("BrandLogoWidth", _model.BrandLogoWidth);
            SaveSetting("BrandLogoHeight", _model.BrandLogoHeight);
            SettingsCatalog.BrandsPerPage = _model.BrandsPerPage;
            SettingsCatalog.ShowCategoryTreeInBrand = _model.ShowCategoryTreeInBrand;
            SettingsCatalog.ShowProductsInBrand = _model.ShowProductsInBrand;

            #endregion

            #region News


            SaveSetting("NewsImageWidth", _model.NewsImageWidth);
            SaveSetting("NewsImageHeight", _model.NewsImageHeight);
            SettingsNews.MainPageText = _model.NewsMainPageText;
            SettingsNews.NewsPerPage = _model.NewsPerPage;
            SettingsNews.NewsMainPageCount = _model.NewsMainPageCount;

            #endregion

            #region CssEditor
            FilePath.FoldersHelper.SaveCSS(_model.CssEditorText, FilePath.CssType.extra);
            #endregion

            #region Other

            if (_model.OtherSettingsSections != null)
            {
                foreach (var section in _model.OtherSettingsSections)
                {
                    if (section.Settings == null)
                        continue;

                    foreach (var setting in section.Settings)
                    {
                        if (setting.Type == ETemplateSettingType.StaticBlockCheckbox)
                        {
                            var block = StaticBlockService.GetPagePartByKey(setting.Name);
                            if (block != null)
                            {
                                block.Enabled = setting.Value.TryParseBool();
                                StaticBlockService.UpdatePagePart(block);
                            }
                        }
                        else
                        {
                            SaveSetting(setting.Name, setting.Value);
                        }
                    }
                }
            }

            #endregion

            CacheManager.Clean();

            new ScreenshotService().UpdateStoreScreenShotInBackground();
        }

        #region Help methods

        private void SaveSetting(string name, string value)
        {
            if (value == null)
                return;

            TemplateSettingsProvider.SetSettingValue(name, value);
        }

        private void SaveSetting(string name, bool value)
        {
            TemplateSettingsProvider.SetSettingValue(name, value.ToString());
        }

        private void SaveSetting(string name, int value)
        {
            TemplateSettingsProvider.SetSettingValue(name, value.ToString());
        }

        #endregion
    }
}