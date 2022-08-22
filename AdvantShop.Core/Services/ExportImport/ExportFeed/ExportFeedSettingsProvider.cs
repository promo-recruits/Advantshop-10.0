﻿using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;
using Newtonsoft.Json;
using System;
using System.Data;
using System.Data.SqlClient;

namespace AdvantShop.ExportImport
{
    public class ExportFeedSettingsProvider
    {
        public static ExportFeedSettings GetExportFeedSettingsFromReader(SqlDataReader reader)
        {
            return new ExportFeedSettings
            {
                ExportFeedId = SQLDataHelper.GetInt(reader, "ExportFeedId"),
                FileName = SQLDataHelper.GetString(reader, "FileName"),
                FileExtention = SQLDataHelper.GetString(reader, "FileExtention"),
                PriceMarginInPercents = SQLDataHelper.GetFloat(reader, "PriceMarginInPercents"),
                PriceMarginInNumbers = SQLDataHelper.GetFloat(reader, "PriceMarginInNumbers"),
                AdditionalUrlTags = SQLDataHelper.GetString(reader, "AdditionalUrlTags"),
                Active = SQLDataHelper.GetBoolean(reader, "Active"),
                IntervalType = SQLDataHelper.GetString(reader, "IntervalType").TryParseEnum<TimeIntervalType>(),
                Interval = SQLDataHelper.GetInt(reader, "Interval"),
                JobStartTime = SQLDataHelper.GetDateTime(reader, "JobStartTime"),
                AdvancedSettings = SQLDataHelper.GetString(reader, "AdvancedSettings"),
                ExportAllProducts = SQLDataHelper.GetBoolean(reader, "ExportAllProducts"),
                ExportAdult = SQLDataHelper.GetBoolean(reader, "ExportAdult", defaultValue: true),

                Value = SQLDataHelper.GetString(reader, "Value")
            };
        }

        public static ExportFeedSettings GetSettings(int exportFeedId)
        {
            var settings = SQLDataAccess.ExecuteReadOne<ExportFeedSettings>(
                "SELECT * FROM [Settings].[ExportFeedSettings] WHERE [ExportFeedId] = @ExportFeedId",
                CommandType.Text,
                GetExportFeedSettingsFromReader,
                new SqlParameter("@ExportFeedId", exportFeedId)
            );

            //портировнаие со старой схемы на новую
            if (settings != null && string.IsNullOrEmpty(settings.AdvancedSettings))
            {
                var advancedSettings = settings.Value;

                var oldSettings = GetSettingsOld<ExportFeedSettings>(exportFeedId);
                if (oldSettings != null)
                {
                    settings = oldSettings;
                    settings.AdvancedSettings = advancedSettings;
                }
            }

            return settings;
        }

        public static ExportFeedSettings GetSettingsByParam(string name, string value)
        {
            return SQLDataAccess.ExecuteReadOne<ExportFeedSettings>(
               string.Format("SELECT * FROM [Settings].[ExportFeedSettings] Where [AdvancedSettings] LIKE '%\"{0}\":\"{1}\"%'", name, value),
               CommandType.Text, GetExportFeedSettingsFromReader);
        }

        public static void SetSettings(int exportFeedId, ExportFeedSettings exportFeedSettings)
        {
            var command = 
                IsExistSettings(exportFeedId) 
                    ? "Update [Settings].[ExportFeedSettings] SET " +
                        "FileName=@FileName, FileExtention=@FileExtention, PriceMarginInPercents=@PriceMarginInPercents, PriceMarginInNumbers=@PriceMarginInNumbers, AdditionalUrlTags=@AdditionalUrlTags, " +
                        "Active=@Active, IntervalType=@IntervalType, Interval=@Interval, JobStartTime=@JobStartTime, AdvancedSettings=@AdvancedSettings, [Value] = @AdvancedSettings, " +
                        "ExportAllProducts=@ExportAllProducts, ExportAdult=@ExportAdult " +
                      "WHERE[ExportFeedId] = @ExportFeedId" 
                    
                    : "Insert Into [Settings].[ExportFeedSettings] ([ExportFeedId], [FileName], [FileExtention], [PriceMarginInPercents], [PriceMarginInNumbers], [AdditionalUrlTags], [Active], " +
                                                                    "[IntervalType], [Interval], [JobStartTime], [AdvancedSettings], [ExportAllProducts], [ExportAdult]) " +
                                                            "Values (@ExportFeedId, @FileName, @FileExtention, @PriceMarginInPercents, @PriceMarginInNumbers, @AdditionalUrlTags, @Active, " +
                                                                    "@IntervalType,@Interval, @JobStartTime, @AdvancedSettings, @ExportAllProducts, @ExportAdult)";

            SQLDataAccess.ExecuteNonQuery(command, CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId),
                new SqlParameter("@FileName", (object) exportFeedSettings.FileName ?? DBNull.Value),
                new SqlParameter("@FileExtention", (object) exportFeedSettings.FileExtention ?? DBNull.Value),
                new SqlParameter("@PriceMarginInPercents", exportFeedSettings.PriceMarginInPercents),
                new SqlParameter("@PriceMarginInNumbers", exportFeedSettings.PriceMarginInNumbers),
                new SqlParameter("@AdditionalUrlTags", (object) exportFeedSettings.AdditionalUrlTags ?? DBNull.Value),
                new SqlParameter("@Active", exportFeedSettings.Active),
                new SqlParameter("@IntervalType", exportFeedSettings.IntervalType),
                new SqlParameter("@Interval", exportFeedSettings.Interval),
                new SqlParameter("@JobStartTime", exportFeedSettings.JobStartTime == DateTime.MinValue ? (object) DBNull.Value : exportFeedSettings.JobStartTime),
                new SqlParameter("@AdvancedSettings", (object) exportFeedSettings.AdvancedSettings ?? DBNull.Value),
                new SqlParameter("@ExportAllProducts", exportFeedSettings.ExportAllProducts),
                new SqlParameter("@ExportAdult", exportFeedSettings.ExportAdult));
        }

