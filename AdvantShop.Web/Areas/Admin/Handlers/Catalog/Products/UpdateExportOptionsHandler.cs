using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class UpdateExportOptionsHandler : AbstractCommandHandler
    {
        private readonly ProductExportOptions _model;
        private Product _product;

        public UpdateExportOptionsHandler(ProductExportOptions model)
        {
            _model = model;
        }

        protected override void Load()
        {
            _product = ProductService.GetProduct(_model.ProductId);
        }

        protected override void Validate()
        {
            if (_product == null)
                throw new BlException(T("Admin.Product.ExportOptions.Error.NotFound"));
            if (_model.YandexProductDiscounted)
            {
                if (_model.YandexProductDiscounted && _model.YandexProductDiscountCondition == EYandexDiscountCondition.None)
                    throw new BlException(T("Admin.Product.ExportOptions.Error.DiscountConditionRequired"));
                if (_model.YandexProductDiscounted && _model.YandexProductDiscountReason.IsNullOrEmpty())
                    throw new BlException(T("Admin.Product.ExportOptions.Error.DiscountReasonRequired"));
                if (_model.YandexProductDiscounted && _model.YandexProductDiscountReason.Length > 3000)
                    throw new BlException(T("Admin.Product.ExportOptions.Error.DiscountReasonLength"));
            }
        }

        protected override void Handle()
        {
            _product.SalesNote = _model.SalesNote;
            _product.Gtin = _model.Gtin;
            _product.GoogleProductCategory = _model.GoogleProductCategory;
            _product.YandexMarketCategory = _model.YandexMarketCategory;
            _product.YandexTypePrefix = _model.YandexTypePrefix;
            _product.YandexModel = _model.YandexModel;
            _product.Adult = _model.Adult;
            _product.ManufacturerWarranty = _model.ManufacturerWarranty;
            _product.Bid = _model.Bid;
            _product.YandexSizeUnit = _model.YandexSizeUnit;
            _product.YandexName = _model.YandexName;
            _product.YandexDeliveryDays = _model.YandexDeliveryDays;
            _product.YandexProductDiscounted = _model.YandexProductDiscounted;
            _product.YandexProductDiscountCondition = _model.YandexProductDiscountCondition;
            _product.YandexProductDiscountReason = _model.YandexProductDiscountReason;

            ProductService.UpdateProduct(_product, true, true);
        }
    }
}
