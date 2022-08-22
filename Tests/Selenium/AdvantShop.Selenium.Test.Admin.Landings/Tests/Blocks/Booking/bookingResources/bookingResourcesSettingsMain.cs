using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Booking.bookingResources
{
    [TestFixture]
    public class bookingResourcesSettingsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing |
                                        ClearType.Booking);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.Service.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.Affiliate.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.ReservationResource.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.ReservationResourceService.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.ReservationResourceTimeOfBooking.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.Category.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateCategory.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateReservationResource.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateService.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\Booking.AffiliateTimeOfBooking.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\Booking\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Booking\\bookingResources\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "bookingResources";
        private string blockType = "Booking";
        private readonly int numberBlock = 1;
        private readonly string blockTitle = "BookingResourcesTitle";
        private readonly string blockSubTitle = "BookingResourcesSubtitle";
        private readonly string blockIcon = "BookingResourcesPicture";
        private readonly string blockColLink = "BookingResourcesColLink";

        [Test]
        public void BackgroundImageUpdate()
        {
            TestName = "BackgroundImageUpdae";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            //from computer
            BlockSettingsBtn(numberBlock);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"img_picture\"]")).GetAttribute("src")
                    .Contains("nophoto_cover.png"), "initial photo in background");
            OpenPopupUpdateImage();
            UpdateLoadImageDesktop("background.jpg");
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("style")
                    .Contains("background-image: url(\"pictures/landing/"), "upload photo via PC");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 div")).GetAttribute("style").Contains("background-image:"),
                "1");

            ReInit();
            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[3].Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"img_picture\"]")).GetAttribute("src")
                    .Contains("nophoto_cover.png"), "no photo in background 2");
            BlockSettingsSave();

            //from URL
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("background-image: url"),
                "upload photo via URL");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 div")).GetAttribute("style").Contains("background-image:"),
                "2");

            ReInit();
            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[3].Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"img_picture\"]")).GetAttribute("src")
                    .Contains("nophoto_cover.png"), "no photo in background 3");
            BlockSettingsSave();

            //from gallery
            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            UpdateLoadImageGallery();
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("background-image:"),
                "upload photo via Gallery");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 div")).GetAttribute("style").Contains("background-image:"),
                "3");

            //lazyload
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1); //is on

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[3].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count ==
                          1); //is off

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeBackgroundBehaviour()
        {
            TestName = "ChangeBackgroundBehaviour";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("fixed"),
                "background without fixed");
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("parallax"),
                "background without parallax");
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("box-shadow:"),
                "background without shadow");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            UpdateLoadImageDesktop("light.jpg");
            BlockSettingsSave();

            BlockSettingsBtn(numberBlock);
            BehaviorBackgroundImage("Фиксированный фон");
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("fixed"),
                "background with fixed");

            BlockSettingsBtn(numberBlock);
            BehaviorBackgroundImage("Параллакс");
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("parallax"),
                "background with parallax");

            BlockSettingsBtn(numberBlock);
            BehaviorBackgroundImage("Не выбрано");
            BlockSettingsSave();
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("fixed"),
                "background without fixed again");
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("parallax"),
                "background without parallax again");

            BlockSettingsBtn(numberBlock);
            moveSlider(5);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("box-shadow:"),
                "background with shadow");

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeColorScheme()
        {
            TestName = "ChangeColorScheme";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"), "background color default");
            BlockSettingsBtn(numberBlock);
            VerifyIsTrue(Driver.FindElement(By.Id("modalSettingsBlock_" + numberBlock)).Displayed,
                "display settings block");

            SetColorScheme("Умеренная");
            Thread.Sleep(1000);
            BlockSettingsSave();
            VerifyAreEqual("rgba(248, 248, 248, 1)",
                Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"), "background color medium");

            BlockSettingsBtn(numberBlock);
            SetColorScheme("Темная");
            Thread.Sleep(1000);
            BlockSettingsSave();
            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"),
                "background color dark");

            BlockSettingsBtn(numberBlock);
            SetColorScheme("Пользовательская");
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector("[data-e2e=\"EditColorSchemeBtn\"]")).Click();
            Thread.Sleep(2000);
            Driver.FindElements(By.CssSelector(".color-viewer-inner"))[23].Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSettingsBlock_colorSchemeCustom .blocks-constructor-btn-confirm"))
                .Click();
            Thread.Sleep(2000);
            VerifyAreEqual("rgba(6, 98, 193, 1)", Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"),
                "background color custom");

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangePadding()
        {
            TestName = "ChangePadding";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            //change padding_top
            VerifyAreEqual("45px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top initial");
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(60);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("60px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top after");

            Refresh();
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(0);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("0px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top changed");

            //change padding_bottom
            VerifyAreEqual("45px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom initial");
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(15);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("15px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom after");

            Refresh();
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(0);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("0px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom changed");

            VerifyFinally(TestName);
        }

        [Test]
        public void HiddenDesktop()
        {
            TestName = "HiddenDesktop";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient("lp/test1");

            //Hide in desktop
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop initial");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            HiddenInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "not displayed block in desktop");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "not displayed block in desktop on page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before on page1");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile before");

            //Show in desktop
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop on page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after on page1");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile after");

            VerifyFinally(TestName);
        }

        [Test]
        public void HiddenMobile()
        {
            TestName = "HiddenMobile";
            VerifyBegin(TestName);

            ReInitClient();
            GoToClient("lp/test1");

            //Hide In Mobile
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop initial");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            HiddenInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop before");
            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "not displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before on page1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "not displayed block in mobile on page1");

            //Show In Mobile
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop after");
            GoToMobile("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-booking-three-columns")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after on page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile on page1");

            VerifyFinally(TestName);
        }

        [Test]
        public void PhotoRounding()
        {
            TestName = "PhotoRounding";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "photo form initial");

            //on
            BlockSettingsBtn(numberBlock);
            RoundPhotoOn();
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo on in admin");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo on after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo on in client");
            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo on in mobile");

            //off
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            RoundPhotoOff();
            BlockSettingsSave();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo off in admin");

            Refresh();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo off after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo off in client");
            GoToMobile("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockIcon + "\"]")).GetAttribute("class")
                    .Contains("round-image"), "round_photo off in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void ServiceLink()
        {
            TestName = "ServiceLink";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //hide link
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 3,
                "ServiceLink initial count");
            VerifyAreEqual("Показать услуги",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Text,
                "ServiceLink initial text");

            BlockSettingsBtn(numberBlock);
            ShowServiceOff();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 0,
                "no ServiceLink in admin");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 0,
                "no ServiceLink after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 0,
                "no ServiceLink in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 0,
                "no ServiceLink in mobile");

            //show link
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ShowServiceOn();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 3,
                "ServiceLink in admin");

            Refresh();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 3,
                "ServiceLink after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 3,
                "ServiceLink in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Count == 3,
                "ServiceLink in mobile");

            //change link
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ChangeServiceLink("ServiceLinkText");
            BlockSettingsSave();
            VerifyAreEqual("ServiceLinkText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Text,
                "ServiceLink text changed");

            Refresh();
            VerifyAreEqual("ServiceLinkText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Text,
                "ServiceLink text changed after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("ServiceLinkText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Text,
                "ServiceLink text changed in client");

            GoToMobile("lp/test1");
            VerifyAreEqual("ServiceLinkText",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + blockColLink + "\"]")).Text,
                "ServiceLink text changed in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void ShowOnPages()
        {
            TestName = "ShowOnPages";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //Show on all pages - on
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"desktop_hidden\"] input")).Selected,
                "off hidden in desktop");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed, "on main page before");
            GoToClient("lp/test1/page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed, "on page1 before");

            //Show on all pages - off
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnThisPage();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed, "on main page after");
            GoToClient("lp/test1/page1");
            VerifyIsFalse(Driver.PageSource.Contains("#block_1 .lp-container"), "on page1 after");

            VerifyFinally(TestName);
        }

        [Test]
        public void SubtitleShow()
        {
            TestName = "SubtitleShow";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock initial");

            //Hide subtitle
            BlockSettingsBtn(numberBlock);
            ShowTitle();
            HiddenSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock on");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock off");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock off in client");

            GoToMobile("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock off in mobile");

            ReInit();
            GoToClient("lp/test1");

            //Without Title
            BlockSettingsBtn(numberBlock);
            ShowSubTitle();
            HiddenTitle();
            BlockSettingsSave();

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock off");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock on 1");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock on in client 1");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock on in mobile 1");

            ReInit();
            GoToClient("lp/test1");

            //Show with title
            BlockSettingsBtn(numberBlock);
            ShowTitle();
            ShowSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock on");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock on 2");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock on in client 2");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Count == 1,
                "subtitle subblock on in mobile 2");

            VerifyFinally(TestName);
        }

        [Test]
        public void TitleShow()
        {
            TestName = "TitleShow";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock initial");

            //off
            BlockSettingsBtn(numberBlock);
            HiddenTitle();
            BlockSettingsSave();

            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock off");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock off in client");

            GoToMobile("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock off in mobile");

            ReInit();
            GoToClient("lp/test1");

            //on
            BlockSettingsBtn(numberBlock);
            ShowTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock on");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock on in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Count == 1,
                "title subblock on in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        [Order(10)]
        public void ConvertHTML()
        {
            TestName = "zConvertHTML";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block before all");
            BlockSettingsBtn(numberBlock);
            ConvertToHtmlCancel();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block before convert");

            BlockSettingsBtn(numberBlock);
            ConvertToHtmlSave();
            VerifyIsFalse(Driver.PageSource.Contains("#block_" + numberBlock), "block after convert 1");
            VerifyIsTrue(Driver.FindElements(By.Id("block_1")).Count == 0, "block after convert 2");

            VerifyFinally(TestName);
        }
    }
}