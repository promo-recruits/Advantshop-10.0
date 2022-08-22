using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.SEO;
using AdvantShop.Web.Admin.Models.Catalog;
using AdvantShop.Web.Infrastructure.Admin;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Admin.Attributes;
using AdvantShop.Web.Admin.Handlers.Catalog.MainPageProducts;
using AdvantShop.Web.Admin.Models.Catalog.MainPageProducts;
using AdvantShop.Web.Admin.ViewModels.Catalog.MainPageProducts;

namespace AdvantShop.Web.Admin.Controllers.Catalog
{
    [Auth(RoleAction.Catalog)]
    [SalesChannel(ESalesChannelType.Store)]
    public partial class MainPageProductsController : BaseAdminController
    {
        public ActionResult Index(EProductOnMain type = EProductOnMain.Best)
        {
            if (type == EProductOnMain.None)
                return Error404();

            var model = new MainPageProductsViewModel(){Type = type};

            switch (type)
            {
                case EProductOnMain.Best:
                    model.Title = T("Admin.MainPageProducts.BestTitle");
                    break;
                case EProductOnMain.New:
                    model.Title = T("Admin.MainPageProducts.NewTitle");
                    break;
                case EProductOnMain.Sale:
                    model.Title = T("Admin.MainPageProducts.SalesTitle");
                    break;
            }

            SetMetaInformation(model.Title);
            SetNgController(NgControllers.NgControllersTypes.MainPageProductsCtrl);

            return View(model);
        }


        public JsonResult GetCatalog(CatalogFilterModel model, EProductOnMain type)
        {
            return Json(new GetMainPageProducts(model, type).Execute());
        }

        #region Edit list
        
        public JsonResult GetMainPageList(EProductOnMain type)
        {
            var description = "";
            var showOnMainPage = false;
            var shuffleList = false;
            bool? displayLatestProductsInNewOnMainPage = null;
            switch (type)
            {
                case EProductOnMain.Best:
                    description = SettingsCatalog.BestDescription;
                    showOnMainPage = SettingsCatalog.ShowBestOnMainPage;
                    shuffleList = SettingsCatalog.ShuffleBestOnMainPage;
                    break;

                case EProductOnMain.New:
                    description = SettingsCatalog.NewDescription;
                    showOnMainPage = SettingsCatalog.ShowNewOnMainPage;
                    displayLatestProductsInNewOnMainPage = SettingsCatalog.DisplayLatestProductsInNewOnMainPage;
                    shuffleList = SettingsCatalog.ShuffleNewOnMainPage;
                    break;

                case EProductOnMain.Sale:
                    description = SettingsCatalog.DiscountDescription;
                    showOnMainPage = SettingsCatalog.ShowSalesOnMainPage;
                    shuffleList = SettingsCatalog.ShuffleSalesOnMainPage;
                    break;
            }
            var meta = MetaInfoService.GetMetaInfo(-1 * (int) type, MetaType.MainPageProducts) ?? new MetaInfo();

            return Json(new
            {
                Type = type,
                Description = description,
                ShowOnMainPage = showOnMainPage,
                UseDefaultMeta = meta.ObjId == 0,
                H1 = meta.H1,
                Title = meta.Title,
                MetaKeywords = meta.MetaKeywords,
                MetaDescription = meta.MetaDescription,
                DisplayLatestProductsInNewOnMainPage = displayLatestProductsInNewOnMainPage,
                ShuffleList = shuffleList
            });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult UpdateMainPageList(EProductOnMain type, string description, bool showOnMainPage, string h1, string title, string metaKeywords, string metaDescription, bool useDefaultMeta, bool? displayLatestProductsInNewOnMainPage, bool shuffleList)
        {
            switch (type)
            {
                case EProductOnMain.Best:
                    SettingsCatalog.BestDescription = description ?? "";
                    SettingsCatalog.ShowBestOnMainPage = showOnMainPage;
                    SettingsCatalog.ShuffleBestOnMainPage = shuffleList;
                    break;

                case EProductOnMain.New:
                    SettingsCatalog.NewDescription = description ?? "";
                    SettingsCatalog.ShowNewOnMainPage = showOnMainPage;
                    SettingsCatalog.DisplayLatestProductsInNewOnMainPage = displayLatestProductsInNewOnMainPage.Value;
                    SettingsCatalog.ShuffleNewOnMainPage = shuffleList;
                    break;

                case EProductOnMain.Sale:
                    SettingsCatalog.DiscountDescription = description ?? "";
                    SettingsCatalog.ShowSalesOnMainPage = showOnMainPage;
                    SettingsCatalog.ShuffleSalesOnMainPage = shuffleList;
                    break;
            }

            if (useDefaultMeta ||
                (title.IsNullOrEmpty() && metaKeywords.IsNullOrEmpty() && metaDescription.IsNullOrEmpty() && h1.IsNullOrEmpty()))
            {
                if (MetaInfoService.IsMetaExist(-1*(int) type, MetaType.MainPageProducts))
                    MetaInfoService.DeleteMetaInfo(-1*(int) type, MetaType.MainPageProducts);
            }
            else
            {
                MetaInfoService.SetMeta(new MetaInfo(0, -1*(int) type, MetaType.MainPageProducts, title, metaKeywords, metaDescription, h1));
            }

            return JsonOk();
        }

        #endregion

        #region Inplace

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult InplaceProduct(MainPageProductModel model, EProductOnMain type)
        {
            if (model.ProductId != 0 && type != EProductOnMain.None)
            {
                ProductOnMain.UpdateProductByType(model.ProductId, model.SortOrder, type);
            }
            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteFromList(EProductOnMain type, int productId)
        {
            ProductOnMain.DeleteProductByType(productId, type);
            return JsonOk();
        }

        #endregion

        #region Commands

        private void Command(CatalogFilterModel command, EProductOnMain type, Action<int, EProductOnMain, CatalogFilterModel> func)
        {
            if (command.SelectMode == SelectModeCommand.None)
            {
                foreach (var id in command.Ids)
                    func(id, type, command);
            }
            else
            {
                var ids = new GetMainPageProducts(command, type).GetItemsIds("[Product].[ProductID]");
                foreach (int id in ids)
                {
                    if (command.Ids == null || !command.Ids.Contains(id))
                        func(id, type, command);
                }
            }
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult DeleteProductsFromList(CatalogFilterModel command, EProductOnMain type)
        {
            Command(command, type, (id, t, c) => ProductOnMain.DeleteProductByType(id, t));
            return Json(true);
        }

        #endregion


        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult AddProducts(EProductOnMain type, List<int> ids)
        {
            if (type == EProductOnMain.None || ids == null || ids.Count == 0)
                return Json(new {result = false});

            foreach (var id in ids)
                ProductOnMain.AddProductByType(id, type);
            
            return Json(new { result = true });
        }

    }
}