using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.ExportImport
{
    public class ExportFeedFacebookService
    {
        public static IEnumerable<ExportFeedProductModel> GetProducts(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedFacebookOptions advancedSettings)
        {
            return
                SQLDataAccess.ExecuteReadIEnumerable<ExportFeedFacebookProduct>(
                    "[Settings].[sp_GetExportFeedProducts]",
                    CommandType.StoredProcedure,
                    60 * 3,
                    reader => new ExportFeedFacebookProduct
                    {
                        ProductId = SQLDataHelper.GetInt(reader, "ProductID"),
                        OfferId = SQLDataHelper.GetInt(reader, "OfferId"),
                        ArtNo = SQLDataHelper.GetString(reader, "ArtNo"),
                        OfferArtNo = SQLDataHelper.GetString(reader, "OfferArtNo"),
                        Amount = SQLDataHelper.GetInt(reader, "Amount"),
                        UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        Price = SQLDataHelper.GetFloat(reader, "Price"),
                        ShippingPrice = SQLDataHelper.GetFloat(reader, "ShippingPrice"),
                        Discount = SQLDataHelper.GetFloat(reader, "Discount"),
                        DiscountAmount = SQLDataHelper.GetFloat(reader, "DiscountAmount"),
                        ParentCategory = SQLDataHelper.GetInt(reader, "ParentCategory"),
                        Name = SQLDataHelper.GetString(reader, "Name"),
                        Description = SQLDataHelper.GetString(reader, "Description"),
                        BriefDescription = SQLDataHelper.GetString(reader, "BriefDescription"),
                        Photos = SQLDataHelper.GetString(reader, "Photos"),
                        ColorId = SQLDataHelper.GetInt(reader, "ColorId"),
                        ColorName = SQLDataHelper.GetString(reader, "ColorName"),
                        SizeId = SQLDataHelper.GetInt(reader, "SizeId"),
                        SizeName = SQLDataHelper.GetString(reader, "SizeName"),
                        BrandName = SQLDataHelper.GetString(reader, "BrandName"),
                        Main = SQLDataHelper.GetBoolean(reader, "Main"),
                        GoogleProductCategory = SQLDataHelper.GetString(reader, "GoogleProductCategory"),
                        Gtin = SQLDataHelper.GetString(reader, "Gtin"),
                        Adult = SQLDataHelper.GetBoolean(reader, "Adult"),
                        CurrencyValue = SQLDataHelper.GetFloat(reader, "CurrencyValue"),
                        AllowPreorder = SQLDataHelper.GetBoolean(reader, "AllowPreOrder")
                    },
                    new SqlParameter("@exportFeedId", exportFeedId),
                    new SqlParameter("@selectedCurrency", advancedSettings.Currency),
                    new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                    new SqlParameter("@allowPreOrder", advancedSettings.AllowPreOrderProducts),
                    new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                    new SqlParameter("@onlyMainOfferToExport", advancedSettings.OnlyMainOfferToExport),
                    new SqlParameter("@sqlMode", FacebookExportSQLMode.GetProducts.ToString()),
                    new SqlParameter("@exportAdult", commonSettings.ExportAdult));
        }

        public static int GetProductsCount(int exportFeedId, ExportFeedSettings commonSettings, ExportFeedFacebookOptions advancedSettings)
        {
            return SQLDataAccess.ExecuteScalar<int>("[Settings].[sp_GetExportFeedProducts]",
                CommandType.StoredProcedure,
                60 * 3,
                new SqlParameter("@exportFeedId", exportFeedId),
                new SqlParameter("@selectedCurrency", advancedSettings.Currency),
                new SqlParameter("@exportNotAvailable", advancedSettings.ExportNotAvailable),
                new SqlParameter("@allowPreOrder", advancedSettings.AllowPreOrderProducts),
                new SqlParameter("@exportAllProducts", commonSettings.ExportAllProducts),
                new SqlParameter("@onlyMainOfferToExport", advancedSettings.OnlyMainOfferToExport),
                new SqlParameter("@sqlMode", FacebookExportSQLMode.GetCountOfProducts.ToString()),
                new SqlParameter("@exportAdult", commonSettings.ExportAdult)
            );
        }

        private enum FacebookExportSQLMode
        {
            GetProducts,
            GetCountOfProducts,
        }
    }
}