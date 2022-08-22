using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using AdvantShop.Configuration;
using AdvantShop.Customers;
using AdvantShop.Helpers;
using AdvantShop.Saas;
using AdvantShop.Trial;

namespace AdvantShop.Core.UrlRewriter
{
    public static class UrlRewriteExtensions
    {
        private const string _header = "If-Modified-Since";

        private static readonly List<string> AlowedUrlsForTechDomainClosed = new List<string>()
        {
            "/adminv2/",
            "/adminv3/",
            "/areas/admin/",
            "/combine/",
            "/fonts/",
            "/modules/"
        };

        public static readonly List<string> AlowedExtensionsForTechDomainClosed = new List<string>() { ".csv", ".yml", ".xml", ".ashx" };

        public const string TechnicalHeaderName = "letitbe";

        public static void StaticFile304(this HttpApplication app)
        {
            if (!app.Request.Url.AbsolutePath.EndsWith(".xml", StringComparison.OrdinalIgnoreCase))
                return;

            var lastString = app.Request.Headers[_header];
            if (string.IsNullOrWhiteSpace(lastString)) return;

            var fileName = app.Request.PhysicalPath;

            var lastModified = File.GetLastWriteTime(fileName);
            var ifModifiedSince = lastString.TryParseDateTimeGMT();

            if (!ifModifiedSince.HasValue)
            {
                app.Request.Headers.Remove(_header);
            }
            if (ifModifiedSince.HasValue && (ifModifiedSince.Value >= lastModified))
            {
                app.Response.StatusCode = 304;
                app.Response.SuppressContent = true;
                return;
            }
            app.Response.Cache.SetLastModified(lastModified);
        }

        public static DateTime? TryParseDateTimeGMT(this string val)
        {
            DateTime intval;
            if (DateTime.TryParseExact(val, "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out intval))
            {
                return intval.ToLocalTime();
            }
            return null;
        }

        public static void RewriteTo404(this HttpApplication app)
        {
            try
            {
                app.Context.Response.Clear();
                app.Context.Response.TrySkipIisCustomErrors = true;
                app.Context.Response.StatusCode = 404;
                app.Context.Response.StatusDescription = HttpWorkerRequest.GetStatusDescription(404);
                app.Context.Response.End();
            }
            catch
            {
            }
        }

        public static Uri GetUrlReferrer(this HttpRequest request)
        {
            try
            {
                return request.UrlReferrer;
            }
            catch (Exception)
            {
            }
            return null;
        }

        public static Uri GetUrlReferrer(this HttpRequestBase request)
        {
            try
            {
                return request.UrlReferrer;
            }
            catch (Exception)
            {
            }
            return null;
        }

        /// <summary>
        /// Is Chrome-Lighthouse useragent for PageSpeed Insights
        /// </summary>
        public static bool IsLighthouse(this HttpRequest request)
        {
            return !string.IsNullOrEmpty(request.UserAgent) && (request.UserAgent.Contains("Chrome-Lighthouse") || request.UserAgent.Contains("PTST"));
        }

        /// <summary>
        /// Is Chrome-Lighthouse useragent for PageSpeed Insights
        /// </summary>
        public static bool IsLighthouse(this HttpRequestBase request)
        {
            return !string.IsNullOrEmpty(request.UserAgent) && (request.UserAgent.Contains("Chrome-Lighthouse") || request.UserAgent.Contains("PTST"));
        }

        public static bool IsTechnicalHeader(this HttpRequest request)
        {
            var header = request.Headers[TechnicalHeaderName];
            return !string.IsNullOrEmpty(header) && header == SettingsLic.AdvId;
        }

        public static bool IsTechnicalHeader(this HttpRequestBase request)
        {
            var header = request.Headers[TechnicalHeaderName];
            return !string.IsNullOrEmpty(header) && header == SettingsLic.AdvId;
        }

        public static bool AllowedUrlForTechDomainClosed(this HttpRequest request)
        {
            var url = request.Url.ToString().ToLower();
            var index = url.IndexOf('?');
            var pathWhitoutQuery = index > 0 ? url.Substring(0, index) : url;

            return AlowedUrlsForTechDomainClosed.Any(pathWhitoutQuery.Contains);
        }

        public static bool AllowedUrlForTechDomainClosed(this HttpRequestBase request)
        {
            var url = request.Url.ToString().ToLower();
            var index = url.IndexOf('?');
            var pathWhitoutQuery = index > 0 ? url.Substring(0, index) : url;

            return AlowedUrlsForTechDomainClosed.Any(pathWhitoutQuery.Contains);
        }

        public static bool IsTechDomainClosed(this HttpRequest request)
        {
            return (TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled)
                    && SettingsMain.IsTechDomain(request.Url) &&
                   !CustomerContext.CurrentCustomer.IsAdmin && !CustomerService.IsTechDomainGuest() &&
                   !request.AllowedUrlForTechDomainClosed() &&
                   !request.IsTechnicalHeader() &&
                   !request.IsLighthouse();
        }

        public static bool IsTechDomainClosed(this HttpRequestBase request)
        {
            return (TrialService.IsTrialEnabled || SaasDataService.IsSaasEnabled)
                   && SettingsMain.IsTechDomain(request.Url) &&
                   !CustomerContext.CurrentCustomer.IsAdmin && !CustomerService.IsTechDomainGuest() &&
                   !request.AllowedUrlForTechDomainClosed() &&
                   !request.IsTechnicalHeader() &&
                   !request.IsLighthouse();




        }

        public static bool IsAppBlock(this HttpRequest request)
        {
            if (!IsAppBlock())
                return false;

            if (BrowsersHelper.IsBot() && !request.RawUrl.StartsWith("/admin"))
                return false;

            return true;
        }

        public static bool IsAppBlock()
        {
            return File.Exists(HostingEnvironment.MapPath("~/app_block.htm"));
        }
        public static bool IsMobileAppNotAvailable(this HttpRequestBase request)
        {
            var isMobileApp = string.Equals(request.QueryString["utm_source"], "mobileApp",
                StringComparison.InvariantCultureIgnoreCase);
            return isMobileApp && SaasDataService.IsSaasEnabled && !SaasDataService.CurrentSaasData.HaveMobileApp;
        }
    }
}
