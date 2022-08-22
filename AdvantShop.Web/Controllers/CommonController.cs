using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Catalog;
using AdvantShop.CMS;
using AdvantShop.Configuration;
using AdvantShop.Core.Common.Captcha;
using AdvantShop.Core.Common.Extensions;
using AdvantShop.Core.Services.InplaceEditor;
using AdvantShop.Core.Services.IPTelephony.CallBack;
using AdvantShop.Core.Services.SEO.MetaData;
using AdvantShop.Core.UrlRewriter;
using AdvantShop.Customers;
using AdvantShop.FilePath;
using AdvantShop.Handlers.Common;
using AdvantShop.Handlers.Menu;
using AdvantShop.Models.Common;
using AdvantShop.Orders;
using AdvantShop.SEO;
using AdvantShop.Trial;
using AdvantShop.ViewModel.Common;
using AdvantShop.Helpers;
using AdvantShop.Repository.Currencies;
using AdvantShop.Repository;
using AdvantShop.Core.Services.IPTelephony;
using AdvantShop.ViewModel.Shared;
using Debug = AdvantShop.Diagnostics.Debug;
using AdvantShop.Core.Caching;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.Web.Infrastructure.Filters;
using AdvantShop.Statistic;
using AdvantShop.Web.Infrastructure.ActionResults;
using Newtonsoft.Json;

