using AdvantShop.Catalog;
using AdvantShop.Core;
using AdvantShop.Web.Admin.Models.Catalog.Products;
using AdvantShop.Web.Infrastructure.Handlers;

namespace AdvantShop.Web.Admin.Handlers.Catalog.Products
{
    public class GetExportOptionsHandler : AbstractCommandHandler<ProductExportOptions>
    {
        private readonly int _productId;
        private Product _product;

        public GetExportOptionsHandler(int productId)
        {
            _productId = productId;
        }

        protected override void Load()
        {
            _product = ProductService.GetProduct(_productId);
        }

        protected override void Validate()
        {
            if (_product == null)
                throw new BlException(T("Admin.Product.ExportOptions.Error.NotFound"));
        }

        protected override ProductExportOptions Handle()
        {
            return new ProductExportOptions()
            {
                ProductId = _product.ProductId,
                SalesNote = _product.SalesNote,
                Gtin = _product.Gtin,
                GoogleProductCategory = _product.GoogleProductCategory,
                YandexMarketCategory = _product.YandexMarketCategory,
                YandexTypePrefix = _product.YandexTypePrefix,
                YandexModel = _product.YandexModel,
                Adult = _product.Adult,
                ManufacturerWarranty = _product.ManufacturerWarranty,
                YandexSizeUnit = _product.YandexSizeUnit,
                Bid = _product.Bid,
                YandexName = _product.YandexName,
                YandexDeliveryDays = _product.YandexDeliveryDays,
                YandexProductDiscounted = _product.YandexProductDiscounted,
                YandexProductDiscountCondition = _product.YandexProductDiscountCondition,
                YandexProductDiscountReason = _product.YandexProductDiscountReason
            };
        }
    }
}
