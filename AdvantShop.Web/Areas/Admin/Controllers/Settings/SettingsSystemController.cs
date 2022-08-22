using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Helpers;
using AdvantShop.Core.Services.Statistic.QuartzJobs;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.System;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Admin.Models.Settings.Jobs;
using AdvantShop.Web.Infrastructure.ActionResults;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsSystemController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.System.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsSystemCtrl);

            var model = new GetSystemSettingsHandler().Execute();
            return View("index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(SystemSettingsModel model)
        {
            try
            {
                new SaveSystemSettingsHandler(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ShowErrorMessages();
            }
            return RedirectToAction("Index");
        }

        public ActionResult AdminCss()
        {
            SetMetaInformation(T("Admin.Settings.System.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsSystemCtrl);

            var model = new AdminCssEditorHandler().GetFileContent();
            return View("AdminCss", "~/Areas/Admin/Views/Settings/_SettingsLayout.cshtml", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult AdminCss(string cssEditorText)
        {
            try
            {
                #region Css editor

                new AdminCssEditorHandler().SaveFileContent(cssEditorText ?? "");

                #endregion

                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }
            catch (BlException ex)
            {
                ModelState.AddModelError("", ex.Message);
                ShowErrorMessages();
            }

            return RedirectToAction("AdminCss", "SettingsSystem");
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckLicense(string licKey)
        {
            return Json(new CommandResult { Result = SettingsLic.Activate(licKey) });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateSiteMaps()
        {
            return Json(new UpdateSiteMapsHandler().Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult FileStorageRecalc()
        {
            if (SettingsMain.CurrentFilesStorageLastUpdateTime < DateTime.Now.AddHours(-1) ||
                CustomerContext.CurrentCustomer.IsVirtual)
            {
                FilesStorageService.RecalcAttachmentsSizeInBackground();
            }

            return Json(new { result = true });
        }

        #region Jobs

        public JsonResult GetSchedulerJobs()
        {
            return Json(new GetSchedulerJobs().Execute());
        }

        public ActionResult GetJobRuns(JobRunsFilterModel filterModel)
        {
            var result = new GetJobRunsHandler(filterModel).Execute();

            if (filterModel.OutputDataType != FilterOutputDataType.Csv)
                return Json(result);

            var fileName = $"export_jobRuns_{DateTime.Now:ddMMyyhhmmss}.csv";
            var fullFilePath = new ExportJobRunsHandler(result, fileName).Execute();
            return File(fullFilePath, "application/octet-stream", fileName);
        }

        public JsonResult GetJobRunStatuses()
        {
            return Json(
                (from object val in Enum.GetValues(typeof(EQuartzJobStatus)) select val.ToString())
                .Select(x => new { label = x, value = x })
            );
        }

        public ActionResult GetJobRunLog(string jobRunId, JobRunLogFilterModel filterModel)
        {
            var isExport = filterModel.OutputDataType == FilterOutputDataType.Csv;
            var result = new GetJobRunLogHandler(jobRunId, filterModel, isExport).Execute();

            if (isExport is false)
                return Json(result);

            var fileName = $"export_jobRunLog_{DateTime.Now:ddMMyyhhmmss}.csv";
            var fullFilePath = new ExportJobRunLogHandler(result, fileName).Execute();
            return File(fullFilePath, "application/octet-stream", fileName);
        }

        public JsonResult GetJobRunLogEvents()
        {
            return Json(
                (from object val in Enum.GetValues(typeof(EQuartzJobEvent)) select val.ToString())
                .Select(x => new { label = x, value = x })
            );
        }
        
        #endregion
    }
}
