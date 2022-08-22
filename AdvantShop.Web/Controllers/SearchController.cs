using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.Services.SEO;
using AdvantShop.Core.Services.Statistic;
using AdvantShop.Customers;
using AdvantShop.Handlers.Catalog;
using AdvantShop.Handlers.Search;
using AdvantShop.Models.Catalog;
using AdvantShop.Models.Search;
using AdvantShop.SEO;
using AdvantShop.Web.Infrastructure.Controllers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class SearchController : BaseClientController
    {
        [AccessByChannel(EProviderSetting.StoreActive)]
        public ActionResult Index(SearchCatalogModel model)
        {
            if (model.Page != null && model.Page < 1)
                return Error404();

            var viewModel = new SearchPagingHandler(model, false).Get();

            ModulesExecuter.Search(model.Q, viewModel.Pager.TotalItemsCount);

            var gtm = GoogleTagManagerContext.Current;
            if (gtm.Enabled)
            {
                gtm.PageType = ePageType.searchresults;
                gtm.ProdIds = new List<string>();
                foreach (var item in viewModel.Products.Products)
                {
                    gtm.ProdIds.Add(item.ArtNo);
                }
            }

            if (model.Q.IsNotEmpty())
            {
                var url = Request.Url.ToString();
                url = url.Substring(url.LastIndexOf("/"), url.Length - url.LastIndexOf("/"));
                var cat = CategoryService.GetCategory(viewModel.Filter.CategoryId);
                var catname = "все категории";
                if (cat != null)
                    catname = cat.Name;
                StatisticService.AddSearchStatistic(url, model.Q, string.Format(T("Search.Index.SearchIn"),
                        catname,
                        viewModel.Filter.PriceFrom,
                        viewModel.Filter.PriceTo == 0 ? "∞" : viewModel.Filter.PriceTo.ToString()),
                        viewModel.Pager.TotalItemsCount,
                        CustomerContext.CurrentCustomer.Id);
                WriteLog(model.Q, Request.Url.AbsoluteUri, ePageType.searchresults);
            }

            if ((viewModel.Pager.TotalPages < viewModel.Pager.CurrentPage && viewModel.Pager.CurrentPage > 1) ||
                viewModel.Pager.CurrentPage < 0)
            {
                return Error404();
            }

            SetNgController(NgControllers.NgControllersTypes.CatalogCtrl);
            SetMetaInformation(new MetaInfo(string.Format("{0} - {1}", SettingsMain.ShopName, T("Search.Index.SearchTitle"))), model.Q, page: model.Page ?? 1);

            return View(viewModel);
        }

        public JsonResult Filter(SearchCatalogModel categoryModel)
        {
            if (categoryModel.Page != null && categoryModel.Page < 1)
                return Json(null);
            
            var model = new SearchPagingHandler(categoryModel, false).GetForFilter();
            var filter = model.Filter;
            filter.Indepth = true;

            var sqlTasks = new List<Task<List<FilterItemModel>>>
            {
                new FilterInputHandler(filter.CategoryId, false, categoryModel.Q).GetAsync(),
                new FilterSelectCategoryHandler(filter.CategoryId).GetAsync()
            };
            
            if (SettingsCatalog.ShowPriceFilter && !SettingsCatalog.HidePrice)
            {
                sqlTasks.Add(new FilterPriceHandler(filter.CategoryId, true, filter.PriceFrom, filter.PriceTo).GetAsync());
            }

            if (SettingsCatalog.ShowProducerFilter && model.Filter.SearchItemsResult != null && model.Filter.SearchItemsResult.Any())
            {
                sqlTasks.Add(
                    new FilterBrandHandler(filter.CategoryId, filter.Indepth, filter.BrandIds, filter.AvailableBrandIds,
                                            showOnlyAvailable: true, productIds: model.Filter.SearchItemsResult)
                        .GetAsync());
            }

            if (SettingsCatalog.ShowColorFilter)
            {
                sqlTasks.Add(
                    new FilterColorHandler(filter.CategoryId, filter.Indepth, filter.ColorIds, filter.AvailableColorIds,
                                            SettingsCatalog.ShowOnlyAvalible || filter.Available)
                        .GetAsync());
            }

            if (SettingsCatalog.ShowSizeFilter)
            {
                sqlTasks.Add(
                    new FilterSizeHandler(filter.CategoryId, filter.Indepth, filter.SizeIds, filter.AvailableSizeIds,
                                            SettingsCatalog.ShowOnlyAvalible || filter.Available)
                        .GetAsync());
            }

            if (model.Filter.SearchItemsResult != null && model.Filter.SearchItemsResult.Any())
            {
                sqlTasks.Add(
                new FilterPropertyHandler(filter.CategoryId, filter.Indepth, filter.PropertyIds,
                                            filter.AvailablePropertyIds, filter.RangePropertyIds,
                                            model.Filter.SearchItemsResult)
                        .GetAsync());
            }

            var resultFilter = sqlTasks.Select(x => x.Result).SelectMany(x => x).Where(x => x != null).ToList();

            return Json(resultFilter);
        }

        public JsonResult FilterProductCount(SearchCatalogModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Q) || (model.Page != null && model.Page < 1))
                return Json(null);

            var viewModel = new SearchPagingHandler(model, false).GetForFilterProductCount();

            return Json(viewModel.TotalItemsCount);
        }

        [HttpGet]
        public JsonResult Autocomplete(string q)
        {
            if (string.IsNullOrWhiteSpace(q))
                return Json(null);

            var handler = new SearchAutocompleteHandler(q);
            var categories = handler.GetCategories();
            var products = handler.GetProducts();

            return Json(new
            {
                Categories = new
                {
                    Title = T("Search.Autocomplete.Categories"),
                    Items = categories
                },
                Products = new
                {
                    Title = T("Search.Autocomplete.Products"),
                    Items = products
                },
                Empty = !products.Any() && !categories.Any()
            });
        }

        [ChildActionOnly]
        public ActionResult SearchBlock(SearchBlockModel search)
        {
            search = search ?? new SearchBlockModel();                        

            if (!String.IsNullOrEmpty(SettingsCatalog.SearchExample))
            {
                var examples = SettingsCatalog.SearchExample.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
                if (examples.Length > 0)
                {
                    var example = examples[(new Random()).Next(0, examples.Length)];
                    example = example.TrimEnd('\r').Trim();
                    search.ExampleText = example;
                    search.ExampleLink = Url.AbsoluteRouteUrl("Search", new { q = example });
                }
            }

            return PartialView("SearchBlock", search);
        }
    }
}