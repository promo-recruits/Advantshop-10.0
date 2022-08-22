using System.Collections.Generic;
using System.Web;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Admin;
using AdvantShop.Web.Admin.Models.Cms.Menus;
using Newtonsoft.Json;

namespace AdvantShop.Web.Admin.Handlers.Shared.Common
{
    public class GetAdminMenuObject
    {
        public List<AdminGroupMenuModel> Execute(bool isDashboard = false, bool isSalesChannelSelected = false, bool isMobileTemplate = false)
        {
            var adminAreaTemplate = AdminAreaTemplate.Current;

            var items = CacheManager.Get(CacheNames.AdminMenu + adminAreaTemplate + isDashboard + "_" + isMobileTemplate, () =>
            {
                var file = adminAreaTemplate == null
                    ? "~/Areas/Admin/menu.json"
                    : "~/Areas/Admin/Templates/" + adminAreaTemplate + "/menu" + (isDashboard ? "_dashboard" : "") + ".json";

                var filePath = HostingEnvironment.MapPath(file);
                if (!string.IsNullOrEmpty(filePath) && System.IO.File.Exists(filePath))
                {
                    var json = System.IO.File.ReadAllText(filePath);

                    var menuObject = JsonConvert.DeserializeObject<List<AdminGroupMenuModel>>(json);
                    if (menuObject != null)
                    {
                        return menuObject;
                    }
                }
                return new List<AdminGroupMenuModel>();
            });

            var groups = items.DeepClone();

            foreach (var group in groups)
            {
                SetVisible(group.Menu, isMobileTemplate);
                
                if (!isSalesChannelSelected)
                    SetSelected(group.Menu);

                SetOpacity(group.Menu);
            }

            return groups;

        }

        private void SetVisible(List<AdminMenuModel> items, bool isMobileTemplate)
        {
            foreach (var menuItem in items)
            {
                menuItem.SetVisible(menuItem, isMobileTemplate);
            }
        }

        private void SetSelected(List<AdminMenuModel> items)
        {
            var controller = HttpContext.Current.Request.RequestContext.RouteData.Values["controller"] as string;
            var action = HttpContext.Current.Request.RequestContext.RouteData.Values["action"] as string;

            if (controller != null)
                controller = controller.ToLower();
            if (action != null)
                action = action.ToLower();

            foreach (var menuItem in items)
            {
                if (menuItem.SetSelected(action, controller))
                    break;
            }
        }

        private void SetOpacity(List<AdminMenuModel> items)
        {            
            foreach (var item in items)
            {
                item.SetActiveInSaas(item);
                item.SetActiveBySetting(item);
            }
        }
    }
}
