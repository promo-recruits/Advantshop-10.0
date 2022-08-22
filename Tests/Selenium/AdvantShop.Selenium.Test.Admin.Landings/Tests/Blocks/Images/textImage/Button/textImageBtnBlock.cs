using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Images.textImage.Button
{
    [TestFixture]
    public class textImageBtnBlock : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Images\\textImages\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\ManyBlocks\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Images\\textImages\\ManyBlocks\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private readonly string FormBtn1 = "textImageBtn1";
        private readonly string FormBtn2 = "textImageBtn2";

        [Test]
        public void Btn1SettingPage()
        {
            TestName = "Btn1SettingPage";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).Count == 0,
                "no block field");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_5");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn1SettingPageChoose()
        {
            TestName = "Btn1SettingPageChoose";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test");
            BtnActionBlockSelectBlock(4);
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).GetAttribute("value"), "new value");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn1SettingPageNoexist()
        {
            TestName = "Btn1SettingPageNoexist";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_99");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn1SettingPageIncorrect()
        {
            TestName = "Btn1SettingPageIncorrect";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test123!@#$%^&*()_+=-?:;№");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Displayed,
                "display btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Displayed,
                "display btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Displayed,
                "display btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn2SettingPage()
        {
            TestName = "Btn2SettingPage";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnDisableButton();
            TabSelect("tabPicturesButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Btn Block");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).Count == 0,
                "no block field");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_5");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn2SettingPageChoose()
        {
            TestName = "Btn2SettingPageChoose";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnDisableButton();
            TabSelect("tabPicturesButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test");
            BtnActionBlockSelectBlock(4);
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).GetAttribute("value"), "new value");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn2SettingPageNoexist()
        {
            TestName = "Btn2SettingPageNoexist";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnDisableButton();
            TabSelect("tabPicturesButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_99");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void Btn2SettingPageIncorrect()
        {
            TestName = "Btn2SettingPageIncorrect";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnDisableButton();
            TabSelect("tabPicturesButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test123!@#$%^&*()_+=-?:;№");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Displayed,
                "display btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Displayed,
                "display btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Displayed,
                "display btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnBothBlock()
        {
            TestName = "BtnBothBlock";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabPicturesButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block 1");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_3");

            TabSelect("tabPicturesButton2");
            BtnEnabledButton(1);
            BtnSetTextButton("Btn Block 2", 1);
            BtnActionButtonSelect("Переход к блоку", 1);
            BtnActionBlockSetBlock("#block_5", 1);
            BlockSettingsSave();

            VerifyAreEqual("Btn Block 1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn1 txt");
            VerifyAreEqual("#block_3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn1 scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url1 by btn");
            VerifyAreEqual("Btn Block 1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn1 txt after click");

            Driver.ScrollTo(By.Id("block_1"));
            VerifyAreEqual("Btn Block 2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn2 txt");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn2 scroll");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url2 by btn");
            VerifyAreEqual("Btn Block 2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn2 txt after click");

            ReInitClient();
            GoToClient("lp/test1");

            VerifyAreEqual("Btn Block 1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn1 txt client");
            VerifyAreEqual("#block_3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn1 scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn1 client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Displayed,
                "display btn1 client");
            VerifyAreEqual("Btn Block 1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn1 txt client after click");

            Driver.ScrollTo(By.Id("block_1"));
            VerifyAreEqual("Btn Block 2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn2 txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn2 scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn2 client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Displayed,
                "display btn2 client");
            VerifyAreEqual("Btn Block 2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn2 txt client after click");

            GoToMobile("lp/test1");

            VerifyAreEqual("Btn Block 1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn1 txt mobile");
            VerifyAreEqual("#block_3",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn1 scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn1 mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Displayed,
                "display btn1 mobile");
            VerifyAreEqual("Btn Block 1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn1 txt mobile after click");

            Driver.ScrollTo(By.Id("block_1"));
            VerifyAreEqual("Btn Block 2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn2 txt mobile");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn2 scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn2 mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Displayed,
                "display btn2 mobile");
            VerifyAreEqual("Btn Block 2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn2 txt mobile after click");

            VerifyFinally(TestName);
        }
    }
}