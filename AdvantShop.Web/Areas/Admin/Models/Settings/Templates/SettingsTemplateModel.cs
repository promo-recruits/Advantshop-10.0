using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Localization;
using AdvantShop.FilePath;

namespace AdvantShop.Web.Admin.Models.Settings.Templates
{
    public class SettingsTemplateModel : IValidatableObject
    {
        #region Common

        public string CurrentTheme { get; set; }
        public string CurrentBackGround { get; set; }
        public string CurrentColorScheme { get; set; }
        
        public string MainPageModeTitle { get; set; }
        public string MainPageMode { get; set; }
        public List<SelectListItem> MainPageModeOptions { get; set; }
        
        public string MenuStyle { get; set; }
        public List<SelectListItem> MenuStyleOptions { get; set; }
        
        public string SearchBlockLocation { get; set; }
        public List<SelectListItem> SearchBlockLocationOptions { get; set; }


        public string TopPanel { get; set; }
        public List<SelectListItem> TopPanelOptions { get; set; }

        public string Header { get; set; }
        public List<SelectListItem> HeaderOptions { get; set; }

        public string TopMenu { get; set; }
        public List<SelectListItem> TopMenuOptions { get; set; }

        public bool CarouselVisibility { get; set; }
        public int CarouselAnimationSpeed { get; set; }
        public int CarouselAnimationDelay { get; set; }
        public bool MainPageProductsVisibility { get; set; }
        public int CountMainPageProductInLine { get; set; }
        public int CountMainPageProductInSection { get; set; }
        public bool BrandCarouselVisibility { get; set; }
        public bool RecentlyViewVisibility { get; set; }
        public bool NewsVisibility { get; set; }
        public bool NewsSubscriptionVisibility { get; set; }
        public bool CheckOrderVisibility { get; set; }
        public bool GiftSertificateVisibility { get; set; }
        public bool WishListVisibility { get; set; }

        public bool IsStoreClosed { get; set; }
        public bool EnableInplace { get; set; }
        public bool DisplayToolBarBottom { get; set; }
        public bool DisplayCityInTopPanel { get; set; }
        public bool ShowCopyright { get; set; }
        public string CopyrightText { get; set; }

        public string AdditionalHeadMetaTag { get; set; }
        public bool ShowUserAgreementText { get; set; }
        public bool AgreementDefaultChecked { get; set; }
        public string UserAgreementText { get; set; }
        public bool DisplayCityBubble { get; set; }
        public bool ShowCookiesPolicyMessage { get; set; }
        public string CookiesPolicyMessage { get; set; }

        public bool MainPageCategoriesVisibility { get; set; }
        public int CountMainPageCategoriesInLine { get; set; }
        public int CountMainPageCategoriesInSection { get; set; }
        public bool TopMenuVisibility { get; set; }

        #endregion

        #region Catalog

        public int CountCategoriesInLine { get; set; }

        public int CountCatalogProductInLine { get; set; }
        public bool ShowQuickView { get; set; }
        public int ProductsPerPage { get; set; }
        public bool ShowProductsCount { get; set; }
        public bool DisplayCategoriesInBottomMenu { get; set; }
        public bool ShowProductArtNo { get; set; }
        public bool EnableProductRating { get; set; }
        public bool EnableCompareProducts { get; set; }
        public bool EnablePhotoPreviews { get; set; }
        public bool ShowCountPhoto { get; set; }
        public bool ShowOnlyAvalible { get; set; }
        public bool MoveNotAvaliableToEnd { get; set; }
        public bool ShowNotAvaliableLable { get; set; }

        public bool FilterVisibility { get; set; }
        public bool ShowPriceFilter { get; set; }
        public bool ShowProducerFilter { get; set; }
        public bool ShowSizeFilter { get; set; }
        public bool ShowPropertiesFilterInProductList { get; set; }
        public bool ExcludingFilters { get; set; }
        public bool ShowColorFilter { get; set; }
        public string SizesHeader { get; set; }
        public string ColorsHeader { get; set; }

        public ColorsViewMode ColorsViewMode { get; set; }
        public List<SelectListItem> ColorsViewModes { get; set; }

        public int ColorIconWidthCatalog { get; set; }
        public int ColorIconHeightCatalog { get; set; }
        public int ColorIconWidthDetails { get; set; }
        public int ColorIconHeightDetails { get; set; }
        public bool ComplexFilter { get; set; }
        public string BuyButtonText { get; set; }
        public bool DisplayBuyButton { get; set; }
        public string PreOrderButtonText { get; set; }
        public bool DisplayPreOrderButton { get; set; }
        public ProductViewMode DefaultCatalogView { get; set; }
        public bool EnableCatalogViewChange { get; set; }
        public List<SelectListItem> DefaultViewList { get; set; }
        public ProductViewMode DefaultSearchView { get; set; }
        public bool EnableSearchViewChange { get; set; }

