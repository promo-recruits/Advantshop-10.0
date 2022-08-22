﻿using System;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Webhook;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.ApiSettings;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsApiController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.API"));
            SetNgController(NgControllers.NgControllersTypes.SettingsCtrl);

            var model = new LoadSaveApiSettingsHandler(null).Load();

            return View("Index", model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(APISettingsModel model)
        {
            new LoadSaveApiSettingsHandler(model).Save();

            ShowMessage(NotifyType.Success, T("Admin.ChangesSuccessfullySaved"));
            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult Generate()
        {
            var key = Guid.NewGuid().ToString().Sha256();
            return Json(key);
        }

        [HttpGet]
        public JsonResult GetWebhooks()
        {
            var webhooks = WebhookUrlListProvider<ApiWebhookUrlList, ApiWebhookUrl>
                .WebhookUrlList
                .UrlList
                .Select(Models.Settings.Api.WebhookModel.FromApiWebhook)
                .ToList();

            return JsonOk(webhooks);
        }

        [HttpPost]
        public JsonResult SaveWebhooks(ApiWebhookUrl[] list)
        {
            var webhookUrlList = new ApiWebhookUrlList();
            webhookUrlList.UrlList = (list ?? new ApiWebhookUrl[] { }).AsQueryable();
            WebhookUrlListProvider<ApiWebhookUrlList, ApiWebhookUrl>.WebhookUrlList = webhookUrlList;

            return GetWebhooks();
        }

        [HttpGet]
        public JsonResult GetEventTypes()
        {
            var list = Enum.GetValues(typeof(ApiWebhookEventType))
                .Cast<ApiWebhookEventType>()
                .Where(x => x != ApiWebhookEventType.None)
                .Select(x => new Models.SelectItemModel(x.Localize(), (int)x))
                .ToList();
            return JsonOk(list);
        }
    }
}
