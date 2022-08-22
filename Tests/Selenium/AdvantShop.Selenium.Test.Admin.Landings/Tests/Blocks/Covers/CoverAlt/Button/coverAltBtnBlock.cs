using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.CoverAlt.Button
{
    [TestFixture]
    public class coverAltBtnBlock : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\ManyBlocks\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\coverAlt\\ManyBlocks\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private readonly string FormBtn1 = "CoversBtn1";
        private readonly string FormBtn2 = "CoversBtn2";

        [Test]
        public void Btn1SettingPage()
        {
            TestName = "Btn1SettingPage";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).Count == 0,
                "no block field");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_5");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
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
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block2");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test");
            BtnActionBlockSelectBlock(4);
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).GetAttribute("value"), "new value");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
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
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block3");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_99");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block3", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block3", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block3", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
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
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block4");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test123!@#$%^&*()_+=-?:;№");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block4", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
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
            VerifyAreEqual("Btn Block4", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
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
            VerifyAreEqual("Btn Block4", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn1 + "\"]")).Text,
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
            TabSelect("tabCoverButton2");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block1", 1);
            BtnActionButtonSelect("Переход к блоку", 1);
            BtnActionBlockSetBlock("#block_5", 1);
            BlockSettingsSave();

            VerifyAreEqual("Btn Block1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block1", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
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
            TabSelect("tabCoverButton2");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block2", 1);
            BtnActionButtonSelect("Переход к блоку", 1);
            BtnActionBlockSetBlock("test", 1);
            BtnActionBlockSelectBlock(4, 1);
            VerifyAreEqual("#block_4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]"))[1].GetAttribute("value"),
                "new value");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block2", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
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
        public void Btn2SettingPageIncorrect()
        {
            TestName = "Btn2SettingPageIncorrect";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block3", 1);
            BtnActionButtonSelect("Переход к блоку", 1);
            BtnActionBlockSetBlock("test123!@#$%^&*()_+=-?:;№", 1);
            BlockSettingsSave();

            VerifyAreEqual("Btn Block3", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
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
            VerifyAreEqual("Btn Block3", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
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
            VerifyAreEqual("Btn Block3", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
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
        public void Btn2SettingPageNoexist()
        {
            TestName = "Btn2SettingPageNoexist";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton2");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block4", 1);
            BtnActionButtonSelect("Переход к блоку", 1);
            BtnActionBlockSetBlock("#block_99", 1);
            BlockSettingsSave();

            VerifyAreEqual("Btn Block4", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block4", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block4", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;

            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn2 + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }
    }
}