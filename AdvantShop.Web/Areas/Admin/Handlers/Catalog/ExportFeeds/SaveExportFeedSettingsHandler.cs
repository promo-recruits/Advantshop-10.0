using System;
using System.Linq;
using AdvantShop.Core.Scheduler;
using AdvantShop.Core.Scheduler.Jobs;
using AdvantShop.ExportImport;
using AdvantShop.Saas;
using AdvantShop.Web.Admin.Models.Catalog.ExportFeeds;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Web.Admin.Handlers.Catalog.ExportFeeds
{
    public class SaveExportFeedSettingsHandler
    {
        private readonly ExportFeedSettingsModel _commonSettings;
        private readonly string _advancedSettings;
        private readonly int _exportFeedId;
        private readonly string _exportFeedName;
        private readonly string _exportFeedDescription;

        public SaveExportFeedSettingsHandler(int exportFeedId, string exportFeedName, string exportFeedDescription, ExportFeedSettingsModel commonSettings, string advancedSettings)
        {
            _exportFeedId = exportFeedId;
            _exportFeedName = exportFeedName;
            _exportFeedDescription = exportFeedDescription;

            _commonSettings = commonSettings;
            _advancedSettings = advancedSettings;
        }

        public CommandResult Execute()
        {
            //_commonSettings.Validate


            var exportFeed = ExportFeedService.GetExportFeed(_exportFeedId);
            if (exportFeed == null)
            {
                return new CommandResult { Result = false, Error = "Not found export feed" };
            }

            if (!string.IsNullOrEmpty(_exportFeedName))
            {
                exportFeed.Name = _exportFeedName;
            }

            exportFeed.Description = _exportFeedDescription;
            ExportFeedService.UpdateExportFeed(exportFeed);

            //удаляем корневую категорию, еслт включено выгружать все категории то добавляем снова
            ExportFeedService.DeleteCategory(exportFeed.Id, 0);

            var exportFeedSettings = ExportFeedSettingsProvider.GetSettings(_exportFeedId);

            var jobEnabled = _commonSettings.Active && !exportFeedSettings.Active;

            exportFeedSettings.Active = _commonSettings.Active;
            exportFeedSettings.Interval = _commonSettings.Interval;
            exportFeedSettings.IntervalType = _commonSettings.IntervalType;
            exportFeedSettings.JobStartTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, _commonSettings.JobStartHour, _commonSettings.JobStartMinute, 0);

            exportFeedSettings.FileName = _commonSettings.FileName;
            exportFeedSettings.FileExtention = _commonSettings.FileExtention;
            exportFeedSettings.PriceMarginInPercents = _commonSettings.PriceMarginInPercents;
            exportFeedSettings.PriceMarginInNumbers = _commonSettings.PriceMarginInNumbers;
            exportFeedSettings.AdditionalUrlTags = _commonSettings.AdditionalUrlTags;

            exportFeedSettings.ExportAllProducts = _commonSettings.ExportCatalogType == EExportFeedCatalogType.AllProducts; //_commonSettings.ExportAllProducts;
            exportFeedSettings.ExportAdult = !_commonSettings.DoNotExportAdult;

            if (!string.IsNullOrEmpty(_advancedSettings))
            {
                exportFeedSettings.AdvancedSettings = _advancedSettings;
            }
            ExportFeedSettingsProvider.SetSettings(_exportFeedId, exportFeedSettings);

            if (exportFeedSettings.ExportAllProducts)
            {
                //добавляем кореневую категорию, при этом не удаляются выбранные отдельные категории
                ExportFeedService.InsertCategory(exportFeed.Id, 0, false);
            }

            //jobSettings, start/stop job
            var item = new TaskSetting
            {
                Enabled = _commonSettings.Active,
                JobType = typeof(GenerateExportFeedJob).ToString(),
                TimeInterval = _commonSettings.Interval,
                TimeHours = _commonSettings.IntervalType == TimeIntervalType.Days ? _commonSettings.JobStartHour : 0,
                TimeMinutes = _commonSettings.IntervalType == TimeIntervalType.Days ? _commonSettings.JobStartMinute : 0,
                TimeType = _commonSettings.IntervalType,
                DataMap = _exportFeedId
            };

            var taskSettings = TaskSettings.ExportFeedSettings;

            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.ExportFeedsAutoUpdate &&
                (exportFeed.Type == EExportFeedType.YandexMarket || exportFeed.Type == EExportFeedType.GoogleMerchentCenter || exportFeed.Type == EExportFeedType.Avito))
            {
                item.Enabled = false;
                jobEnabled = false;
            }

            var setting = taskSettings.FirstOrDefault(x => x.JobType == item.JobType && Convert.ToInt32(x.DataMap) == Convert.ToInt32(item.DataMap));
            if (setting != null)
            {
                taskSettings.Remove(setting);
                taskSettings.Add(item);
            }
            else
            {
                taskSettings.Add(item);
            }

            TaskSettings.ExportFeedSettings = taskSettings;
            TaskManager.TaskManagerInstance().ManagedTask(taskSettings);

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_EditExportFeed, exportFeed.Type.ToString());
            if (jobEnabled)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_EnableJob, exportFeed.Type.ToString());

            return new CommandResult { Result = true };
        }
    }
}
