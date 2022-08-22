using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Models;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.SQL2;
using AdvantShop.Customers;
using AdvantShop.Models.Brand;
using AdvantShop.Models.Catalog;
using AdvantShop.ViewModel.Catalog;


namespace AdvantShop.Handlers.Brands
{
    public class BrandProductPagingHandler
    {
        private readonly BrandModel _brandModel;
        private readonly int _currentPageIndex;
        private readonly bool _isMobile;
        
        private SqlPaging _paging;
        private ProductListViewModel _model;

        public BrandProductPagingHandler(BrandModel brandModel, bool isMobile)
        {
            _brandModel = brandModel;
            _currentPageIndex = brandModel.Page ?? 1;
            _isMobile = isMobile;
        }

        public ProductListViewModel GetForBrandItem()
        {
            _model = new ProductListViewModel() {Filter = new CategoryFiltering()};
            
            BuildPaging();
            BuildSorting();
            BuildFilter();

            var totalCount = _paging.TotalRowsCount;
            var totalPages = _paging.PageCount(totalCount);

            _model.Pager = new Pager()
            {
                TotalItemsCount = totalCount,
                TotalPages = totalPages,
                CurrentPage = _currentPageIndex
            };

            if ((totalPages < _currentPageIndex && _currentPageIndex > 1) || _currentPageIndex < 0)
            {
                return _model;
            }

            var products = _paging.PageItemsList<ProductModel>();

            _model.Products = new ProductViewModel(products, _isMobile);

            GetViewMode();

            return _model;
        }

        private void BuildPaging()
        {
            _paging = new SqlPaging();
            _paging.Select(
                "Product.ProductID",
                "CountPhoto",
                "Photo.PhotoId",
                "Photo.PhotoName",
                "Photo.PhotoNameSize1",
                "Photo.PhotoNameSize2",
                "Photo.Description as PhotoDescription",
                "Product.ArtNo",
                "Product.Name",
                "Recomended",
                "Product.Bestseller",
                "Product.New",
                "Product.OnSale as Sale",
                "Product.Discount",
                "Product.DiscountAmount",
                "Product.BriefDescription",
                "Product.MinAmount",
                "Product.MaxAmount",
                "Product.Enabled",
                "Product.AllowPreOrder",
                "Product.Ratio",
                "Product.ManualRatio",
                "Product.UrlPath",
                "Product.DateAdded",
                "Offer.OfferID",
                "Offer.Amount".AsSqlField("AmountOffer"),
                "Offer.ColorID",
                "MaxAvailable AS Amount",
                "ShoppingCartItemId",
                "Comments",
                "CurrencyValue",
				"ProductExt.Gifts",
                "Brand.BrandName as BrandName",
                "Brand.UrlPath as BrandUrlPath"
                );

            _paging.From("[Catalog].[Product]");
            _paging.Left_Join("[Catalog].[ProductExt] ON [Product].[ProductID] = [ProductExt].[ProductID]");
            _paging.Left_Join("[Catalog].[Photo] ON [Photo].[PhotoId] = [ProductExt].[PhotoId]");
            _paging.Left_Join("[Catalog].[Offer] ON [ProductExt].[OfferID] = [Offer].[OfferID]");
            _paging.Inner_Join("[Catalog].[Currency] ON [Currency].[CurrencyID] = [Product].[CurrencyID]");
            _paging.Inner_Join("[Catalog].[Brand] ON [Product].[BrandID] = [Brand].[BrandID] AND [Brand].BrandId = {0}", _brandModel.BrandId);


            _paging.Left_Join("[Catalog].[ShoppingCart] ON [ShoppingCart].[OfferID] = [Offer].[OfferID] AND [ShoppingCart].[ShoppingCartType] = 3 AND [ShoppingCart].[CustomerID] = {0}", CustomerContext.CustomerId);
            
            if (SettingsCatalog.ComplexFilter)
            {
                _paging.Select(
                      "Colors",
                      "NotSamePrices as MultiPrices",
                      "MinPrice as BasePrice"
                 );
            }
            else
            {
                _paging.Select(
                        "null as Colors",
                        "0 as MultiPrices",
                        "Price as BasePrice"
                   );
            }

            _paging.ItemsPerPage = _currentPageIndex != 0 ? SettingsCatalog.ProductsPerPage : int.MaxValue;
            _paging.CurrentPageIndex = _currentPageIndex != 0 ? _currentPageIndex : 1;
        }

        private void BuildFilter()
        {
            _paging.Where("Product.Enabled={0}", true);
            _paging.Where("Product.Hidden={0}".IgnoreInCustomData(), false);
            _paging.Where("AND CategoryEnabled={0}", true);
            _paging.Where("AND Offer.Main={0} AND Offer.Main IS NOT NULL", true);

            if (SettingsCatalog.ShowOnlyAvalible)
            {
                _paging.Where("AND MaxAvailable > 0");
            }
        }

        private void BuildSorting()
        {
            var sort = _brandModel.Sort != null ? (ESortOrder)_brandModel.Sort : ESortOrder.NoSorting;
            _model.Filter.Sorting = sort;

            if (SettingsCatalog.MoveNotAvaliableToEnd)
            {
                _paging.OrderByDesc("(CASE WHEN Price=0 THEN 0 ELSE 1 END)".AsSqlField("TempSort"));
                _paging.OrderByDesc("AmountSort".AsSqlField("TempAmountSort"));
            }

            switch (sort)
            {
                case ESortOrder.AscByName:
                    _paging.OrderBy("Product.Name".AsSqlField("NameSort"));
                    break;

                case ESortOrder.DescByName:
                    _paging.OrderByDesc("Product.Name".AsSqlField("NameSort"));
                    break;

                case ESortOrder.AscByPrice:
                    _paging.OrderBy("PriceTemp");
                    break;

                case ESortOrder.DescByPrice:
                    _paging.OrderByDesc("PriceTemp");
                    break;

                case ESortOrder.AscByRatio:
                    _paging.OrderBy("(CASE WHEN ManualRatio is NULL THEN Ratio ELSE ManualRatio END)".AsSqlField("RatioSort"));
                    break;

                case ESortOrder.DescByRatio:
                    _paging.OrderByDesc("(CASE WHEN ManualRatio is NULL THEN Ratio ELSE ManualRatio END)".AsSqlField("RatioSort"));
                    break;

                case ESortOrder.AscByAddingDate:
                    _paging.OrderBy("DateAdded".AsSqlField("DateAddedSort"));
                    break;

                case ESortOrder.DescByAddingDate:
                    _paging.OrderByDesc("DateAdded".AsSqlField("DateAddedSort"));
                    break;

                case ESortOrder.DescByPopular:
                    _paging.OrderByDesc("SortPopular");
                    break;

                case ESortOrder.DescByDiscount:
                    //_paging.OrderByDesc("SortDiscount");
                    _paging.OrderByDesc("Discount".AsSqlField("DiscountSorting"));
                    _paging.OrderByDesc("DiscountAmount".AsSqlField("DiscountAmountSorting"));
                    break;

                default:
                    break;
            }
        }

        private void GetViewMode()
        {
            var mode = SettingsCatalog.GetViewMode(SettingsCatalog.EnabledCatalogViewChange, "viewmode", SettingsCatalog.DefaultCatalogView, false);

            _model.Filter.ViewMode = mode.ToString().ToLower();
            _model.Filter.AllowChangeViewMode = SettingsCatalog.EnabledCatalogViewChange;
        }
    }
}