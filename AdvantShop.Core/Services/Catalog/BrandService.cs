//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Helpers;
using AdvantShop.SEO;
using System.Linq;
using AdvantShop.Core.SQL2;

namespace AdvantShop.Catalog
{

    public class BrandProductCount
    {
        public int InCategoryCount { set; get; }
        public int InChildsCategoryCount { set; get; }

        public int CategoryId { set; get; }
        public int ParentId { set; get; }
        public string Url { set; get; }
        public string Name { set; get; }
        public int Level { set; get; }
    }

    public class BrandService
    {
        private const string BrandIdCaheKey = "BrandId_";

        private static string GetBrandCategorySql(string sql, bool onlyVisibleCategories = false)
        {
            return @"with brandCategory as (
                        SELECT p.BrandId,
                                                pc.CategoryId,
                                                COUNT(*) AS [Count],
                                                COUNT(c.enabled) AS [CountDeep]
                                        FROM [Catalog].Product p
                                                INNER JOIN [Catalog].ProductCategories pc ON pc.ProductId = p.ProductID
                                                INNER JOIN [Catalog].[ProductExt] pExt ON p.ProductID = pExt.ProductID
                                                INNER JOIN [Catalog].Category c ON c.CategoryID = pc.CategoryId
                                        WHERE p.BrandId IS NOT NULL               
                                                AND p.[Enabled] = 1
                                                AND p.Hidden = 0
                                                AND p.CategoryEnabled = 1
                                                AND c.Enabled = 1 " + 
                                                (onlyVisibleCategories ? "AND c.Hidden = 0 " : "") + 
                                        @"GROUP BY pc.CategoryId,
                                                    p.BrandId
                        ) " + sql;
        }

        public static List<char> GetEngBrandChars()
        {
            return new List<char>(){ 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm',
                                         'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

        }

        public static List<char> GetRusBrandChars()
        {
            return new List<char>(){ 'а', 'б', 'в', 'г', 'д', 'е', 'ё', 'ж', 'з', 'и','й', 'к', 'л', 'м', 'н', 'о',
                                         'п', 'р', 'с', 'т', 'у', 'ф', 'х', 'ц', 'ч', 'ш', 'щ', 'э', 'ю', 'я'};
        }

        #region Get Add Update Delete

        public static void DeleteBrand(int brandId)
        {
            DeleteBrandLogo(brandId);
            SQLDataAccess.ExecuteNonQuery("Delete From Catalog.Brand Where BrandID=@BrandID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@BrandId", Value = brandId });
        }