namespace AdvantShop.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public partial class CommonController : BaseClientController
    {
        public ActionResult ClosedStore()
        {
            if (!SettingsMain.IsStoreClosed)
                return RedirectToRoute("Home");

            SettingsDesign.IsMobileTemplate = false;

            SetMetaInformation(null, string.Empty);
            return View();
        }

        public JsonResult GetDesign()
        {
            return Json(new GetDesignHandler().Get());
        }

        public JsonResult GetDesignBuilder()
        {
            var model = new GetDesignNewBuilderHandler().Execute();
            return Json(model);
        }


        public ActionResult AdditionalPhones()
        {
            var phones = new GetAdditionalPhones().Execute().Phones;

            if (string.IsNullOrEmpty(SettingsMain.Phone))
                return new EmptyResult();

            return PartialView(phones);
        }

        [ChildActionOnly]
        public ActionResult NewBuilderButton()
        {
            var customer = CustomerContext.CurrentCustomer;

            var isShowOnLoad = false;
            
            var cookieValue = CommonHelper.GetCookieString("trialBuilderShowed").TryParseBool(true);

            if ((cookieValue == null || cookieValue == false) && CustomerContext.CurrentCustomer.IsAdmin && TrialService.IsTrialEnabled)
            {
                CommonHelper.SetCookie("trialBuilderShowed", "true", true);
                isShowOnLoad = true;
            }

            if (customer.IsAdmin || Demo.IsDemoEnabled)
                return PartialView("~/Views/Common/DesignBuilderButton.cshtml", isShowOnLoad);

            return new EmptyResult();
        }

        public void SetCurrency(string currencyIso)
        {
            CurrencyService.CurrentCurrency = CurrencyService.Currency(currencyIso);
        }

        public JsonResult SaveDesign(string theme, string colorscheme, string structure, string background)
        {
            if (CustomerContext.CurrentCustomer.CustomerRole != Role.Administrator && !Demo.IsDemoEnabled &&
                !TrialService.IsTrialEnabled || theme.IsNullOrEmpty() || colorscheme.IsNullOrEmpty() || background.IsNullOrEmpty())
            {
                return Json("error", "text");
            }
            
            new SaveDesignHandler().Save(theme, colorscheme, structure, background);

            if (!SettingsCongratulationsDashboard.DesignDone)
            {
                SettingsCongratulationsDashboard.DesignDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DesignDone);
            }

            return Json("success", "text");
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult SaveDesignNewBuilder(DesignNewBuilderModel data)
        {
            if (CustomerContext.CurrentCustomer.CustomerRole != Role.Administrator && !Demo.IsDemoEnabled && !TrialService.IsTrialEnabled)
            {
                return JsonError();
            }

            try
            {
                new SaveDesignNewBuilderHandler(data).Execute();
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
                return JsonError();
            }

            if (!SettingsCongratulationsDashboard.DesignDone)
            {
                SettingsCongratulationsDashboard.DesignDone = true;
                Track.TrackService.TrackEvent(Track.ETrackEvent.Dashboard_DesignDone);
            }

            return JsonOk();
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult DebugJs(string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
                Debug.Log.Error(HttpUtility.HtmlEncode(message));

            return new EmptyResult();
        }

        [ChildActionOnly]
        public ActionResult Logo(LogoModel logo)
        {
            if (!logo.Visible)
                return new EmptyResult();

            var isInplace = InplaceEditorService.CanUseInplace(RoleAction.Settings);

            if (logo.ImgSource.IsNullOrEmpty() && InplaceEditorService.CanUseInplace(RoleAction.Settings))
            {
                logo.ImgSource = UrlService.GetUrl("images/nophoto-logo.png");
            }
            
            var alt = !string.IsNullOrEmpty(logo.ImgAlt) ? string.Format(" alt=\"{0}\"", logo.ImgAlt) : string.Empty;
            var cssClass = !string.IsNullOrEmpty(logo.CssClass) ? " " + logo.CssClass : string.Empty;

            logo.LogoGeneratorEditOnPageLoad = Request["logoGeneratorEditOnPageLoad"].TryParseBool();

            logo.LogoGeneratorEnabled = !SettingsDesign.IsMobileTemplate &&
                                        CustomerContext.CurrentCustomer.CustomerRole == Role.Administrator &&
                                        (isInplace || logo.LogoGeneratorEditOnPageLoad);

            if (!string.IsNullOrEmpty(logo.ImgSource))
            {
                var imgWidth = SettingsMain.LogoImageWidth > 0 ? SettingsMain.LogoImageWidth : SettingsDesign.LogoImageWidth;
                var imgHeight = SettingsMain.LogoImageHeight > 0 ? SettingsMain.LogoImageHeight : SettingsDesign.LogoImageHeight;


                logo.Html = string.Format(
                    "<img id=\"logo\" src=\"{0}\"{1} {3} class=\"site-head-logo-picture{2}\" {4} {5}/>",
                    logo.ImgSource, alt, cssClass,
                    Extensions.InplaceExtensions.InplaceImageLogo(),
                    logo.LogoGeneratorEnabled ? "data-logo-generator-preview-img" : "",
                    !logo.LogoGeneratorEnabled && !InplaceEditorService.CanUseInplace(RoleAction.Settings) && imgWidth > 0 && imgHeight > 0 ? String.Format("width=\"{0}\" height=\"{1}\"", imgWidth, imgHeight) : "");
            }

            logo.DisplayHref = Request.Path != "/";

            return PartialView("Logo", logo);
        }


        [ChildActionOnly]
        public ActionResult Favicon(FaviconModel faviconModel, string imgSource)
        {
            var path = imgSource.IsNotEmpty() ? imgSource  : SettingsMain.FaviconImageName.IsNotEmpty() ? SettingsMain.FaviconImageName : "favicon.ico";

            faviconModel.ImgSource = FoldersHelper.GetPathRelative(FolderType.Pictures, path, faviconModel.ForAdmin);

            if (!string.IsNullOrEmpty(faviconModel.ImgSource))
            {
                const string imgTag = "<img id=\"favicon\" src=\"{0}\" {1} />";
                const string linkTag = "<link rel=\"{0}\" type=\"{1}\" href=\"{2}\"{3} />";

                //Source
                string source = UrlService.GetUrl(faviconModel.ImgSource);

                // styleClass
                string styleClass = !string.IsNullOrEmpty(faviconModel.CssClassImage) ? string.Format("class=\"{0}\"", faviconModel.CssClassImage) : string.Empty;

                //Source
                string rel = Request.Browser.Browser == "IE" ? "SHORTCUT ICON" : "shortcut icon";

                //content type for yandex
                string fileExtension = FileHelpers.GetExtension(faviconModel.ImgSource);
                var contentType = string.Empty;
                if (fileExtension == ".ico")
                    contentType = "image/x-icon";
                else if (fileExtension == ".png")
                    contentType = "image/png";
                else if (fileExtension == ".gif")
                    contentType = "image/gif";

                faviconModel.Html = faviconModel.GetOnlyImage ? string.Format(imgTag, source, styleClass) : string.Format(linkTag, rel, contentType, source, string.Empty);
            }

            return PartialView("Favicon", faviconModel);
        }


        [ChildActionOnly]
        public ActionResult MenuTop()
        {

            var currentMode = !Demo.IsDemoEnabled || !CommonHelper.GetCookieString("structure").IsNotEmpty()
               ? SettingsDesign.MainPageMode
               : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString("structure"));

            if (currentMode != SettingsDesign.eMainPageMode.Default)
                return new EmptyResult();

            var model = new MenuTopHanlder().GetTopMenuItems();
            return PartialView("MenuTop", model);
        }

        [ChildActionOnly]
        public ActionResult MenuBottom(EMenuBottomType type = EMenuBottomType.Default, bool isShowSocial = true)
        {
            var model = new MenuBottomHanlder().Get();
            model.MenuBottomType = type;
            model.IsShowSocial = isShowSocial;

            return PartialView("MenuBottom", model);
        }

        public ActionResult Copyright()
        {
            return PartialView("Copyright",
                new CopyrightModel
                {
                    Text = SettingsDesign.CopyrightTextFormatted,
                    Visible = SettingsDesign.ShowCopyright
                });
        }


        [ChildActionOnly]
        public ActionResult ToolBarBottom(ToolBarBottomViewModel toolbarModel)
        {
            if (!SettingsDesign.DisplayToolBarBottom || MobileHelper.IsMobileBrowser())
                return new EmptyResult();

            var controller = Request.RequestContext.RouteData.Values["controller"] as string;

            toolbarModel.isCart = controller == "Cart";
            toolbarModel.ShowConfirmButton = controller != "Checkout";
            
            return PartialView(toolbarModel);
        }


        [ChildActionOnly]
        public ActionResult Telephony()
        {
            if (SettingsSocialWidget.IsActive)
                return new EmptyResult();

            var callBack = IPTelephonyOperator.Current.CallBack;
            if (callBack == null || !callBack.Enabled || (Saas.SaasDataService.IsSaasEnabled && !Saas.SaasDataService.CurrentSaasData.HaveTelephony))
                return new EmptyResult();

            var cookieValue = CommonHelper.GetCookieString("telephonyUserMode");

            var model = new TelephonyViewModel
            {
                TimeInterval = SettingsTelephony.CallBackTimeInterval,
                IsWorkTime = callBack.IsWorkTime(),
                ShowMode =
                    cookieValue.IsNotEmpty()
                        ? cookieValue.TryParseEnum<ECallBackShowMode>()
                        : SettingsTelephony.CallBackShowMode
            };


            return PartialView(model);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CallBack(string phone, bool check = false)
        {
            var callBack = IPTelephonyOperator.Current.CallBack;
            if (callBack == null || !callBack.Enabled || phone.IsNullOrEmpty())
                return new JsonResult();

            var result = callBack.MakeRequest(phone, check);
            return Json(result);
        }

        [ChildActionOnly]
        public ActionResult CookiesPolicy()
        {
            var cookieName = string.Format("{0}_CookiesPopicyAccepted", SettingsMain.SiteUrlPlain);

            if (!SettingsNotifications.ShowCookiesPolicyMessage || CommonHelper.GetCookieString(HttpUtility.UrlEncode(cookieName)) == "true")
                return new EmptyResult();

            return PartialView((object)cookieName);
        }

        [ChildActionOnly]
        public ActionResult MetaData()
        {
            if (!SettingsSEO.OpenGraphEnabled)
                return new EmptyResult();

            var ogModelContext = MetaDataContext.CurrentObject;

            if (ogModelContext == null)
                return PartialView(new OpenGraphModel());

            var ogModel = new OpenGraphModel()
            {
                SiteName = ogModelContext.SiteName,
                Url = ogModelContext.Url,
                Type = ogModelContext.Type,
                Images = ogModelContext.Images
            };

            return PartialView(ogModel);
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public JsonResult CheckOrder(string orderNumber)
        {
            if (!string.IsNullOrEmpty(orderNumber))
                return Json(new {StatusName = T("Checkout.CheckOrder.StatusCommentNotFound")});
            
            var statusInf = OrderService.GetStatusInfo(orderNumber);
            return
                Json(statusInf != null
                    ? new
                    {
                        StatusName = statusInf.Hidden ? statusInf.PreviousStatus : statusInf.Status,
                        StatusComment = statusInf.Comment
                    }
                    : new
                    {
                        StatusName = T("Checkout.CheckOrder.StatusCommentNotFound"),
                        StatusComment = ""
                    });
        }

        [ChildActionOnly]
        public ActionResult GoogleAnalytics()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();

            return
                Content(
                    new GoogleAnalyticsString(SettingsSEO.GoogleAnalyticsNumber, SettingsSEO.GoogleAnalyticsEnabled)
                        .GetGoogleAnalyticsString(SettingsSEO.GoogleAnalyticsEnableDemogrReports));
        }


        [ChildActionOnly]
        public ActionResult TopPanel()
        {
            return PartialView(new TopPanelHandler().Get());
        }

        [ChildActionOnly]
        public ActionResult Preview()
        {
            return PartialView(new TopPanelHandler().Get());
        }

        [ChildActionOnly]
        public ActionResult BreadCrumbs(List<BreadCrumbs> breadCrumbs)
        {
            if (breadCrumbs == null || breadCrumbs.Count == 0)
                return new EmptyResult();

            return PartialView(breadCrumbs);
        }

        [ChildActionOnly]
        public ActionResult Rating(int objId, double rating, string url, bool readOnly = true, string binding = null)
        {
            return PartialView("_Rating", new RatingViewModel(rating)
            {
                ObjId = objId,
                Url = url,
                ReadOnly = readOnly,
                Binding = binding
            });
        }

        [ChildActionOnly]
        public ActionResult ZonePopover()
        {
            if (Request.IsLighthouse())
                return new EmptyResult();
            
            var cookieValue = CommonHelper.GetCookieString("zonePopoverVisible").ToLower();
            var settingValue = SettingsDesign.DisplayCityBubble;
            var settingValueString = settingValue.ToString().ToLower();
            var displayPopup = false;
            var expiresDate = new TimeSpan(364, 0, 0, 0, 0);

            if (string.IsNullOrEmpty(cookieValue) || cookieValue != settingValueString)
            {
                // Внутри меняется глобальный Last-Modified, но т.к. ZonePopover грузится в ту же секунду, что и страница, браузер считает страницу не измененной ...
                CommonHelper.SetCookie("zonePopoverVisible", settingValueString, expiresDate, false);
                displayPopup = settingValue;
            }

            if (!displayPopup)
                return new EmptyResult();

            return PartialView(new ZonePopoverViewModel() { City = IpZoneContext.CurrentZone.City });
        }

        [HttpPost, ValidateJsonAntiForgeryToken]
        public ActionResult ResetLastModified()
        {
            // ... сбрасываем глобальный Last-Modified после отображения всплывашки
            Core.Common.DataModificationFlag.ResetLastModified();
            return Content(null);
        }

        [ChildActionOnly]
        public ActionResult Captcha(string ngModel, string ngModelSource = null, string captchaId = null, CaptchaMode? captchaMode = null, int? codeLength = null)
        {
            var model = new CaptchaViewModel()
            {
                CaptchaId = captchaId,
                NgModel = ngModel,
                NgModelSource = ngModelSource,
                CaptchaMode = captchaMode != null ? captchaMode.Value : SettingsMain.CaptchaMode,
                CodeLength = codeLength != null ? codeLength.Value : SettingsMain.CaptchaLength
            };

            return PartialView(model);
        }
        
        [ChildActionOnly]
        public ActionResult Captcha_old(string ngModel, string NgModelSource = null)
        {
            var captcha = CaptchaService_old.GetNewCaptcha();
            if (captcha == null)
                return new EmptyResult();

            var model = new CaptchaViewModel()
            {
                CaptchaBase64Text = captcha.Base64Text,
                CaptchaEncodedBase64Text = captcha.EncodedBase64Text,
                CaptchaCode = captcha.Code,
                CaptchaSource = captcha.Source,
                NgModel = ngModel,
                NgModelSource = NgModelSource
            };

            return PartialView(model);
        }


        [HttpGet]
        public ActionResult GetCaptchaText(string captchatext)
        {
            if (string.IsNullOrWhiteSpace(captchatext))
                return new EmptyResult();

            var stream = CaptchaService_old.GetImage(captchatext);

            return new FileStreamResult(stream, "image/jpeg");
        }

        [ChildActionOnly]
        public ActionResult MenuGeneral()
        {
            var currentMode = !Demo.IsDemoEnabled || !CommonHelper.GetCookieString("structure").IsNotEmpty()
                                ? SettingsDesign.MainPageMode
                                : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString("structure"));
            var menuHandler = new MenuHandler();

            var model = new MenuViewModel()
            {
                MenuItems = currentMode != SettingsDesign.eMainPageMode.Default
                    ? menuHandler.GetMenuItems()
                    : menuHandler.GetCatalogMenuItems(0).SubItems,
                ViewMode = SettingsDesign.eMenuStyle.Classic
            };

            return PartialView("MenuGeneral", model);

        }

        [ChildActionOnly]
        public ActionResult MenuBlock()
        {
            var model = new MenuBlockViewModel();

            var currentMode = !Demo.IsDemoEnabled || !CommonHelper.GetCookieString("structure").IsNotEmpty()
                ? SettingsDesign.MainPageMode
                : (SettingsDesign.eMainPageMode)Enum.Parse(typeof(SettingsDesign.eMainPageMode), CommonHelper.GetCookieString("structure"));

            switch (currentMode)
            {
                case SettingsDesign.eMainPageMode.Default:
                    model.Layout = "_ColumnsOne";
                    break;
                case SettingsDesign.eMainPageMode.TwoColumns:
                    model.Layout = "_ColumnsTwo";
                    break;
                case SettingsDesign.eMainPageMode.ThreeColumns:
                    model.Layout = "_ColumnsThree";
                    break;
            }

            return PartialView("_Menu", model);
        }

        [ChildActionOnly]
        public ActionResult SocialButtons()
        {
            if (!SettingsSocial.SocialShareEnabled)
                return new EmptyResult();

            var model = new SocialButtonsViewModel()
            {
                Mode = SettingsSocial.SocialShareCustomEnabled
                    ? "custom"
                    : "default_" + SettingsMain.Language
            };

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult DiscountByTime()
        {
            if (DiscountByTimeService.ShowPopup &&
                CommonHelper.GetCookieString("discountbytime").IsNullOrEmpty() &&
                DiscountByTimeService.GetDiscountByTime() != 0)
            {
                CommonHelper.SetCookie("discountbytime", "true", new TimeSpan(12, 0, 0), true);

                return PartialView("DiscountByTime", DiscountByTimeService.PopupText);
            }
            return new EmptyResult();
        }

        public ActionResult CancelTemplatePreview()
        {
            if (CustomerContext.CurrentCustomer.IsAdmin)
            {
                SettingsDesign.PreviewTemplate = null;
                CacheManager.Clean();
            }

            var url = Request.GetUrlReferrer() != null ? Request.GetUrlReferrer().ToString() : SettingsMain.SiteUrl;

            return Redirect(url);
        }

        public ActionResult ApplyTemplate()
        {
            var previewTemplate = SettingsDesign.PreviewTemplate;

            if (CustomerContext.CurrentCustomer.IsAdmin && previewTemplate != null)
            {
                SettingsDesign.ChangeTemplate(previewTemplate);
                CacheManager.Clean();
            }

            var url = Request.GetUrlReferrer() != null ? Request.GetUrlReferrer().ToString() : SettingsMain.SiteUrl;

            return Redirect(url);
        }

        public ActionResult LiveCounter()
        {
            return Content("");
        }

        [ChildActionOnly]
        public ActionResult Inplace(bool enabled, string inplaceMinAsset = "inplaceMin", string inplaceMaxAsset = "inplaceMax")
        {

            var model = new InplaceViewModel()
            {
                Enabled = enabled,
                InplaceMinAsset = inplaceMinAsset,
                InplaceMaxAsset = inplaceMaxAsset
            };

            return PartialView(model);
        }

        [HttpPost]
        public JsonResult ResizePictures()
        {
            if (CommonStatistic.IsRun) return Json(new CommandResult() { Result = false, Error = T("Admin.Designs.NotPossibleToCompressPhotos") });

            try
            {
                CommonStatistic.StartNew(() =>
                {
                    CommonStatistic.TotalRow = PhotoService.GetCountPhotos(0, PhotoType.Product);
                    Helpers.FileHelpers.ResizeAllProductPhotos();
                },
                    "settingstemplate#?settingsTab=catalog",
                    "Пережатие фотографии товаров");
            }
            catch (Exception ex)
            {
                Debug.Log.Error(ex);
            }

            Track.TrackService.TrackEvent(Track.ETrackEvent.Shop_Design_ResizePictures);

            return Json(new CommandResult() { Result = true });
        }

        public ActionResult Counter(bool force = false)
        {
            if ((TrialService.IsTrialEnabled && CustomerContext.CurrentCustomer.IsAdmin) || force)
                return Content(TrialService.TrialCounter);
            else
                return new EmptyResult();
        }

        [ChildActionOnly]
        public ActionResult TrialBuilder()
        {
            return new EmptyResult();
        }

    }

    // Common controller with session state
    public partial class CommonExtController : BaseClientController
    {
        [HttpPost]
        public ActionResult GetCaptchaHtml(string ngModel, string ngModelSource = null, string captchaId = null)
        {
            var model = new CaptchaViewModel()
            {
                NgModel = ngModel,
                NgModelSource = ngModelSource,
                CaptchaId = captchaId
            };

            return PartialView("~/Views/Common/Captcha.cshtml", model);
        }
    }
}