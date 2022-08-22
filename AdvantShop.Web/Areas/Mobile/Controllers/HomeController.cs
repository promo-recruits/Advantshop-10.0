﻿using System.Web.Mvc;
using System.Web.SessionState;
using AdvantShop.Areas.Mobile.Handlers.Home;
using AdvantShop.Areas.Mobile.Models.Home;
using AdvantShop.Configuration;
using AdvantShop.Core.Services.CMS;
using AdvantShop.Core.Services.Configuration.Settings;
using AdvantShop.FilePath;
using AdvantShop.Repository;
using AdvantShop.Helpers;
using AdvantShop.Core.Services.Catalog;

namespace AdvantShop.Areas.Mobile.Controllers
{
    [SessionState(SessionStateBehavior.Disabled)]
    public class HomeController : BaseMobileController
    {
        // GET: Mobile/Home
        public ActionResult Index()
        {
            var model = new HomeMobileHandler().Get();

            SetMobileTitle(T("MainPage") );
            SetMetaInformation(null, string.Empty);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult Logo()
        {

            if (string.IsNullOrEmpty(SettingsMain.LogoImageName) && !SettingsMobile.IsMobileTemplateActive)
                return new EmptyResult();

            var width = 0;
            var height = 0;

            if (SettingsMobile.LogoType == SettingsMobile.eLogoType.Desktop.ToString())
            {
                width = SettingsMain.LogoImageWidth;
                height = SettingsMain.LogoImageHeight;
            }
            else if (SettingsMobile.LogoType == SettingsMobile.eLogoType.Mobile.ToString())
            {
                width = SettingsMobile.LogoImageWidth;
                height = SettingsMobile.LogoImageHeight;
            }

            var model = new LogoMobileModel
            {
                LogoAlt = SettingsMain.LogoImageAlt,
                Text = SettingsMobile.DisplayHeaderTitle ? SettingsMain.ShopName : SettingsMobile.HeaderCustomTitle,
                ImgSource = FoldersHelper.GetPath(FolderType.Pictures,
                    SettingsMobile.LogoType == SettingsMobile.eLogoType.Desktop.ToString()
                        ? SettingsMain.LogoImageName
                        : SettingsMobile.LogoImageName, false),
                Width = width,
                Height = height
            };

            SettingsMobile.eLogoType _logoType;
            bool isParsedLogoType = SettingsMobile.eLogoType.TryParse(SettingsMobile.LogoType, out _logoType);

            model.LogoType = isParsedLogoType ? _logoType : SettingsMobile.eLogoType.Text;

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult Carousel()
        {
            if (!SettingsMobile.DisplaySlider)
                return new EmptyResult();

            var sliders = CarouselService.GetAllCarouselsMainPage(ECarouselPageMode.Mobile);
            if (sliders.Count == 0)
                return new EmptyResult();

            return PartialView("Carousel", new CarouselMobileViewModel() {
                Sliders = sliders,
                Speed = SettingsDesign.CarouselAnimationSpeed,
                Pause = SettingsDesign.CarouselAnimationDelay
            });
        }

        [ChildActionOnly]
        public ActionResult MainPageProducts(ProductViewModel products)
        {
            var model = new MainPageProductsMobileViewModel()
            {
                Products = products,
                MainPageCatalogView = SettingsMobile.MainPageCatalogView
        };

            return PartialView("_MainPageProducts", model);
        }

        public ActionResult ToFullVersion()
        {
            // Todo: add cookie

            return RedirectToRoute("Home");
        }
    }
}