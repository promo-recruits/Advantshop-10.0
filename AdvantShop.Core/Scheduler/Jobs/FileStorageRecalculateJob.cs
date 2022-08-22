using System;
using System.Threading;
using AdvantShop.Configuration;
using AdvantShop.Core.Scheduler.QuartzJobLogging;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Diagnostics;
using Quartz;

namespace AdvantShop.Core.Scheduler.Jobs
{
    internal class FileStorageRecalculateJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            DoJob(context);

            Debug.Log.Info($"FilesStorageService.RecalcAttachmentsSize took {SettingsMain.CurrentFilesStorageSwTime} milliseconds");
        }

        private static void DoJob(IJobExecutionContext context, bool retry = false)
        {
            var result = FilesStorageService.RecalcAttachmentsSize();

            if (retry || result)
                return;

            context.LogInformation($"{nameof(FilesStorageService.RecalcAttachmentsSize)} was already running");

            Thread.Sleep(TimeSpan.FromMinutes(new Random().Next(1, 5)));
            DoJob(context, true);
        }
    }
}