        public static int AddBrand(Brand brand)
        {
            if (!GetBrandIdByName(brand.Name).IsDefault())
                return 0;

            brand.BrandId = SQLDataHelper.GetInt(SQLDataAccess.ExecuteScalar(
                "[Catalog].[sp_AddBrand]",
                CommandType.StoredProcedure,
                new SqlParameter("@BrandName", brand.Name),
                new SqlParameter("@BrandDescription", brand.Description ?? (object)DBNull.Value),
                new SqlParameter("@BrandBriefDescription", brand.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", brand.Enabled),
                new SqlParameter("@SortOrder", brand.SortOrder),
                new SqlParameter("@CountryID", brand.CountryId == 0 ? (object)DBNull.Value : brand.CountryId),
                new SqlParameter("@CountryOfManufactureID", brand.CountryOfManufactureId == 0 ? (object)DBNull.Value : brand.CountryOfManufactureId),
                new SqlParameter("@UrlPath", brand.UrlPath),
                new SqlParameter("@BrandSiteUrl", brand.BrandSiteUrl.IsNotEmpty() ? brand.BrandSiteUrl : (object)DBNull.Value)
                ));

            if (brand.BrandId == 0)
                return 0;

            if (brand.Meta != null)
            {
                if (!brand.Meta.Title.IsNullOrEmpty() || !brand.Meta.MetaKeywords.IsNullOrEmpty() ||
                    !brand.Meta.MetaDescription.IsNullOrEmpty() || !brand.Meta.H1.IsNullOrEmpty())
                {
                    brand.Meta.ObjId = brand.BrandId;
                    MetaInfoService.SetMeta(brand.Meta);
                }
            }

            CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(BrandIdCaheKey);

            return brand.BrandId;
        }

        public static void UpdateBrand(Brand brand)
        {
            var existingBrand = GetBrandIdByName(brand.Name);
            if (existingBrand != 0 && (brand.BrandId == 0 || brand.BrandId != existingBrand))
                return;

            SQLDataAccess.ExecuteNonQuery(
                "[Catalog].[sp_UpdateBrandById]",
                CommandType.StoredProcedure,
                new SqlParameter("@BrandID", brand.BrandId),
                new SqlParameter("@BrandName", brand.Name),
                new SqlParameter("@BrandDescription", brand.Description ?? (object)DBNull.Value),
                new SqlParameter("@BrandBriefDescription", brand.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Enabled", brand.Enabled),
                new SqlParameter("@SortOrder", brand.SortOrder),
                new SqlParameter("@CountryID", brand.CountryId == 0 ? (object)DBNull.Value : brand.CountryId),
                new SqlParameter("@CountryOfManufactureID", brand.CountryOfManufactureId == 0 ? (object)DBNull.Value : brand.CountryOfManufactureId),
                new SqlParameter("@UrlPath", brand.UrlPath),
                new SqlParameter("@BrandSiteUrl", brand.BrandSiteUrl.IsNotEmpty() ? brand.BrandSiteUrl : (object)DBNull.Value)
                );

            if (brand.Meta != null)
            {
                if (brand.Meta.Title.IsNullOrEmpty() && brand.Meta.MetaKeywords.IsNullOrEmpty() && brand.Meta.MetaDescription.IsNullOrEmpty() && brand.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(brand.BrandId, MetaType.Brand))
                    {
                        MetaInfoService.DeleteMetaInfo(brand.BrandId, MetaType.Brand);
                    }
                }
                else
                    MetaInfoService.SetMeta(brand.Meta);
            }

            CacheManager.RemoveByPattern(CacheNames.BrandsInCategory);
            CacheManager.RemoveByPattern(CacheNames.MenuCatalog);
            CacheManager.RemoveByPattern(BrandIdCaheKey);
        }

        private static Brand GetBrandFromReader(SqlDataReader reader)
        {
            return new Brand
            {
                BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                Name = SQLDataHelper.GetString(reader, "BrandName"),
                Description = SQLDataHelper.GetString(reader, "BrandDescription", string.Empty),
                BriefDescription = SQLDataHelper.GetString(reader, "BrandBriefDescription", string.Empty),
                //BrandLogo = SQLDataHelper.GetString(reader, "BrandLogo"),
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID", 0),
                CountryOfManufactureId = SQLDataHelper.GetInt(reader, "CountryOfManufactureID", 0),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                BrandSiteUrl = SQLDataHelper.GetString(reader, "BrandSiteUrl")
            };
        }

        public static Brand GetBrandById(int brandId)
        {
            if (brandId == 0)
                return null;

            return SQLDataAccess.ExecuteReadOne<Brand>("Select * From Catalog.Brand where BrandID=@BrandID", CommandType.Text,
                                                            GetBrandFromReader, new SqlParameter("@BrandID", brandId));
        }

        public static Brand GetBrand(string url)
        {
            return SQLDataAccess.ExecuteReadOne("Select * From Catalog.Brand where UrlPath=@UrlPath", CommandType.Text,
                GetBrandFromReader, new SqlParameter("@UrlPath", url));
        }

        public static Brand GetBrandByName(string brandName)
        {
            return SQLDataAccess.ExecuteReadOne("SELECT * FROM Catalog.Brand WHERE LOWER ([BrandName])=@BrandName",
                CommandType.Text,
                GetBrandFromReader, new SqlParameter("@BrandName", brandName.ToLower()));
        }

        public static IEnumerable<int> GetAllBrandIDs(bool onlyDemo = false)
        {
            return SQLDataAccess.ExecuteReadColumn<int>(
                "SELECT BrandID FROM [Catalog].[Brand]" + (onlyDemo ? " WHERE IsDemo = 1" : string.Empty),
                CommandType.Text, "BrandID");
        }

        public static List<Brand> GetBrands()
        {
            return SQLDataAccess.ExecuteReadList<Brand>("Select * from Catalog.Brand order by BrandName", CommandType.Text, GetBrandFromReader);
        }

        public static List<Brand> GetBrandsOnLimit(int count)
        {
            return SQLDataAccess.ExecuteReadList<Brand>("SELECT TOP(@Count) * FROM Catalog.Brand WHERE enabled=1 AND EXISTS (Select 1 FROM Catalog.Photo WHERE Photo.ObjId=Brand.BrandID AND Type=@Type) ORDER BY SortOrder",
                CommandType.Text,
                GetBrandFromReader, 
                new SqlParameter("@Count", count),
                new SqlParameter("@Type", PhotoType.Brand.ToString()));
        }

        public static List<Brand> GetBrands(bool haveProducts, bool enabled = true)
        {
            var cmd = haveProducts
                ? "SELECT * FROM [Catalog].[Brand] left join Catalog.Photo on Photo.ObjId=Brand.BrandID and Type=@Type Where " + (enabled ? "enabled=1 and " : "") + "(SELECT COUNT(ProductID) From [Catalog].[Product] Where [Product].[BrandID]=Brand.[BrandID]) > 0 ORDER BY [SortOrder], [BrandName]"
                : "SELECT * FROM [Catalog].[Brand] left join Catalog.Photo on Photo.ObjId=Brand.BrandID and Type=@Type " + (enabled ? "Where enabled=1" : "") + " Order by [SortOrder], [BrandName]";
            return SQLDataAccess.ExecuteReadList(cmd, CommandType.Text, reader => new Brand
            {
                BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                Name = SQLDataHelper.GetString(reader, "BrandName"),
                Description = SQLDataHelper.GetString(reader, "BrandDescription", string.Empty),
                BriefDescription = SQLDataHelper.GetString(reader, "BrandBriefDescription", string.Empty),
                BrandLogo =
                    new BrandPhoto(SQLDataHelper.GetInt(reader, "PhotoId"), SQLDataHelper.GetInt(reader, "ObjId"))
                    { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") },
                Enabled = SQLDataHelper.GetBoolean(reader, "Enabled", true),
                SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                CountryId = SQLDataHelper.GetInt(reader, "CountryID", 0),
                CountryOfManufactureId = SQLDataHelper.GetInt(reader, "CountryOfManufactureID", 0),
                UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                BrandSiteUrl = SQLDataHelper.GetString(reader, "BrandSiteUrl")
            },
            new SqlParameter("@Type", PhotoType.Brand.ToString()));
        }
        
        public static List<Brand> GetBrandsBySearch(int limit, int currentPage, string q)
        {
            var paging = new Core.SQL2.SqlPaging(currentPage, limit);
            paging.Select("BrandId", "BrandName".AsSqlField("Name"));
            paging.From("Catalog.Brand");
            paging.Where("exists (Select 1 From Catalog.Product Where Product.BrandId = Brand.BrandId)");
            paging.OrderBy("BrandName");
            
            if (!string.IsNullOrWhiteSpace(q))
                paging.Where("BrandName like '%' + {0} + '%'", q);

            return paging.PageItemsList<Brand>();
        }

        #endregion

        #region ProductLinks

        public static void DeleteProductLink(int productId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalig.Product Set BrandID=Null Where ProductID=@ProductID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productId });
        }

