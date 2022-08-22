<%@ WebHandler Language="C#" Class="Admin.HttpHandlers.Modules.SetModuleActive" %>

using System.Linq;
using System.Text;
using System.Web;
using AdvantShop.Core.HttpHandlers;
using AdvantShop.Core.Modules;
using AdvantShop.Trial;
using AdvantShop.Core.Services.Localization;
using Newtonsoft.Json;

namespace Admin.HttpHandlers.Modules
{
    public class SetModuleActive : AdminHandler, IHttpHandler
    {
        public new void ProcessRequest(HttpContext context)
        {
            if (!Authorize(context))
                return;

            var active = false;

            if (string.IsNullOrEmpty(context.Request["modulestringid"]) || !bool.TryParse(context.Request["active"], out active))
            {
                ReturnResult(context, false, "error", null);
            }

            string stringId = context.Request["modulestringid"];

            ModulesRepository.SetActiveModule(stringId, active);

            TrialService.TrackEvent(active ? TrialEvents.ActivateModule : TrialEvents.DeactivateModule, stringId);

            if (stringId == "YaMetrika" && active)
            {
                TrialService.TrackEvent(TrialEvents.SetUpYandexMentrika, string.Empty);
            }

            AdvantShop.Core.Modules.Module module = null;

            if (AdvantShop.Saas.SaasDataService.IsSaasEnabled)
            {
                module =
                    ModulesService.GetModules().Items.FirstOrDefault(x => x.StringId.ToLower() == stringId.ToLower());
            }


            ReturnResult(context, active, active ? LocalizationService.GetResource("Admin.Modules.Details.Active") : LocalizationService.GetResource("Admin.Modules.Details.NotActive"), module);
        }

        private static void ReturnResult(HttpContext context, bool active, string result, AdvantShop.Core.Modules.Module module)
        {
            context.Response.ContentType = "application/JSON";
            context.Response.ContentEncoding = Encoding.UTF8;

            context.Response.Write(JsonConvert.SerializeObject(new
            {
                active,
                state = result,
                saasAndPaid = module != null && module.Price > 0f
            }));
            context.Response.End();
        }
    }
}