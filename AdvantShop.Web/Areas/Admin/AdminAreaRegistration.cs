using System.Web.Mvc;

namespace AdvantShop.Web.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "AdminV2"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_login",
                "AdminV2/login",
                new { controller = "Account", action = "Login", area = "AdminV2" }
            );

            context.MapRoute(
                "Admin_project",
                "AdminV2/projects/{taskGroupId}",
                new { controller = "Tasks", action = "Project", area = "AdminV2" }
            );

            context.MapRoute(
                "Admin_default",
                "AdminV2/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "AdminV2", id = UrlParameter.Optional }
            );

            context.MapRoute(
                "Webhook",
                "webhook/{controller}/{action}/{apikey}",
                new { controller = "Home", action = "Index", area = "AdminV2", apikey = UrlParameter.Optional }
            );
        }
    }

    public class AdminV3AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get { return "AdminV3"; }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Admin_v3_project",
                "AdminV3/projects/{taskGroupId}",
                new { controller = "Tasks", action = "Project", area = "AdminV3" }
            );

            context.MapRoute(
                "Admin_v3_default",
                "AdminV3/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", area = "AdminV3", id = UrlParameter.Optional }
            );

            
        }
    }
}