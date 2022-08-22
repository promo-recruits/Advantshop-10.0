using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderMenu
{
    [TestFixture]
    public class headerMenuSettingsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Headers\\headerMenu\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "headerMenu";
        private string blockType = "Headers";
        private readonly int numberBlock = 1;

        [Test]
        public void BackgroundImageUpdate()
        {
            TestName = "BackgroundImageUpdate";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //from computer
            BlockSettingsBtn(numberBlock);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"img_picture\"]")).GetAttribute("src")
                    .Contains("nophoto_cover.png"), "no photo in background 1");
            OpenPopupUpdateImage();
            UpdateLoadImageDesktop("images2.jpg");
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
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
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
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
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
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1,
                "lazy-load initial");

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1,
                "lazy-load off");

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
            UpdateLoadImageDesktop("dark.jpg");
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
            VerifyAreEqual("0px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top before");
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(50);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("50px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top after");

            Refresh();
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(0);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("0px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top initial");

            //change padding_bottom
            VerifyAreEqual("0px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom before");
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(35);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("35px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom after");

            Refresh();
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(0);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("0px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom initial");

            VerifyFinally(TestName);
        }

        [Test]
        public void FixedScroll()
        {
            TestName = "FixedScroll";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            AddBlockByBtnBig("Characteristics", "characteristics");
            Thread.Sleep(2000);
            AddBlockByBtnBig("Characteristics", "characteristics");
            Thread.Sleep(2000);

            Driver.ScrollTo(By.Id("block_1"));
            Thread.Sleep(2000);
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-block-id=\"1\"]")).GetAttribute("class")
                    .Contains("data-transformer"), "bothScroll are off 1");

            //Desktop
            BlockSettingsBtn(numberBlock);
            FixedOnScroll();
            BlockSettingsSave();
            VerifyAreEqual("'(min-width: 64em)'",
                Driver.FindElement(By.CssSelector("[data-string-id=\"block_1\"]"))
                    .GetAttribute("data-responsive-options"), "fixedDesktopScroll is on");

            ReInitClient();
            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-default"),
                "scroll default in desktop 1");

            Driver.ScrollTo(By.Id("block_3"));
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-over"),
                "scroll over in desktop 1");

            //Mobile
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            NoFixedOnScroll();
            FixedOnScrollMobile();
            BlockSettingsSave();
            VerifyAreEqual("'(max-width: 63em)'",
                Driver.FindElement(By.CssSelector("[data-string-id=\"block_1\"]"))
                    .GetAttribute("data-responsive-options"), "fixedMobileScroll is on");

            ReInitClient();
            GoToMobile("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-default"),
                "scroll default in mobile 1");

            Driver.ScrollTo(By.Id("block_3"));
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-over"),
                "scroll over in mobile 1");

            //Both
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            FixedOnScroll();
            FixedOnScrollMobile();
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-string-id=\"block_1\"][data-transformer]")).Count == 1,
                "bothScroll are on 1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-string-id=\"block_1\"][data-responsive-option]")).Count == 0,
                "bothScroll are on 2");

            ReInitClient();
            GoToClient("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-default"),
                "scroll default in desktop 2");
            Driver.ScrollTo(By.Id("block_3"));
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-over"),
                "scroll over in desktop 2");

            GoToMobile("lp/test1");
            Driver.ScrollTo(By.Id("block_1"));
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-default"),
                "scroll default in mobile 2");
            Driver.ScrollTo(By.Id("block_3"));
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("transformer-scroll-over"),
                "scroll over in mobile 2");

            //No one
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            NoFixedOnScrollMobile();
            NoFixedOnScroll();
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-block-id=\"1\"]")).GetAttribute("class")
                    .Contains("data-transformer"), "bothScroll are off 2");

            VerifyFinally(TestName);
        }

        [Test]
        public void HiddenDesktop()
        {
            TestName = "HiddenDesktop";
            VerifyBegin(TestName);

            //Hide in desktop
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop initial");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            HiddenInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "not displayed block in desktop");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before");

            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "not displayed block in desktop on page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before on page1");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile before");

            //Show in desktop
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop on page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after on page1");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile after");

            VerifyFinally(TestName);
        }

        [Test]
        public void HiddenMobile()
        {
            TestName = "HiddenMobile";
            VerifyBegin(TestName);

            //Hide In Mobile
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop initial");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            HiddenInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop before");
            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "not displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before on page1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "not displayed block in mobile on page1");

            //Show In Mobile
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in desktop after");
            GoToMobile("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-header")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after on page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed,
                "displayed block in mobile on page1");

            VerifyFinally(TestName);
        }

        [Test]
        public void runDownBlock()
        {
            TestName = "runDownBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-string-id=\"block_1\"]")).GetAttribute("class")
                    .Contains("lp-block--run_block"), "run_down is off before");

            //on
            BlockSettingsBtn(numberBlock);
            RunDown();
            BlockSettingsSave();
            Thread.Sleep(2000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("lp-block--run_block"),
                "run_down is on in client");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("lp-block--run_block"),
                "run_down is on in mobile");

            //off
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            NoRunDown();
            BlockSettingsSave();
            Thread.Sleep(2000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("lp-block--run_block"),
                "run_down is off in client");
            GoToMobile("lp/test1");
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("lp-block--run_block"),
                "run_down is off in mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void ShowOnPages()
        {
            TestName = "ShowOnPages";
            VerifyBegin(TestName);

            //Show on all pages - on
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"desktop_hidden\"] input")).Selected,
                "off hidden in desktop");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed, "on main page before");
            GoToClient("lp/test1/page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed, "on page1 before");

            //Show on all pages - off
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnThisPage();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-header")).Displayed, "on main page after");
            GoToClient("lp/test1/page1");
            VerifyIsFalse(Driver.PageSource.Contains("#block_1 .lp-header"), "on page1 after");

            VerifyFinally(TestName);
        }

        [Test]
        public void zConvertHTML()
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