using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Handlers.Design;
using AdvantShop.Web.Admin.Models.Settings.Templates;

namespace AdvantShop.Web.Admin.Handlers.Settings.Templates
{
    public class SaveTemplateSettings
    {
        private readonly SettingsTemplateModel _model;

        public SaveTemplateSettings(SettingsTemplateModel model)
        {
            _model = model;
        }

        public bool Execute()
        {
            #region Common

            SaveSetting("MainPageMode", _model.MainPageMode);
            SaveSetting("MenuStyle", _model.MenuStyle);
            SaveSetting("SearchBlockLocation", _model.SearchBlockLocation);
            SaveSetting("RecentlyViewVisibility", _model.RecentlyViewVisibility);
            SaveSetting("WishListVisibility", _model.WishListVisibility);

            SaveSetting("TopPanel", _model.TopPanel);
            SaveSetting("Header", _model.Header);
            SaveSetting("TopMenu", _model.TopMenu);
            SaveSetting("TopMenuVisibility", _model.TopMenuVisibility);

            SettingsMain.IsStoreClosed = _model.IsStoreClosed;
            SettingsMain.EnableInplace = _model.EnableInplace;
            SettingsDesign.DisplayToolBarBottom = _model.DisplayToolBarBottom;
            SettingsDesign.DisplayCityInTopPanel = _model.DisplayCityInTopPanel;
            SettingsDesign.DefaultCityIfNotAutodetect = _model.DefaultCityIfNotAutodetect;
            SettingsDesign.ShowCopyright = _model.ShowCopyright;
            SettingsDesign.CopyrightText = _model.CopyrightText;

            SettingsSEO.CustomMetaString = _model.AdditionalHeadMetaTag;
            SettingsCheckout.IsShowUserAgreementTextValue = _model.ShowUserAgreementText;
            SettingsCheckout.AgreementDefaultChecked = _model.AgreementDefaultChecked;
            SettingsCheckout.UserAgreementText = _model.UserAgreementText;
            SettingsDesign.DisplayCityBubble = _model.DisplayCityBubble;
            SettingsNotifications.ShowCookiesPolicyMessage = _model.ShowCookiesPolicyMessage;
            SettingsNotifications.CookiesPolicyMessage = _model.CookiesPolicyMessage;

            #endregion

            #region Main page

            SaveSetting("CarouselVisibility", _model.CarouselVisibility);
            SaveSetting("CarouselAnimationSpeed", _model.CarouselAnimationSpeed);
            SaveSetting("CarouselAnimationDelay", _model.CarouselAnimationDelay);

            SaveSetting("MainPageProductsVisibility", _model.MainPageProductsVisibility);
            SaveSetting("CountMainPageProductInSection", _model.CountMainPageProductInSection);
            SaveSetting("CountMainPageProductInLine", _model.CountMainPageProductInLine);

            SaveSetting("BrandCarouselVisibility", _model.BrandCarouselVisibility);
            SaveSetting("NewsVisibility", _model.NewsVisibility);
            SaveSetting("NewsSubscriptionVisibility", _model.NewsSubscriptionVisibility);
            SaveSetting("CheckOrderVisibility", _model.CheckOrderVisibility);
            SaveSetting("GiftSertificateVisibility", _model.GiftSertificateVisibility);

            SaveSetting("MainPageCategoriesVisibility", _model.MainPageCategoriesVisibility);
            SaveSetting("CountMainPageCategoriesInSection", _model.CountMainPageCategoriesInSection);
            SaveSetting("CountMainPageCategoriesInLine", _model.CountMainPageCategoriesInLine);

            #endregion

            #region Catalog

            SaveSetting("CountCategoriesInLine", _model.CountCategoriesInLine);
            SaveSetting("CountCatalogProductInLine", _model.CountCatalogProductInLine);

            SettingsCatalog.ShowQuickView = _model.ShowQuickView;
            SettingsCatalog.ProductsPerPage = _model.ProductsPerPage;
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
            SettingsCatalog.ShowPropertiesFilterInProductList = _model.ShowPropertiesFilterInProductList;
            SettingsCatalog.ExcludingFilters = _model.ExcludingFilters;
            SettingsCatalog.ShowColorFilter = _model.ShowColorFilter;
            SettingsCatalog.SizesHeader = _model.SizesHeader;
            SettingsCatalog.ColorsHeader = _model.ColorsHeader;
            SettingsCatalog.ColorsViewMode = _model.ColorsViewMode;

            SettingsPictureSize.ColorIconWidthCatalog = _model.ColorIconWidthCatalog;
            SettingsPictureSize.ColorIconHeightCatalog = _model.ColorIconHeightCatalog;
            SettingsPictureSize.ColorIconWidthDetails = _model.ColorIconWidthDetails;
            SettingsPictureSize.ColorIconHeightDetails = _model.ColorIconHeightDetails;
            SettingsCatalog.ComplexFilter = _model.ComplexFilter;

            SettingsCatalog.BuyButtonText = _model.BuyButtonText;
            SettingsCatalog.DisplayBuyButton = _model.DisplayBuyButton;
            SettingsCatalog.PreOrderButtonText = _model.PreOrderButtonText;
            SettingsCatalog.DisplayPreOrderButton = _model.DisplayPreOrderButton;

            SettingsCatalog.DefaultCatalogView = _model.DefaultCatalogView;
            SettingsCatalog.EnabledCatalogViewChange = _model.EnableCatalogViewChange;

            SettingsCatalog.DefaultSearchView = _model.DefaultSearchView;
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

            SettingsDesign.ShowShippingsMethodsInDetails = _model.ShowShippingsMethodsInDetails;

            SettingsDesign.ShippingsMethodsInDetailsCount = _model.ShippingsMethodsInDetailsCount;
            SettingsCatalog.RelatedProductName = _model.RelatedProductName;
            SettingsCatalog.AlternativeProductName = _model.AlternativeProductName;
            SettingsDesign.RelatedProductSourceType = _model.RelatedProductSourceType;
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
            SettingsCatalog.DefaultSortOrderProductInBrand = _model.DefaultSortOrderProductInBrand;

            #endregion

            #region News


            SaveSetting("NewsImageWidth", _model.NewsImageWidth);
            SaveSetting("NewsImageHeight", _model.NewsImageHeight);
            SettingsNews.NewsPerPage = _model.NewsPerPage;
            SettingsNews.NewsMainPageCount = _model.NewsMainPageCount;

            #endregion

            #region Other

            SettingsNews.MainPageText = _model.MainPageText;

            if (_model.OtherSettings != null)
            {
                foreach (var otherSetting in _model.OtherSettings)
                {
                    if (otherSetting.Type == ETemplateSettingType.StaticBlockCheckbox)
                    {
                        var block = StaticBlockService.GetPagePartByKey(otherSetting.Name);
                        if (block != null)
                        {
                            block.Enabled = otherSetting.Value.TryParseBool();
                            StaticBlockService.UpdatePagePart(block);
                        }
                    }
                    else
                    {
                        SaveSetting(otherSetting.Name, otherSetting.Value);
                    }
                }
            }

            #endregion

            #region Css editor

            new CssEditorHandler().SaveFileContent(_model.CssEditorText ?? "");

            #endregion

            CacheManager.RemoveByPattern(CacheNames.MenuPrefix);

            return true;
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
