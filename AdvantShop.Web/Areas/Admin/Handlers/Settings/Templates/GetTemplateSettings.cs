using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Web.Admin.Handlers.Design;
using AdvantShop.Web.Admin.Models.Settings.Templates;

namespace AdvantShop.Web.Admin.Handlers.Settings.Templates
{
    public class GetTemplateSettings
    {
        private List<TemplateSetting> _templateSettings;

        public SettingsTemplateModel Execute()
        {
            var model = new SettingsTemplateModel();

            var currentTemplateSettings = TemplateSettingsProvider.GetTemplateSettingsBox();
            _templateSettings = currentTemplateSettings.Settings ?? new List<TemplateSetting>();

            #region Common

            var setting = TryGetTemplateSetting("MainPageMode");
            if (setting != null)
            {
                model.MainPageMode = setting.Value;
                model.MainPageModeOptions = GetTemplateOptions(setting.Options);
            }

            setting = TryGetTemplateSetting("MenuStyle");
            if (setting != null)
            {
                model.MenuStyle = setting.Value;
                model.MenuStyleOptions = GetTemplateOptions(setting.Options);
            }

            setting = TryGetTemplateSetting("SearchBlockLocation");
            if (setting != null)
            {
                model.SearchBlockLocation = setting.Value;
                model.SearchBlockLocationOptions = GetTemplateOptions(setting.Options);
            }

            setting = TryGetTemplateSetting("TopPanel");
            if (setting != null)
            {
                model.TopPanel = setting.Value;
                model.TopPanelOptions = GetTemplateOptions(setting.Options);
            }

            setting = TryGetTemplateSetting("Header");
            if (setting != null)
            {
                model.Header = setting.Value;
                model.HeaderOptions = GetTemplateOptions(setting.Options);
            }

            setting = TryGetTemplateSetting("TopMenu");
            if (setting != null)
            {
                model.TopMenu = setting.Value;
                model.TopMenuOptions = GetTemplateOptions(setting.Options);
            }

            model.RecentlyViewVisibility = TryGetTemplateSettingValue("RecentlyViewVisibility").TryParseBool();
            model.WishListVisibility = TryGetTemplateSettingValue("WishListVisibility").TryParseBool();

            model.IsStoreClosed = SettingsMain.IsStoreClosed;
            model.EnableInplace = SettingsMain.EnableInplace;
            model.DisplayToolBarBottom = SettingsDesign.DisplayToolBarBottom;
            model.DisplayCityInTopPanel = SettingsDesign.DisplayCityInTopPanel;
            model.DefaultCityIfNotAutodetect = SettingsDesign.DefaultCityIfNotAutodetect;
            model.ShowCopyright = SettingsDesign.ShowCopyright;
            model.CopyrightText = SettingsDesign.CopyrightText;

            model.AdditionalHeadMetaTag = SettingsSEO.CustomMetaString;
            model.ShowUserAgreementText = SettingsCheckout.IsShowUserAgreementTextValue;
            model.AgreementDefaultChecked = SettingsCheckout.AgreementDefaultChecked;
            model.UserAgreementText = SettingsCheckout.UserAgreementText;
            model.DisplayCityBubble = SettingsDesign.DisplayCityBubble;
            model.ShowCookiesPolicyMessage = SettingsNotifications.ShowCookiesPolicyMessage;
            model.CookiesPolicyMessage = SettingsNotifications.CookiesPolicyMessage;

            model.TopMenuVisibility = TryGetTemplateSettingValue("TopMenuVisibility").TryParseBool();

            #endregion

            #region Main page

            model.CarouselVisibility = TryGetTemplateSettingValue("CarouselVisibility").TryParseBool();
            model.CarouselAnimationSpeed = TryGetTemplateSettingValue("CarouselAnimationSpeed").TryParseInt();
            model.CarouselAnimationDelay = TryGetTemplateSettingValue("CarouselAnimationDelay").TryParseInt();

            model.MainPageProductsVisibility = TryGetTemplateSettingValue("MainPageProductsVisibility").TryParseBool();
            model.CountMainPageProductInSection = TryGetTemplateSettingValue("CountMainPageProductInSection").TryParseInt();
            model.CountMainPageProductInLine = TryGetTemplateSettingValue("CountMainPageProductInLine").TryParseInt();
            
            model.BrandCarouselVisibility = TryGetTemplateSettingValue("BrandCarouselVisibility").TryParseBool();
            model.NewsVisibility = TryGetTemplateSettingValue("NewsVisibility").TryParseBool();
            model.NewsSubscriptionVisibility = TryGetTemplateSettingValue("NewsSubscriptionVisibility").TryParseBool();
            model.CheckOrderVisibility = TryGetTemplateSettingValue("CheckOrderVisibility").TryParseBool();
            model.GiftSertificateVisibility = TryGetTemplateSettingValue("GiftSertificateVisibility").TryParseBool();

            model.MainPageCategoriesVisibility = TryGetTemplateSettingValue("MainPageCategoriesVisibility").TryParseBool();
            model.CountMainPageCategoriesInSection = TryGetTemplateSettingValue("CountMainPageCategoriesInSection").TryParseInt();
            model.CountMainPageCategoriesInLine = TryGetTemplateSettingValue("CountMainPageCategoriesInLine").TryParseInt();

            #endregion

            #region Catalog

            model.CountCategoriesInLine = TryGetTemplateSettingValue("CountCategoriesInLine").TryParseInt();
            model.CountCatalogProductInLine = TryGetTemplateSettingValue("CountCatalogProductInLine").TryParseInt();

            model.ShowQuickView = SettingsCatalog.ShowQuickView;
            model.ProductsPerPage = SettingsCatalog.ProductsPerPage;
            model.ShowProductsCount = SettingsCatalog.ShowProductsCount;
            model.DisplayCategoriesInBottomMenu = SettingsCatalog.DisplayCategoriesInBottomMenu;
            model.ShowProductArtNo = SettingsCatalog.ShowProductArtNo;
            model.EnableProductRating = SettingsCatalog.EnableProductRating;
            model.EnableCompareProducts = SettingsCatalog.EnableCompareProducts;
            model.EnablePhotoPreviews = SettingsCatalog.EnablePhotoPreviews;
            model.ShowCountPhoto = SettingsCatalog.ShowCountPhoto;
            model.ShowOnlyAvalible = SettingsCatalog.ShowOnlyAvalible;
            model.MoveNotAvaliableToEnd = SettingsCatalog.MoveNotAvaliableToEnd;
            model.ShowNotAvaliableLable = SettingsCatalog.ShowNotAvaliableLable;

            model.FilterVisibility = SettingsDesign.FilterVisibility;
            model.ShowPriceFilter = SettingsCatalog.ShowPriceFilter;
            model.ShowProducerFilter = SettingsCatalog.ShowProducerFilter;
            model.ShowSizeFilter = SettingsCatalog.ShowSizeFilter;
            model.ShowPropertiesFilterInProductList = SettingsCatalog.ShowPropertiesFilterInProductList;
            model.ExcludingFilters = SettingsCatalog.ExcludingFilters;
            model.ShowColorFilter = SettingsCatalog.ShowColorFilter;
            model.SizesHeader = SettingsCatalog.SizesHeader;
            model.ColorsHeader = SettingsCatalog.ColorsHeader;
            model.ColorsViewMode = SettingsCatalog.ColorsViewMode;

            model.ColorsViewModes = new List<SelectListItem>();

            foreach (ColorsViewMode item in Enum.GetValues(typeof(ColorsViewMode)))
            {
                model.ColorsViewModes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            model.ColorIconWidthCatalog = SettingsPictureSize.ColorIconWidthCatalog;
            model.ColorIconHeightCatalog = SettingsPictureSize.ColorIconHeightCatalog;
            model.ColorIconWidthDetails = SettingsPictureSize.ColorIconWidthDetails;
            model.ColorIconHeightDetails = SettingsPictureSize.ColorIconHeightDetails;
            model.ComplexFilter = SettingsCatalog.ComplexFilter;

            model.BuyButtonText = SettingsCatalog.BuyButtonText;
            model.DisplayBuyButton = SettingsCatalog.DisplayBuyButton;
            model.PreOrderButtonText = SettingsCatalog.PreOrderButtonText;
            model.DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton;

            model.DefaultCatalogView = SettingsCatalog.DefaultCatalogView;
            model.EnableCatalogViewChange = SettingsCatalog.EnabledCatalogViewChange;

            model.DefaultSearchView = SettingsCatalog.DefaultSearchView;
            model.EnableSearchViewChange = SettingsCatalog.EnabledSearchViewChange;

            model.DefaultViewList = new List<SelectListItem>();

            foreach (ProductViewMode item in Enum.GetValues(typeof(ProductViewMode)))
            {
                if (item != ProductViewMode.Single) {
                    model.DefaultViewList.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
                }
            }


            model.BigProductImageWidth = TryGetTemplateSettingValue("BigProductImageWidth").TryParseInt();
            model.BigProductImageHeight = TryGetTemplateSettingValue("BigProductImageHeight").TryParseInt();

            model.MiddleProductImageWidth = TryGetTemplateSettingValue("MiddleProductImageWidth").TryParseInt();
            model.MiddleProductImageHeight = TryGetTemplateSettingValue("MiddleProductImageHeight").TryParseInt();

            model.SmallProductImageWidth = TryGetTemplateSettingValue("SmallProductImageWidth").TryParseInt();
            model.SmallProductImageHeight = TryGetTemplateSettingValue("SmallProductImageHeight").TryParseInt();

            model.XSmallProductImageWidth = TryGetTemplateSettingValue("XSmallProductImageWidth").TryParseInt();
            model.XSmallProductImageHeight = TryGetTemplateSettingValue("XSmallProductImageHeight").TryParseInt();

            model.BigCategoryImageWidth = TryGetTemplateSettingValue("BigCategoryImageWidth").TryParseInt();
            model.BigCategoryImageHeight = TryGetTemplateSettingValue("BigCategoryImageHeight").TryParseInt();

            model.SmallCategoryImageWidth = TryGetTemplateSettingValue("SmallCategoryImageWidth").TryParseInt();
            model.SmallCategoryImageHeight = TryGetTemplateSettingValue("SmallCategoryImageHeight").TryParseInt();

            #endregion

            #region Product

            model.DisplayWeight = SettingsCatalog.DisplayWeight;
            model.DisplayDimensions = SettingsCatalog.DisplayDimensions;
            model.ShowStockAvailability = SettingsCatalog.ShowStockAvailability;
            //model.CompressBigImage = SettingsCatalog.CompressBigImage;
            model.EnableZoom = SettingsDesign.EnableZoom;
            model.AllowReviews = SettingsCatalog.AllowReviews;
            model.ModerateReviews = SettingsCatalog.ModerateReviews;
            model.ReviewsVoiteOnlyRegisteredUsers = SettingsCatalog.ReviewsVoiteOnlyRegisteredUsers;
            model.DisplayReviewsImage = SettingsCatalog.DisplayReviewsImage;
            model.AllowReviewsImageUploading = SettingsCatalog.AllowReviewsImageUploading;
            model.ReviewImageWidth = SettingsPictureSize.ReviewImageWidth;
            model.ReviewImageHeight = SettingsPictureSize.ReviewImageHeight;
            
            model.ShowShippingsMethodsInDetails = SettingsDesign.ShowShippingsMethodsInDetails;
            model.ShowShippingsMethods = new List<SelectListItem>();
            foreach (SettingsDesign.eShowShippingsInDetails item in Enum.GetValues(typeof(SettingsDesign.eShowShippingsInDetails)))
            {
                model.ShowShippingsMethods.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            model.ShippingsMethodsInDetailsCount = SettingsDesign.ShippingsMethodsInDetailsCount;
            model.RelatedProductName = SettingsCatalog.RelatedProductName;
            model.AlternativeProductName = SettingsCatalog.AlternativeProductName;
            model.RelatedProductSourceType = SettingsDesign.RelatedProductSourceType;
            model.RelatedProductTypes = new List<SelectListItem>();
            foreach (SettingsDesign.eRelatedProductSourceType item in Enum.GetValues(typeof(SettingsDesign.eRelatedProductSourceType)))
            {
                model.RelatedProductTypes.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }

            model.RelatedProductsMaxCount = SettingsCatalog.RelatedProductsMaxCount;

            #endregion

            #region Checkout

            model.PaymentIconWidth = TryGetTemplateSettingValue("PaymentIconWidth");
            model.PaymentIconHeight = TryGetTemplateSettingValue("PaymentIconHeight");
            model.ShippingIconWidth = TryGetTemplateSettingValue("ShippingIconWidth");
            model.ShippingIconHeight = TryGetTemplateSettingValue("ShippingIconHeight");

            #endregion

            #region Brands

            model.BrandLogoWidth = TryGetTemplateSettingValue("BrandLogoWidth");
            model.BrandLogoHeight = TryGetTemplateSettingValue("BrandLogoHeight");
            model.BrandsPerPage = SettingsCatalog.BrandsPerPage;
            model.ShowCategoryTreeInBrand = SettingsCatalog.ShowCategoryTreeInBrand;
            model.ShowProductsInBrand = SettingsCatalog.ShowProductsInBrand;
            model.DefaultSortOrderProductInBrandOption = new List<SelectListItem>();
            foreach (ESortOrder item in Enum.GetValues(typeof(ESortOrder)))
            {
                model.DefaultSortOrderProductInBrandOption.Add(new SelectListItem() { Text = item.Localize(), Value = item.ToString() });
            }
            model.DefaultSortOrderProductInBrand = SettingsCatalog.DefaultSortOrderProductInBrand;

            #endregion

            #region News

            model.NewsImageWidth = TryGetTemplateSettingValue("NewsImageWidth");
            model.NewsImageHeight = TryGetTemplateSettingValue("NewsImageHeight");
            model.NewsPerPage = SettingsNews.NewsPerPage;
            model.NewsMainPageCount = SettingsNews.NewsMainPageCount;

            #endregion

            #region Other
            
            model.MainPageText = SettingsNews.MainPageText;

            model.OtherSettingsSections = new List<TemplateSettingSection>();

            var additionalSettings = _templateSettings.Where(x => x.IsAdditional).ToList();
            if (additionalSettings.Count > 0)
            {
                foreach (var additionalSetting in additionalSettings.GroupBy(x => x.SectionName))
                {
                    var name = additionalSetting.Key;

                    ETemplateSettingSection sectionType;
                    if (Enum.TryParse(additionalSetting.Key, out sectionType))
                        name = sectionType.Localize();

                    model.OtherSettingsSections.Add(new TemplateSettingSection()
                    {
                        Name = name,
                        Settings = additionalSettings.Where(x => x.SectionName == additionalSetting.Key).ToList()
                    });
                }
            }

            #endregion

            #region Css editor

            model.CssEditorText = new CssEditorHandler().GetFileContent();

            #endregion

            var hiddenSettings = TemplateSettingsProvider.GetHiddenSettings();
            model.HiddenSettings = hiddenSettings;

            return model;
        }

        #region Help methods

        private TemplateSetting TryGetTemplateSetting(string name)
        {
            return _templateSettings.Find(x => x.Name == name);
        }

        private string TryGetTemplateSettingValue(string name)
        {
            var setting = TryGetTemplateSetting(name);
            return setting != null  ? setting.Value : "";
        }

        private List<SelectListItem> GetTemplateOptions(List<TemplateOptionSetting> options)
        {
            return options != null
                ? options.Select(x => new SelectListItem() {Text = x.Title, Value = x.Value}).ToList()
                : new List<SelectListItem>();
        }

        #endregion
    }
}
