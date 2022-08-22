using System.Web.Mvc;
using AdvantShop.Core.Controls;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Customers;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Settings.Templates;
using AdvantShop.Web.Admin.Models.Settings.Templates;
using AdvantShop.Web.Infrastructure.Controllers;

namespace AdvantShop.Web.Admin.Controllers.Settings
{
    [Auth(RoleAction.Settings)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class SettingsTemplateController : BaseAdminController
    {
        public ActionResult Index()
        {
            var model = new GetTemplateSettings().Execute();

            SetMetaInformation(T("Admin.Settings.Template.Title"));
            SetNgController(NgControllers.NgControllersTypes.SettingsTemplateCtrl);

            return View(model);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Index(SettingsTemplateModel model)
        {
            if (ModelState.IsValid)
            {
                new SaveTemplateSettings(model).Execute();
                ShowMessage(NotifyType.Success, T("Admin.Settings.SaveSuccess"));
            }

            ShowErrorMessages();

            return RedirectToAction("Index");
        }
    }
}