        public static bool IsExistSettings(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<bool>(
                "Select Count(ExportFeedId) From [Settings].[ExportFeedSettings] Where ExportFeedId=@ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId));
        }

        public static string GetAdvancedSettings(int exportFeedId)
        {
            return SQLDataAccess.ExecuteScalar<string>(
                 "Select AdvancedSettings From [Settings].[ExportFeedSettings] WHERE[ExportFeedId] = @ExportFeedId",
                 CommandType.Text,
                 new SqlParameter("@ExportFeedId", exportFeedId));
        }

        public static void SetAdvancedSettings(int exportFeedId, string advancedSettings)
        {
            SQLDataAccess.ExecuteNonQuery(
                 "Update [Settings].[ExportFeedSettings] Set AdvancedSettings = @AdvancedSettings WHERE[ExportFeedId] = @ExportFeedId",
                 CommandType.Text,
                 new SqlParameter("@ExportFeedId", exportFeedId),
                 new SqlParameter("@AdvancedSettings", advancedSettings));
        }

        public static T ConvertAdvancedSettings<T>(string advancedSettings)
        {
            return JsonConvert.DeserializeObject<T>(advancedSettings);
        }

        [Obsolete]
        public static T GetSettingsOld<T>(int exportFeedId) where T : new()
        {
            var result = SQLDataAccess.ExecuteScalar<string>(
                "SELECT [Value] FROM [Settings].[ExportFeedSettings] WHERE [ExportFeedId] = @ExportFeedId",
                CommandType.Text,
                new SqlParameter("@ExportFeedId", exportFeedId));

            if (result == null)
            {
                return new T();
            }

            return JsonConvert.DeserializeObject<T>(result);
        }

        [Obsolete]
        public static void SetSettings<T>(int exportFeedId, T settings)
        {
            SQLDataAccess.ExecuteNonQuery("IF (SELECT COUNT([Value]) FROM [Settings].[ExportFeedSettings] WHERE [ExportFeedId] = @ExportFeedId) > 0 " +
                                          "Begin UPDATE [Settings].[ExportFeedSettings] SET Value = @settingValue WHERE [ExportFeedId] = @ExportFeedId; End " +
                                          "Else INSERT INTO [Settings].[ExportFeedSettings]([ExportFeedId],[Value]) VALUES(@ExportFeedId,@SettingValue);",
                                            CommandType.Text,
                                            new SqlParameter("@ExportFeedId", exportFeedId),
                                            new SqlParameter("@SettingValue", JsonConvert.SerializeObject(settings)));
        }

        [Obsolete]
        public static void SetDefaultCurrencyForModule(string moduleName, string currncyValueOld)
        {
            SQLDataAccess.ExecuteNonQuery("UPDATE [Settings].[Settings] SET Value = @CurrencyNew WHERE Name = @SettingName AND Value = @CurrencyOld;",
                                            CommandType.Text,
                                            new SqlParameter("@SettingName", moduleName + "Currency"),
                                            new SqlParameter("@CurrencyOld", currncyValueOld),
                                            new SqlParameter("@CurrencyNew", SettingsCatalog.DefaultCurrencyIso3));
        }

        [Obsolete]
        public static void UpdateCurrencyToDefault(string currncyValueOld)
        {
            SetDefaultCurrencyForModule("YandexMarket", currncyValueOld);
            SetDefaultCurrencyForModule("YahooShopping", currncyValueOld);
            SetDefaultCurrencyForModule("Shopzilla", currncyValueOld);
            SetDefaultCurrencyForModule("ShoppingCom", currncyValueOld);
            SetDefaultCurrencyForModule("PriceGrabber", currncyValueOld);
            SetDefaultCurrencyForModule("GoogleBase", currncyValueOld);
            SetDefaultCurrencyForModule("Amazon", currncyValueOld);
        }
    }
}