using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AdvantShop.Configuration;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Landing.Domains;
using AdvantShop.Core.SQL;
using AdvantShop.Helpers;

namespace AdvantShop.Core.Services.Landing
{
    public static class LandingHelper
    {
        private const string CacheKey = "LandingPage_lp_domains";
        private const string PageCacheKey = "LandingPage_lp_domains_page_";

        private static List<string> excludeUrls = new List<string>() { "lp/", "landing", "areas/landing/", "product/", "productext/",  "common/", "cart/",
                                                                       "checkout/", "coupon/", "reviews/", "location/", "printorder/", "bonuses/", "catalog/",
                                                                       "myaccount/", "user/", "signalr/", ".well-known/",
                                                                       "paymentnotification/", "paymentreturnurl/", "paymentreceipt/", "fail",
                                                                       "zadarma/", "yandextelephony/", "sipuni/", "mangoadvantshop/", "webhook/", "api/",
                                                                       "registration", "forgotpassword", "moduleroute/", "logout", "logogenerator/", "preorder/lp/",
                                                                       "module/"
                                                                     };

        public static bool IsLandingUrl(HttpApplication app, Uri uri, string extention, out string rewriteUrl)
        {
            rewriteUrl = null;

            if (!string.IsNullOrEmpty(extention))
                return false;

            var domains = GetLandingDomains();
            if (domains.Count == 0)
                return false;

            var host = uri.Host; //.Replace("www.", "");

            var domain = domains.FirstOrDefault(x => host == x.DomainUrl || host == StringHelper.ToPuny(x.DomainUrl));
            if (domain == null)
                return false;

            var lpId = 0;
            var pageUrl = uri.AbsolutePath.ToLower().Trim('/');

            if (excludeUrls.Any(x => pageUrl.StartsWith(x)))
                return false;

            if (pageUrl.StartsWith("admin"))
            {
                if (host.Replace("www.", "") != SettingsMain.SiteUrlPlain && host.Replace("www.", "") != StringHelper.ToPuny(SettingsMain.SiteUrlPlain))
                    app.Response.RedirectPermanent(SettingsMain.SiteUrl + "/adminv2");

                return false;
            }

            if (!string.IsNullOrEmpty(pageUrl))
            {
                lpId = GetLpPageId(domain.LandingSiteId, pageUrl);
                if (lpId == 0)
                {
                    rewriteUrl = "~/landing/landing/error404page";
                    return true;
                }
            }
            else
            {
                lpId = GetMainLpPageId(domain.LandingSiteId);
            }

            rewriteUrl = "~/landing/landing?landingId=" + lpId + (!string.IsNullOrEmpty(uri.Query) ? "&" + uri.Query.TrimStart('?') : "");

            return true;
        }

        public static bool IsLandingDomain(Uri uri, out int lpSiteId)
        {
            lpSiteId = 0;
            
            var domains = GetLandingDomains();
            if (domains.Count == 0)
                return false;

            var host = uri.Host; //.Replace("www.", "");

            var domain = domains.FirstOrDefault(x => host == x.DomainUrl || host == StringHelper.ToPuny(x.DomainUrl));
            if (domain == null)
                return false;

            lpSiteId = domain.LandingSiteId;

            return true;
        }

        private static List<LpDomain> GetLandingDomains()
        {
            return CacheManager.Get(CacheKey, 6*60, () => SQLDataAccess.Query<LpDomain>("Select * From [CMS].[LandingDomain]").ToList());
        }

        private static int GetLpPageId(int siteId, string pageUrl)
        {
            return CacheManager.Get(PageCacheKey + siteId + "_" + pageUrl, 6*60,
                () =>
                    SQLDataAccess.Query<int>(
                        "Select top(1) Id From [CMS].[Landing] Where LandingSiteId=@siteId and Url=@pageUrl",
                        new {siteId, pageUrl})
                        .FirstOrDefault());
        }

        private static int GetMainLpPageId(int siteId)
        {
            return CacheManager.Get(PageCacheKey + siteId + "ismain", 6 * 60,
                () =>
                    SQLDataAccess.Query<int>(
                        "Select top(1) Id From [CMS].[Landing] Where LandingSiteId=@siteId and IsMain=1",
                        new { siteId })
                        .FirstOrDefault());
        }

        public static string LandingRedirectUrl
        {
            get { return HttpContext.Current != null ? HttpContext.Current.Items["LandingRedirectUrl"] as string : null; }
            set { HttpContext.Current.Items["LandingRedirectUrl"] = value; }
        }
    }
}
