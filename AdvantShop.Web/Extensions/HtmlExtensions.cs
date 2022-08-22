using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.Localization;
using AdvantShop.Customers;
using AdvantShop.Repository;
using AdvantShop.ViewModel.StaticBlock;
using AdvantShop.Web.Infrastructure.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.Diagnostics;
using AdvantShop.Core;

namespace AdvantShop.Extensions
{
    public static class HtmlExtensions
    {
        public static MvcHtmlString StaticBlock(this HtmlHelper helper, string key, string cssClass = null, bool onlyContent = false)
        {
            var sb = StaticBlockService.GetPagePartByKeyWithCache(key);

            if (sb == null || !sb.Enabled || DebugMode.IsDebugMode(eDebugMode.StaticBlocks))
                return MvcHtmlString.Create("");

            if (onlyContent)
                return MvcHtmlString.Create(sb.Content);

            var content = sb.Content;
            var canUseInplace = InplaceEditorService.CanUseInplace(RoleAction.Store);

            if (content.IsNotEmpty() && canUseInplace)
            {
                content = InplaceEditorService.PrepareContent(content);
            }

            var sbModel = new StaticBlockViewModel()
            {
                CssClass = cssClass,
                InplaceAttributes = canUseInplace ? helper.InplaceStaticBlock(sb.StaticBlockId) : new HtmlString(""),
                OnlyContent = onlyContent,
                Content = content,
                Key = sb.Key,
                CanUseInplace = canUseInplace
            };

            return helper.Partial("_StaticBlock", sbModel);
        }

        public static MvcHtmlString Captcha(this HtmlHelper helper, string ngModel, string ngModelSource = null,
                                                                    string captchaId = null, CaptchaMode? captchaMode = null, 
                                                                    int? codeLength = null)
        {
            return helper.Action("Captcha", "Common", new {ngModel, ngModelSource, captchaId, captchaMode, codeLength});
        }

        public static MvcHtmlString Rating(this HtmlHelper helper, double rating, int objId = 0, string url = null,
            bool readOnly = true, string binding = null)
        {
            return helper.Action("Rating", "Common",
                new RouteValueDictionary()
                {
                    {"objId", objId},
                    {"rating", rating},
                    {"url", url},
                    {"readOnly", readOnly},
                    {"binding", binding},
                    { "area", ""}
                });
        }

        public static HtmlString RenderCustomOptions(this HtmlHelper helper, List<EvaluatedCustomOptions> evlist)
        {
            if (evlist == null || evlist.Count == 0)
                return new MvcHtmlString("");

            var html = new StringBuilder("<ul class=\"cart-full-properties\">");
            foreach (var ev in evlist)
            {
                html.AppendFormat(
                    "<li class=\"cart-full-properties-item\"><div class=\"cart-full-properties-name cs-light\">{0}{2}</div> <div class=\"cart-full-properties-value\">{1}</div></li>",
                    ev.CustomOptionTitle, ev.OptionTitle, !string.IsNullOrEmpty(ev.OptionTitle) ? ":" : "");
            }
            html.Append("</ul>");

            return new HtmlString(html.ToString());
        }

        public static HtmlString GetCityPhone(this HtmlHelper helper, bool encode = false, bool isPhoneLink = false)
        {
            return new HtmlString(encode ? helper.AttributeEncode(CityService.GetPhone(isPhoneLink)) : CityService.GetPhone(isPhoneLink));
        }
        
        public static MvcHtmlString SingleBreadCrumb(this HtmlHelper helper, string name)
        {
            var breadCrumbs = new List<BreadCrumbs>();
            var url = new UrlHelper(helper.ViewContext.RequestContext);
            breadCrumbs.Add(new BreadCrumbs(LocalizationService.GetResource("MainPage"), url.AbsoluteRouteUrl("Home")));
            breadCrumbs.Add(new BreadCrumbs(name, string.Empty));
            return helper.Action("BreadCrumbs", "Common", new RouteValueDictionary() {{"breadCrumbs", breadCrumbs}});
        }

        public static HtmlString Numerals(this HtmlHelper helper, float count, string zeroText, string oneText, string twoText, string fiveText)
        {
            return new HtmlString(count + " " + Strings.Numerals(count, zeroText, oneText, twoText, fiveText));
        }

        public static HtmlString Numerals(this HtmlHelper helper, float count, IHtmlString zeroText, IHtmlString oneText, IHtmlString twoText, IHtmlString fiveText)
        {
            return new HtmlString(count + " " + Strings.Numerals(count, zeroText, oneText, twoText, fiveText));
        }


        public static HtmlString GetCustomerManager(this HtmlHelper helper)
        {
            return new HtmlString(CustomerService.GetCurrentCustomerManager());
        }


        public static string GetToolbarClass(this HtmlHelper helper)
        {
            return SettingsDesign.DisplayToolBarBottom || !AdvantShop.Helpers.MobileHelper.IsMobileBrowser() ? "toolbar-bottom-enabled" :"toolbar-bottom-disabled";
        }

        public static HtmlString Preload(this HtmlHelper helper, string fileName, string ext, string type)
        {
            var pathData = new AssetsTool.PathData("");

            var fName = pathData.GetFileName(fileName, ext);
            if (fName != null) {
                var path = pathData.GetPath(fName);
                return new HtmlString(string.Format("<link rel=\"preload\" as=\"{0}\" href=\"{1}\" crossorigin>", type, path));
            }
            return new HtmlString("");
        }


    }
}