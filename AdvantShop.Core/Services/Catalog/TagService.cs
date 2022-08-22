﻿using System;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.SQL;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using AdvantShop.SEO;
using AdvantShop.Catalog;
using AdvantShop.Configuration;

namespace AdvantShop.Core.Services.Catalog
{
    public class TagService
    {
        private const string TagCacheName = "Tags";

        public static Tag Get(int id)
        {
            return SQLDataAccess.Query<Tag>("select * from Catalog.Tag where Id=@id", new { id }).FirstOrDefault();
        }


        public static Tag Get(string name)
        {
            return SQLDataAccess.Query<Tag>("select * from Catalog.Tag where Name=@name", new { name }).FirstOrDefault();
        }

        public static List<Tag> GetAllTags()
        {
            return SQLDataAccess.Query<Tag>("select * from Catalog.Tag ORDER BY [SortOrder]").ToList();
        }

        public static Tag GetByUrl(string url)
        {
            return SQLDataAccess.Query<Tag>("select * from Catalog.Tag where UrlPath=@url", new { url }).FirstOrDefault();
        }
        
        public static int Add(Tag model)
        {
            model.Id = SQLDataAccess.ExecuteScalar<int>(
                "Insert into Catalog.Tag (Name, Enabled, Description, BriefDescription, UrlPath, SortOrder, VisibilityForUsers) values (@Name,@Enabled,@Description, @BriefDescription, @UrlPath, @SortOrder, @VisibilityForUsers); Select SCOPE_IDENTITY();",
                CommandType.Text,
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Enabled", model.Enabled),
                new SqlParameter("@BriefDescription", model.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                new SqlParameter("@UrlPath", model.UrlPath),
                new SqlParameter("@SortOrder", model.SortOrder),
                new SqlParameter("@VisibilityForUsers", model.VisibilityForUsers)                
                );

            if (model.Meta != null)
            {
                if (!model.Meta.Title.IsNullOrEmpty() || !model.Meta.MetaKeywords.IsNullOrEmpty() || !model.Meta.MetaDescription.IsNullOrEmpty() || !model.Meta.H1.IsNullOrEmpty())
                {
                    model.Meta.ObjId = model.Id;
                    MetaInfoService.SetMeta(model.Meta);
                }
            }

            CacheManager.RemoveByPattern(TagCacheName);

            return model.Id;
        }

        public static void Update(Tag model)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Update Catalog.Tag set Name=@Name,Enabled=@Enabled, Description=@Description, BriefDescription=@BriefDescription, UrlPath=@UrlPath, SortOrder=@SortOrder, VisibilityForUsers=@VisibilityForUsers Where Id=@Id",
                CommandType.Text,
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@Name", model.Name),
                new SqlParameter("@Enabled", model.Enabled),
                new SqlParameter("@BriefDescription", model.BriefDescription ?? (object)DBNull.Value),
                new SqlParameter("@Description", model.Description ?? (object)DBNull.Value),
                new SqlParameter("@UrlPath", model.UrlPath),
                new SqlParameter("@SortOrder", model.SortOrder),
                new SqlParameter("@VisibilityForUsers", model.VisibilityForUsers)
                
                );

            // ---- Meta
            if (model.Meta != null)
            {
                if (model.Meta.Title.IsNullOrEmpty() && model.Meta.MetaKeywords.IsNullOrEmpty() && model.Meta.MetaDescription.IsNullOrEmpty() && model.Meta.H1.IsNullOrEmpty())
                {
                    if (MetaInfoService.IsMetaExist(model.Id, MetaType.Tag))
                        MetaInfoService.DeleteMetaInfo(model.Id, MetaType.Tag);
                }
                else
                    MetaInfoService.SetMeta(model.Meta);
            }

            CacheManager.RemoveByPattern(CacheNames.GetCategoryCacheObjectPrefix());
            CacheManager.RemoveByPattern(TagCacheName);
        }

        public static int AddMap(int objid, int tagId, ETagType type, int sortOrder)
        {
            CacheManager.RemoveByPattern(TagCacheName);

            return
                SQLDataAccess.ExecuteScalar<int>(
                    "insert into catalog.TagMap (ObjId,TagId,Type,SortOrder) values (@ObjId,@TagId,@Type,@SortOrder)", CommandType.Text,
                    new SqlParameter("@ObjId", objid), 
                    new SqlParameter("@TagId", tagId),
                    new SqlParameter("@Type", type.ToString()),
                    new SqlParameter("@SortOrder", sortOrder));
        }

        public static void Delete(int id)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from Catalog.Tag where Id=@Id", CommandType.Text, new SqlParameter("@Id", id));

