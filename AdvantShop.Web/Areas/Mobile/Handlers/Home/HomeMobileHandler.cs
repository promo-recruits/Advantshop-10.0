using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.Home;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Catalog;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;

namespace AdvantShop.Areas.Mobile.Handlers.Home
{
    public class HomeMobileHandler
    {
        public HomeMobileViewModel Get()
        {
            var model = new HomeMobileViewModel
            {
                CategoriesUrl = new List<SelectListItem>
                {
                    new SelectListItem
                    {
                        Text = LocalizationService.GetResource("Home.Index.Catalog"),
                        Value = "catalog/"
                    }
                }
            };

            var url = new UrlHelper(HttpContext.Current.Request.RequestContext);

            foreach (var category in CategoryService.GetChildCategoriesByCategoryId(0, false).Where(cat => cat.Enabled && !cat.Hidden))
            {
                model.CategoriesUrl.Add(new SelectListItem
                {
                    Text = category.Name,
                    Value = url.RouteUrl("category", new { url = category.UrlPath }, url.RequestContext.HttpContext.Request.Url?.Scheme)
                });
            }

            var itemsCount = SettingsMobile.MainPageProductsCount;
            if (itemsCount > 0)
            {
                model.Bestsellers = GetByType(EProductOnMain.Best, itemsCount);

                var countNew = SettingsCatalog.ShowNewOnMainPage
                    ? ProductOnMain.GetProductCountByType(EProductOnMain.New)
                    : 0;
                if (countNew == 0 && SettingsCatalog.ShowNewOnMainPage && SettingsCatalog.DisplayLatestProductsInNewOnMainPage)
                {
                    model.HideNewProductsLink = true;
                    model.NewArrivals = true;
                }
                model.NewProducts = countNew > 0
                    ? GetByType(EProductOnMain.New, itemsCount)
                    : GetByType(EProductOnMain.NewArrivals, itemsCount);

                model.Sales = GetByType(EProductOnMain.Sale, itemsCount);

                var productLists = ProductListService.GetMainPageList();
                foreach (var productList in productLists)
                {
                    var products = ProductListService.GetProducts(productList.Id, itemsCount);
                    if (products.Count > 0)
                    {
                        var productListModel = new ProductViewModel(products, true)
                        {
                            Id = productList.Id,
                            Title = productList.Name,
                            DisplayCategory = true,
                            HideMarkers = true
                        };
                        model.ProductLists.Add(productListModel);
                    }
                }
            }

            if (CustomerContext.CurrentCustomer.IsAdmin)
                Track.TrackService.TrackEvent(Track.ETrackEvent.Trial_VisitMobileVersion);

            return model;
        }

        private static ProductViewModel GetByType(EProductOnMain type, int itemsCount)
        {
            if (type == EProductOnMain.Best && !SettingsCatalog.ShowBestOnMainPage ||
                type == EProductOnMain.New && !SettingsCatalog.ShowNewOnMainPage ||
                type == EProductOnMain.NewArrivals && (!SettingsCatalog.ShowNewOnMainPage || !SettingsCatalog.DisplayLatestProductsInNewOnMainPage) ||
                type == EProductOnMain.Sale && !SettingsCatalog.ShowSalesOnMainPage)
            {
                return new ProductViewModel(null, true);
            }

            var model = new ProductViewModel(ProductOnMain.GetProductsByType(type, itemsCount), true)
            {
                HideMarkers = true
            };

            return model;
        }
    }
}
