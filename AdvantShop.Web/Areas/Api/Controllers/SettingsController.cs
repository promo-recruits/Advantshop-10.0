using System.Collections.Generic;
using System.Web.Mvc;
using AdvantShop.Areas.Api.Attributes;
using AdvantShop.Areas.Api.Handlers.Settings;
using AdvantShop.Areas.Api.Models.Settings;
using AdvantShop.Web.Infrastructure.Filters;

namespace AdvantShop.Areas.Api.Controllers
{
    [LogRequest, AuthApi]
    public class SettingsController : BaseApiController
    {
        // GET api/settings?keys=settingName1,settingName2
        [HttpGet]
        public JsonResult Get(string keys) => JsonApi(new GetSettings(keys));
    }
}