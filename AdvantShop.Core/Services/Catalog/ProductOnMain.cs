//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.SQL;
using AdvantShop.Diagnostics;
using AdvantShop.Helpers;

namespace AdvantShop.Catalog
{
    public enum EProductOnMain
    {
        None = 0,
        Best = 1,
        New = 2,
        Sale = 3,
        List = 4,
        NewArrivals = 5
    }

    public static class ProductOnMain
    {
        public static List<int> GetProductIdByType(EProductOnMain type, bool withPositiveSortOrder = false)
        {
            string sqlCmd;
            switch (type)
            {
                case EProductOnMain.Best:
                    sqlCmd = "select ProductId from Catalog.Product where Bestseller=1" + (withPositiveSortOrder ? " and SortBestseller>=0" : string.Empty);
                    break;
                case EProductOnMain.New:
                    sqlCmd = "select ProductId from Catalog.Product where New=1" + (withPositiveSortOrder ? " and SortNew>=0" : string.Empty);
                    break;
                case EProductOnMain.Sale:
                    sqlCmd = "select ProductId from Catalog.Product where (Discount > 0 or DiscountAmount > 0)" + (withPositiveSortOrder ? " and SortDiscount>=0" : string.Empty);
                    break;
                default:
                    throw new NotImplementedException();
            }
            return SQLDataAccess.ExecuteReadColumn<int>(sqlCmd, CommandType.Text, "ProductId", new SqlParameter("@type", (int)type));
        }

