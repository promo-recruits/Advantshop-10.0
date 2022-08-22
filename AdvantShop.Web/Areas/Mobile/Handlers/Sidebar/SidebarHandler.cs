using System.Linq;
using System.Web.Mvc;
using AdvantShop.Areas.Mobile.Models.Sidebar;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.Repository.Currencies;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Areas.Mobile.Handlers.Sidebar
{
    public class SidebarHandler
    {
        public SidebarMobileViewModel Get()
        {
            var model = new SidebarMobileViewModel()
            {
                Customer = CustomerContext.CurrentCustomer,
                StoreName = SettingsMain.ShopName,
                DisplayCity = SettingsMobile.DisplayCity,
                CurrentCity = SettingsMobile.DisplayCity ? IpZoneContext.CurrentZone.City : string.Empty,
                IsShowAdminLink = CustomerContext.CurrentCustomer.Enabled && (CustomerContext.CurrentCustomer.IsAdmin || CustomerContext.CurrentCustomer.IsModerator)
            };

            var isRegistered = CustomerContext.CurrentCustomer.RegistredUser;
            var cacheName = !isRegistered
                                ? CacheNames.GetMainMenuCacheObjectName() + "TopMenu_Mobile" 
                                : CacheNames.GetMainMenuAuthCacheObjectName() + "TopMenu_Mobile";

            var menuType = isRegistered
                                ? EMenuItemShowMode.Authorized
                                : EMenuItemShowMode.NotAuthorized;

            var menuItems = CacheManager.Get(cacheName, () => MenuService.GetMenuItems(0, EMenuType.Mobile, menuType).ToList()).DeepCloneJson(); // don't modify object in cache
            foreach (var menuItem in menuItems.Where(menuItem => UrlService.IsCurrentUrl(menuItem.UrlPath)))
                menuItem.Selected = true;
            model.Menu = menuItems;

            model.IsShowCurrency = SettingsCatalog.AllowToChangeCurrency;

            if (model.IsShowCurrency)
            {
                var currentCurrency = CurrencyService.CurrentCurrency;
                model.CurrentCurrency = CurrencyService.CurrentCurrency;
                foreach (var currency in CurrencyService.GetAllCurrencies(true))
                {
                    model.Currencies.Add(new SelectListItem()
                    {
                        Text = currency.Name,
                        Value = currency.Iso3,
                        Selected = currency.Iso3 == currentCurrency.Iso3
                    });
                }
            }

            model.CatalogMenuViewMode = SettingsMobile.CatalogMenuViewMode.TryParseEnum<SettingsMobile.eCatalogMenuViewMode>(SettingsMobile.eCatalogMenuViewMode.RootCategories);

            return model;
        }
    }
}