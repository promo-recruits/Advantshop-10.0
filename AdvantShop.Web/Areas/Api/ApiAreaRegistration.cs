using System.Web.Mvc;
using System.Web.Routing;

namespace AdvantShop.Areas.Api
{
    public class ApiAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Api";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "Api_1C",
                url: "api/1c/{action}/",
                defaults: new {controller = "OneC", action = "Index"},
                namespaces: new[] {"AdvantShop.Areas.Api.Controllers"}
                );

            #region Customers

            context.MapRoute(
                name: "Api_Customers_Get",
                url: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "Get" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET"), id = "[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}" }
            );

            context.MapRoute(
                name: "Api_Customers_Update",
                url: "api/customers/{id}",
                defaults: new { controller = "Customers", action = "Update", id = UrlParameter.Optional },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST"), id = "[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}" }
            );

            context.MapRoute(
                name: "Api_Customers_Bonuses",
                url: "api/customers/{id}/bonuses",
                defaults: new { controller = "Customers", action = "Bonuses", id = UrlParameter.Optional },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET"), id = "[A-Z0-9]{8}-([A-Z0-9]{4}-){3}[A-Z0-9]{12}" }
            );

            context.MapRoute(
                name: "Api_Customers_Filter",
                url: "api/customers",
                defaults: new { controller = "Customers", action = "Filter" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" }
            );

            #endregion

            #region Bonuses

            context.MapRoute(
                name: "Api_Bonuses_GetSettings",
                url: "api/bonus-cards/settings",
                defaults: new { controller = "BonusCards", action = "GetSettings" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            context.MapRoute(
                name: "Api_Bonuses_SaveSettings",
                url: "api/bonus-cards/settings",
                defaults: new { controller = "BonusCards", action = "SaveSettings" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );


            context.MapRoute(
                name: "Api_Bonuses_GetCard",
                url: "api/bonus-cards/{id}",
                defaults: new { controller = "BonusCards", action = "Card" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            context.MapRoute(
                name: "Api_Bonuses_Create",
                url: "api/bonus-cards/add",
                defaults: new { controller = "BonusCards", action = "Create" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Bonuses_GetCardTransactions",
                url: "api/bonus-cards/{id}/transactions",
                defaults: new { controller = "BonusCards", action = "Transactions" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            context.MapRoute(
                name: "Api_Bonuses_AddMainBonuses",
                url: "api/bonus-cards/{id}/main-bonuses/accept",
                defaults: new { controller = "BonusCards", action = "AcceptMainBonuses" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Bonuses_SubstractMainBonuses",
                url: "api/bonus-cards/{id}/main-bonuses/substract",
                defaults: new { controller = "BonusCards", action = "SubstractMainBonuses" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Bonuses_GetAdditionalBonuses",
                url: "api/bonus-cards/{id}/additional-bonuses",
                defaults: new { controller = "BonusCards", action = "GetAdditionalBonuses" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            context.MapRoute(
                name: "Api_Bonuses_AcceptAdditionalBonuses",
                url: "api/bonus-cards/{id}/additional-bonuses/accept",
                defaults: new { controller = "BonusCards", action = "AcceptAdditionalBonuses" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Bonuses_SubstractAdditionalBonuses",
                url: "api/bonus-cards/{id}/additional-bonuses/substract",
                defaults: new { controller = "BonusCards", action = "SubstractAdditionalBonuses" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            #endregion

            #region Bonus grades

            context.MapRoute(
                name: "Api_Grades_GetGrades",
                url: "api/bonus-grades",
                defaults: new { controller = "BonusGrades", action = "Grades" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );

            #endregion
            
            #region Categories
            
            context.MapRoute(
                name: "Api_Categories_Get",
                url: "api/categories/{id}",
                defaults: new { controller = "Categories", action = "Get" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );
            
            context.MapRoute(
                name: "Api_Categories_Add",
                url: "api/categories/add",
                defaults: new { controller = "Categories", action = "Add" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            context.MapRoute(
                name: "Api_Categories_Delete",
                url: "api/categories/{id}/delete",
                defaults: new { controller = "Categories", action = "Delete" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            #region Pictures
            
            context.MapRoute(
                name: "Api_Categories_Picture_AddByLink",
                url: "api/categories/{id}/picture/addbylink",
                defaults: new { controller = "Categories", action = "PictureAddByLink" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            context.MapRoute(
                name: "Api_Categories_Picture_Add",
                url: "api/categories/{id}/picture/add",
                defaults: new { controller = "Categories", action = "PictureAdd" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Categories_Picture_Delete",
                url: "api/categories/{id}/picture/delete",
                defaults: new { controller = "Categories", action = "PictureDelete" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            // mini picture
            context.MapRoute(
                name: "Api_Categories_MiniPicture_AddByLink",
                url: "api/categories/{id}/mini-picture/addbylink",
                defaults: new { controller = "Categories", action = "MiniPictureAddByLink" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            context.MapRoute(
                name: "Api_Categories_MiniPicture_Add",
                url: "api/categories/{id}/mini-picture/add",
                defaults: new { controller = "Categories", action = "MiniPictureAdd" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Categories_MiniPicture_Delete",
                url: "api/categories/{id}/mini-picture/delete",
                defaults: new { controller = "Categories", action = "MiniPictureDelete" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            // icon
            context.MapRoute(
                name: "Api_Categories_MenuIconPicture_AddByLink",
                url: "api/categories/{id}/menu-icon-picture/addbylink",
                defaults: new { controller = "Categories", action = "MenuIconPictureAddByLink" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            context.MapRoute(
                name: "Api_Categories_MenuIconPicture_Add",
                url: "api/categories/{id}/menu-icon-picture/add",
                defaults: new { controller = "Categories", action = "MenuIconPictureAdd" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );

            context.MapRoute(
                name: "Api_Categories_MenuIconPicture_Delete",
                url: "api/categories/{id}/menu-icon-picture/delete",
                defaults: new { controller = "Categories", action = "MenuIconPictureDelete" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            #endregion
            
            context.MapRoute(
                name: "Api_Categories_Update",
                url: "api/categories/{id}",
                defaults: new { controller = "Categories", action = "Update" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("POST") }
            );
            
            context.MapRoute(
                name: "Api_Categories_Filter",
                url: "api/categories",
                defaults: new { controller = "Categories", action = "Filter" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );
            
            #endregion
            
            #region Settings 
            
            context.MapRoute(
                name: "Api_Settings_Get",
                url: "api/settings",
                defaults: new { controller = "Settings", action = "Get" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );
            
            #endregion
            
            #region Carousels 
            
            context.MapRoute(
                name: "Api_Carousels_Get",
                url: "api/carousels",
                defaults: new { controller = "Carousels", action = "Get" },
                namespaces: new[] { "AdvantShop.Areas.Api.Controllers" },
                constraints: new { httpMethod = new HttpMethodConstraint("GET") }
            );
            
            #endregion
            
            context.MapRoute(
                name: "Api_Default",
                url: "api/{controller}/{action}/{id}",
                defaults: new {controller = "Home", action = "Index", id = UrlParameter.Optional},
                namespaces: new[] {"AdvantShop.Areas.Api.Controllers"}
            );
        }
    }
}