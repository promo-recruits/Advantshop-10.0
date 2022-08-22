using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.DownloadableContent;
using AdvantShop.Core.Services.Landing;
using AdvantShop.Core.Services.SalesChannels;
using AdvantShop.Core.Services.Screenshot;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Web.Admin.ViewModels.Home;

namespace AdvantShop.Web.Admin.Handlers.Home
{
    public class GetDashboard
    {
        public DashboardViewModel Execute()
        {
            var model = new DashboardViewModel()
            {
                ActionText = ShopActionsService.GetLast()
            };

            var mainSiteUrl = SettingsMain.SiteUrl.ToLower();

            var channel = SalesChannelService.GetByType(ESalesChannelType.Store);
            if (channel != null && channel.Enabled)
            {
                model.Sites.Add(new DashboardSiteItem()
                {
                    Id = -1,
                    Name = SettingsMain.ShopName,
                    Type = DashboardSiteItemType.Store,
                    Domain = UrlService.GetUrl().Trim('/'), //SettingsMain.SiteUrl, // как определить урл магазина?
                    PreviewIframeUrl = UrlService.GetUrl(),

                    EditUrl = "design",
                    ViewUrl = UrlService.GetUrl(),
                    ScreenShot = SettingsMain.StoreScreenShot,

                    Published = true,
                    IsMainSite = UrlService.GetUrl().Trim('/') == mainSiteUrl,

                    ChangeDomainUrl = "service/domainsmanage"
                });
            }

            channel = SalesChannelService.GetByType(ESalesChannelType.Funnel);
            if (channel != null && channel.Enabled)
            {
                var funnels = new LpSiteService().GetList();
                var screenShotService = new ScreenshotService();

                foreach (var funnel in funnels)
                {
                    var url = !string.IsNullOrEmpty(funnel.DomainUrl)
                        ? "http://" + funnel.DomainUrl
                        : LpService.GetTechUrl(funnel.Url, "", true);

                    model.Sites.Add(new DashboardSiteItem()
                    {
                        Id = funnel.Id,
                        Name = funnel.Name,
                        Type = DashboardSiteItemType.Funnel,
                        Domain = !string.IsNullOrEmpty(funnel.DomainUrl) ? "http://" + funnel.DomainUrl : null,
                        PreviewIframeUrl = url + "?previewInAdmin=true",

                        EditUrl = "funnels/site/" + funnel.Id,
                        ViewUrl = url,
                        ScreenShot = funnel.ScreenShot,

                        Published = funnel.Enabled,
                        IsMainSite = url == mainSiteUrl,

                        ChangeDomainUrl = "funnels/site/" + funnel.Id + "#?landingAdminTab=settings&landingSettingsTab=domains"
                    });
                }

                // update funnel screenshot
                Task.Run(() =>
                {
                    foreach (var funnel in funnels)
                    {
                        if (funnel.ScreenShotDate == null || funnel.ModifiedDate == null ||
                            funnel.ScreenShotDate < funnel.ModifiedDate)
                        {
                            screenShotService.UpdateFunnelScreenShotInBackground(funnel);
                        }
                        Thread.Sleep(300);
                    }
                });
            }

            model.Domains =
                model.Sites.Where(x => !string.IsNullOrEmpty(x.Domain))
                    .Select(
                        x =>
                            new DashboardSiteDomain()
                            {
                                Name = x.Name,
                                Url = x.Domain.ToLower(),
                                Type = x.Type,
                                TypeStr = x.TypeStr,
                                IsMainSite = x.IsMainSite
                            })
                    .ToList();

            model.SelectedDomain = model.Domains.FirstOrDefault(x => x.IsMainSite);

            return model;
        }
    }
}
