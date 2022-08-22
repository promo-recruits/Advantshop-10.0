using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Services.servicesThreeColumns
{
    [TestFixture]
    public class LandingsServicesColumnsEdit : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Services\\servicesThreeColumns\\CMS.LandingSubBlock.csv"
            );

            Init();
        }

        private string blockName = "servicesThreeColumns";
        private string blockType = "Services";
        private readonly int numberBlock = 1;
        private string blockNameClient = "lp-block-services-three-columns";
        private string blockNameClientfull = "lp-block-services-three-columns";
        private readonly string blockTitle = "TitleBlock";
        private readonly string blockSubTitle = "SubTitleBlock";
        private string blockContent = "ServicesContent";


        [Test]
        public void BtnServicesSetting()
        {
            TestName = "EditServicesInplace";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnEnabledButton();
            BtnSetTextButton("Name Btn");
            BlockSettingsSave();

            VerifyAreEqual("Name Btn",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]"))[0].Text,
                " ServicesItem btn text 1");
            VerifyAreEqual("Name Btn",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]"))[1].Text,
                " ServicesItem btn text 2");
            VerifyAreEqual("Name Btn",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]"))[2].Text,
                " ServicesItem btn text 3");

            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1,
                " ServicesItem btn enabled 1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                3, " ServicesItem btn enabled all");
            Refresh();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                3, " ServicesItem btn enabled all refresh");
            VerifyAreEqual("Name Btn",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]"))[0].Text,
                " ServicesItem btn text 1 refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                3, " ServicesItem btn enabled all client");
            VerifyAreEqual("Name Btn",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]"))[0].Text,
                " ServicesItem btn text 1 client");

            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                3, " ServicesItem btn enabled all mobile");
            VerifyAreEqual("Name Btn",
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]"))[0].Text,
                " ServicesItem btn text 1 mobile");

            ReInit();
            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn();
            TabSelect("tabServiceButton");
            BtnDisableButton();
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0,
                "no ServicesItem btn Disable 1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                0, "no ServicesItem btn Disable all");
            Refresh();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                0, "no ServicesItem btn Disable all refresh");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesItem\"]")).Text.Contains("Name Btn"),
                "no ServicesItem btn Disable text refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                0, " ServicesItem btn enabled all client");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesItem\"]")).Text.Contains("Name Btn"),
                "no ServicesItem btn Disable text mobile");

            GoToMobile("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"] [data-e2e=\"ServicesBtn\"]")).Count ==
                0, " ServicesItem btn enabled all mobile");
            VerifyIsFalse(Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesItem\"]")).Text.Contains("Name Btn"),
                "no ServicesItem btn Disable text mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditServicesInplace()
        {
            TestName = "EditServicesInplace";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized"))
                .SendKeys("New Title");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).SendKeys("New Services");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized"))
                .SendKeys("New SubTitle");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).SendKeys("New Services text");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Click();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Clear();
            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).SendKeys("9999 y.e.");

            Driver.FindElement(By.Id("block_" + numberBlock))
                .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Click();

            VerifyAreEqual("New Services text",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text, "Services text");
            VerifyAreEqual("9999 y.e.",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "value price");
            VerifyAreEqual("New Services",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text, "Services");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle ");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title ");

            Refresh();

            VerifyAreEqual("New Services text",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text,
                "Services text after refresh");
            VerifyAreEqual("9999 y.e.",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "value Services after refresh");
            VerifyAreEqual("New Services",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text, "Services after refresh");
            VerifyAreEqual("New SubTitle",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockSubTitle + "\"] .inplace-initialized")).Text,
                "subtitle Services after refresh");
            VerifyAreEqual("New Title",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"" + blockTitle + "\"] .inplace-initialized")).Text,
                "title Services after refresh");

            BlockSettingsBtn();
            TabSelect("tabServiceItems");

            VerifyAreEqual("New Services",
                Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header");
            VerifyAreEqual("New Services text",
                Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name");
            VerifyAreEqual("9999 y.e.", Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace value");
            VerifyAreEqual("Мобильная версия",
                Driver.GetGridCell(2, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header 5");
            VerifyAreEqual("Клиент может совершать покупки в вашме магазине с любого устройства",
                Driver.GetGridCell(2, "text").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name 5");
            VerifyAreEqual("5 600 руб", Driver.GetGridCell(2, "price").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace value 5");

            BlockSettingsClose();

            VerifyFinally(TestName);
        }

        [Test]
        public void EditServicesInplaceSettings()
        {
            TestName = "EditServicesInplaceSettings";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");
            Thread.Sleep(2000);

            BlockSettingsBtn(numberBlock);

            TabSelect("tabServiceButton");
            BtnEnabledButton();

            TabSelect("tabServiceItems");

            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).SendKeys("New name By Setting");

            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).SendKeys("text By Setting");

            Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).SendKeys("555 $");

            BlockSettingsSave();

            VerifyAreEqual("New name By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text, "name ");
            VerifyAreEqual("text By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text, "value ");
            VerifyAreEqual("555 $",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "Price ");

            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1, "ServicesItem btn new 1");
            Refresh();

            VerifyAreEqual("New name By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text, "name  after refresh");
            VerifyAreEqual("text By Setting",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text, "value  after refresh");
            VerifyAreEqual("555 $",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "Price  after refresh");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");
            VerifyAreEqual("New name By Setting",
                Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).GetAttribute("value"), "settings  header");
            VerifyAreEqual("text By Setting",
                Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).GetAttribute("value"), "settings  name");
            VerifyAreEqual("555 $", Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings  value");

            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).SendKeys("new header");

            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).SendKeys("new text");

            Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).SendKeys("new price");

            Driver.GetGridCell(0, "show_price").FindElement(By.TagName("span")).Click();
            Driver.GetGridCell(0, "show_button").FindElement(By.TagName("span")).Click();

            BlockSettingsSave();

            VerifyAreEqual("new header",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItem Header new 1");
            VerifyAreEqual("new text",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text, "ServicesItem Text new 1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "no ServicesItem price new 1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0,
                "no ServicesItem btn new 1");

            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceButton");
            BtnDisableButton();
            TabSelect("tabServiceItems");
            Driver.GetGridCell(0, "show_button").FindElement(By.TagName("span")).Click();
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0,
                "no ServicesItem btn enabled");

            VerifyFinally(TestName);
        }

        [Test]
        public void EditServicesPictureInplace()
        {
            TestName = "EditServicesPictureInplace";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_1")).FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img"))
                    .GetAttribute("src").Contains("areas/landing/images/services/serviceThreeColumns1.jpg"),
                "block picture initial");
            pathInit = Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".picture-upload-modal")).Displayed, "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".picture-upload-modal .modal-header")).Text, "modal is correct");

            //from PC
            UpdateLoadImageDesktop("light.jpg");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img"))
                .GetAttribute("src"); //pathPC
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via PC in client");

            //from URL
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src"),
                "pathPC vs pathURL");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img"))
                .GetAttribute("src"); //pathURL
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathURL");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via URL in client");

            //from Gallery
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]"));
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateLoadImageGallery();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");
            VerifyAreNotEqual(path,
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src"),
                "pathURL vs pathG");
            path = Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src"); //pathG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathG");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ServicesLoadPic\"]"))[0]
                    .FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on initial");

            //lazy-load
            ReInit();
            GoToClient("lp/test1");

            Driver.MouseFocus(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ServicesLoadPic\"]"))[0]
                    .FindElements(By.CssSelector("img[data-qazy]")).Count == 0, "lazy-load off");

            ReInit();
            GoToClient("lp/test1");
            Driver.MouseFocus(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]"));
            Thread.Sleep(1000);
            Driver.FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".adv-modal-close")).Click();
            Thread.Sleep(1000);

            ReInitClient();

            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ServicesLoadPic\"]"))[0]
                    .FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on");

            ReInit();
            VerifyFinally(TestName);
        }

        [Test]
        public void EditServicesPictureSetting()
        {
            TestName = "EditServicesPictureSetting";
            VerifyBegin(TestName);
            var pathInit = "";
            var path = "";

            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");

            pathInit = Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src");

            MouseFocusGrid();
            Driver.GetGridCell(0, "picture").FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".picture-upload-modal")).Displayed, "modal is dysplayed");
            VerifyAreEqual("Обновить изображение",
                Driver.FindElement(By.CssSelector(".picture-upload-modal .modal-header")).Text, "modal is correct");

            //from PC            
            UpdateLoadImageDesktop("images.jpg");
            VerifyIsTrue(
                Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC");
            path = Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src"); //pathPC
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC check");
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via PC client");

            //delete picture
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");
            MouseFocusGrid();
            Driver.GetGridCell(0, "picture").FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            DelImageSave();
            Driver.FindElement(By.CssSelector(".picture-upload-modal .adv-modal-close")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "picture").FindElements(By.TagName("img")).Count == 0, "no photo");
            VerifyIsTrue(
                Driver.GetGridCell(0, "picture").FindElements(By.CssSelector(".lp-grid__image-wrap.lp-grid__no-photo"))
                    .Count == 1, "no photo btn load");
            BlockSettingsSave();

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("images/nophoto.jpg"), "no photo check");
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathPC del");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("images/nophoto.jpg"), "no photo  after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]")).FindElements(By.TagName("img"))
                    .Count == 0, "no photo  client");

            //from URL
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");

            Driver.FindElement(By.CssSelector(".lp-grid__image-wrap.lp-grid__no-photo")).Click();

            UpdateImageByUrl(
                "https://s2.best-wallpaper.net/wallpaper/1280x800/1902/Warm-light-circles-bright_1280x800.jpg");
            VerifyIsTrue(
                Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL");
            VerifyAreNotEqual(path, Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src"),
                "pathPC vs pathURL");
            pathInit = Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src"); //pathURL
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathURL");

            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL check");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL after refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via URL client");

            //from Gallery
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");

            MouseFocusGrid();
            Driver.GetGridCell(0, "picture").FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();

            UpdateLoadImageGallery(2);
            VerifyIsTrue(
                Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");
            VerifyAreNotEqual(path, Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src"),
                "pathURL vs pathG");
            path = Driver.GetGridCell(0, "picture").FindElement(By.TagName("img")).GetAttribute("src"); //pathG
            VerifyAreNotEqual(pathInit, path, "pathInit vs pathG");
            BlockSettingsSave();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");

            Refresh();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery refresh");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains(path), "upload via Gallery in client");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ServicesLoadPic\"]"))[0]
                    .FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on initial");

            //lazy-load
            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");

            MouseFocusGrid();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "picture").FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".picture-upload-modal .adv-modal-close")).Click();
            Thread.Sleep(1000);
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ServicesLoadPic\"]"))[0]
                    .FindElements(By.CssSelector("img[data-qazy]")).Count == 0, "lazy-load off");

            ReInit();
            GoToClient("lp/test1");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");
            MouseFocusGrid();
            Thread.Sleep(1000);
            Driver.GetGridCell(0, "picture").FindElement(By.CssSelector(".subblock-inplace-image-trigger")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"lazyLoadEnabled\"] span")).Click();
            Driver.FindElement(By.CssSelector(".picture-upload-modal .adv-modal-close")).Click();
            Thread.Sleep(1000);
            BlockSettingsSave();
            ReInitClient();

            GoToClient("lp/test1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("#block_1 [data-e2e=\"ServicesLoadPic\"]"))[0]
                    .FindElements(By.CssSelector("img[data-qazy]")).Count == 1, "lazy-load on");


            VerifyFinally(TestName);
        }

        [Test]
        public void EditServicesSettingsAdd()
        {
            TestName = "EditServicesSettingsAdd";
            VerifyBegin(TestName);
            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn(numberBlock);
            HiddenTitle();
            HiddenSubTitle();
            TabSelect("tabServiceButton");
            BtnEnabledButton();

            TabSelect("tabServiceItems");
            DelAllColumns("ServicesGrid");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no header settings");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['price']\"]")).Count == 0,
                "no price settings");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['show_button']\"]")).Count == 0,
                "no show_button settings");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesGrid\"]")).Text.Contains("Нет элементов"),
                "no elem ServicesGrid settings");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-content")).Text.Contains("Добавить новый элемент"),
                "btm add elem settings");
            BlockSettingsSave();
            Thread.Sleep(2000);

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesLoadPic\"]")).Count == 0,
                "no pictures");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Count == 0,
                "no ServicesItemHeader");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Count == 0,
                "no ServicesItemText");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "no ServicesPrice");


            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['header']\"]")).Count == 0,
                "no header settings refresh");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['price']\"]")).Count == 0,
                "no price settings refresh");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['show_button']\"]")).Count == 0,
                "no show_button settings refresh");

            AddNewElem("header1", "text1", "price1");
            AddNewElem("header2", "text2", "price2", true, false);
            AddNewElem("header3", "text3", "price3", false);
            AddNewElem("header4", "text4", "price4", false, false);

            BlockSettingsSave();

            VerifyAreEqual("header1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItem Header new 1");
            VerifyAreEqual("text1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text, "ServicesItem Text new 1");
            VerifyAreEqual("price1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "Services Price new 1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 1");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1, "ServicesItem btn new 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery");

            VerifyAreEqual("header2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[1].Text,
                "ServicesItem Header new 2");
            VerifyAreEqual("text2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]"))[1].Text,
                "ServicesItem Text new 2");
            VerifyAreEqual("price2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]"))[1].Text, "Services Price new 2");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[1]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 2");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[1]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0, "ServicesItem btn new 2");


            VerifyAreEqual("header3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[2].Text,
                "ServicesItem Header new 3");
            VerifyAreEqual("text3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]"))[2].Text,
                "ServicesItem Text new 3");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[2]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "ServicesItem price new 3");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[2]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1, "ServicesItem btn new 3");

            VerifyAreEqual("header4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[3].Text,
                "ServicesItem Header new 4");
            VerifyAreEqual("text4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]"))[3].Text,
                "ServicesItem Text new 4");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[3]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "ServicesItem price new 4");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[3]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0, "ServicesItem btn new 4");

            Refresh();
            VerifyAreEqual("header1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItem Header new 1 after refresh");
            VerifyAreEqual("text1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text,
                "ServicesItem Text new 1 after refresh");
            VerifyAreEqual("price1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text,
                "Services Price new 1 after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 1 after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1,
                "ServicesItem btn new 1 after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery after refresh");

            VerifyAreEqual("header2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[1].Text,
                "ServicesItem Header new 2 after refresh");
            VerifyAreEqual("text2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]"))[1].Text,
                "ServicesItem Text new 2 after refresh");
            VerifyAreEqual("price2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]"))[1].Text,
                "Services Price new 2 after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[1]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 2 after refresh");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[1]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0,
                "ServicesItem btn new 2 after refresh");

            ReInitClient();
            GoToClient("lp/test1?inplace=true");
            VerifyAreEqual("header1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItem Header new 1 client");
            VerifyAreEqual("text1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text,
                "ServicesItem Text new 1 client");
            VerifyAreEqual("price1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "Services Price new 1 client");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 1 client");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1,
                "ServicesItem btn new 1 client");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery client");

            VerifyAreEqual("header4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[3].Text,
                "ServicesItem Header client 4");
            VerifyAreEqual("text4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]"))[3].Text,
                "ServicesItem Text client 4");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[3]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "ServicesItem price client 4");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[3]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0,
                "ServicesItem btn client 4");


            GoToMobile("lp/test1?inplace=true");
            VerifyAreEqual("header1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItem Header new 1 Mobile");
            VerifyAreEqual("text1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemText\"]")).Text,
                "ServicesItem Text new 1 Mobile");
            VerifyAreEqual("price1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Text, "Services Price new 1 Mobile");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 1,
                "ServicesItem price new 1 Mobile");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[0]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 1,
                "ServicesItem btn new 1 Mobile");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesLoadPic\"] img")).GetAttribute("src")
                    .Contains("pictures/landing"), "upload via Gallery Mobile");

            VerifyAreEqual("header4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[3].Text,
                "ServicesItem Header Mobile 4");
            VerifyAreEqual("text4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemText\"]"))[3].Text,
                "ServicesItem Text Mobile 4");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[3]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesPrice\"]")).Count == 0,
                "ServicesItem price Mobile 4");
            VerifyIsTrue(
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItem\"]"))[3]
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesBtn\"]")).Count == 0,
                "ServicesItem btn Mobile 4");

            ReInit();
            GoToClient("lp/test1?inplace=true");
            BlockSettingsBtn(numberBlock);
            TabSelect("tabServiceItems");

            VerifyAreEqual("header1", Driver.GetGridCell(0, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header");
            VerifyAreEqual("text1", Driver.GetGridCell(0, "text").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name");
            VerifyAreEqual("price1", Driver.GetGridCell(0, "price").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace price");
            VerifyAreEqual("header4", Driver.GetGridCell(3, "header").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace header 4");
            VerifyAreEqual("text4", Driver.GetGridCell(3, "text").FindElement(By.TagName("textarea")).GetAttribute("value"),
                "settings inplace name 4");
            VerifyAreEqual("price4", Driver.GetGridCell(3, "price").FindElement(By.TagName("input")).GetAttribute("value"),
                "settings inplace price 4");

            //Drag and drop

            DragDrop(2, 0, "ServicesGrid");
            DragDrop(1, 3, "ServicesGrid");
            BlockSettingsSave();

            VerifyAreEqual("header3",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElement(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]")).Text,
                "ServicesItem Header drag 1");
            VerifyAreEqual("header2",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[1].Text,
                "ServicesItem Header drag 2");
            VerifyAreEqual("header4",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[2].Text,
                "ServicesItem Header drag 3");
            VerifyAreEqual("header1",
                Driver.FindElement(By.Id("block_" + numberBlock))
                    .FindElements(By.CssSelector("[data-e2e=\"ServicesItemHeader\"]"))[3].Text,
                "ServicesItem Header drag 4");


            VerifyFinally(TestName);
        }


        public void MouseFocusGrid()
        {
            var action = new Actions(Driver);
            var elem = Driver.GetGridCell(0, "picture");
            action.MoveToElement(elem);
            action.Perform();
            Thread.Sleep(500);
        }

        public void AddNewElem(string header, string text, string price, bool show_price = true,
            bool show_button = true)
        {
            var count = Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesGrid\"]"))
                .FindElements(By.CssSelector(".lp-table__body [data-e2e=\"ItemDel\"]")).Count;
            Driver.FindElement(By.CssSelector("[data-e2e=\"ServicesGrid\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"AddNewElem\"]")).Click();

            Driver.GetGridCell(count, "header").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "header").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "header").FindElement(By.TagName("input")).SendKeys(header);

            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).Click();
            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).Clear();
            Driver.GetGridCell(count, "text").FindElement(By.TagName("textarea")).SendKeys(text);

            Driver.GetGridCell(count, "price").FindElement(By.TagName("input")).Click();
            Driver.GetGridCell(count, "price").FindElement(By.TagName("input")).Clear();
            Driver.GetGridCell(count, "price").FindElement(By.TagName("input")).SendKeys(price);

            if (show_price) Driver.GetGridCell(count, "show_price").FindElement(By.TagName("span")).Click();
            if (show_button) Driver.GetGridCell(count, "show_button").FindElement(By.TagName("span")).Click();

            Driver.FindElement(By.CssSelector(".lp-grid__image-wrap.lp-grid__no-photo")).Click();
            UpdateLoadImageGallery(count);
        }
    }
}