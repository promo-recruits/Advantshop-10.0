using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.CMS;
using AdvantShop.Core.Services.Configuration.Settings;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class CommonController : BaseMobileController
    {
        [ChildActionOnly]
        public ActionResult BreadCrumbs(List<BreadCrumbs> breadCrumbs)
        {
            if (breadCrumbs == null || breadCrumbs.Count <= 2)
                return new EmptyResult();

            return PartialView(breadCrumbs);
        }
        
        [ChildActionOnly]
        public ActionResult BottomPanel()
        {
            if (!SettingsMobile.ShowBottomPanel)
                return new EmptyResult();

            return PartialView();
        }
    }
}
