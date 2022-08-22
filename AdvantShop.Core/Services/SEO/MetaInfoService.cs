//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Core.Caching;

namespace AdvantShop.SEO
{
    public enum MetaType
    {
        Default,
        Product,
        Category,
        News,
        NewsCategory,
        StaticPage,
        Brand,
        Tag,
        MainPageProducts,
        Offer,
        Landing
    }

    public class MetaVariable
    {
        public MetaVariable(string value, string text)
        {
            Value = value;
            Text = text;
        }

        public string Value { get; set; }
        public string Text { get; set; }
    }


    public static class MetaInfoService
    {
        public static MetaInfo GetFormatedMetaInfo(MetaInfo meta, string name, string categoryName = null, string brandName = null, string price = null, List<string> tags = null, string offerArtNo = null, string productArtNo = null, int page = 0)
        {
            if (meta == null) return null;

            var strTags = "";
            if (tags != null)
                strTags = tags.AggregateString(" ");
            meta.Title = GlobalStringVariableService.TranslateExpression(meta.Title, meta.Type, name, 
                categoryName: categoryName, 
                brandName: brandName, 
                price: price, 
                tags: strTags, 
                offerArtNo: offerArtNo,
                productArtNo: productArtNo,
                page: page);
            meta.MetaKeywords = GlobalStringVariableService.TranslateExpression(meta.MetaKeywords, meta.Type, name,
                categoryName: categoryName,
                brandName: brandName,
                price: price, 
                tags: strTags,
                offerArtNo: offerArtNo,
                productArtNo: productArtNo,
                page: page);
            meta.MetaDescription = GlobalStringVariableService.TranslateExpression(meta.MetaDescription, meta.Type, name, 
                categoryName: categoryName, 
                brandName: brandName, 
                price: price, 
                tags: strTags,
                offerArtNo: offerArtNo,
                productArtNo: productArtNo,
                page: page);
            meta.H1 = GlobalStringVariableService.TranslateExpression(meta.H1, meta.Type, name, 
                categoryName: categoryName, 
                brandName: brandName, 
                price: price, 
                tags: strTags,
                offerArtNo: offerArtNo,
                productArtNo: productArtNo,
                page: page);
            return meta;
        }

        /// <summary>
        /// Get metainfo by metaid and type
        /// </summary>
        /// <param name="metaid"></param>
        /// <returns></returns>
        public static MetaInfo GetMetaInfo(int metaid)
        {
            return SQLDataAccess.ExecuteReadOne("Select * from SEO.MetaInfo where MetaID=@MetaID", CommandType.Text,
                GetFromReader, new SqlParameter("@MetaID", metaid));
        }

        public static MetaInfo GetMetaInfo(int objId, MetaType type)
        {
            return SQLDataAccess.ExecuteReadOne(
                "Select * from SEO.MetaInfo where ObjId=@objId and Type=@type", CommandType.Text,
                GetFromReader,
                new SqlParameter("@objId", objId),
                new SqlParameter("@type", type.ToString()));
        }

        public static MetaInfo GetFromReader(SqlDataReader reader)
        {
            return new MetaInfo(SQLDataHelper.GetInt(reader, "MetaID"),
                                SQLDataHelper.GetInt(reader, "ObjId"),
                                (MetaType)Enum.Parse(typeof(MetaType), SQLDataHelper.GetString(reader, "Type"), true),
                                SQLDataHelper.GetString(reader, "Title"),
                                SQLDataHelper.GetString(reader, "MetaKeywords"),
                                SQLDataHelper.GetString(reader, "MetaDescription"),
                                SQLDataHelper.GetString(reader, "H1"));
        }

        /// <summary>
        /// Get default metainfo
        /// </summary>
        /// <returns></returns>
        public static MetaInfo GetDefaultMetaInfo()
        {
            return GetDefaultMetaInfo(MetaType.Default, string.Empty);
        }

        public static MetaInfo GetDefaultMetaInfo(MetaType metaType, string h1)
        {
            return new MetaInfo(0, 0, metaType, SettingsSEO.GetDefaultTitle(metaType),
                SettingsSEO.GetDefaultMetaKeywords(metaType), SettingsSEO.GetDefaultMetaDescription(metaType),
                SettingsSEO.GetDefaultH1(metaType), true);
        }


        public static void SetMeta(MetaInfo meta)
        {
            if (meta.IsDefaultMeta)
                return;

            if (IsMetaExist(meta.ObjId, meta.Type))
            {
                UpdateMetaInfo(meta);
            }
            else
            {
                meta.MetaId = InsertMetaInfo(meta);
            }
        }

