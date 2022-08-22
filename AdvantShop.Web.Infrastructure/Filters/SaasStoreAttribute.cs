﻿using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Web.Infrastructure.Filters
{
    public class SaasStoreAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.IsChildAction)
                return;


            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                SessionServices.StartSession(HttpContext.Current);
                return;
            }


            // saas
            
            if (SaasDataService.IsSaasEnabled)
            {
                if (!SaasDataService.CurrentSaasData.IsCorrect)
                {
                    var controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                    var action = filterContext.RouteData.Values["action"].ToString().ToLower();

                    if (!(controller.Equals("error") && action.Equals("liccheck")))
                    {
                        filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("error/liccheck"));
                    }
                }
                //else if (!SaasDataService.CurrentSaasData.IsWork)
                //{
                //    filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("app_offline.html"));
                //}
            }

            if (!SettingsLic.ActiveLic &&
                !(CustomerContext.CurrentCustomer.IsVirtual || Demo.IsDemoEnabled || TrialService.IsTrialEnabled ||
                  SaasDataService.IsSaasEnabled))
            {
                var controller = filterContext.RouteData.Values["controller"].ToString().ToLower();
                var action = filterContext.RouteData.Values["action"].ToString().ToLower();

                if (!(controller.Equals("error") && action.Equals("liccheck")))
                {
                    filterContext.Result = new RedirectResult(UrlService.GetAbsoluteLink("error/liccheck"));
                }
            }
        }
    }
}
