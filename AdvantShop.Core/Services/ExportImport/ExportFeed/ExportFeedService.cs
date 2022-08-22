using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.Jobs;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using AdvantShop.Configuration;
using AdvantShop.Saas;

namespace AdvantShop.ExportImport
{
    public class ExportFeedService
    {
        #region Add,Update,Delete

        public static int AddExportFeed(ExportFeed exportFeed)
        {
            return SQLDataAccess.ExecuteScalar<int>("INSERT INTO [Settings].[ExportFeed] ([Name], [Type], [Description],[LastExport],[LastExportFileFullName]) VALUES(@Name, @Type, @Description,NULL,NULL);  SELECT scope_identity();",
                                         CommandType.Text,
                                         new SqlParameter("@Name", exportFeed.Name),
                                         new SqlParameter("@Type", exportFeed.Type.ToString()),
                                         new SqlParameter("@Description", exportFeed.Description ?? (object)DBNull.Value));
        }

        public static ExportFeed GetExportFeedFromReader(SqlDataReader reader)
        {
            return new ExportFeed
            {
                Id = SQLDataHelper.GetInt(reader, "Id"),
                Name = SQLDataHelper.GetString(reader, "Name"),
                Type = SQLDataHelper.GetString(reader, "Type").TryParseEnum<EExportFeedType>(),
                Description = SQLDataHelper.GetString(reader, "Description"),
                LastExport = SQLDataHelper.GetNullableDateTime(reader, "LastExport"),
                LastExportFileFullName = SQLDataHelper.GetString(reader, "LastExportFileFullName")
            };
        }

        public static ExportFeed GetExportFeed(int id)
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeed>("SELECT * FROM [Settings].[ExportFeed] WHERE [Id] = @id",
                                         CommandType.Text,
                                         GetExportFeedFromReader,
                                         new SqlParameter("@Id", id));
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static ExportFeed GetExportFeedFirst()
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeed>("SELECT Top(1) * FROM [Settings].[ExportFeed] Order By [Type],[Id]",
                                         CommandType.Text,
                                         GetExportFeedFromReader);
        }

        public static ExportFeed GetExportFeedFirstByType(EExportFeedType type)
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeed>("SELECT Top(1) * FROM [Settings].[ExportFeed] WHERE [Type] = @Type Order By [Type],[Id]",
                                         CommandType.Text,
                                         GetExportFeedFromReader,
                                         new SqlParameter("@Type", type.ToString()));
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public static List<ExportFeed> GetExportFeeds()
        {
            return SQLDataAccess.ExecuteReadList<ExportFeed>("SELECT * FROM [Settings].[ExportFeed] Order By [Type],[Id]",
                                       CommandType.Text,
                                       GetExportFeedFromReader);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="exportFeedType"></param>
        /// <returns></returns>
        public static List<ExportFeed> GetExportFeeds(EExportFeedType exportFeedType)
        {
            return SQLDataAccess.ExecuteReadList<ExportFeed>("SELECT * FROM [Settings].[ExportFeed] Where [Type] = @ExportFeedType",
                                       CommandType.Text,
                                       GetExportFeedFromReader,
                                       new SqlParameter("@ExportFeedType", exportFeedType.ToString()));
        }

        public static void UpdateExportFeed(ExportFeed exportFeed)
        {
            SQLDataAccess.ExecuteNonQuery("Update [Settings].[ExportFeed] Set [Name]=@Name, [Type]=@Type, [Description]=@Description, [LastExport]=@LastExport, [LastExportFileFullName]=@LastExportFileFullName Where [Id]=@Id",
                                       CommandType.Text,
                                       new SqlParameter("@Id", exportFeed.Id),
                                       new SqlParameter("@Name", exportFeed.Name),
                                       new SqlParameter("@Type", exportFeed.Type.ToString()),
                                       new SqlParameter("@Description", exportFeed.Description ?? (object)DBNull.Value),
                                       new SqlParameter("@LastExport", exportFeed.LastExport ?? (object)DBNull.Value),
                                       new SqlParameter("@LastExportFileFullName", string.IsNullOrEmpty(exportFeed.LastExportFileFullName)
                                            ? (object)DBNull.Value : exportFeed.LastExportFileFullName));
        }

        public static void DeleteExportFeed(int id)
        {
            DeleteTaskJob(id);
            SQLDataAccess.ExecuteNonQuery("Delete FROM [Settings].[ExportFeed] Where [Id]=@Id",
                                       CommandType.Text,
                                       new SqlParameter("@Id", id));
        }

        public static void DeleteTaskJob(int exportFeedId)
        {
            var feed = GetExportFeed(exportFeedId);
            if (feed == null)
                return;
            
            var feedSettings = ExportFeedSettingsProvider.GetSettings(feed.Id);
            if (feedSettings == null)
                return;

            var setting = GetTaskSettingByExportFeed(feed, feedSettings);
            TaskManager.TaskManagerInstance().RemoveTask(setting.GetUniqueName(), TaskManager.TaskGroup);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="id">export feed id</param>
        /// <param name="name">file name</param>
        /// <param name="extention">file extention</param>
        /// <returns></returns>
        public static bool IsExistFile(int id, string name, string extention)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                string.Format("SELECT  CASE WHEN (Select Count(ExportFeedId) From [Settings].[ExportFeedSettings] " +
                              "Where ExportFeedId <> @ExportFeedId and Value Like '%\"FileName\":\"{0}\"%' AND Value Like '%\"FileExtention\":\"{1}\"%' ) > 0 THEN 1 ELSE 0 END ",
                name, extention),
                CommandType.Text,
                new SqlParameter("@ExportFeedId", id));
        }