        public static void AddProductLink(int productId, int brandId)
        {
            SQLDataAccess.ExecuteNonQuery("Update Catalog.Product Set BrandID=@BrandID Where ProductID=@ProductID", CommandType.Text,
                                            new SqlParameter { ParameterName = "@ProductID", Value = productId },
                                            new SqlParameter { ParameterName = "@BrandId", Value = brandId });
        }

        #endregion

        public static List<Brand> GetBrandsByProductOnMain(EProductOnMain type, int? listId)
        {
            var subCmd = string.Empty;
            switch (type)
            {
                case EProductOnMain.New:
                    subCmd = "New=1";
                    break;
                case EProductOnMain.Best:
                    subCmd = "Bestseller=1";
                    break;
                case EProductOnMain.Sale:
                    subCmd = "(Discount>0 or DiscountAmount>0)";
                    break;
                case EProductOnMain.List:
                    if (listId == null) return null;
                    subCmd =
                        "productid in (Select Productid from [Catalog].[Product_ProductList] where [Product_ProductList].listid = @listId)";
                    break;
                case EProductOnMain.NewArrivals:
                    return new List<Brand>();
            }

            var cmd =
                "Select BrandID, BrandName, UrlPath, SortOrder " +
                "from Catalog.Brand " +
                "where BrandID in (select BrandID from Catalog.Product where enabled=1 and categoryEnabled=1 and " + subCmd + " ) and enabled=1 " +
                "order by SortOrder, BrandName";

            return SQLDataAccess.ExecuteReadList(cmd, CommandType.Text,
                reader => new Brand
                {
                    BrandId = SQLDataHelper.GetInt(reader, "BrandID"),
                    Name = SQLDataHelper.GetString(reader, "BrandName"),
                    UrlPath = SQLDataHelper.GetString(reader, "UrlPath")
                }, 
                listId.HasValue ? new SqlParameter("@listId", listId) : null);
        }

