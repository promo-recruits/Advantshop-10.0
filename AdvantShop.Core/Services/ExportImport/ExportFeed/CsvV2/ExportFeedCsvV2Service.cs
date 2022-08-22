//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.ChangeHistories;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Localization;
using AdvantShop.Repository.Currencies;
using AdvantShop.Saas;
using AdvantShop.SEO;
using AdvantShop.Taxes;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;

namespace AdvantShop.ExportImport
{
    public class ExportFeedCsvV2Service
    {
        public static IEnumerable<ExportFeedProductModel> GetProducts(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedCsvV2Options advancedSettings)
        {
            return SQLDataAccess.ExecuteReadIEnumerable(
                   "[Settings].[sp_GetCsvProducts]",
                    CommandType.StoredProcedure,
                    reader => GetCsvProductsFromReader(reader, commonSettings, advancedSettings),
                    new SqlParameter("@exportFeedId", exportFeedId),
                    new SqlParameter("@onlyCount", false),
                    new SqlParameter("@exportNoInCategory", advancedSettings.CsvExportNoInCategory),
                    new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                    new SqlParameter("@exportNotAvailable", true),
                    new SqlParameter("@exportAdult", commonSettings.ExportAdult)
                    );
        }

        public static int GetProductsCount(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedCsvV2Options advancedSettings)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "[Settings].[sp_GetCsvProducts]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@onlyCount", true),
                new SqlParameter("@exportNoInCategory", advancedSettings.CsvExportNoInCategory),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@exportNotAvailable", true),
                new SqlParameter("@exportAdult", commonSettings.ExportAdult)
                );
        }

        public static int GetMaxProductCategoriesCount(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedCsvV2Options advancedSettings)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "[Settings].[sp_GetCsvMaxProductCategoriesCount]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@exportNotAvailable", true)
                );
        }

        public static List<string> GetProductPropertyNames(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedCsvV2Options advancedSettings)
        {
            return SQLDataAccess.ExecuteReadColumn<string>(
                "[Settings].[sp_GetCsvProductPropertyNames]",
                CommandType.StoredProcedure,
                "Name", null, 60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@exportNotAvailable", true),
                new SqlParameter("@exportNoInCategory", advancedSettings.CsvExportNoInCategory)
                );
        }

        public static IEnumerable<ExportFeedCategories> GetCategories(int exportFeedId)
        {
            return SQLDataAccess.ExecuteReadIEnumerable("[Settings].[sp_GetExportFeedCategories]",
                CommandType.StoredProcedure,
                reader => new ExportFeedCategories
                {
                    Id = SQLDataHelper.GetInt(reader, "CategoryID"),
                    ParentCategory = SQLDataHelper.GetInt(reader, "ParentCategory"),
                    Name = SQLDataHelper.GetString(reader, "Name")
                },
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@onlyCount", false),
                new SqlParameter("@exportNotAvailable", false));
        }

        public static int GetCategoriesCount(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedCategories]",
                CommandType.StoredProcedure,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@onlyCount", true),
                new SqlParameter("@exportNotAvailable", false));
        }

        private static ExportFeedCsvV2Product GetCsvProductsFromReader(SqlDataReader reader, ExportFeedSettings commonSettings, ExportFeedCsvV2Options advancedSettings)
        {
            var product = ProductService.GetProductFromReader(reader);
            var fieldMapping = advancedSettings.FieldMapping;

            var productCsv = new ExportFeedCsvV2Product
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                Enabled = product.Enabled ? "+" : "-",
                Unit = product.Unit,
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent.ToString("F2") : null,
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount.ToString("F2") : null,
                ShippingPrice = product.ShippingPrice.HasValue ? product.ShippingPrice.Value.ToString("F2") : null,
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                MarkerNew = product.New ? "+" : "-",
                MarkerBestseller = product.BestSeller ? "+" : "-",
                MarkerRecomended = product.Recomended ? "+" : "-",
                MarkerOnSale = product.OnSale ? "+" : "-",
                ManualRatio = product.ManualRatio.HasValue ? product.ManualRatio.Value.ToString("F2") : null,
                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                YandexSalesNotes = product.SalesNote,
                YandexDeliveryDays = product.YandexDeliveryDays,
                YandexTypePrefix = product.YandexTypePrefix,
                YandexName = product.YandexName,
                YandexModel = product.YandexModel,
                YandexSizeUnit = product.YandexSizeUnit,
                YandexDiscounted = product.YandexProductDiscounted ? "+" : "-",
                YandexDiscountCondition = product.YandexProductDiscountCondition != EYandexDiscountCondition.None ? product.YandexProductDiscountCondition.StrName() : null,
                YandexDiscountReason = product.YandexProductDiscountReason,
                YandexBid = product.Bid.ToString(),
                GoogleGtin = product.Gtin,
                GoogleProductCategory = product.GoogleProductCategory,
                Adult = product.Adult ? "+" : "-",
                ManufacturerWarranty = product.ManufacturerWarranty ? "+" : "-",
                MinAmount = product.MinAmount.HasValue ? product.MinAmount.Value.ToString("F2") : null,
                MaxAmount = product.MaxAmount.HasValue ? product.MaxAmount.Value.ToString("F2") : null,
                Multiplicity = product.Multiplicity.ToString("F5").Trim('0'),
                PaymentSubjectType = product.PaymentSubjectType.Localize(),
                PaymentMethodType = product.PaymentMethodType.Localize()
            };

            var offers = new List<Offer>();
            if (fieldMapping.Any(x => x.IsOfferField()))
                offers = OfferService.GetProductOffers(product.ProductId).OrderByDescending(x => x.Main).ToList();

            var photos = new List<ProductPhoto>();
            if (fieldMapping.Any(field => field == EProductField.OfferPhotos || field == EProductField.Photos))
                photos = PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product);

            productCsv.Offers = offers.Select(offer => new ExportFeedCsvV2Offer
            {
                OfferId = offer.OfferId,
                ArtNo = offer.ArtNo,
                Price = (offer.BasePrice + 
                         (offer.BasePrice * commonSettings.PriceMarginInPercents / 100 + commonSettings.PriceMarginInNumbers)).ToString("F2"),
                PurchasePrice = offer.SupplyPrice.ToString("F2"),
                Amount = offer.Amount.ToString(CultureInfo.InvariantCulture),
                Size = offer.Size != null ? offer.Size.SizeName : null,
                Color = offer.Color != null ? offer.Color.ColorName : null,
                OfferPhotos = offer.ColorID.HasValue
                    ? photos.Where(photo => photo.ColorID == offer.ColorID).Select(photo => photo.PhotoName).AggregateString(advancedSettings.CsvColumSeparator)
                    : null,
                Weight = offer.GetWeight().ToFormatString(),
                Dimensions = offer.GetDimensions(),
                BarCode = offer.BarCode,
            }).ToList();

            if (fieldMapping.Contains(EProductField.Category))
            {
                productCsv.Categories = CategoryService.GetCategoriesPathAndSort(product.ProductId)
                    .Select(x => new ExportFeedCsvV2Category { Path = x.Key, Sort = x.Value }).ToList();
            }

            if (fieldMapping.Contains(EProductField.Currency))
            {
                productCsv.Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3;
            }

            if (fieldMapping.Contains(EProductField.Photos))
            {
                productCsv.Photos = photos.Where(photo => !photo.ColorID.HasValue).Select(photo => photo.PhotoName).AggregateString(advancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(EProductField.Property))
            {
                var propertyValues = PropertyService.GetPropertyValuesByProductId(product.ProductId);
                productCsv.Properties = propertyValues.GroupBy(x => x.Property.Name)
                    .ToDictionary(x => x.Key, x => x.Select(y => y.Value).AggregateString(advancedSettings.CsvColumSeparator));
            }

            if (fieldMapping.Contains(EProductField.SeoTitle) || fieldMapping.Contains(EProductField.SeoH1) ||
                fieldMapping.Contains(EProductField.SeoMetaKeywords) || fieldMapping.Contains(EProductField.SeoMetaDescription))
            {
                var meta = MetaInfoService.GetMetaInfo(productCsv.ProductId, MetaType.Product) ??
                           new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);
                productCsv.H1 = meta.H1;
                productCsv.Title = meta.Title;
                productCsv.MetaKeywords = meta.MetaKeywords;
                productCsv.MetaDescription = meta.MetaDescription;
            }
            if (fieldMapping.Contains(EProductField.Related))
            {
                productCsv.Related =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Related, advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Alternative))
            {
                productCsv.Alternative =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Alternative, advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Videos))
            {
                var videos = ProductVideoService.GetProductVideos(product.ProductId);
                productCsv.Videos = ProductVideoService.VideoToString(videos);
            }
            if (fieldMapping.Contains(EProductField.Producer))
            {
                productCsv.Producer = BrandService.BrandToString(product.BrandId);
            }
            if (fieldMapping.Contains(EProductField.CustomOptions))
            {
                productCsv.CustomOptions =
                    CustomOptionsService.CustomOptionsToString(
                        CustomOptionsService.GetCustomOptionsByProductId(product.ProductId));
            }
            // AvitoProductProperties
            if (fieldMapping.Contains(EProductField.AvitoProductProperties))
            {
                productCsv.AvitoProductProperties = ExportFeedAvitoService.GetProductPropertiesInString(product.ProductId, advancedSettings.CsvColumSeparator, advancedSettings.CsvPropertySeparator);
            }
            if (fieldMapping.Contains(EProductField.Tags) &&
                (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                var tags = TagService.Gets(product.ProductId, ETagType.Product);
                productCsv.Tags = tags.Select(item => item.Name).AggregateString(advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Gifts))
            {
                productCsv.Gifts = OfferService.ProductGiftsToString(product.ProductId,
                    advancedSettings.CsvColumSeparator);
            }
            if (fieldMapping.Contains(EProductField.Tax) && product.TaxId.HasValue)
            {
                var tax = TaxService.GetTax(product.TaxId.Value);
                if (tax != null)
                    productCsv.Tax = tax.Name;
            }

            if (fieldMapping.Contains(EProductField.ModifiedDate))
            {
                var lastChanges = ChangeHistoryService.GetLast(product.ProductId, ChangeHistoryObjType.Product);
                productCsv.ModifiedDate = lastChanges != null ? Culture.ConvertDate(lastChanges.ModificationTime) : "";
            }

            return productCsv;
        }
    }
}