using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Statistic;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.ExportCategories;
using AdvantShop.Web.Admin.Models.Catalog.ExportCategories;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    public partial class ExportCategoriesController : BaseAdminController
    {
        public ActionResult Index()
        {
            SetNgController(NgControllers.NgControllersTypes.ExportCategoriesCtrl);

            ExportCategoriesModel model = new GetExportCategoriesHandler().Execute();

            return View(model);
        }
        public ActionResult _Index()
        {
            ExportCategoriesModel model = new GetExportCategoriesHandler().Execute();

            return PartialView("Index", model);
        }

        public ActionResult Export()
        {
            SetNgController(NgControllers.NgControllersTypes.ExportCategoriesCtrl);

            if (CommonStatistic.IsRun) 
            {
                return View("ExportCategoriesProgress");
            };

            CommonStatistic.StartNew(() =>
                {
                    var filePath = new StartingExportCategoriesHandler().Execute(useCommonStatistic: true);
                    CommonStatistic.FileName = "../" + filePath;
                },
                "exportcategories/export",
                LocalizationService.GetResource("Admin.ExportCategories.CategoriesExport"));

            return View("ExportCategoriesProgress");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveExportCategoriesSettings(string separator, string encoding, List<string> exportCategoriesFields)
        {
            var result = new SaveExportCategoriesFieldsHandler(separator, encoding, exportCategoriesFields).Execute();

            return result ? JsonOk() : JsonError(T("Admin.Catalog.ErrorSavingSettings"));
        }
    }
}