        #region BrandCategory

        public static List<Brand> GetBrandsByCategoryId(int categoryId, bool inDepth, List<int> productIds = null, bool onlyAvailable = false)
        {
            var cmd = "[Catalog].[sp_GetBrandsByCategoryId]";
            var cmdType = CommandType.StoredProcedure;

            if (productIds != null && productIds.Count > 0)
            {
                cmd =
                    @"Select Brand.BrandId, Brand.BrandName, Brand.UrlPath, Brand.SortOrder, Photo.PhotoName 
                     From Catalog.Brand 
                     Inner Join (Select Distinct BrandId From [Catalog].[Product] Where ProductId in (" + string.Join(",", productIds) + @")) as t On t.BrandID = Brand.BrandID 
                     Left Join Catalog.Photo on Photo.ObjId = Brand.BrandID and Type = @Type 
                     Where Brand.Enabled=1";
                cmdType = CommandType.Text;
            }

            return SQLDataAccess.ExecuteReadList(cmd, cmdType,
                    reader => new Brand
                    {
                        BrandId = SQLDataHelper.GetInt(reader, "BrandId"),
                        Name = SQLDataHelper.GetString(reader, "BrandName"),
                        UrlPath = SQLDataHelper.GetString(reader, "UrlPath"),
                        BrandLogo = new BrandPhoto() { PhotoName = SQLDataHelper.GetString(reader, "PhotoName") },
                        SortOrder = SQLDataHelper.GetInt(reader, "SortOrder"),
                    },
                    new SqlParameter("@CategoryId", categoryId),
                    new SqlParameter("@Type", PhotoType.Brand.ToString()),
                    new SqlParameter("@Indepth", inDepth),
                    new SqlParameter("@OnlyAvailable", onlyAvailable))
                .OrderBy(x => x.SortOrder)
                .ThenBy(x => x.Name)
                .ToList();
        }

        public static Dictionary<int, BrandProductCount> GetCategoriesByBrand(int brandId)
        {
            return
                SQLDataAccess.ExecuteReadDictionary<int, BrandProductCount>(GetBrandCategorySql("select CategoryId, [Count], [CountDeep] from brandCategory where BrandId=@BrandID", onlyVisibleCategories: true),
                    CommandType.Text, "CategoryID", 
                    reader => new BrandProductCount
                    {
                        InCategoryCount = SQLDataHelper.GetInt(reader, "Count"),
                        InChildsCategoryCount = SQLDataHelper.GetInt(reader, "CountDeep")
                    }, 
                    new SqlParameter("@BrandID", brandId));
        }

