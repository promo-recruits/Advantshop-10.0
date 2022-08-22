using System;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using AdvantShop.App.Landing;
using AdvantShop.Areas.AdminMobile;
using AdvantShop.Areas.Api;
using AdvantShop.Areas.Mobile;
using AdvantShop.Areas.Partners;
using AdvantShop.Configuration;
using AdvantShop.Controllers;
using AdvantShop.Core;
using AdvantShop.Diagnostics;
using AdvantShop.Web.Admin;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Web.Infrastructure.Templates;

namespace AdvantShop
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            MvcHandler.DisableMvcResponseHeader = true;

            // Replace the default RazorViewEngine with our custom RazorThemeViewEngine
            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new RazorThemeViewEngine());
            ConfigureAntiForgeryTokens();

            //AreaRegistration.RegisterAllAreas();
            RegisterArea<LandingAreaRegistration>(RouteTable.Routes);
            RegisterArea<AdminMobileAreaRegistration>(RouteTable.Routes);
            RegisterArea<ApiAreaRegistration>(RouteTable.Routes);
            RegisterArea<MobileAreaRegistration>(RouteTable.Routes);
            RegisterArea<AdminAreaRegistration>(RouteTable.Routes);
            RegisterArea<AdminV3AreaRegistration>(RouteTable.Routes);
            RegisterArea<PartnersAreaRegistration>(RouteTable.Routes);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BinderConfig.Regist();

            //exclude filter if need
            var providers = FilterProviders.Providers.ToArray();
#if DEBUG
            if (SettingProvider.GetConfigSettingValue("Profiling") == "true")
            {
                Array.Resize(ref providers, providers.Length + 1);
                providers[providers.Length - 1] = new ProfilingActionFilterProvider();
            }
#endif

            FilterProviders.Providers.Clear();
            FilterProviders.Providers.Add(new ExcludeFilterProvider(providers));

            ApplicationService.StartApplication(HttpContext.Current);

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            SessionServices.StartSession(HttpContext.Current);
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            if (AppServiceStartAction.state == PingDbState.NoError)
            {
                Localization.Culture.InitializeCulture();
            }
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_PreSendRequestHeaders()
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }


        protected void Application_Error(object sender, EventArgs e)
        {
            var ex = Server.GetLastError();

            var httpException = ex as HttpException;
            if (httpException != null)
            {
                var routeData = new RouteData();
                routeData.Values.Add("controller", "Error");

                switch (httpException.GetHttpCode())
                {
                    case 400:
                        Debug.Log.Warn(ex.Message, ex);
                        routeData.Values.Add("action", "BadRequest");
                        break;
                    case 403:
                        Debug.Log.Warn(ex.Message, ex);
                        routeData.Values.Add("action", "Forbidden");
                        break;
                    case 404:
                        Debug.Log.Error(ex.Message, ex);
                        routeData.Values.Add("action", "NotFound");
                        break;
                    default:
#if DEBUG
                        return;
#endif
                        routeData.Values.Add("action", "InternalServerError");
                        Debug.Log.Error(ex.Message, ex);
                        break;
                }

                Response.Clear();
                Server.ClearError();
                Response.TrySkipIisCustomErrors = true;

                IController errorController = new ErrorController();
                errorController.Execute(new RequestContext(new HttpContextWrapper(Context), routeData));
            }
            else
            {
                Debug.Log.Error(ex.Message, ex);
            }
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }

        private static void RegisterArea<T>(RouteCollection routes) where T : AreaRegistration
        {
            var registration = (AreaRegistration)Activator.CreateInstance(typeof(T));
            var registrationContext = new AreaRegistrationContext(registration.AreaName, routes, null);
            var areaNamespace = registration.GetType().Namespace;

            if (!String.IsNullOrEmpty(areaNamespace))
                registrationContext.Namespaces.Add(areaNamespace + ".*");

            registration.RegisterArea(registrationContext);
        }

        private static void ConfigureAntiForgeryTokens()
        {
            AntiForgeryConfig.CookieName = "f";
            AntiForgeryConfig.SuppressXFrameOptionsHeader = true;
            // AntiForgeryConfig.RequireSsl = true;
        }
    }
}