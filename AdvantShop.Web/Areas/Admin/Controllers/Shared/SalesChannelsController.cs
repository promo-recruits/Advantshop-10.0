using System.Linq;
using System.Web.Mvc;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Web.Admin.Controllers.Shared
{
    public partial class SalesChannelsController : BaseAdminController
    {
        [HttpGet]
        public JsonResult GetList()
        {
            var items = SalesChannelService.GetList().Where(x => x.IsShowInList());
            return Json(items);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Add(ESalesChannelType type, string moduleStringId)
        {
            var salesChannel = SalesChannelService.GetByType(type, moduleStringId);
            if (salesChannel == null)
                return JsonError();
            
            if (salesChannel.Type == ESalesChannelType.Module && AttachedModules.GetModuleById(salesChannel.ModuleStringId) == null)
            {
                return JsonOk(new {url = "modules/market?name=" + salesChannel.ModuleStringId.ToLower()});
            }
            
            salesChannel.Enabled = true;

            var menuUrl = salesChannel.MenuUrlAction;
            var url = menuUrl != null
                ? Url.AbsoluteActionUrl(menuUrl.Action, menuUrl.Controller, menuUrl.RouteDictionary)
                : salesChannel.Url;

            return JsonOk(new {url});
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult Delete(ESalesChannelType type, string moduleStringId)
        {
            var salesChannel = SalesChannelService.GetByType(type, moduleStringId);
            if (salesChannel != null)
                salesChannel.Enabled = false;

            if (salesChannel != null && salesChannel.Type == ESalesChannelType.Module)
                return JsonOk(new { url = "modules/market?name=" + salesChannel.ModuleStringId.ToLower() });

            return JsonOk();
        }

    }
}