        public static List<BrandProductCount> GetParentCategoriesbyChildsId(List<int> list)
        {
            if (list == null || !list.Any())
            {
                return new List<BrandProductCount>();
            }
            var _params = list.AggregateString(",");
            var sqlcomand = @";with parents as 
                                (
                                   select CategoryID, ParentCategory
                                   from Catalog.Category
                                   where CategoryID in ({0})
                                   union all
                                   select C.CategoryID, C.ParentCategory 
                                   from Catalog.Category c
                                   join parents p on C.CategoryID = P.ParentCategory                                   
                                   AND (C.CategoryID<>C.ParentCategory Or C.CategoryID <>0)
                                   AND C.Hidden = 0 AND C.Enabled = 1
                                )
                                Select c.CategoryId, c.ParentCategory, c.Name, c.UrlPath, c.CatLevel from 
                                ( select distinct *  from parents) t 
                                inner join Catalog.Category c on  c.CategoryID = t.CategoryID 
                                Order by c.SortOrder";
            return SQLDataAccess.ExecuteReadList(string.Format(sqlcomand, _params), CommandType.Text, reader => new BrandProductCount
            {
                CategoryId = SQLDataHelper.GetInt(reader, "CategoryId"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                ParentId = SQLDataHelper.GetInt(reader, "ParentCategory"),
                Url = SQLDataHelper.GetString(reader, "UrlPath"),
                Level = SQLDataHelper.GetInt(reader, "CatLevel")
            });
        }

        #endregion

        public static bool IsBrandEnabled(int brandId)
        {
            var res = SQLDataAccess.ExecuteScalar<bool>(
                "SELECT [Enabled] FROM [Catalog].[Brand] WHERE [BrandId] = @brandId", CommandType.Text,
                new SqlParameter { ParameterName = "@brandId", Value = brandId });

            return res;
        }

        /// <summary>
        /// get products count
        /// </summary>
        /// <returns></returns>
        public static int GetProductsCount(int brandId)
        {
            var res = SQLDataAccess.ExecuteScalar<int>(
                "SELECT Count([ProductID]) FROM [Catalog].[Product] Where BrandID=@BrandID", CommandType.Text,
                new SqlParameter { ParameterName = "@BrandID", Value = brandId });
            return res;
        }

        public static void DeleteBrandLogo(int brandId)
        {
            PhotoService.DeletePhotos(brandId, PhotoType.Brand);
        }

        public static int GetBrandIdByName(string brandName)
        {
            return CacheManager.Get(BrandIdCaheKey + brandName.ToLower(), () =>
                SQLDataAccess.ExecuteScalar<int>("select BrandID from Catalog.Brand where LOWER (BrandName)=@BrandName", CommandType.Text,
                    new SqlParameter("@BrandName", brandName.ToLower()))
            );
        }

        public static string BrandToString(int brandId)
        {
            var brand = GetBrandById(brandId);
            return brand != null ? brand.Name : string.Empty;
        }

        public static int BrandFromString(string brandName)
        {
            if (string.IsNullOrWhiteSpace(brandName))
                return 0;

            var brandId = GetBrandIdByName(brandName);
            if (brandId != 0)
                return brandId;
            var brand = new Brand
            {
                Enabled = true,
                Name = brandName,
                Description = brandName,
                UrlPath = UrlService.GetAvailableValidUrl(0, ParamType.Brand, brandName),
                Meta = null
            };
            return AddBrand(brand);
        }

        public static int GetBrandsCount(string condition = null, params SqlParameter[] parameters)
        {
            if (condition == null)
            {
                return SQLDataAccess.ExecuteScalar<int>("SELECT Count([BrandId]) FROM [Catalog].[Brand]",
                                                           CommandType.Text);
            }
            return SQLDataAccess.ExecuteScalar<int>("SELECT Count([BrandId]) FROM [Catalog].[Brand]" + " " + condition,
                                                    CommandType.Text, parameters);
        }

        public static void SetActive(int brandId, bool active)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [Catalog].[Brand] Set Enabled = @Enabled Where BrandId = @BrandId",
                 CommandType.Text,
                 new SqlParameter("@BrandId", brandId),
                 new SqlParameter("@Enabled", active));
        }
    }
}