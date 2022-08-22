//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using AdvantShop.Core.Common.Attributes;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Diagnostics;
using AdvantShop.ExportImport;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    public class GenerateExportFeedJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            if (!context.CanStart()) 
                return;

            JobDataMap dataMap = context.JobDetail.JobDataMap;
            var jobData = dataMap.Get(TaskManager.DataMap) as TaskSetting;

            if (jobData?.DataMap == null || !int.TryParse(jobData.DataMap.ToString(), out var exportFeedId))
                return;

            var exportFeed = ExportFeedService.GetExportFeed(exportFeedId);
            if (exportFeed == null)
            {
                TaskManager.TaskManagerInstance().RemoveTask(jobData.GetUniqueName(), TaskManager.TaskGroup);
                return;
            }

            var exportFeedSettings = ExportFeedSettingsProvider.GetSettings(exportFeed.Id);
            if (exportFeedSettings == null)
                return;
            
            if (SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.ExportFeedsAutoUpdate &&
                (exportFeed.Type == EExportFeedType.YandexMarket ||
                 exportFeed.Type == EExportFeedType.GoogleMerchentCenter ||
                 exportFeed.Type == EExportFeedType.Avito))
            {
                return;
            }

            try
            {
                var type = ReflectionExt.GetTypeByAttributeValue<ExportFeedKeyAttribute>(typeof(BaseExportFeed), atr => atr.Value, exportFeed.Type.ToString());
                var currentExportFeed = (BaseExportFeed)Activator.CreateInstance(type, (object)false);

                var filePath = exportFeedSettings.FileFullPath;
                var directory = filePath.Substring(0, filePath.LastIndexOf('\\'));

                if (!string.IsNullOrEmpty(directory))
                    FileHelpers.CreateDirectory(directory);

                currentExportFeed.Export(exportFeed.Id);

                exportFeed.LastExport = DateTime.Now;
                exportFeed.LastExportFileFullName = exportFeedSettings.FileFullName;

                ExportFeedService.UpdateExportFeed(exportFeed);

                if (exportFeed.Type != EExportFeedType.None)
                    Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_ExportFeeds_ExportAuto, exportFeed.Type.ToString());
            }
            catch (Exception ex)
            {
                context.LogError(ex.Message);
                Debug.Log.Error(ex);
            }

            context.WriteLastRun();
        }
    }
}