        public static bool IsMetaExist(int objId, MetaType type)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select Count(MetaID) from [SEO].[MetaInfo] where ObjId=@ObjId and Type=@Type", CommandType.Text,
                                                    new SqlParameter("@ObjId", objId),
                                                    new SqlParameter("@Type", type.ToString())) > 0;
        }

        private static int InsertMetaInfo(MetaInfo meta)
        {
            return
                SQLDataAccess.ExecuteScalar<int>("[SEO].[sp_AddMetaInfo]", CommandType.StoredProcedure,
                    new SqlParameter("@Title", meta.Title ?? SettingsSEO.DefaultMetaTitle ?? ""),
                    new SqlParameter("@MetaKeywords", meta.MetaKeywords ?? SettingsSEO.DefaultMetaKeywords ?? ""),
                    new SqlParameter("@MetaDescription", meta.MetaDescription ?? SettingsSEO.DefaultMetaDescription ?? ""),
                    new SqlParameter("@H1", meta.H1 ?? SettingsSEO.DefaultH1 ?? ""),
                    new SqlParameter("@ObjId", meta.ObjId),
                    new SqlParameter("@Type", meta.Type.ToString()));
        }

        private static void UpdateMetaInfo(MetaInfo meta)
        {
            SQLDataAccess.ExecuteNonQuery(
                "UPDATE [SEO].[MetaInfo] SET [Title] = @Title, [MetaKeywords] = @MetaKeywords,[MetaDescription] = @MetaDescription, [H1]=@H1 where ObjId=@ObjId and Type=@Type",
                CommandType.Text,
                new SqlParameter("@Title", meta.Title ?? string.Empty),
                new SqlParameter("@MetaKeywords", meta.MetaKeywords ?? string.Empty),
                new SqlParameter("@MetaDescription", meta.MetaDescription ?? string.Empty),
                new SqlParameter("@H1", meta.H1 ?? string.Empty),
                new SqlParameter("@ObjId", meta.ObjId),
                new SqlParameter("@Type", meta.Type.ToString()));
        }

        public static void DeleteMetaInfo(int metaId)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [SEO].[MetaInfo] WHERE MetaID=@MetaID", CommandType.Text,
                                          new SqlParameter("@MetaID", metaId));
        }

        public static void DeleteMetaInfo(int objId, MetaType type)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [SEO].[MetaInfo] WHERE ObjId=@objId and Type=@type",
                                          CommandType.Text,
                                          new SqlParameter("@objId", objId),
                                          new SqlParameter("@type", type.ToString()));
        }

        public static void DeleteMetaInfoByType(MetaType type)
        {
            SQLDataAccess.ExecuteNonQuery("DELETE FROM [SEO].[MetaInfo] WHERE Type=@type",
                                          CommandType.Text,
                                          new SqlParameter("@type", type.ToString()));
        }

        public static List<MetaVariable> GetMetaVariables(MetaType type)
        {
            var result = new List<MetaVariable>();
            
            var variables = GetMetaVariables();

            if (type != MetaType.Default)
                result.AddRange(variables[MetaType.Default]);

            result.AddRange(variables[type]);

            return result;
        }

        private static Dictionary<MetaType, List<MetaVariable>> GetMetaVariables()
        {
            return CacheManager.Get("MetaVariables", () => new Dictionary<MetaType, List<MetaVariable>>()
            {
                {
                    MetaType.Default, new List<MetaVariable>()
                    {
                        new MetaVariable("#STORE_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Default.StoreName"))
                    }
                },
                {
                    MetaType.Product, new List<MetaVariable>()
                    {
                        new MetaVariable("#PRODUCT_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Product.ProductName")),
                        new MetaVariable("#CATEGORY_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.CategoryName")),
                        new MetaVariable("#BRAND_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Brand.BrandName")),
                        new MetaVariable("#PRICE#",
                            LocalizationService.GetResource("Admin.MetaVariables.Product.Price")),
                        new MetaVariable("#TAGS#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.Tags")),
                        new MetaVariable("#PRODUCT_ARTNO#",
                            LocalizationService.GetResource("Admin.MetaVariables.Product.ProductArtNo")),
                        new MetaVariable("#OFFER_ARTNO#",
                            LocalizationService.GetResource("Admin.MetaVariables.Product.OfferArtNo"))
                    }
                },
                {
                    MetaType.Category, new List<MetaVariable>()
                    {
                        new MetaVariable("#CATEGORY_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.CategoryName")),
                        new MetaVariable("#PAGE#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.Page")),
                        new MetaVariable("#TAGS#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.Tags"))
                    }
                },
                {
                    MetaType.Brand, new List<MetaVariable>()
                    {
                        new MetaVariable("#BRAND_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Brand.BrandName")),
                        new MetaVariable("#PAGE#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.Page"))
                    }
                },
                {
                    MetaType.Tag, new List<MetaVariable>()
                    {
                        new MetaVariable("#TAG_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Tag.TagName")),
                        new MetaVariable("#CATEGORY_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.CategoryName")),
                        new MetaVariable("#PAGE#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.Page")),
                    }
                },
                {
                    MetaType.StaticPage, new List<MetaVariable>()
                    {
                        new MetaVariable("#PAGE_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.StaticPage.PageName"))
                    }
                },
                {
                    MetaType.News, new List<MetaVariable>()
                    {
                        new MetaVariable("#NEWS_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.News.Title"))
                    }
                },
                {
                    MetaType.NewsCategory, new List<MetaVariable>()
                    {

                        new MetaVariable("#NEWSCATEGORY_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.NewsCategory.Name"))
                    }
                },
                {
                    MetaType.MainPageProducts, new List<MetaVariable>()
                    {
                        new MetaVariable("#PAGE_NAME#",
                            LocalizationService.GetResource("Admin.MetaVariables.MainPageProducts.PageName")),
                        new MetaVariable("#PAGE#",
                            LocalizationService.GetResource("Admin.MetaVariables.Category.Page")),
                    }
                }
            });
        }
    }
}
