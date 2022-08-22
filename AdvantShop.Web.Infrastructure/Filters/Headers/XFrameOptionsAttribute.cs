using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using System;
using System.Web.Mvc;
using AdvantShop.Configuration;

namespace AdvantShop.Web.Infrastructure.Filters.Headers
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class XFrameOptionsAttribute : HttpHeaderAttributeBase
    {
        public XFrameOptionsPolicy Policy { get; set; }
        public string AllowUrl { get; set; }

        public XFrameOptionsAttribute(XFrameOptionsPolicy disabled)
        {
            this.Policy = disabled;
        }

        public override void SetHttpHeadersOnActionExecuted(ActionExecutedContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                return;
            }

            var request = filterContext.HttpContext.Request;
            var url = request.Url.ToString();
            
            var socialType = UrlService.IsSocialUrl(url);
            if (socialType != UrlService.ESocialType.none)
                return;
            
            var referrer = request.GetUrlReferrer();

            if (SettingsGeneral.DisableXFrameOptionsHeader ||
                Demo.IsDemoEnabled ||
                (referrer != null && (referrer.Host.Contains("webvisor.com") || referrer.Host.Contains("yandex.net") || referrer.Host == request.Url.Host)))
            {
                return;
            }

            var response = filterContext.HttpContext.Response;

            switch (Policy)
            {
                case XFrameOptionsPolicy.Disabled:
                    return;
                case XFrameOptionsPolicy.Deny:
                    response.Headers[HeaderConstants.XFrameOptionsHeader] = "Deny";
                    return;
                case XFrameOptionsPolicy.SameOrigin:
                    response.Headers[HeaderConstants.XFrameOptionsHeader] = "SameOrigin";
                    return;
                case XFrameOptionsPolicy.AllowFrom:
                    response.Headers[HeaderConstants.XFrameOptionsHeader] = "ALLOW-FROM " + AllowUrl;
                    return;
                default:
                    throw new NotImplementedException("Wrong XFrameOptionsPolicy " + Policy);
            }
        }
    }

    public enum XFrameOptionsPolicy
    {
        //Specifies that the X-Frame-Options header should not be set in the HTTP response.
        Disabled,

        //Specifies that the X-Frame-Options header should be set in the HTTP response, instructing the browser to not
        //display the page when it is loaded in an iframe.
        Deny,

        //Specifies that the X-Frame-Options header should be set in the HTTP response, instructing the browser to
        //display the page when it is loaded in an iframe - but only if the iframe is from the same origin as the page.
        SameOrigin,

        //The page can only be displayed in a frame on the specified origin. not work in chrome and safari
        AllowFrom
    }
}
