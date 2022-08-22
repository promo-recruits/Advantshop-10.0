//--------------------------------------------------
// Project: AdvantShop.NET
// Web site: http:\\www.advantshop.net
//--------------------------------------------------

using System;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Modules;
using AdvantShop.Core.Modules.Interfaces;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Trial;
using System.IO;
using AdvantShop.Core.Services.Landing;
using AdvantShop.FilePath;
using AdvantShop.Core.Services.Diagnostics;
using System.Web.Mvc;

namespace AdvantShop.Core.UrlRewriter
{
    public class HttpUrlRewrite : IHttpModule
    {
        #region IHttpModule Members

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.BeginRequest += OnBeginRequest;
        }

        #endregion

        private static void OnBeginRequest(object sender, EventArgs e)
        {
            var app = (HttpApplication)sender;
            string strCurrentUrl = app.Request.RawUrl.ToLower();
            app.StaticFile304();
            
            // Check cn
            if (AppServiceStartAction.state != PingDbState.NoError)
            {
                // Nothing here
                // just return
                return;
            }

            if (UrlService.IsIpBanned(app.Request.UserHostAddress))
            {
                app.Response.Clear();
                app.Response.Write("error: " + app.Request.UserHostAddress);
                app.Response.End();
                return;
            }

            if (strCurrentUrl.Contains("adminv2/userfiles") || strCurrentUrl.Contains("adminv3/userfiles"))
            {
                app.Context.RewritePath(strCurrentUrl.Replace("adminv2/userfiles", "userfiles").Replace("adminv3/userfiles", "userfiles"));
                return;
            }

            if (UrlService.IsDebugUrl(strCurrentUrl) || strCurrentUrl.Contains("/signalr/hubs"))
                return;

            // Check original pictures
            if (strCurrentUrl.Contains("/pictures/product/original/"))
            {
                app.RewriteTo404();
                return;
            }

            if (strCurrentUrl.Contains("/content/price_temp"))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer == null ||
                    !(customer.IsAdmin || customer.IsVirtual || TrialService.IsTrialEnabled ||
                      (customer.IsModerator &&
                       RoleActionService.GetCustomerRoleActionsByCustomerId(customer.Id).Any(x => x.Role == RoleAction.Orders || x.Role == RoleAction.Catalog))))
                {
                    app.RewriteTo404();
                    return;
                }
            }

            if (strCurrentUrl == "/favicon.ico" && !string.IsNullOrEmpty(SettingsMain.FaviconImageName))
            {
                app.Context.RewritePath(strCurrentUrl.Replace("favicon.ico", FoldersHelper.GetPathRelative(FolderType.Pictures, SettingsMain.FaviconImageName, false)));
                return;
            }

            string path = strCurrentUrl;
            if (app.Request.ApplicationPath != "/")
            {
                if (app.Request.ApplicationPath != null)
                    path = path.Replace(app.Request.ApplicationPath.ToLower(), "");
            }

            if (strCurrentUrl.Contains("robots.txt"))
            {
                if (MobileHelper.IsMobileByUrl())
                {
                    app.Context.RewritePath("areas/mobile/robots.txt");
                    return;
                }
                var lpSiteId = 0;
                if (LandingHelper.IsLandingDomain(app.Request.Url, out lpSiteId))
                {
                    app.Context.RewritePath("pictures/landing/" + lpSiteId + "/robots.txt");
                    return;
                }
            }
            
            var index = path.IndexOf('?');
            var pathWhitoutQuery = index > 0 ? path.Substring(0, index) : path;

            if (pathWhitoutQuery.Contains("/content/attachments/"))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer == null || !(customer.IsAdmin || customer.IsVirtual || customer.IsModerator || TrialService.IsTrialEnabled))
                {
                    app.Response.Redirect(UrlService.GetAdminUrl("login") + "?from=" + strCurrentUrl);
                    return;
                }
            }

            if (pathWhitoutQuery.Contains("/" + FoldersHelper.PhotoFoldersPath[FolderType.TemplateDocx]))
            {
                var customer = CustomerContext.CurrentCustomer;
                if (customer == null || !(customer.IsAdmin || customer.IsVirtual || customer.IsModerator || TrialService.IsTrialEnabled))
                {
                    app.Response.Redirect(UrlService.GetAdminUrl("login") + "?from=" + strCurrentUrl);
                    return;
                }
            }

            if (pathWhitoutQuery.Contains("/sitemap"))
            {
                var lpSiteId = 0;
                if (LandingHelper.IsLandingDomain(app.Request.Url, out lpSiteId))
                {
                    app.Context.RewritePath("pictures/landing/" + lpSiteId + pathWhitoutQuery);
                    return;
                }
            }

            var extention = FileHelpers.GetExtension(pathWhitoutQuery);

            if (!string.IsNullOrEmpty(extention) &&
                !UrlRewriteExtensions.AlowedExtensionsForTechDomainClosed.Contains(extention) &&
                app.Request.IsTechDomainClosed())
            {
                app.Response.Redirect(UrlService.GetUrl("errorext/techdomainclosed"));
                return;
            }

            if (string.IsNullOrEmpty(extention) && app.Request.IsAppBlock())
            {
                app.Context.RewritePath("~/app_block.htm");
                return;
            }

            if (UrlService.ExtentionNotToRedirect.Contains(extention) && !pathWhitoutQuery.Contains("/robots.txt") && !pathWhitoutQuery.Contains("/sitemap"))
            {
                if (UrlService.ExtentionOpenInBrowser.Contains(extention) && app.Request["OpenInBrowser"] == "true")
                {
                    app.Response.Clear();
                    app.Response.ContentType = "text/plain";
                    app.Response.AddHeader("content-disposition", "inline;filename=" + Path.GetFileName(app.Request.FilePath));
                    app.Response.WriteFile(System.Web.Hosting.HostingEnvironment.MapPath(app.Request.FilePath));
                    app.Response.End();
                }
                return;
            }

            if (strCurrentUrl.Contains("/module/"))
            {
                app.Context.RewritePath(strCurrentUrl.Replace("/module/", "/"));
                return;
            }

            if (path.Contains("adv-admin.aspx") || pathWhitoutQuery == "/admin")
            {
                int lpSiteId;
                app.Response.Redirect((LandingHelper.IsLandingDomain(app.Request.Url, out lpSiteId) ? SettingsMain.SiteUrl + "/" : "") + "adminv2/login");
            }

            var absoluteUri = (app.Request.Headers["x-forwarded-proto"] == "https"
                ? app.Request.Url.AbsoluteUri.Replace("http://", "https://")
                : app.Request.Url.AbsoluteUri).ToLower();

            var pathAndQuery = app.Request.Url.PathAndQuery;

            if (absoluteUri.EndsWith(pathAndQuery, StringComparison.Ordinal))
                absoluteUri = absoluteUri.Substring(0, absoluteUri.Length - pathAndQuery.Length) + strCurrentUrl;
            
            //301 redirect if need
            if (SettingsSEO.Enabled301Redirects && 
                !path.Contains("/admin/") && !path.Contains("/api/") && !path.Contains("paymentnotification") && 
                !DebugMode.IsDebugMode(eDebugMode.Redirects) && 
                !(new HttpRequestWrapper(app.Request).IsAjaxRequest() || app.Request.HttpMethod == "POST") && 
                !app.Request.IsTechDomainClosed())
            {
                var newUrl = UrlService.GetRedirect301(path.Trim('/'), absoluteUri.Trim('/'));
                if (newUrl.IsNotEmpty())
                {
                    var query = app.Request.Url.Query;
                    if (newUrl.EndsWith(query, StringComparison.OrdinalIgnoreCase))
                        newUrl = newUrl.Substring(0, newUrl.Length - query.Length) + query;

                    app.Response.RedirectPermanent(newUrl);
                    return;
                }

                var dirPath = path.Split("?").FirstOrDefault();
                if (dirPath != "/" && dirPath.EndsWith("/"))
                {
                    app.Response.RedirectPermanent(absoluteUri.Split("?").FirstOrDefault().Trim('/') + app.Request.Url.Query);
                    return;
                }

                // checking double slashes in path
                var requestUrl = (app.Request.ServerVariables["REQUEST_URI"] ?? "").ToLower();
                var rewriteUrl = (app.Request.ServerVariables["UNENCODED_URL"] ?? "").ToLower();

                if (requestUrl != null && rewriteUrl != null)
                {
                    if (rewriteUrl.Split('?')[0].Contains("//") && !requestUrl.Contains("//"))
                    {
                        app.Response.RedirectPermanent(requestUrl);
                        return;
                    }
                    // checking double qoutes in path
                    if (requestUrl.Split('?')[0].Contains("\""))
                    {
                        app.Response.RedirectPermanent(requestUrl.Replace("\"", ""));
                        return;
                    }
                }
            }

            string rewriteLpUrl;
            if (LandingHelper.IsLandingUrl(app, app.Request.Url, extention, out rewriteLpUrl))
            {
                app.Context.RewritePath(rewriteLpUrl);
                return;
            }

            UrlService.ESocialType socialType;

            if (MobileHelper.IsMobileEnabled()) //&& !strCurrentUrl.Contains("/adminv3/") && !strCurrentUrl.Contains("/adminv2/"))
            {
                SettingsDesign.IsMobileTemplate = SettingsMobile.IsMobileTemplateActive;

                if (!SettingsDesign.IsMobileTemplate && MobileHelper.IsMobileByUrl())
                {
                    app.Context.RewritePath("error/forbidden");
                    return;
                }
            }

            if ((socialType = UrlService.IsSocialUrl(absoluteUri)) != UrlService.ESocialType.none)
            {
                if ((socialType == UrlService.ESocialType.vk && !SettingsDesign.IsVkTemplateActive) ||
                    (socialType == UrlService.ESocialType.fb && !SettingsDesign.IsFbTemplateActive))
                {
                    app.Context.RewritePath("error/forbidden");
                    return;
                }
            }

            var modules = AttachedModules.GetModules<IModuleUrlRewrite>();
            foreach (var moduleType in modules)
            {
                var moduleObject = (IModuleUrlRewrite)Activator.CreateInstance(moduleType);
                var newUrl = path;
                if (moduleObject != null && moduleObject.RewritePath(path, ref newUrl))
                {
                    // for trial and virtual path
                    if (newUrl.StartsWith("/"))
                        newUrl = "~" + newUrl;
                    app.Context.RewritePath(newUrl);
                    return;
                }
            }
        }
    }
}