            CacheManager.RemoveByPattern(CacheNames.GetCategoryCacheObjectPrefix());
            CacheManager.RemoveByPattern(TagCacheName);
        }

        public static void DeleteMap(int objId, ETagType type)
        {
            SQLDataAccess.ExecuteNonQuery("Delete from Catalog.TagMap where ObjId=@ObjId and Type=@Type", CommandType.Text, new SqlParameter("@ObjId", objId), new SqlParameter("@Type", type.ToString()));
            CacheManager.RemoveByPattern(TagCacheName);
        }

        public static List<Tag> Gets(int objId, ETagType type, bool onlyEnabled = false)
        {
            return
                SQLDataAccess.Query<Tag>(
                    "Select Id, Name, Enabled, UrlPath, VisibilityForUsers from Catalog.Tag Inner Join Catalog.TagMap on Tag.Id= TagMap.TagId Where TagMap.ObjId=@ObjId and TagMap.Type=@Type" +
                    (onlyEnabled ? " AND Enabled=1" : "") +
                    " Order by TagMap.SortOrder ",
                    new {ObjId = objId, Type = type.ToString()}).ToList();
        }

        /// <summary>
        /// Теги в категории, у которых есть товары в наличии
        /// </summary>
        public static List<Tag> GetCategoryTags(int categoryId)
        {
            return
                CacheManager.Get(TagCacheName + "_category_" + categoryId,
                    () => SQLDataAccess.Query<Tag>(
                        "Select distinct t.Id, t.Name, t.Enabled, t.UrlPath, t.VisibilityForUsers, tmc.SortOrder " +
                        "From Catalog.Tag as t " +

                        "Inner Join Catalog.TagMap tmc on t.Id = tmc.TagId " +

                        "Inner Join Catalog.TagMap tm On t.id = tm.TagId " +
                        "Inner Join Catalog.Product On Product.ProductId=tm.ObjId and tm.Type='Product' " +
                        "Inner Join Catalog.ProductCategories On ProductCategories.ProductID = Product.ProductId " +
                        (SettingsCatalog.ShowOnlyAvalible
                            ? "Inner Join Catalog.ProductExt On Product.ProductId = ProductExt.ProductId "
                            : "") +

                        "Where tmc.ObjId = @categoryId and tmc.Type='Category' and t.Enabled = 1 and " +
                        "ProductCategories.CategoryID = @categoryId and Product.Enabled = 1 " +
                        (SettingsCatalog.ShowOnlyAvalible ? "and MaxAvailable > 0 " : "") +

                        "Order by tmc.SortOrder",
                        new {categoryId}).ToList()
                    );
        }

        public static List<Tag> Gets(string name)
        {
            return SQLDataAccess.Query<Tag>("select Id, Name from Catalog.Tag where Name like '%'+ @Name +'%'", new { Name = name }).ToList();
        }

        public static List<Tag> GetAutocompleteTags(int count = 1000, bool onlyEnabled = true)
        {
            return SQLDataAccess.Query<Tag>("Select Top(@count) Id, Name, UrlPath From Catalog.Tag " +
                (onlyEnabled ? " Where Enabled = 1" : string.Empty) + "ORDER BY [SortOrder]",
                new {count}).ToList();
        }

        public static List<Tag> GetTagsByProductOnMain(EProductOnMain type, int? productListId)
        {
            var sql =
                "Select tag.Id, tag.Name, tag.UrlPath, tag.VisibilityForUsers " +
                "From Catalog.Tag tag " +
                "Where tag.Id In (" +
                "Select tag.Id From Catalog.Tag as t " +
                "Inner Join Catalog.TagMap tm On t.id = tm.TagId " +
                "Inner Join Catalog.Product On Product.ProductId=tm.ObjId and Type='Product' {0}) " +
                "Order By tag.SortOrder";
                 
            if (type == EProductOnMain.Best)
            {
                sql = string.Format(sql, "where t.Enabled = 1 and Bestseller=1 and Product.Enabled = 1");
            }
            else if (type == EProductOnMain.New)
            {
                sql = string.Format(sql, "where t.Enabled = 1 and New=1 and Product.Enabled = 1");
            }
            else if (type == EProductOnMain.Sale)
            {
                sql = string.Format(sql, "where t.Enabled = 1 and (Discount>0 or DiscountAmount>0) and Product.Enabled = 1");
            }
            else if (type == EProductOnMain.List && productListId != 0)
            {
                sql = string.Format(sql, 
                    "left join [Catalog].[Product_ProductList] ON [Product_ProductList].[ProductId] = [Product].[ProductId] " +
                    "where t.Enabled = 1 and [Product_ProductList].[ListId] = " + productListId);
            }
            else if (type == EProductOnMain.NewArrivals)
            {
                return new List<Tag>();
            }

            return SQLDataAccess.Query<Tag>(sql).ToList();
        }
    }
}
