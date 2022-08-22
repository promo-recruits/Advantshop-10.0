using AdvantShop.Helpers;
using System;
using System.Web.Mvc;

namespace AdvantShop.Web.Infrastructure.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class MobileSessionAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            // Ставим куку с временем жизни Session, чтобы пофиксить баг в сафари при повторном запуске браузера
            CommonHelper.SetCookie("mobileSession", DateTime.Now.ToString(), isSession: true);
        }
    }
}
