using System;
using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Controls;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Seo;
using AdvantShop.Web.Admin.Models.Settings;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    public partial class SettingsSeoController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation(T("Admin.Settings.SEO.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsSeoCtrl);

            var model = new GetSeoSettings().Execute();

            return View("index", model);
        }

        [HttpPost]
        public ActionResult Index(SEOSettingsModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    new SaveSeoSettings(model).Execute();
                    ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
                }
                catch (BlException ex)
                {
                    ShowMessage(NotifyType.Error, ex.Message);
                }
            }
            else
            {
                foreach (var modelState in ViewData.ModelState.Values)
                    foreach (var error in modelState.Errors)
                    {
                        ShowMessage(NotifyType.Error, error.ErrorMessage);
                    }
            }
            return RedirectToAction("Index");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ResetMetaInfoByType(MetaType metaType)
        {
            MetaInfoService.DeleteMetaInfoByType(metaType);
            return Json(new { result = true });
        }

    }
}