        public int BigProductImageWidth { get; set; }
        public int BigProductImageHeight { get; set; }
        public int MiddleProductImageWidth { get; set; }
        public int MiddleProductImageHeight { get; set; }
        public int SmallProductImageWidth { get; set; }
        public int SmallProductImageHeight { get; set; }
        public int XSmallProductImageWidth { get; set; }
        public int XSmallProductImageHeight { get; set; }
        public int BigCategoryImageWidth { get; set; }
        public int BigCategoryImageHeight { get; set; }
        public int SmallCategoryImageWidth { get; set; }
        public int SmallCategoryImageHeight { get; set; }

        #endregion

        #region Product

        public bool DisplayWeight { get; set; }
        public bool DisplayDimensions { get; set; }
        public bool ShowStockAvailability { get; set; }
        //public bool CompressBigImage { get; set; }
        public bool EnableZoom { get; set; }
        public bool AllowReviews { get; set; }
        public bool ModerateReviews { get; set; }
        public bool ReviewsVoiteOnlyRegisteredUsers { get; set; }
        public bool DisplayReviewsImage { get; set; }
        public bool AllowReviewsImageUploading { get; set; }
        public int ReviewImageWidth { get; set; }
        public int ReviewImageHeight { get; set; }
        public SettingsDesign.eShowShippingsInDetails ShowShippingsMethodsInDetails { get; set; }
        public List<SelectListItem> ShowShippingsMethods { get; set; }
        public int ShippingsMethodsInDetailsCount { get; set; }
        public string RelatedProductName { get; set; }
        public string AlternativeProductName { get; set; }
        public SettingsDesign.eRelatedProductSourceType RelatedProductSourceType { get; set; }
        public List<SelectListItem> RelatedProductTypes { get; set; }
        public int RelatedProductsMaxCount { get; set; }

        #endregion

        #region Checkout

        public string PaymentIconWidth { get; set; }
        public string PaymentIconHeight { get; set; }
        public string ShippingIconWidth { get; set; }
        public string ShippingIconHeight { get; set; }

        #endregion

        #region Brands

        public string BrandLogoWidth { get; set; }
        public string BrandLogoHeight { get; set; }
        public int BrandsPerPage { get; set; }
        public bool ShowProductsInBrand { get; set; }
        public bool ShowCategoryTreeInBrand { get; set; }
        public List<SelectListItem> DefaultSortOrderProductInBrandOption { get; set; }
        public ESortOrder DefaultSortOrderProductInBrand { get; set; }

        #endregion

        #region News

        public string NewsImageWidth { get; set; }
        public string NewsImageHeight { get; set; }
        public int NewsPerPage { get; set; }
        public int NewsMainPageCount { get; set; }

        #endregion

        #region Other

        public string MainPageText { get; set; }

        #endregion

        #region CssEditor

        public string CssEditorText { get; set; }

        #endregion

        #region Validation

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (ProductsPerPage <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtProductsPerPage"), new[] { "ProductsPerPage" });
            }

            if (string.IsNullOrEmpty(SizesHeader))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtSizesHeader"), new[] { "SizesHeader" });
            }

            if (string.IsNullOrEmpty(ColorsHeader))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorsHeader"), new[] { "ColorsHeader" });
            }

            if (string.IsNullOrEmpty(BuyButtonText))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtBuyButtonText"), new[] { "BuyButtonText" });
            }

            if (string.IsNullOrEmpty(PreOrderButtonText))
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtPreOrderButtonText"), new[] { "PreOrderButtonText" });
            }

            if (ColorIconWidthCatalog <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconWidthCatalog"), new[] { "ColorIconWidthCatalog" });
            }

            if (ColorIconHeightCatalog <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconHeightCatalog"), new[] { "ColorIconHeightCatalog" });
            }

            if (ColorIconWidthDetails <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconWidthDetails"), new[] { "ColorIconWidthDetails" });
            }

            if (ColorIconHeightDetails <= 0)
            {
                yield return new ValidationResult(LocalizationService.GetResource("Admin.Settings.Catalog.ErrorAtColorIconHeightDetails"), new[] { "ColorIconHeightDetails" });
            }
        }

        #endregion

        public List<TemplateSettingSection> OtherSettingsSections { get; set; }
        public List<TemplateSetting> OtherSettings { get; set; }


        public string DefaultCityIfNotAutodetect { get; set; }
        public List<string> HiddenSettings { get; set; }
    }
}
