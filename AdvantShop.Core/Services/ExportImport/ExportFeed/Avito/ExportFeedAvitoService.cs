//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.FilePath;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace AdvantShop.ExportImport
{
    public class ExportFeedAvitoService
    {
        public static IEnumerable<ExportFeedCategories> GetCategories(int exportFeedId, bool exportNotAvailable)
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
                new SqlParameter("@exportNotAvailable", exportNotAvailable));
        }

        public static IEnumerable<ExportFeedProductModel> GetProducts(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedAvitoOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteReadIEnumerable(
                 "[Settings].[sp_GetCsvProducts]",
                  CommandType.StoredProcedure,
                  reader => GetAvitoProductsFromReader(reader, commonSettings),
                  new SqlParameter("@exportFeedId", exportFeedId),
                  new SqlParameter("@onlyCount", false),
                  new SqlParameter("@exportNoInCategory", false),
                  new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                  new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                  new SqlParameter("@exportAdult", commonSettings.ExportAdult)
                  );
        }

        private static ExportFeedAvitoProduct GetAvitoProductsFromReader(SqlDataReader reader, ExportFeedSettings commonSettings)
        {
            var product = ProductService.GetProductFromReader(reader);
            var offerForExport = OfferService.GetMainOfferForExport(product.ProductId);

            var productCsv = new ExportFeedAvitoProduct
            {
                ProductId = product.ProductId,
                ArtNo = product.ArtNo,
                Name = product.Name,
                UrlPath = product.UrlPath,
                Enabled = product.Enabled ? "+" : "-",
                Unit = product.Unit,
                ShippingPrice = product.ShippingPrice == null ? "" : product.ShippingPrice.Value.ToString("F2"),
                YandexDeliveryDays = product.YandexDeliveryDays == null ? "" : product.YandexDeliveryDays,
                Discount = product.Discount.Type == DiscountType.Percent ? product.Discount.Percent : 0,
                DiscountAmount = product.Discount.Type == DiscountType.Amount ? product.Discount.Amount : 0,
                Weight = (offerForExport != null ? offerForExport.GetWeight() : 0).ToString("F2"),
                Size = (offerForExport != null ? offerForExport.GetDimensions() : "0x0x0"),
                BarCode = offerForExport != null ? offerForExport.BarCode : string.Empty,
                BriefDescription = product.BriefDescription,
                Description = product.Description,
                OrderByRequest = product.AllowPreOrder ? "+" : "-",
                SalesNote = product.SalesNote,
                Gtin = product.Gtin,
                GoogleProductCategory = product.GoogleProductCategory,
                YandexProductCategory = product.YandexMarketCategory,
                YandexTypePrefix = product.YandexTypePrefix,
                YandexModel = product.YandexModel,
                YandexSizeUnit = product.YandexSizeUnit,
                Adult = product.Adult ? "+" : "-",
                ManufacturerWarranty = product.ManufacturerWarranty ? "+" : "-",
                MinAmount = product.MinAmount == null ? "" : product.MinAmount.Value.ToString("F2"),
                MaxAmount = product.MaxAmount == null ? "" : product.MaxAmount.Value.ToString("F2"),
                Multiplicity = product.Multiplicity.ToString("F5").Trim('0'),
                Bid = product.Bid.ToString(),
                YandexName = product.YandexName ?? string.Empty,
                Properties = product.ProductPropertyValues
                    .Select(x => 
                     String.Format("{0}: {1}", 
                     x.Property.NameDisplayed, 
                     x.Value)).AggregateString(",")
            };

            productCsv.Producer = BrandService.BrandToString(product.BrandId);

            foreach (var photo in PhotoService.GetPhotos<ProductPhoto>(product.ProductId, PhotoType.Product))
            {
                productCsv.Photos += FoldersHelper.GetImageProductPath(ProductImageType.Big, photo.PhotoName, false) + ",";
                productCsv.PhotosIds += photo.PhotoId + ",";
            }

            var firstOffer = product.Offers.FirstOrDefault() ?? new Offer();
            productCsv.Price = firstOffer.BasePrice;
            productCsv.PurchasePrice = firstOffer.SupplyPrice;
            productCsv.Amount = firstOffer.Amount;

            productCsv.Colors = string.Empty;
            productCsv.Sizes = string.Empty;

            foreach (var offer in product.Offers)
            {
                if (offer.Color != null && !productCsv.Colors.Contains(offer.Color.ColorName + ";"))
                {
                    productCsv.Colors += offer.Color.ColorName + ";";
                }
                if (offer.Size != null && !productCsv.Sizes.Contains(offer.Size.SizeName + ";"))
                {
                    productCsv.Sizes += offer.Size.SizeName + ";";
                }
            }

            productCsv.Currency = CurrencyService.GetCurrency(product.CurrencyID, true).Iso3;

            return productCsv;
        }

        public static int GetProductsCount(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedAvitoOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteScalar<int>(
                "[Settings].[sp_GetCsvProducts]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@onlyCount", true),
                new SqlParameter("@exportNoInCategory", false),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                new SqlParameter("@exportAdult", commonSettings.ExportAdult)
                );
        }

        public static int GetCategoriesCount(int exportFeedId, bool exportNotAvailable)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedCategories]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@onlyCount", true),
                new SqlParameter("@exportNotAvailable", exportNotAvailable));
        }

        public static List<ExportFeedAvitoProductProperty> GetProductProperties(int productId)
        {
            return SQLDataAccess.ExecuteReadList<ExportFeedAvitoProductProperty>("Select * From [Catalog].[AvitoProductProperties] Where ProductId = @ProductId",
                CommandType.Text,
                (reader) =>
                {
                    return new ExportFeedAvitoProductProperty
                    {
                        ProductId = SQLDataHelper.GetInt(reader, "ProductId"),
                        PropertyName = SQLDataHelper.GetString(reader, "PropertyName"),
                        PropertyValue = SQLDataHelper.GetString(reader, "PropertyValue")
                    };
                },
                new SqlParameter("@ProductId", productId));
        }

        public static void DeleteProductProperties(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Delete From [Catalog].[AvitoProductProperties] Where ProductId = @ProductId",
                CommandType.Text,
                new SqlParameter("@ProductId", productId));
        }

        public static bool AddProductProperties(List<ExportFeedAvitoProductProperty> productProperties)
        {
            if (productProperties == null || (productProperties.GroupBy(item => item.PropertyName).Any(item => item.Count() > 1)))
            {
                return false;
            }

            foreach (var productProperty in productProperties)
            {
                if (string.IsNullOrEmpty(productProperty.PropertyName) || string.IsNullOrEmpty(productProperty.PropertyValue))
                {
                    continue;
                }

                SQLDataAccess.ExecuteNonQuery(
                    "Insert Into [Catalog].[AvitoProductProperties] (ProductId, PropertyName, PropertyValue) Values (@ProductId,@PropertyName,@PropertyValue)",
                    CommandType.Text,
                    new SqlParameter("@ProductId", productProperty.ProductId),
                    new SqlParameter("@PropertyName", productProperty.PropertyName),
                    new SqlParameter("@PropertyValue", productProperty.PropertyValue));
            }

            return true;
        }

        public static void ImportProductProperties(int productId, string productProperties, string columSeparator, string propertySeparator)
        {
            DeleteProductProperties(productId);

            var items = productProperties.Split(new[] { columSeparator }, StringSplitOptions.RemoveEmptyEntries);
            var avitoProductProperties = new List<ExportFeedAvitoProductProperty>();

            foreach (string s in items)
            {
                var temp = s.SupperTrim().Split(new[] { propertySeparator }, StringSplitOptions.RemoveEmptyEntries);
                if (temp.Length == 2)
                {
                    avitoProductProperties.Add(new ExportFeedAvitoProductProperty
                    {
                        ProductId = productId,
                        PropertyName = temp[0].SupperTrim(),
                        PropertyValue = temp[1].SupperTrim()
                    });
                }
            }

            if (avitoProductProperties.Count != 0)
            {
                AddProductProperties(avitoProductProperties);
            }
        }

        public static string GetProductPropertiesInString(int productId, string columSeparator, string propertySeparator)
        {
            var productProperties = GetProductProperties(productId);
            var stringProperties = string.Empty;
            for (int i = 0; i < productProperties.Count; i++)
            {
                if (i == 0)
                {
                    stringProperties += productProperties[i].PropertyName + propertySeparator + productProperties[i].PropertyValue;
                }
                else
                {
                    stringProperties += columSeparator + productProperties[i].PropertyName + propertySeparator + productProperties[i].PropertyValue;
                }
            }
            return stringProperties;
        }
    }
}