        public static List<int> GetProductIdByType(EProductOnMain type, int count)
        {
            return CacheManager.Get(CacheNames.MainPageProductsCacheName(type.ToString(), count) + "_ids", 0, () =>
            {
                var sqlCmd =
                    "Select Top(@Count) [Product].[ProductID] " +
                    (SettingsCatalog.MoveNotAvaliableToEnd ? ",(CASE WHEN Price=0 THEN 0 ELSE 1 END) as TempSort, AmountSort as TempAmountSort " : "") +
                    "From [Catalog].[Product] " +
                    "Left Join [Catalog].[ProductExt]  On [Product].[ProductID] = [ProductExt].[ProductID]  " +
                    "Left Join [Catalog].[Offer] On [ProductExt].[OfferID] = [Offer].[OfferID] " +

                    "Where Product.Enabled=1 and Product.Hidden=0 and CategoryEnabled=1 {0}  " +
                    (SettingsCatalog.ShowOnlyAvalible ? " AND MaxAvailable>0" : string.Empty) + " Order by {2}{1}";

                var moveNotAvaliableToEnd = SettingsCatalog.MoveNotAvaliableToEnd ? "TempSort desc, TempAmountSort desc," : "";
                switch (type)
                {
                    case EProductOnMain.Best:
                        sqlCmd = string.Format(sqlCmd, "and Bestseller=1", "SortBestseller, Product.ProductId desc", moveNotAvaliableToEnd);
                        break;

                    case EProductOnMain.New:
                        sqlCmd = string.Format(sqlCmd, "and New=1", "SortNew, Product.ProductId desc", moveNotAvaliableToEnd);
                        break;
                    case EProductOnMain.NewArrivals:
                        sqlCmd = string.Format(sqlCmd, "", "Product.ProductId desc", moveNotAvaliableToEnd);
                        break;

                    case EProductOnMain.Sale:
                        sqlCmd = string.Format(sqlCmd, "and (Discount>0 or DiscountAmount>0)", "SortDiscount, ProductId desc", moveNotAvaliableToEnd);
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return
                    SQLDataAccess.ExecuteReadList(sqlCmd, CommandType.Text,
                        reader => SQLDataHelper.GetInt(reader, "ProductId"), new SqlParameter("@Count", count))
                        .ToList();
            });
        }

        public static List<ProductModel> GetProductsByType(EProductOnMain type, int count)
        {
            try
            {
                return CacheManager.Get(CacheNames.MainPageProductsCacheName(type.ToString(), count), () =>
                {
                    var sqlCmd =
                        ";with cte (ProductId) as (select top(@Count) [Product].ProductId " +
                        "from catalog.Product " +
                        "inner join [Catalog].[ProductExt] on [ProductExt].ProductId = Product.ProductId " +
                        "where Product.Enabled = 1 and Product.Hidden = 0 and CategoryEnabled = 1 {0} " +
                        (SettingsCatalog.ShowOnlyAvalible ? " AND MaxAvailable>0 " : "") +
                        "Order by {2}{1})" +

                        "Select [Product].[ProductID], Product.ArtNo, Product.Name, Recomended as Recomend, Bestseller, New, OnSale as Sales, Discount, DiscountAmount, " +
                        "Product.Enabled, Product.UrlPath, AllowPreOrder, Ratio, ManualRatio, Offer.OfferID, MaxAvailable AS Amount, MinAmount, MaxAmount, Offer.Amount AS AmountOffer, MinPrice as BasePrice," +
                        "CountPhoto, Photo.PhotoId, PhotoName, PhotoNameSize1, PhotoNameSize2, Photo.Description as PhotoDescription, Offer.ColorID, Product.DateAdded, NotSamePrices as MultiPrices," +
                        "null as AdditionalPhoto, " +
                        (SettingsCatalog.ComplexFilter ? "Colors," : "null as Colors,") +
                        "CurrencyValue, Comments, Gifts " +
                        "From cte inner join [Catalog].[Product] on cte.ProductId = [Product].ProductId " +
                        "Left Join [Catalog].[ProductExt]  On [Product].[ProductID] = [ProductExt].[ProductID]  " +
                        "Inner Join Catalog.Currency On Currency.CurrencyID = Product.CurrencyID " +
                        "Left Join [Catalog].[Photo] On [Photo].[PhotoId] = [ProductExt].[PhotoId] And Type=@Type " +
                        "Left Join [Catalog].[Offer] On [ProductExt].[OfferID] = [Offer].[OfferID] " +
                        "Order by {2}{1}";

                    var moveNotAvaliableToEnd = SettingsCatalog.MoveNotAvaliableToEnd ? "(CASE WHEN PriceTemp=0 THEN 0 ELSE 1 END) desc, AmountSort desc," : "";
                    switch (type)
                    {
                        case EProductOnMain.Best:
                            sqlCmd = string.Format(sqlCmd, "and Bestseller=1", "SortBestseller, Product.ProductId desc", moveNotAvaliableToEnd);
                            break;

                        case EProductOnMain.New:
                            sqlCmd = string.Format(sqlCmd, "and New=1", "SortNew, Product.ProductId desc", moveNotAvaliableToEnd);
                            break;
                        case EProductOnMain.NewArrivals:
                            sqlCmd = string.Format(sqlCmd, "", "Product.ProductId desc", moveNotAvaliableToEnd);
                            break;

                        case EProductOnMain.Sale:
                            sqlCmd = string.Format(sqlCmd, "and (Discount>0 or DiscountAmount>0)", "SortDiscount, ProductId desc", moveNotAvaliableToEnd);
                            break;

                        default:
                            throw new NotImplementedException();
                    }

                    return
                         SQLDataAccess.Query<ProductModel>(sqlCmd, new { count, Type = PhotoType.Product.ToString() })
                             .ToList();
                });
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            return null;
        }


        public static DataTable GetAdminProductsByType(EProductOnMain type, int count)
        {
            string sqlCmd;
            switch (type)
            {
                case EProductOnMain.Best:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where Bestseller=1 order by SortBestseller";
                    break;
                case EProductOnMain.New:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where New=1 order by SortNew";
                    break;
                case EProductOnMain.Sale:
                    sqlCmd = "select Top(@count) Product.ProductId, Name from Catalog.Product where (Discount > 0 or DiscountAmount > 0) order by SortDiscount";
                    break;
                default:
                    throw new NotImplementedException();
            }
            return SQLDataAccess.ExecuteTable(sqlCmd, CommandType.Text, new SqlParameter { ParameterName = "@count", Value = count });
        }

        public static int GetProductCountByType(EProductOnMain type, bool enabled = true)
        {
            return CacheManager.Get(CacheNames.MainPageProductsCountCacheName(type.ToString(), enabled), () =>
            {
                var sql = "select Count(ProductId) from Catalog.Product where " +
                          (enabled ? "Enabled=1 and Hidden=0 and CategoryEnabled=1 and" : "");

                switch (type)
                {
                    case EProductOnMain.Best:
                        sql += " bestseller=1";
                        break;
                    case EProductOnMain.New:
                        sql += " new=1";
                        break;
                    case EProductOnMain.NewArrivals:
                        sql += " new=0";
                        break;
                    case EProductOnMain.Sale:
                        sql += " (Discount > 0 or DiscountAmount > 0)";
                        break;
                    default:
                        throw new NotImplementedException();
                }
                return SQLDataAccess.ExecuteScalar<int>(sql, CommandType.Text);
            });
        }

        public static bool IsExistsProductByType(EProductOnMain type)
        {
            string sqlCmd = "if exists(select 1 from Catalog.Product where Enabled=1 and Hidden=0 and CategoryEnabled=1 and {0}) Select 1 else Select 0";
            switch (type)
            {
                case EProductOnMain.Best:
                    sqlCmd = string.Format(sqlCmd, "bestseller=1");
                    break;
                case EProductOnMain.New:
                    sqlCmd = string.Format(sqlCmd, "new=1");
                    break;
                case EProductOnMain.Sale:
                    sqlCmd = string.Format(sqlCmd, "(Discount > 0 or DiscountAmount > 0)");
                    break;
                default:
                    throw new NotImplementedException();
            }
            return Convert.ToBoolean(SQLDataAccess.ExecuteScalar(sqlCmd, CommandType.Text));
        }


        public static void AddProductByType(int productId, EProductOnMain type)
        {
            string sqlCmd;
            switch (type)
            {
                case EProductOnMain.Best:
                    sqlCmd = "Update Catalog.Product set SortBestseller=(Select min(SortBestseller)-10 from Catalog.Product), Bestseller=1 where ProductId=@productId";
                    break;
                case EProductOnMain.New:
                    sqlCmd = "Update Catalog.Product set SortNew=(Select min(SortNew)-10 from Catalog.Product), New=1 where ProductId=@productId";
                    break;
                case EProductOnMain.Sale:
                    sqlCmd = "Update Catalog.Product set SortDiscount=(Select min(SortDiscount)-10 from Catalog.Product) where ProductId=@productId";
                    break;
                default:
                    throw new NotImplementedException();
            }
            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text, new SqlParameter("@productId", productId));

            ClearCache();
        }

        public static void DeleteProductByType(int prodcutId, EProductOnMain type)
        {
            string sqlCmd;
            switch (type)
            {
                case EProductOnMain.Best:
                    sqlCmd = "Update Catalog.Product set SortBestseller=0, Bestseller=0 where ProductId=@productId";
                    break;
                case EProductOnMain.New:
                    sqlCmd = "Update Catalog.Product set SortNew=0, New=0 where ProductId=@productId";
                    break;
                case EProductOnMain.Sale:
                    sqlCmd = "Update Catalog.Product set SortDiscount=0, Discount=0, DiscountAmount=0 where ProductId=@productId";
                    break;
                default:
                    throw new NotImplementedException();
            }

            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text, new SqlParameter("@productId", prodcutId));

            ClearCache();
        }

