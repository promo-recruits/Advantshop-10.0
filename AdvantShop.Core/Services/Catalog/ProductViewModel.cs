using System;
using System.Collections.Generic;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using Newtonsoft.Json;

namespace AdvantShop.Core.Services.Catalog
{
    public partial class ProductViewModel
    {
        public ProductViewModel(List<ProductModel> products)
        {
            HidePrice = SettingsCatalog.HidePrice;
            TextInsteadOfPrice = SettingsCatalog.TextInsteadOfPrice;

            DisplayBuyButton = SettingsCatalog.DisplayBuyButton && !SettingsCatalog.HidePrice;
            DisplayPreOrderButton = SettingsCatalog.DisplayPreOrderButton && !SettingsCatalog.HidePrice;
            AllowBuyOutOfStockProducts = SettingsCheckout.OutOfStockAction == eOutOfStockAction.Cart;

            DisplayRating = SettingsCatalog.EnableProductRating;
            DisplayComparison = SettingsCatalog.EnableCompareProducts;
            DisplayPhotoPreviews = SettingsCatalog.EnablePhotoPreviews;
            DisplayPhotoCount = SettingsCatalog.ShowCountPhoto;
            DisplayQuickView = SettingsCatalog.ShowQuickView;
            DisplayProductArtNo = SettingsCatalog.ShowProductArtNo;
            DisplayReviewCount = SettingsCatalog.AllowReviews;
            ShowNotAvaliableLable = SettingsCatalog.ShowNotAvaliableLable;

            BuyButtonText = SettingsCatalog.BuyButtonText;
            PreOrderButtonText = SettingsCatalog.PreOrderButtonText;

            ColorImageHeight = SettingsPictureSize.ColorIconHeightCatalog;
            ColorImageWidth = SettingsPictureSize.ColorIconWidthCatalog;
            
            PhotoWidth = SettingsPictureSize.SmallProductImageWidth;
            PhotoHeight = SettingsPictureSize.SmallProductImageHeight;

            PhotoPreviewWidth = SettingsPictureSize.XSmallProductImageWidth;
            PhotoPreviewHeight = SettingsPictureSize.XSmallProductImageHeight;

            PhotoXSmallWidth = SettingsPictureSize.XSmallProductImageWidth;
            PhotoSmallWidth = SettingsPictureSize.SmallProductImageWidth;
            PhotoMiddleWidth = SettingsPictureSize.MiddleProductImageWidth;
            PhotoBigWidth = SettingsPictureSize.BigProductImageWidth;
            
            Products = products ?? new List<ProductModel>();

            CountProductsInLine = SettingsDesign.CountCatalogProductInLine;

            ColorsViewMode = SettingsCatalog.ColorsViewMode;

            if (Products != null && Products.Count > 0)
            {
                var productDiscounts = new List<ProductDiscount>();
                var discountByTime = DiscountByTimeService.GetDiscountByTime();
                var customerGroup = CustomerContext.CurrentCustomer.CustomerGroup;

                var discountModules = AttachedModules.GetModules<IDiscount>();
                foreach (var discountModule in discountModules)
                {
                    if (discountModule != null)
                    {
                        var classInstance = (IDiscount) Activator.CreateInstance(discountModule);
                        productDiscounts.AddRange(classInstance.GetProductDiscountsList());
                    }
                }

                foreach (var product in Products)
                {
                    product.DiscountByDatetime = discountByTime;
                    product.CustomerGroup = customerGroup;
                    product.ProductDiscounts = productDiscounts;
                }
            }
        }

        public ProductViewModel(List<ProductModel> products, bool isMobile) : this(products)
        {
            IsMobile = isMobile;

            if (isMobile)
            {
                BlockProductPhotoHeight = SettingsMobile.BlockProductPhotoHeight;
                ShowNotAvailableLabel = SettingsCatalog.ShowNotAvaliableLable;

                var productViewMode =
                    SettingsCatalog.GetViewMode(SettingsMobile.EnableCatalogViewChange, "mobile_viewmode",
                                                SettingsMobile.DefaultCatalogView, true);

                ProductImageType = productViewMode == ProductViewMode.Single
                    ? ProductImageType.Big
                    : ProductImageType.Middle;
            }
        }

        public bool IsMobile { get; private set; }

        public int Id { get; set; }
        public string Title { get; set; }
        public bool DisplayBuyButton { get; set; }
        public bool DisplayPreOrderButton { get; set; }
        public bool AllowBuyOutOfStockProducts { get; set; }

        public bool DisplayPhotoPreviews { get; set; }
        public bool DisplayComparison { get; set; }
        public bool DisplayRating { get; set; }
        public bool DisplayCategory { get; set; }
        public bool DisplayQuickView { get; set; }
        public bool DisplayProductArtNo { get; set; }
        public bool DisplayPhotoCount { get; set; }
        public bool DisplayReviewCount { get; set; }
        public bool ShowNotAvaliableLable { get; set; }

        public bool HidePrice { get; set; }
        public string TextInsteadOfPrice { get; set; }

        public int CountProductsInLine { get; set; }

        public string BuyButtonText { get; set; }
        public string PreOrderButtonText { get; set; }

        public int ColorImageHeight { get; set; }
        public int ColorImageWidth { get; set; }
        public string SelectedColors { get; set; }

        private List<int> _selectedColorsList;

        public List<int> SelectedColorsList
        {
            get
            {
                if (_selectedColorsList == null && SelectedColors != null && SelectedColors.Length > 2)
                {
                    _selectedColorsList = JsonConvert.DeserializeObject<List<int>>(SelectedColors);
                }

                return _selectedColorsList;
            }
        }

        public int? SelectedSizeId { get; set; }

        public int PhotoWidth { get; set; }
        public int PhotoHeight { get; set; }

        public int PhotoPreviewWidth { get; set; }
        public int PhotoPreviewHeight { get; set; }
        
        public int PhotoXSmallWidth { get; set; }
        public int PhotoSmallWidth { get; set; }
        public int PhotoMiddleWidth { get; set; }
        public int PhotoBigWidth { get; set; }
        
        public List<ProductModel> Products { get; private set; }

        public eLazyLoadType LazyLoadType { get; set; }

        public ColorsViewMode ColorsViewMode { get; set; }

        public int BlockProductPhotoHeight { get; set; }
        public bool ShowNotAvailableLabel { get; set; }
        public ProductImageType ProductImageType { get; set; }
        public bool HideMarkers { get; set; }

        public string WrapCssClass { get; set; }
        public bool ShowAmountsTableInCatalog { get; private set; }
        
    }

    public enum eLazyLoadType
    {
        Default = 0,
        Carousel = 1
    }
}