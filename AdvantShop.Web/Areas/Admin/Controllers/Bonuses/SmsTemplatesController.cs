using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Bonuses.Service;
using AdvantShop.Core.Services.Bonuses.Sms;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Bonuses.SmsTemplates;
using AdvantShop.Web.Admin.Models.Bonuses.SmsTemplates;

namespace AdvantShop.Web.Admin.Controllers.Bonuses
{
    [Auth(RoleAction.BonusSystem)]
    [SaasFeature(Saas.ESaasProperty.BonusSystem)]
    [SalesChannel(ESalesChannelType.Bonus)]
    public class SmsTemplatesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.SmsTemplates.Index.Title"));
            SetNgController(NgControllers.NgControllersTypes.SmsTemplatesCtrl);
            return View();
        }

        public JsonResult GetSmsTemplates()
        {
            var result =
                SmsTemplateService.GetAll()
                    .Select(x => new { SmsTypeId = x.SmsTypeId.ToString(), SmsType = x.SmsTypeId.Localize(), x.SmsBody })
                    .ToList();
            return Json(new
            {
                TotalItemsCount = result.Count,
                DataItems = result
            });
        }


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSmsTemplateMass(List<ESmsType> ids, string selectMode)
        {
            if (selectMode == "all")
            {
                var t = SmsTemplateService.GetAll();
                foreach (var id in t)
                {
                    SmsTemplateService.Delete(id.SmsTypeId);
                }
            }
            if (ids == null) return Json(false);
            foreach (var id in ids)
            {
                SmsTemplateService.Delete(id);
            }
            return Json(true);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSmsTemplate(ESmsType id)
        {
            SmsTemplateService.Delete(id);
            return Json(true);
        }

        [HttpGet]
        public JsonResult AddEditSmsTemplate(ESmsType smsTypeId, bool isNew)
        {
            var model = new SmsTemplateModel(smsTypeId);
            if (!isNew)
            {
                var t = SmsTemplateService.Get(smsTypeId);
                model.SmsBody = t != null ? t.SmsBody : "";
            }
            return JsonOk(model);
        }

       [HttpPost, ValidateJsonAntiForgeryToken, ValidateAjax]
        public JsonResult AddEditSmsTemplate(SmsTemplateModel model)
        {
            return ProcessJsonResult(new AddEditSmsTemplateHandler(model));
        }

        public ActionResult SmsLog()
        {
            SetMetaInformation(T("Admin.SmsTemplates.SmsLog.Title"));
            SetNgController(NgControllers.NgControllersTypes.SmsTemplatesCtrl);
            return View();
        }

        public JsonResult GetSmsLogs(SmsLogFilterModel model)
        {
            try
            {
                if (ModelState.IsValid)
                    return Json(new GetSmsLogHandler(model).Execute());
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }
            return Json(new SmsLogFilterModel());
        }
    }
}
