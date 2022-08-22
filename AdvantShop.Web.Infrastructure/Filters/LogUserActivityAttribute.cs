using System.Web.Mvc;
using AdvantShop.Core;
using AdvantShop.Core.Services.Loging;
using AdvantShop.Helpers;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class LogUserActivityAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                return;
            }

            if (filterContext.IsChildAction || filterContext.HttpContext == null)
                return;

            var request = filterContext.HttpContext.Request;

            if (request.IsAjaxRequest() || request.HttpMethod == "POST" || Helpers.BrowsersHelper.IsBot())
                return;

            var loger = LogingManager.GetTrafficSourceLoger();
            loger.LogTrafficSource();
        }
    }
}