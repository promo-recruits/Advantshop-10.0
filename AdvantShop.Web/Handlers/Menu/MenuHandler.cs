﻿using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Customers;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;

namespace AdvantShop.Handlers.Menu
{
    public class MenuHandler
    {
        private const int CacheTime = 60;
        private readonly UrlHelper _urlHelper;

        public MenuHandler()
        {
            _urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
        }

        /// <summary>
        /// Get catalog menu items
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public MenuItemModel GetCatalogMenuItems(int categoryId)
        {
            var url = UrlService.GenerateBaseUrl();

            var menuItem = CacheManager.Get(CacheNames.MenuCatalog + categoryId + "_" + url, CacheTime, () =>
            {
                if (categoryId != 0)
                {
                    var subCategories = CategoryService.GetChildCategoriesByCategoryIdForMenu(categoryId).Where(cat=> !cat.Hidden).ToList();
                    categoryId = subCategories.Count > 0 ? subCategories[0].ParentCategoryId : 0;
                }
                var menuItems = new MenuItemModel()
                {
                    SubItems = GetRootSubcategories(categoryId)
                };
                foreach (var subCategory in menuItems.SubItems.Where(x => x.HasChild ))
                {                   
                    subCategory.SubItems = GetSubcategories(subCategory.ItemId);

                    if (subCategory.SubItems.Count == 0)
                        subCategory.HasChild = false;

                    if (subCategory.DisplaySubItems)
                    {
                        foreach (var subChildrenCategory in subCategory.SubItems.Where(x => x.HasChild))
                        {
                            subChildrenCategory.SubItems = GetSubcategories(subChildrenCategory.ItemId);
                        }
                    }

                    if (subCategory.DisplayBrandsInMenu)
                    {                        
                        subCategory.Brands = BrandService.GetBrandsByCategoryId(subCategory.ItemId, true).Select(brand => new MenuBrandModel
                        {
                            Name = brand.Name,
                            UrlPath = brand.UrlPath,
                            BrandLogo = new MenuBrandLogoModel { PhotoName = brand.BrandLogo.PhotoName } // BrandLogo preloaded
                        }).ToList();
                    }
                }
                return menuItems;
            });

            int? currentCategoryId;
            if (HttpContext.Current != null && (currentCategoryId = (int?)HttpContext.Current.Items["CurrentCategoryId"]).HasValue)
            {
                menuItem = menuItem.DeepCloneJson(); // don't modify object in cache
                var categoryIds = CategoryService.GetParentCategoryIds(currentCategoryId.Value);
                var parentMenuItem = menuItem.SubItems.FirstOrDefault(subItem => categoryIds.Contains(subItem.ItemId));
                if (parentMenuItem != null)
                    parentMenuItem.Selected = true;
            }

            return menuItem;
        }

        /// <summary>
        /// Get main menu items
        /// </summary>
        /// <returns></returns>
        public List<MenuItemModel> GetMenuItems()
        {
            var isRegistered = CustomerContext.CurrentCustomer.RegistredUser;
            var cacheName = !isRegistered
                                ? CacheNames.GetMainMenuCacheObjectName() + "MainMenu"
                                : CacheNames.GetMainMenuAuthCacheObjectName() + "MainMenu";
            var menuType = isRegistered
                                ? EMenuItemShowMode.Authorized
                                : EMenuItemShowMode.NotAuthorized;

            var menuItems = CacheManager.Get(cacheName, CacheTime, () =>
            {
                var mItems = MenuService.GetMenuItems(0, EMenuType.Top, menuType);

                foreach (var item in mItems.Where(item => item.HasChild))
                    item.SubItems = MenuService.GetMenuItems(item.ItemId, EMenuType.Top, menuType);

                return mItems;
            }).DeepCloneJson(); // don't modify object in cache

            foreach(var menuItem in menuItems)
            {
                if (UrlService.IsCurrentUrl(menuItem.UrlPath) || menuItem.SubItems.Any(subItem => UrlService.IsCurrentUrl(subItem.UrlPath)))
                    menuItem.Selected = true;
            }

            return menuItems;
        }


        #region Help methods

        private List<MenuItemModel> GetSubcategories(int categoryId)
        {
            
            var subcategories =
                CategoryService.GetChildCategoriesByCategoryId(categoryId, false)
                    .Where(cat => cat.Enabled && !cat.Hidden)
                    .Select(c => new MenuItemModel()
                    {
                        ItemId = c.CategoryId,
                        ItemParentId = c.ParentCategoryId,
                        Name = c.Name,
                        UrlPath = _urlHelper.AbsoluteRouteUrl("Category", new { url = c.UrlPath }),
                        HasChild = c.HasChild,
                        DisplayBrandsInMenu = c.DisplayBrandsInMenu,
                        DisplaySubItems = c.DisplaySubCategoriesInMenu,
                        ProductsCount = SettingsCatalog.ShowOnlyAvalible ? c.Available_Products_Count : c.ProductsCount
                    }).ToList();

            return subcategories;
        }

        private List<MenuItemModel> GetRootSubcategories(int categoryId)
        {
            return
                CategoryService.GetChildCategoriesByCategoryIdForMenu(categoryId).Where(cat=> !cat.Hidden)
                    .Select(c => new MenuItemModel()
                    {
                        ItemId = c.CategoryId,
                        ItemParentId = c.ParentCategoryId,
                        Name = c.Name,
                        UrlPath = _urlHelper.AbsoluteRouteUrl("Category", new { url = c.UrlPath }),
                        HasChild = c.HasChild,
                        DisplayBrandsInMenu = c.DisplayBrandsInMenu,
                        DisplaySubItems = c.DisplaySubCategoriesInMenu,
                        IconPath = c.Icon != null ? c.Icon.IconSrc() : "",
                        ProductsCount = SettingsCatalog.ShowOnlyAvalible ? c.Available_Products_Count : c.ProductsCount
                    }).ToList();
        }

        #endregion
    }
}
