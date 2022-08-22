using System;
using System.Collections.Generic;
using System.Linq;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL2;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Core.Services.FullSearch;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Web.Admin.Handlers.Catalog
{
    public class GetCatalog
    {
        private readonly CatalogFilterModel _filterModel;
        private SqlPaging _paging;

        public GetCatalog(CatalogFilterModel filterModel)
        {
            _filterModel = filterModel;
        }

        public FilterResult<CatalogProductModel> Execute()
        {
            var model = new FilterResult<CatalogProductModel>();

            GetPaging();

            model.TotalItemsCount = _paging.TotalRowsCount;
            model.TotalPageCount = _paging.PageCount();
            model.TotalString = string.Format("Найдено товаров: {0}{1}", model.TotalItemsCount, _paging.LimitReached ? "+" : "");

            if (model.TotalPageCount < _filterModel.Page && _filterModel.Page > 1)
            {
                return model;
            }

            model.DataItems = _paging.PageItemsList<CatalogProductModel>();

            return model;
        }

        public List<T> GetItemsIds<T>(string fieldName)
        {
            GetPaging();

            return _paging.ItemsIds<T>(fieldName);
        }

        private void GetPaging()
        {
            _paging = new SqlPaging("AdminCatalogPaging", SettingsAdmin.CatalogGridWithoutLimits ? default(int?) : 10000)
            {
                ItemsPerPage = _filterModel.ItemsPerPage,
                CurrentPageIndex = _filterModel.Page
            };

            _paging.Select(
                "Product.ProductID",
                "Product.Name",
                "[Settings].[ArtNoToString](Product.ProductID)".AsSqlField("ArtNo"),
                "Product.ArtNo".AsSqlField("ProductArtNo"),
                "PhotoName",
                //"(Select Count(ProductID) From Catalog.ProductCategories Where ProductID=Product.ProductID)".AsSqlField("ProductCategoriesCount"),
                "Price",
                "(Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID)".AsSqlField("Amount"),
                "(Select count (offerid) from catalog.Offer where Offer.ProductID=Product.productID)".AsSqlField("OffersCount"),
                "Enabled",
                "[Currency].Code".AsSqlField("CurrencyCode"),
                "[Currency].CurrencyIso3",
                "[Currency].CurrencyValue",
                "[Currency].IsCodeBefore",
                (_filterModel.ShowMethod == ECatalogShowMethod.Normal ? "ProductCategories.SortOrder" : "-1 as SortOrder"),
                "Offer.ColorID",
                "Offer.SizeID"
                );

            _paging.From("[Catalog].[Product]");

            _paging.Left_Join("[Catalog].[Offer] ON [Product].[ProductID] = [Offer].[ProductID] and [Offer].[Main] = 1");
            _paging.Left_Join("[Catalog].[Photo] ON [Product].[ProductID] = [Photo].[ObjId]  and Type='Product' AND [Photo].[Main] = 1");
            _paging.Left_Join("[Catalog].[Currency] ON [Product].[CurrencyID] = [Currency].[CurrencyID]");

            switch (_filterModel.ShowMethod)
            {
                case ECatalogShowMethod.AllProducts:
                    //_paging.Left_Join("[Catalog].[ProductCategories] ON [Catalog].[ProductCategories].[ProductID] = [Product].[ProductID]");
                    break;

                case ECatalogShowMethod.OnlyWithoutCategories:
                    _paging.Inner_Join("(Select ProductId from Catalog.Product where ProductId not in (Select ProductId from Catalog.ProductCategories)) as tmp on tmp.ProductId=[Product].[ProductID]");
                    break;

                case ECatalogShowMethod.Normal:
                    _paging.Inner_Join("[Catalog].[ProductCategories] on [ProductCategories].[ProductId] = [Product].[ProductID]");
                    break;
            }

            if (_filterModel.ShowMethod == ECatalogShowMethod.Normal && _filterModel.CategoryId != null)
            {
                _paging.Where("ProductCategories.CategoryID = {0}", _filterModel.CategoryId);
            }

            Filter();
            Sorting();
        }

        private void Filter()
        {
            switch (_filterModel.Type)
            {
                case EProductOnMain.Best:
                    _paging.Where("Bestseller = 1");
                    break;

                case EProductOnMain.New:
                    _paging.Where("New = 1");
                    break;

                case EProductOnMain.Sale:
                    _paging.Where("(Discount <> 0 or DiscountAmount <> 0)");
                    break;
            }

            if (!string.IsNullOrEmpty(_filterModel.Search))
            {
                var res = ProductSeacherAdmin.Search(_filterModel.Search);
                var productIds = res.SearchResultItems.Aggregate("", (current, item) => current + (item.Id + "/"));
                _paging.Inner_Join(
                    "(select item, sort from [Settings].[ParsingBySeperator]({0},'/')) as dtt on Product.ProductId=convert(int, dtt.item)",
                    productIds);

                if (_filterModel.SortingType == FilterSortingType.None)
                {
                    _paging.OrderBy("dtt.sort");
                }
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.ArtNo))
            {
                _paging.Where("(Product.ArtNo LIKE '%'+{0}+'%' OR [Settings].[ArtNoToString](Product.ProductID) LIKE  '%'+{0}+'%')", _filterModel.ArtNo);
            }

            if (!string.IsNullOrWhiteSpace(_filterModel.Name))
            {
                _paging.Where("Product.Name LIKE '%'+{0}+'%'", _filterModel.Name);
            }


            if (_filterModel.ColorId.IsNotEmpty())
            {
                var color = ColorService.GetColor(_filterModel.ColorId.TryParseInt());
                if (color != null)
                {
                    //_paging.Where("Offer.ColorID={0}", _filterModel.ColorId);
                    _paging.Where("(SELECT COUNT([Offer].OfferID) FROM [Catalog].[Offer] WHERE [Offer].ProductID = Product.ProductID AND [Offer].ColorID={0}) > 0", _filterModel.ColorId);
                }
            }

            if (_filterModel.SizeId.IsNotEmpty())
            {
                var size = SizeService.GetSize(_filterModel.SizeId.TryParseInt());
                if (size != null)
                {
                    //_paging.Where("Offer.SizeID={0}", _filterModel.SizeId);
                    _paging.Where("(SELECT COUNT([Offer].OfferID) FROM [Catalog].[Offer] WHERE [Offer].ProductID = Product.ProductID AND [Offer].SizeID={0}) > 0", _filterModel.SizeId);
                }
            }

            if (_filterModel.PriceFrom.HasValue)
            {
                _paging.Where("ISNULL(Price, 0) >= {0}", _filterModel.PriceFrom.Value);
            }

            if (_filterModel.PriceTo.HasValue)
            {
                _paging.Where("ISNULL(Price, 0) <= {0}", _filterModel.PriceTo.Value);
            }

            if (_filterModel.AmountFrom != null)
            {
                _paging.Where("(ISNULL((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID), 0) >= {0})",  _filterModel.AmountFrom.Value);
            }

            if (_filterModel.AmountTo != null)
            {
                _paging.Where("(ISNULL((Select sum (Amount) from catalog.Offer where Offer.ProductID=Product.productID), 0) <= {0})", _filterModel.AmountTo.Value);
            }


            if (_filterModel.SortingFrom != null && _filterModel.SortingTo != null && _filterModel.ShowMethod == ECatalogShowMethod.Normal)
            {
                _paging.Where("(ProductCategories.SortOrder >= {0} and ProductCategories.SortOrder <= {1})", _filterModel.SortingFrom, _filterModel.SortingTo);
            }

            if (_filterModel.HasPhoto != null)
            {
                _paging.Where(string.Format("PhotoName is {0} null", _filterModel.HasPhoto.Value ? "not" : ""));
            }

            if (_filterModel.BrandId != null)
            {
                _paging.Where("BrandId = {0}", _filterModel.BrandId.Value);
            }

            if (_filterModel.Enabled != null)
            {
                _paging.Where("Enabled = {0}", _filterModel.Enabled.Value ? "1" : "0");
            }

            if (_filterModel.ExcludeIds != null)
            {
                var exludeIds = _filterModel.ExcludeIds.Split(',').Select(x => x.TryParseInt()).Where(x => x != 0).ToList();
                if (exludeIds.Count > 0)
                {
                    _paging.Where("Product.ProductId not in (" + String.Join(",", exludeIds) + ")");
                }
            }

            if(_filterModel.PropertyId.IsNotEmpty())
            {
                var property = PropertyService.GetPropertyById(_filterModel.PropertyId.TryParseInt());
                if(property != null)
                {
                    _paging.Where("(SELECT COUNT([ProductPropertyValue].ProductID) " +
                        "FROM [Catalog].[ProductPropertyValue] " +
                        "JOIN [Catalog].[PropertyValue] ON [ProductPropertyValue].[PropertyValueID] = [PropertyValue].[PropertyValueID] " +
                        "WHERE [ProductPropertyValue].[ProductID] = Product.ProductId AND [PropertyValue].PropertyID = {0}) > 0", property.PropertyId);
                }
            }

            if (_filterModel.PropertyValueId.IsNotEmpty())
            {
                var propertyValue = PropertyService.GetPropertyValueById(_filterModel.PropertyValueId.TryParseInt());
                if (propertyValue != null)
                {
                    _paging.Where("(SELECT COUNT([ProductPropertyValue].ProductID) " +
                        "FROM [Catalog].[ProductPropertyValue] " +
                        "WHERE [ProductPropertyValue].[ProductID] = Product.ProductId AND [ProductPropertyValue].PropertyValueID = {0}) > 0", propertyValue.PropertyValueId);
                }
            }

            if(_filterModel.Tags != null && _filterModel.Tags.Count > 0)
            {
                _paging.Where("(SELECT COUNT(*) FROM [Catalog].[TagMap] WHERE [TagMap].[ObjId] = [Product].[ProductId] AND [TagMap].[Type] = {0} AND [TagMap].[TagId] in (" +
                    string.Join(",", _filterModel.Tags.Select(x => x.ToString()).ToArray()) + ")) >= " + _filterModel.Tags.Count.ToString(), ETagType.Product.ToString());
            }

            if (_filterModel.DiscountFrom.HasValue)
            {
                _paging.Where("Discount >= {0}", _filterModel.DiscountFrom.Value);
            }

            if (_filterModel.DiscountTo.HasValue)
            {
                _paging.Where("Discount <= {0}", _filterModel.DiscountTo.Value);
            }

            if (_filterModel.DiscountAmountFrom.HasValue)
            {
                _paging.Where("DiscountAmount >= {0}", _filterModel.DiscountAmountFrom.Value);
            }

            if (_filterModel.DiscountAmountTo.HasValue)
            {
                _paging.Where("DiscountAmount <= {0}", _filterModel.DiscountAmountTo.Value);
            }
        }

        private void Sorting()
        {
            if (string.IsNullOrEmpty(_filterModel.Sorting) || _filterModel.SortingType == FilterSortingType.None)
            {
                _paging.OrderBy(
                    _filterModel.ShowMethod != ECatalogShowMethod.Normal
                        ? "ProductArtNo"
                        : "ProductCategories.SortOrder");
                return;
            }

            var sorting = _filterModel.Sorting.ToLower();
            var sortingForApply = sorting == "pricestring" ? "price" : sorting;

            var field = _paging.SelectFields().FirstOrDefault(x => x.FieldName == sortingForApply);
            if (field != null)
            {
                if (_filterModel.SortingType == FilterSortingType.Asc)
                {
                    _paging.OrderBy(sortingForApply);
                }
                else
                {
                    _paging.OrderByDesc(sortingForApply);
                }
            }
        }
    }
}