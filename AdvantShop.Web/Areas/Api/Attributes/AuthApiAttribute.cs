using System.Web.Mvc;
using AdvantShop.Core.Services.Api;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Web.Infrastructure.ActionResults;

namespace AdvantShop.Areas.Api.Attributes
{
    public class AuthApiAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (string.IsNullOrWhiteSpace(SettingsApi.ApiKey))
            {
                filterContext.Result = ErrorResult("Check apikey");
            }
            else
            {
                var apikey = filterContext.HttpContext.Request["apikey"];
                var authApiKey = filterContext.HttpContext.Request.Headers["Authorization"];
                
                if (string.IsNullOrWhiteSpace(apikey) && string.IsNullOrWhiteSpace(authApiKey))
                {
                    filterContext.Result = ErrorResult("Invalid apikey");
                }
                else if (!string.IsNullOrWhiteSpace(apikey) && apikey != SettingsApi.ApiKey)
                {
                    filterContext.Result = ErrorResult("Invalid apikey");
                }
                else if (!string.IsNullOrWhiteSpace(authApiKey) && authApiKey != SettingsApi.ApiKey)
                {
                    filterContext.Result = ErrorResult("Invalid apikey in authorization");
                }
            }
        }

        private ActionResult ErrorResult(string error)
        {
            return new JsonNetResult {Data = new ApiError(error)};
        }
    }
}