using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Characteristics.CharacteristicsBlock
{
    [TestFixture]
    public class LandingsCharacteristicsSettingsMain : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunicationSimple\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Characteristics\\Characteristics\\CMS.LandingSubBlock.csv"
            );

            Init();
        }

        private readonly string blockName = "characteristics";
        private readonly string blockType = "Characteristics";
        private readonly int numberBlock = 1;
        private readonly string blockNameClient = "lp-block-characteristics";
        private readonly string blockNameClientfull = "lp-block-characteristics";


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
            Driver.FindElements(By.CssSelector(".color-viewer-inner"))[4].Click();
            Thread.Sleep(2000);
            Driver.FindElement(By.CssSelector("#modalSettingsBlock_colorSchemeCustom .blocks-constructor-btn-confirm"))
                .Click();
            Thread.Sleep(2000);
            VerifyAreEqual("rgba(229, 148, 163, 1)",
                Driver.FindElement(By.Id("block_1")).GetCssValue("background-color"), "background color custom");

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
                "padding_top before");
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(70);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("70px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top after");

            Refresh();
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingTop(5);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("5px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-top"),
                "padding_top initial");

            //change padding_bottom
            VerifyAreEqual("45px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom before");
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(25);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("25px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom after");

            Refresh();
            Thread.Sleep(2000);
            BlockSettingsBtn(numberBlock);
            moveSliderPaddingBottom(5);
            BlockSettingsSave();
            Thread.Sleep(2000);
            VerifyAreEqual("5px", Driver.FindElement(By.Id("block_1")).GetCssValue("padding-bottom"),
                "padding_bottom initial");

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeImgBackground()
        {
            TestName = "ChangezBackgroundImage";
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
            UpdateImageByUrl("https://cs8.pikabu.ru/post_img/big/2016/06/23/6/1466672239198666433.png");
            BlockSettingsSave();
            Thread.Sleep(7000);
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).GetAttribute("style")
                    .Contains("background-image: url(\"pictures/landing"), "upload photo via URL");

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
            ReInitClient();
            GoToClient("lp/test1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1,
                "lazy load on"); //is on

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            OpenPopupUpdateImage();
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] input")).Selected,
                "lazy load on btn");

            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElements(By.CssSelector(".adv-modal-close"))[1].Click();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElements(By.CssSelector("#block_1 div[data-qazy-background]")).Count == 1,
                "lazy load off"); //is off

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
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 div")).GetAttribute("style").Contains("background-image:"),
                "nophoto admin");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 div")).GetAttribute("style").Contains("background-image:"),
                "nophoto desktop");
            GoToMobile("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 div")).GetAttribute("style").Contains("background-image:"),
                "nophoto mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void ChangeImgBehaviour()
        {
            TestName = "ChangeImgBehaviour";
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
        public void HiddenMobile()
        {
            TestName = "HiddenMobile";
            VerifyBegin(TestName);

            //Hide In Mobile
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop initial");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            HiddenInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop before");
            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "not displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class before on page1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "not displayed block in mobile on page1");

            //Show In Mobile
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowInMobile();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop after");
            GoToMobile("lp/test1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile");
            GoToMobile("lp/test1/page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-mobile"), "block`s class after on page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile on page1");

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
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop initial");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile initial");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnAllPage();
            HiddenInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "not displayed block in desktop");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsFalse(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "not displayed block in desktop on page1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class before on page1");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile before");

            //Show in desktop
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowInDesktop();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after");
            GoToClient("lp/test1/page1");
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in desktop on page1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClientfull)).GetAttribute("class")
                    .Contains("hidden-device-desktop"), "block`s class after on page1");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "displayed block in mobile after");

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
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "on main page before");
            GoToClient("lp/test1/page1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "on page1 before");

            //Show on all pages - off
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowOnThisPage();
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("#block_1 ." + blockNameClient)).Displayed,
                "on main page after");
            GoToClient("lp/test1/page1");
            VerifyIsFalse(Driver.PageSource.Contains("#block_1 ." + blockNameClient), "on page1 after");

            VerifyFinally(TestName);
        }

        [Test]
        public void ShowTitles()
        {
            TestName = "ShowTitles";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            HiddenTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 0,
                "no Characteristics title admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 0,
                "no Characteristics title desktop");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 0,
                "no Characteristics title mobile");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowTitle();
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 1,
                "Characteristics title admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 1,
                "Characteristics title desktop");
            VerifyAreEqual("Характеристики",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Text,
                "Characteristics title desktop text");
            GoToMobile("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 1,
                "Characteristics title mobile");
            VerifyAreEqual("Характеристики",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Text,
                "Characteristics title mobile text");

            VerifyFinally(TestName);
        }

        [Test]
        public void ShowSubTitles()
        {
            TestName = "ShowSubTitles";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            HiddenSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Count == 0,
                "no Characteristics title admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Count == 0,
                "no Characteristics title desktop");
            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Count == 0,
                "no Characteristics title mobile");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            ShowSubTitle();
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Count == 1,
                "Characteristics title admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Count == 1,
                "Characteristics title desktop");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Text,
                "Characteristics title desktop text");
            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Count == 1,
                "Characteristics title mobile");
            VerifyAreEqual("Реализуй все с Advantshop",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"SubTitleCharacteristics\"]")).Text,
                "Characteristics title mobile text");

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
            VerifyIsTrue(select3.SelectedOption.Text.Contains("12 колонок"));

            WidthColumn("3 колонки");
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-3")).Count == 2,
                "Characteristics WidthColumn 3 admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-3")).Count == 2,
                "Characteristics WidthColumn 3 desktop");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            WidthColumn("10 колонок");
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-10")).Count == 2,
                "Characteristics WidthColumn 10 admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-3")).Count == 0,
                "no Characteristics WidthColumn 3 admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-10")).Count == 2,
                "Characteristics WidthColumn 10 desktop");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            WidthColumn("12 колонок");
            BlockSettingsSave();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-12")).Count == 2,
                "Characteristics WidthColumn 12 admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-10")).Count == 0,
                "no Characteristics WidthColumn 10 admin");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-3")).Count == 0,
                "no Characteristics WidthColumn 3 admin");
            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 .col-xs-12.col-md-12")).Count == 2,
                "Characteristics WidthColumn 12 desktop");

            VerifyFinally(TestName);
        }

        [Test]
        public void UpdateBlockSettings()
        {
            TestName = "UpdateBlockSettings";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            BlockSettingsBtn(numberBlock);
            UpdateBlockCancel();
            BlockSettingsSave();
            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block update cancel");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 1,
                "Characteristics title desktop cancel");
            VerifyAreEqual("Характеристики",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Text,
                "Characteristics title desktop text cancel");

            BlockSettingsBtn(numberBlock);
            UpdateBlockSave();

            VerifyIsTrue(Driver.FindElement(By.Id("block_" + numberBlock)).Displayed, "block update save");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Count == 1,
                "Characteristics title desktop save");
            VerifyAreEqual("Характеристики",
                Driver.FindElement(By.CssSelector("#block_1 [data-e2e=\"TitleCharacteristics\"]")).Text,
                "Characteristics title desktop text save");
            VerifyFinally(TestName);
        }

        [Test]
        public void zConvertHTML()
        {
            TestName = "zConvertHTML";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1");

            AddBlockByBtnBig(blockType, blockName);

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