        #endregion Add,Update,Delete

        #region Categories and Products

        public static bool CheckCategory(int exportFeedId, int catId)
        {
            return SQLDataAccess.ExecuteScalar<int>("Select count(*) from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId and CategoryID=@CategoryID",
                                                    CommandType.Text,
                                                    new SqlParameter("@exportFeedId", exportFeedId),
                                                    new SqlParameter("@CategoryID", catId)) > 0;
        }

        public static bool CheckCategoryHierical(int exportFeedId, int catId)
        {
            return
                SQLDataAccess.ExecuteScalar<int>(
                    "Select count(*) from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId " +
                    " and CategoryID in (Select id from [Settings].[GetParentsCategoryByChild](@CategoryID) union select 0) and CategoryID<> @CategoryID",
                    CommandType.Text, new SqlParameter("@exportFeedId", exportFeedId), new SqlParameter("@CategoryID", catId)) > 0;
        }

        public static void InsertCategory(int exportFeedId, int catId)
        {
            InsertCategory(exportFeedId, catId, false);
        }

        public static void InsertCategory(int exportFeedId, int catId, bool opened)
        {
            if (CheckCategory(exportFeedId, catId)) return;
            SQLDataAccess.ExecuteScalar<int>("Insert into Settings.ExportFeedSelectedCategories (ExportFeedId, CategoryID, Opened) VALUES (@exportFeedId, @CategoryID, @Opened)",
                                                   CommandType.Text,
                                                   new SqlParameter("@exportFeedId", exportFeedId),
                                                   new SqlParameter("@CategoryID", catId),
                                                   new SqlParameter("@Opened", opened));
        }

