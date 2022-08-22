using AdvantShop.Core.UrlRewriter;
using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Extensions
{
    public static class UrlExtensions
    {
        public static string AbsoluteRouteUrl(this UrlHelper url, string routeName, object routeValues = null)
        {
            return UrlService.GenerateBaseUrl() + url.RouteUrl(routeName, routeValues);
        }

        public static string AbsoluteActionUrl(this UrlHelper url, string actionName, string controllerName, object routeValues = null)
        {
            return url.Action(actionName, controllerName, routeValues, UrlService.IsSecureConnection(url.RequestContext.HttpContext.Request) ? "https" : "http");
        }
    }
}