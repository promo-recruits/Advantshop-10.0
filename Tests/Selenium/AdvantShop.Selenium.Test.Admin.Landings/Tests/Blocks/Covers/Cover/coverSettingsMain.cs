using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.Cover
{
    [TestFixture]
    public class coverSettingsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingForm.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\Cover\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private string blockName = "cover";
        private string blockType = "Covers";
        private readonly int numberBlock = 1;

        [Test]
        public void BackgroundChangeBehaviour()
        {
            TestName = "BackgroundChangeBehaviour";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");

            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("fixed"),
                "background without fixed");
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("parallax"),
                "background without parallax");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("box-shadow: rgba(0, 0, 0, 0.5)"),
                "initial background with shadow");

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
            moveSlider(-5);
            BlockSettingsSave();
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("style").Contains("box-shadow:"),
                "background without shadow");

            VerifyFinally(TestName);
        }

        [Test]
        public void BackgroundImageUpdate()
        {
            TestName = "BackgroundImageUpdate";
            VerifyBegin(TestName);

            GoToClient("lp/test1");

            //from computer
            BlockSettingsBtn(numberBlock);
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"img_picture\"]")).GetAttribute("src")
                    .Contains("/areas/landing/frontend/images/covers/cover.jpg"), "initial photo in background");
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
            //ReInitClient();
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
                "lazy-load is off");

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeColorScheme()
        {
            TestName = "ChangeColorScheme";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            DelImageSave();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            VerifyAreEqual("rgba(0, 0, 0, 1)", Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"),
                "background color default");

            BlockSettingsBtn(numberBlock);
            SetColorScheme("Светлая");
            Thread.Sleep(1000);
            BlockSettingsSave();
            VerifyAreEqual("rgba(255, 255, 255, 1)",
                Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"), "background color light");

            BlockSettingsBtn(numberBlock);
            SetColorScheme("Умеренная");
            Thread.Sleep(1000);
            BlockSettingsSave();
            VerifyAreEqual("rgba(248, 248, 248, 1)",
                Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"), "background color medium");

            BlockSettingsBtn(numberBlock);
            SetColorScheme("Пользовательская");
            Thread.Sleep(1000);
            EditUserScheme();
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
            VerifyAreEqual("180px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top initial");
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
                "padding_top changed");

            //change padding_bottom
            VerifyAreEqual("180px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom initial");
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
                "padding_bottom changed");

            VerifyFinally(TestName);
        }

        [Test]
        public void FullHeight()
        {
            TestName = "FullHeight";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            //FullHeight - on
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("block-full-height"),
                "block height initial");
            BlockSettingsBtn(numberBlock);
            FullHeightOn();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("block-full-height"),
                "block height on in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("block-full-height"), "block height on in client");

            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("block-full-height"), "block height on in mobile");

            //FullHeight - off
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            FullHeightOff();
            BlockSettingsSave();
            VerifyIsFalse(Driver.FindElement(By.Id("block_1")).GetAttribute("class").Contains("block-full-height"),
                "block height off in admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("block-full-height"), "block height off in client");

            GoToMobile("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("block-full-height"), "block height off in mobile");

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
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "not displayed block in desktop on page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
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
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in desktop on page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
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
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "not displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
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
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 .lp-block-cover")).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after on page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 .lp-container")).Displayed,
                "displayed block in mobile on page1");

            VerifyFinally(TestName);
        }

        [Test]
        public void ScrollInBlock()
        {
            TestName = "ScrollInBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            //Scroll on
            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scroll initial");

            BlockSettingsBtn(numberBlock);
            ScrollToBlockOn();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scrollOn");
            VerifyAreEqual("blocks-constructor-container[data-block-id=\"1\"]",
                Driver.FindElement(By.CssSelector("#block_1 .scroll-to-block-trigger"))
                    .GetAttribute("data-scroll-to-block"), "scrollOn in block_1");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scrollOn client");
            VerifyAreEqual("#block_1",
                Driver.FindElement(By.CssSelector("#block_1 .scroll-to-block-trigger"))
                    .GetAttribute("data-scroll-to-block"), "scrollOn in block_1 client");

            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scrollOn mobile");
            VerifyAreEqual("#block_1",
                Driver.FindElement(By.CssSelector("#block_1 .scroll-to-block-trigger"))
                    .GetAttribute("data-scroll-to-block"), "scrollOn in block_1 mobile");

            //Scroll off
            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            ScrollToBlockOff();
            BlockSettingsSave();
            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scrollOff client");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scrollOff client");

            GoToMobile("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 .scroll-to-block-trigger")).Count == 1,
                "scrollOff mobile");

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
        public void WidthColumn()
        {
            TestName = "WidthColumn";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            var selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"WidthColumn\"]"));
            var select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("8 колонок"));

            WidthColumn("6 колонок");
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-6")).Count == 1,
                "cover WidthColumn 6 admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-6")).Count == 1,
                "cover WidthColumn 6 desktop");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            WidthColumn("10 колонок");
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-10")).Count == 1,
                "cover WidthColumn 10 admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-6")).Count == 0,
                "no cover WidthColumn 6 admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-10")).Count == 1,
                "cover WidthColumn 10 desktop");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            WidthColumn("12 колонок");
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-12")).Count == 1,
                "cover WidthColumn 12 admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-10")).Count == 0,
                "no cover WidthColumn 10 admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-6")).Count == 0,
                "no cover WidthColumn 6 admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-12")).Count == 1,
                "cover WidthColumn 12 desktop");

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