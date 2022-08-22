//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using AdvantShop.Catalog;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.ExportImport
{
    public class ExportFeedYandexService
    {
        public static IEnumerable<ExportFeedCategories> GetCategories(int exportFeedId, bool exportNotAvailable)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<ExportFeedCategories>("[Settings].[sp_GetExportFeedCategories]",
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

        public static IEnumerable<ExportFeedProductModel> GetProducts(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedYandexOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteReadIEnumerable<ExportFeedYandexProduct>(
                "[Settings].[sp_GetExportFeedProducts]",
                CommandType.StoredProcedure,
                60 * 3,
                reader => new ExportFeedYandexProduct
                {
                    ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                    OfferId = SQLDataHelper.GetInt(reader, "OfferId"),
                    ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                    OfferArtNo = SQLDataHelper.GetString(reader, "OfferArtNo"),
                    Amount = SQLDataHelper.GetInt(reader, "Amount"),
                    UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                    Price = SQLDataHelper.GetFloat(reader, "Price"),
                    ShippingPrice = SQLDataHelper.GetNullableFloat(reader, "ShippingPrice"),
                    Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                    DiscountAmount = SQLDataHelper.GetFloat(reader, "DiscountAmount"),
                    ParentCategory = SQLDataHelper.GetInt(reader, "ParentCategory"),
                    Name = SQLDataHelper.GetString(reader, "Name"),
                    Description = SQLDataHelper.GetString(reader, "Description"),
                    BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                    Photos = SQLDataHelper.GetString(reader, "Photos"),
                    SalesNote = SQLDataHelper.GetString(reader, "SalesNote"),
                    ColorId = SQLDataHelper.GetInt(reader, "ColorId"),
                    ColorName = SQLDataHelper.GetString(reader, "ColorName"),
                    SizeId = SQLDataHelper.GetInt(reader, "SizeId"),
                    SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                    BrandName = SQLDataHelper.GetString(reader, "BrandName"),
                    Main = SQLDataHelper.GetBoolean(reader, "Main"),
                    YandexTypePrefix = SQLDataHelper.GetString(reader, "YandexTypePrefix"),
                    YandexModel = SQLDataHelper.GetString(reader, "YandexModel"),
                    Adult = SQLDataHelper.GetBoolean(reader, "Adult"),
                    CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                    ManufacturerWarranty = SQLDataHelper.GetBoolean(reader, "ManufacturerWarranty"),
                    YandexName = SQLDataHelper.GetString(reader, "YandexName"),
                    YandexDeliveryDays = SQLDataHelper.GetString(reader, "YandexDeliveryDays"),

                    Enabled = SQLDataHelper.GetBoolean(reader, "Enabled"),
                    Weight = SQLDataHelper.GetFloat(reader, "Weight"),
                    SupplyPrice = SQLDataHelper.GetFloat(reader, "SupplyPrice"),
                    BrandCountry = SQLDataHelper.GetString(reader, "BrandCountry"),
                    BrandCountryManufacture = SQLDataHelper.GetString(reader, "BrandCountryManufacture"),
                    BarCode = SQLDataHelper.GetString(reader, "BarCode"),
                    Bid = SQLDataHelper.GetFloat(reader, "Bid"),
                    YandexSizeUnit = SQLDataHelper.GetString(reader, "YandexSizeUnit"),
                    AllowPreorder = SQLDataHelper.GetBoolean(reader, "AllowPreorder"),
                    Multiplicity = SQLDataHelper.GetFloat(reader, "Multiplicity"),
                    MinAmount = SQLDataHelper.GetFloat(reader, "MinAmount"),
                    Length = SQLDataHelper.GetNullableFloat(reader, "Length"),
                    Height = SQLDataHelper.GetNullableFloat(reader, "Height"),
                    Width = SQLDataHelper.GetNullableFloat(reader, "Width"),
                    YandexProductDiscounted = SQLDataHelper.GetBoolean(reader, "YandexProductDiscounted"),
                    YandexProductDiscountCondition = SQLDataHelper.GetString(reader, "YandexProductDiscountCondition").TryParseEnum<EYandexDiscountCondition>(),
                    YandexProductDiscountReason = SQLDataHelper.GetString(reader, "YandexProductDiscountReason")
                },
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@selectedCurrency", advancedSettings.Currency),
                new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                new SqlParameter("@allowPreOrder", advancedSettings.AllowPreOrderProducts ?? false),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@onlyMainOfferToExport", advancedSettings.OnlyMainOfferToExport),
                new SqlParameter("@sqlMode", YandexExportSQLMode.GetProducts.ToString()),
                new SqlParameter("@exportAdult", commonSettings.ExportAdult));
        }

        public static HashSet<int> GetOfferIds(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedYandexOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteReadHashSet<int>(
                "[Settings].[sp_GetExportFeedProducts]",
                CommandType.StoredProcedure,
                60 * 3,
                reader => SQLDataHelper.GetInt(reader, "OfferId"),
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@selectedCurrency", advancedSettings.Currency),
                new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                new SqlParameter("@allowPreOrder", advancedSettings.AllowPreOrderProducts ?? false),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@onlyMainOfferToExport", advancedSettings.OnlyMainOfferToExport),
                new SqlParameter("@sqlMode", YandexExportSQLMode.GetOfferIds.ToString()),
                new SqlParameter("@exportAdult", commonSettings.ExportAdult));
        }

        public static int GetProductsCount(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedYandexOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedProducts]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@selectedCurrency", advancedSettings.Currency),
                new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                new SqlParameter("@allowPreOrder", advancedSettings.AllowPreOrderProducts ?? false),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@onlyMainOfferToExport", advancedSettings.OnlyMainOfferToExport),
                new SqlParameter("@sqlMode", YandexExportSQLMode.GetCountOfProducts.ToString()),
                new SqlParameter("@exportAdult", commonSettings.ExportAdult));
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

        private enum YandexExportSQLMode
        {
            GetProducts,
            GetCountOfProducts,
            GetOfferIds
        }
    }
}