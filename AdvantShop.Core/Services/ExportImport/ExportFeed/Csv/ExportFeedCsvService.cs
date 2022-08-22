//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
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
    public class ExportFeedCsvService
    {
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

        public static IEnumerable<ExportFeedProductModel> GetProducts(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedCsvOptions advancedSettings)
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

        public static IEnumerable<ExportFeedProductModel> GetAllProducts(ExportFeedSettings commonSettings, ExportFeedCsvOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteReadIEnumerable(
                   "[Settings].[sp_GetAllCsvProducts]",
                    CommandType.StoredProcedure,
                    reader => GetCsvProductsFromReader(reader, commonSettings, advancedSettings),
                    new SqlParameter("@onlyCount", false));
        }

        public static int GetProductsCount(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedCsvOptions advancedSettings)
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

        public static int GetAllCsvProductsCount()
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "[Settings].[sp_GetAllCsvProducts]",
                CommandType.StoredProcedure,
                new SqlParameter("@onlyCount", true));
        }

        public static int GetCategoriesCount(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedCategories]",
                CommandType.StoredProcedure,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@onlyCount", true),
                new SqlParameter("@exportNotAvailable", false));
        }

        private static ExportFeedCsvProduct GetCsvProductsFromReader(SqlDataReader reader, ExportFeedSettings commonSettings, ExportFeedCsvOptions advancedSettings)
        {
            var product = ProductService.GetProductFromReader(reader);
            var offerForExport = OfferService.GetMainOfferForExport(product.ProductId);

            var fieldMapping = advancedSettings.FieldMapping;

            var productCsv = new ExportFeedCsvProduct
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                Enabled = product.Enabled ? "+" : "-",
                Unit = product.Unit,
                ShippingPrice = product.ShippingPrice == null ? "" : product.ShippingPrice.Value.ToString("F2"),
                YandexDeliveryDays = product.YandexDeliveryDays == null ? "" : product.YandexDeliveryDays,
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent.ToString("F2") : "",
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount.ToString("F2") : "",
                Weight = (offerForExport?.GetWeight() ?? 0).ToFormatString(),
                Size = (offerForExport != null ? offerForExport.GetDimensions() : "0x0x0"),
                BarCode = offerForExport != null ? offerForExport.BarCode : string.Empty,
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                SalesNote = product.SalesNote,
                Gtin = product.Gtin,
                GoogleProductCategory = product.GoogleProductCategory,
                YandexTypePrefix = product.YandexTypePrefix,
                YandexModel = product.YandexModel,
                YandexSizeUnit = product.YandexSizeUnit,
                YandexDiscounted = product.YandexProductDiscounted ? "+" : "-",
                YandexDiscountCondition = product.YandexProductDiscountCondition != EYandexDiscountCondition.None ? product.YandexProductDiscountCondition.StrName() : null,
                YandexDiscountReason = product.YandexProductDiscountReason,

                Adult = product.Adult ? "+" : "-",
                ManufacturerWarranty = product.ManufacturerWarranty ? "+" : "-",
                MinAmount = product.MinAmount == null ? "" : product.MinAmount.Value.ToString("F2"),
                MaxAmount = product.MaxAmount == null ? "" : product.MaxAmount.Value.ToString("F2"),
                Multiplicity = product.Multiplicity.ToString("F5").Trim('0'),
                Bid = product.Bid.ToString(),
                YandexName = product.YandexName ?? string.Empty,
                ManualRatio = product.ManualRatio == null ? "" : product.ManualRatio.Value.ToString("F2")
            };

            if (fieldMapping.Contains(ProductFields.Producer))
            {
                productCsv.Producer = BrandService.BrandToString(product.BrandId);
            }

            if (fieldMapping.Contains(ProductFields.Category))
            {
                productCsv.Category = CategoryService.GetCategoryStringByProductId(product.ProductId, advancedSettings.CsvColumSeparator);
            }

            //if (fieldMapping.Contains(ProductFields.Sorting))
            if (advancedSettings.CsvCategorySort)
            {
                productCsv.Sorting = CategoryService.GetCategoryStringByProductId(product.ProductId, advancedSettings.CsvColumSeparator, true);
            }

            if (fieldMapping.Contains(ProductFields.Markers))
            {
                productCsv.Markers = ProductService.MarkersToString(product.BestSeller, product.New,
                    product.Recomended, product.OnSale, advancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.Photos))
            {
                productCsv.Photos =
                    PhotoService.PhotoToString(
                        PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product),
                        advancedSettings.CsvColumSeparator, advancedSettings.CsvPropertySeparator);
            }

            if (fieldMapping.Contains(ProductFields.Videos))
            {
                var videos = ProductVideoService.GetProductVideos(product.ProductId);
                productCsv.Videos = ProductVideoService.VideoToString(videos);
            }

            if (fieldMapping.Contains(ProductFields.Properties))
            {
                productCsv.Properties =
                    PropertyService.PropertiesToString(
                        PropertyService.GetPropertyValuesByProductId(product.ProductId),
                        advancedSettings.CsvColumSeparator, advancedSettings.CsvPropertySeparator);
            }

            if (fieldMapping.Contains(ProductFields.Related))
            {
                productCsv.Related =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Related, advancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.Alternative))
            {
                productCsv.Alternative =
                    ProductService.LinkedProductToString(product.ProductId,
                        RelatedType.Alternative, advancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.CustomOption))
            {
                productCsv.CustomOption =
                    CustomOptionsService.CustomOptionsToString(
                        CustomOptionsService.GetCustomOptionsByProductId(product.ProductId));
            }

            if (fieldMapping.Contains(ProductFields.Gifts))
            {
                productCsv.Gifts = OfferService.ProductGiftsToString(product.ProductId,
                    advancedSettings.CsvColumSeparator);
            }

            if (fieldMapping.Contains(ProductFields.Title) ||
                fieldMapping.Contains(ProductFields.H1) ||
                fieldMapping.Contains(ProductFields.MetaKeywords) ||
                fieldMapping.Contains(ProductFields.MetaDescription))
            {
                var meta = MetaInfoService.GetMetaInfo(productCsv.ProductId, MetaType.Product) ??
                           new MetaInfo(0, 0, MetaType.Product, string.Empty, string.Empty, string.Empty, string.Empty);

                productCsv.Title = meta.Title;
                productCsv.H1 = meta.H1;
                productCsv.MetaKeywords = meta.MetaKeywords;
                productCsv.MetaDescription = meta.MetaDescription;
            }

            if (fieldMapping.Contains(ProductFields.Price) ||
                fieldMapping.Contains(ProductFields.PurchasePrice) ||
                fieldMapping.Contains(ProductFields.Amount) ||
                fieldMapping.Contains(ProductFields.MultiOffer))
            {
                if (commonSettings.PriceMarginInPercents != 0 || commonSettings.PriceMarginInNumbers != 0)
                {
                    foreach (var offer in product.Offers)
                    {
                        var markup = offer.BasePrice * commonSettings.PriceMarginInPercents / 100 + commonSettings.PriceMarginInNumbers;

                        offer.BasePrice += markup;
                    }
                }

                if (!product.Offers.Any(offer => offer.ColorID.HasValue || offer.SizeID.HasValue || offer.ArtNo != product.ArtNo) && !advancedSettings.AllOffersToMultiOfferColumn)
                {
                    var offer = product.Offers.FirstOrDefault() ?? new Offer();
                    productCsv.Price = offer.BasePrice.ToString("F2");
                    productCsv.PurchasePrice = offer.SupplyPrice.ToString("F2");
                    productCsv.Amount = offer.Amount.ToString(CultureInfo.InvariantCulture);
                    productCsv.MultiOffer = string.Empty;
                }
                else
                {
                    productCsv.Price = string.Empty;
                    productCsv.PurchasePrice = string.Empty;
                    productCsv.Amount = string.Empty;
                    productCsv.MultiOffer = OfferService.OffersToString(product.Offers,
                        advancedSettings.CsvColumSeparator, advancedSettings.CsvPropertySeparator);
                }
            }

            if (fieldMapping.Contains(ProductFields.Currency))
            {
                productCsv.Currency = CurrencyService.GetCurrency(product.CurrencyID).Iso3;
            }

            if (fieldMapping.Contains(ProductFields.Tags) &&
                (!SaasDataService.IsSaasEnabled || SaasDataService.CurrentSaasData.HaveTags))
            {
                var tags = TagService.Gets(product.ProductId, ETagType.Product);
                if (tags != null)
                {
                    productCsv.Tags =
                        tags.Select(item => item.Name).AggregateString(advancedSettings.CsvColumSeparator);
                }
            }

            if (fieldMapping.Contains(ProductFields.Tax))
            {
                if (product.TaxId != null)
                {
                    var tax = TaxService.GetTax(product.TaxId.Value);
                    if (tax != null)
                        productCsv.Tax = tax.Name;
                }
            }

            if (fieldMapping.Contains(ProductFields.PaymentMethodType))
            {
                productCsv.PaymentMethodType = product.PaymentMethodType.ToString();
            }

            if (fieldMapping.Contains(ProductFields.PaymentSubjectType))
            {
                productCsv.PaymentSubjectType = product.PaymentSubjectType.ToString();
            }

            //if (fieldMapping.Contains(ProductFields.Store) ||
            //    fieldMapping.Contains(ProductFields.Funnel) ||
            //    fieldMapping.Contains(ProductFields.Vk) ||
            //    fieldMapping.Contains(ProductFields.Instagram) ||
            //    fieldMapping.Contains(ProductFields.Yandex) ||
            //    fieldMapping.Contains(ProductFields.Avito) ||
            //    fieldMapping.Contains(ProductFields.Google) ||
            //    fieldMapping.Contains(ProductFields.Facebook) ||
            //    fieldMapping.Contains(ProductFields.Bonus) ||
            //    fieldMapping.Contains(ProductFields.Referal))
            //{
            //    var productEnableInSalesChannel = SalesChannelService.GetExcludedProductSalesChannelList(product.ProductId);
            //    productCsv.Store = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Store.ToString()) ? "-" : "+";
            //    productCsv.Funnel = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Funnel.ToString()) ? "-" : "+";
            //    productCsv.Vk = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Vk.ToString()) ? "-" : "+";
            //    productCsv.Instagram = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Instagram.ToString()) ? "-" : "+";
            //    productCsv.Yandex = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Yandex.ToString()) ? "-" : "+";
            //    productCsv.Avito = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Avito.ToString()) ? "-" : "+";
            //    productCsv.Google = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Google.ToString()) ? "-" : "+";
            //    productCsv.Facebook = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Facebook.ToString()) ? "-" : "+";
            //    productCsv.Bonus = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Bonus.ToString()) ? "-" : "+";
            //    productCsv.Referal = productEnableInSalesChannel.Any(item => item == ESalesChannelsKeys.Referal.ToString()) ? "-" : "+";
            //}

            return productCsv;
        }
    }
}