        public static void UpdateProductByType(int productId, int sortOrder, EProductOnMain type)
        {
            string sqlCmd;
            switch (type)
            {
                case EProductOnMain.Best:
                    sqlCmd = "Update Catalog.Product set SortBestseller=@sortOrder where ProductId=@productId and Bestseller=1";
                    break;
                case EProductOnMain.New:
                    sqlCmd = "Update Catalog.Product set SortNew=@sortOrder where ProductId=@productId and New=1";
                    break;
                case EProductOnMain.Sale:
                    sqlCmd = "Update Catalog.Product set SortDiscount=@sortOrder where ProductId=@productId";
                    break;
                default:
                    throw new NotImplementedException();
            }

            SQLDataAccess.ExecuteNonQuery(sqlCmd, CommandType.Text, new SqlParameter("@productId", productId), new SqlParameter("@sortOrder", sortOrder));

            ClearCache();
        }

        public static void ClearCache()
        {
            CacheManager.RemoveByPattern(CacheNames.SQLPagingItems);
            CacheManager.RemoveByPattern(CacheNames.SQLPagingCount);
        }

        public static void ShuffleLists()
        {
            if (SettingsCatalog.ShuffleBestOnMainPage)
            {
                ShuffleList(EProductOnMain.Best);
            }

            if (SettingsCatalog.ShuffleNewOnMainPage)
            {
                ShuffleList(EProductOnMain.New);
            }

            if (SettingsCatalog.ShuffleSalesOnMainPage)
            {
                ShuffleList(EProductOnMain.Sale);
            }

            foreach (var list in ProductListService.GetList().Where(x => x.ShuffleList))
            {
                ShuffleList(EProductOnMain.List, list.Id);
            }
        }

        private static void ShuffleList(EProductOnMain type, int? id = null)
        {
            var productIds = new List<int>();
            switch (type)
            {
                case EProductOnMain.Best:
                case EProductOnMain.New:
                case EProductOnMain.Sale:
                    productIds = GetProductIdByType(type, true);
                    break;

                case EProductOnMain.List:
                    if (id.HasValue)
                        productIds = ProductListService.GetProductIds(id.Value, true);
                    break;

                default:
                    throw new NotImplementedException();
            }

            if (productIds.Count == 0)
                return;

            Random rnd = new Random();

            var sortOrder = 0;
            foreach (var productId in productIds.OrderBy(x => rnd.Next()))
            {
                switch (type)
                {
                    case EProductOnMain.Best:
                    case EProductOnMain.New:
                    case EProductOnMain.Sale:
                        UpdateProductByType(productId, sortOrder, type);
                        break;

                    case EProductOnMain.List:
                        if (id.HasValue)
                            ProductListService.UpdateProduct(id.Value, productId, sortOrder);
                        break;

                    default:
                        throw new NotImplementedException();
                }
                sortOrder += 10;
            }
        }
    }
}