        public static void DeleteCategory(int exportFeedId, int catId)
        {
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId and CategoryID=@CategoryID",
                                                   CommandType.Text,
                                                   new SqlParameter("@exportFeedId", exportFeedId),
                                                   new SqlParameter("@CategoryID", catId));
        }

        /// <summary>
        /// for adminv2
        /// </summary>
        /// <param name="exportFeedId"></param>
        /// <param name="categories"></param>
        public static void InsertCategories(int exportFeedId, List<ExportFeedSelectedCategory> categories)
        {
            SQLDataAccess.ExecuteNonQuery(
                "Delete From Settings.ExportFeedSelectedCategories Where ExportFeedId = @ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId));
            if (categories == null)
            {
                return;
            }

            foreach (var category in categories)
            {
                InsertCategory(exportFeedId, category.CategoryId, category.Opened);
            }
        }

        /// <summary>
        /// for adminv2
        /// </summary>
        /// <param name="exportFeedId"></param>
        /// <returns></returns>
        public static bool IsExportAllCategories(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "Select CASE WHEN Count(CategoryID) = 1 THEN 1  Else 0  END From Settings.ExportFeedSelectedCategories Where ExportFeedId = @ExportFeedId and CategoryID = 0",
                CommandType.Text,
                new SqlParameter("@exportFeedId", exportFeedId));
        }

        public static EExportFeedCatalogType GetExportFeedCatalogType(int exportFeedId)
        {
            var isExportAllCategories = IsExportAllCategories(exportFeedId);
            return isExportAllCategories ? EExportFeedCatalogType.AllProducts : EExportFeedCatalogType.Categories;
        }

        public static List<ExportFeedSelectedCategory> GetExportFeedCategoriesId(int exportFeedId)
        {
            return SQLDataAccess.Query<ExportFeedSelectedCategory>(
                "Select [CategoryId], [Opened] From Settings.ExportFeedSelectedCategories Where ExportFeedId = @ExportFeedId",
                new { exportFeedId = exportFeedId }).ToList();
        }

        public static void DeleteModule(int exportFeedId)
        {
            SQLDataAccess.ExecuteScalar<int>("Delete from Settings.ExportFeedSelectedCategories where ExportFeedId=@exportFeedId",
                                                   CommandType.Text, new SqlParameter("@exportFeedId", exportFeedId));
        }

        public static void AddExcludeProduct(int exportFeedId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "IF (SELECT COUNT([ProductId]) FROM [Settings].[ExportFeedExcludedProducts] WHERE [ExportFeedId] = @ExportFeedId AND [ProductId] = @ProductId) = 0 " +
                "BEGIN INSERT INTO [Settings].[ExportFeedExcludedProducts] ([ExportFeedId],[ProductId]) VALUES (@ExportFeedId,@ProductId) END",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@ProductId", productId));
        }

        public static void DeleteExcludeProduct(int exportFeedId, int productId)
        {
            SQLDataAccess.ExecuteNonQuery(
                "DELETE FROM [Settings].[ExportFeedExcludedProducts] WHERE [ExportFeedId]=@ExportFeedId AND [ProductId]=@ProductId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@ProductId", productId));
        }
        #endregion Categories and Products


        public static TaskSetting GetTaskSettingByExportFeed(ExportFeed feed, ExportFeedSettings feedSettings)
        {
            var item = new TaskSetting
            {
                Enabled = feedSettings.Active,
                JobType = typeof(GenerateExportFeedJob).ToString(),
                TimeInterval = feedSettings.Interval,
                TimeHours = feedSettings.IntervalType == TimeIntervalType.Days ? feedSettings.JobStartTime.Hour : 0,
                TimeMinutes = feedSettings.IntervalType == TimeIntervalType.Days ? feedSettings.JobStartTime.Minute : 0,
                TimeType = feedSettings.IntervalType,
                DataMap = feed.Id
            };

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.ExportFeedsAutoUpdate &&
                (feed.Type == EExportFeedType.YandexMarket ||
                 feed.Type == EExportFeedType.GoogleMerchentCenter || 
                 feed.Type == EExportFeedType.Avito))
            {
                item.Enabled = false;
            }

            return item;
        }
        
        public static List<TaskSetting> GetExportFeedTaskSettings()
        {
            var taskSettings = new List<TaskSetting>();
            foreach (var feed in GetExportFeeds())
            {
                var feedSettings = ExportFeedSettingsProvider.GetSettings(feed.Id);
                if (feedSettings == null)
                    continue;

                taskSettings.Add(GetTaskSettingByExportFeed(feed, feedSettings));
            }
            return taskSettings;
        }
    }
}