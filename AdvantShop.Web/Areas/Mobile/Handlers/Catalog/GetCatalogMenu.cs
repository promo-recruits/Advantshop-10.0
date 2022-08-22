using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.Catalog;
using AdvantShop.Catalog;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Configuration;

namespace AdvantShop.Areas.Mobile.Handlers.Catalog
{
    public class GetCatalogMenu
    {
        private readonly UrlHelper _urlHelper;
        private bool _showRoot;
        private bool _isRootItems;
        private int _categoryId;

        public GetCatalogMenu(int categoryId, bool showRoot = false, bool isRootItems = false)
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
            _showRoot = showRoot;
            _isRootItems = isRootItems;
            _categoryId = categoryId;
        }

        public CatalogMenuModel Execute()
        {
            var model = new CatalogMenuModel
            {
                Items = CacheManager.Get(CacheNames.MenuCatalog + "_mobile_menu_" + _categoryId, () => GetCategoryItems(_categoryId, _showRoot, !_isRootItems)),
                ShowMenuLinkAll = SettingsMobile.ShowMenuLinkAll,
                CatalogMenuViewMode = SettingsMobile.CatalogMenuViewMode.TryParseEnum(SettingsMobile.eCatalogMenuViewMode.RootCategories),
                RootCategoryName = CategoryService.GetCategory(0).Name,
                IsRootItems = _isRootItems,
                PhotoWidth = SettingsPictureSize.IconCategoryImageWidth,
                PhotoHeight = SettingsPictureSize.IconCategoryImageHeight,
                ShowProductsCount = SettingsCatalog.ShowProductsCount
            }; 

            return model;
        }

        private List<CatalogMenuItem> GetCategoryItems(int parentCategoryId, bool showRoot = false, bool isNeedParent = false, bool isStopRecursion = false)
        {
            var list = new List<CatalogMenuItem>();

            var rootCategoriesList = CategoryService.GetChildCategoriesByCategoryIdForMenu(parentCategoryId).Where(cat => !cat.Hidden).Select(x => new CatalogMenuItem()
            {
                Name = x.Name,
                Url = _urlHelper.RouteUrl("Category", new { url = x.UrlPath }),
                SubItems = x.HasChild && isStopRecursion == false ? GetCategoryItems(x.CategoryId, false, false, true) : new List<CatalogMenuItem>(),
                Id = x.CategoryId,
                Icon = x.Icon,
                HasChild = x.HasChild,
                ProductsCount = SettingsCatalog.ShowOnlyAvalible ? x.Available_Products_Count : x.ProductsCount
            }).ToList();


            if (showRoot)
            {
                var root = CategoryService.GetCategory(0);
                list.Add(new CatalogMenuItem()
                {
                    Name = root.Name,
                    Url = _urlHelper.RouteUrl("CatalogRoot"),
                    SubItems = rootCategoriesList,
                    Id = 0,
                    Icon = new CategoryPhoto(),
                    HasChild = rootCategoriesList.Any(),
                    ProductsCount = SettingsCatalog.ShowOnlyAvalible ? root.Available_Products_Count : root.ProductsCount
                });
            }
            else if (isNeedParent)
            {
                var parent = CategoryService.GetCategory(_categoryId);
                list.Add(new CatalogMenuItem()
                {
                    Name = parent.Name,
                    Url = _urlHelper.RouteUrl("Category", new { url = parent.UrlPath }),
                    SubItems = rootCategoriesList,
                    Id = parent.CategoryId,
                    Icon = parent.Icon,
                    HasChild = rootCategoriesList.Any(),
                    ProductsCount = SettingsCatalog.ShowOnlyAvalible ? parent.Available_Products_Count : parent.ProductsCount
                });
            }
            else
            {
                list = rootCategoriesList;
            }

            return list;
        }
    }
}