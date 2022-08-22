using System;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.Services.Configuration;
using AdvantShop.Core.UrlRewriter;

namespace AdvantShop.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AccessByChannel : ActionFilterAttribute
    {
        private readonly EProviderSetting _providerSetting;
        private readonly ETypeRedirect _redirect;

        public AccessByChannel(EProviderSetting providerSetting) : this(providerSetting, ETypeRedirect.AdminPanel)
        { }

        public AccessByChannel(EProviderSetting providerSetting, ETypeRedirect redirect)
        {
            _providerSetting = providerSetting;
            _redirect = redirect;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (AppServiceStartAction.state != PingDbState.NoError)
                return;

            if (filterContext.IsChildAction)
                return;
            
            var settignValue = SettingProvider.GetSqlSettingValue(_providerSetting);
            var value = false;
            var providerSetting = string.IsNullOrEmpty(settignValue) || (Boolean.TryParse(settignValue, out value) && value);
            
            if (providerSetting)
                return;

            var controller = (string)filterContext.RouteData.Values["controller"];
            var action = (string)filterContext.RouteData.Values["action"];

            var strCompare = StringComparison.OrdinalIgnoreCase;
            if ((controller.Equals("error", strCompare) && action.Equals("liccheck", strCompare)) ||
                (controller.Equals("user", strCompare) && action.Equals("logout", strCompare)))
                return;

            if (_redirect == ETypeRedirect.Empty)
                filterContext.Result = new EmptyResult();

            if (_redirect == ETypeRedirect.AdminPanel)
                filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("adminv2/login"));

            if (_redirect == ETypeRedirect.Error404)
                filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("error/notfound"));
        }
    }
}
