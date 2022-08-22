using AdvantShop.Configuration;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.Landing.Templates;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Handlers.Dashboard;
using AdvantShop.Web.Admin.Handlers.Home;
using AdvantShop.Web.Admin.ViewModels.Home;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using System.Web.Mvc;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class DashboardController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetMetaInformation("Мои сайты");
            SetNgController(NgControllers.NgControllersTypes.DashboardSitesCtrl);

            return View();
        }

        public JsonResult GetDashBoard()
        {
            return Json(new GetDashboard().Execute());
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteSite(int id, DashboardSiteItemType type)
        {
            switch (type)
            {
                case DashboardSiteItemType.Funnel:
                    {
                        var service = new LpSiteService();
                        var funnel = service.Get(id);
                        if (funnel != null)
                        {
                            service.Delete(id);
                            return JsonOk();
                        }
                        break;
                    }

                case DashboardSiteItemType.Store:
                    {
                        var store = SalesChannelService.GetByType(ESalesChannelType.Store);
                        store.Enabled = false;
                        return JsonOk();
                    }
            }

            return JsonError("Не удалось удалить сайт");
        }

        public ActionResult CreateSite(string mode)
        {
            SetMetaInformation("Создание нового сайта");
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            var model = new GetCreateSiteModel(mode).Execute();

            return View(model);
        }

        public ActionResult Preview()
        {
            SetMetaInformation("Создание нового сайта");
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            return View();
        }

        public ActionResult TrialFirstSession()
        {

            if (SalesChannelService.IsFirstTimeCreateStore())
            {
                SetMetaInformation("Создание нового сайта");
                SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

                return View();
            }

            return RedirectToAction("Index", "Home");
        }


        public JsonResult GetSiteTemplates(LpSiteCategory category)
        {
            return Json(new GetSiteTemplates(category).Execute());
        }

        public ActionResult CreateTemplate(string id, string mode)
        {
            if (string.IsNullOrEmpty(id))
                return RedirectToAction("CreateSite", new {mode});

            var model = new GetCreateTemplateModel(id).Execute();
            if (model == null)
                return Error404();

            SetMetaInformation("Создание нового сайта");
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            model.Mode = mode;

            return View(model);
        }

        public ActionResult CreateFunnel(string id)
        {
            var model = new GetCreateFunnelModel(id).Execute();
            if (model == null)
                return Error404();

            SetMetaInformation("Создание нового сайта");
            SetNgController(NgControllers.NgControllersTypes.CreateSiteCtrl);

            return View(model);
        }

        //[HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CreateScreenShots()
        {
            new CreateDashboardScreenShots().Execute();
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeMainUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
                SettingsMain.SiteUrl = url;

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult ChangeEnabled(int id, DashboardSiteItemType type, bool enabled)
        {
            switch (type)
            {
                case DashboardSiteItemType.Funnel:
                    var service = new LpSiteService();
                    var funnel = service.Get(id);
                    if (funnel == null)
                        return JsonError();

                    funnel.Enabled = enabled;

                    service.Update(funnel);
                    break;

                case DashboardSiteItemType.Store:
                    var store = SalesChannelService.GetByType(ESalesChannelType.Store);
                    store.Enabled = enabled;
                    break;

                default:
                    return JsonError();
            }
            return JsonOk();
        }
    }
}