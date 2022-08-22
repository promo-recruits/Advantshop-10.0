using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Headers.HeaderCommunication.Button
{
    [TestFixture]
    public class headerCommunicationBtnBlock : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\ManyBlocks\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Headers\\headerCommunication\\ManyBlocks\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private readonly string FormBtn = "HeadersBtn";

        [Test]
        public void BtnSettingBlock()
        {
            TestName = "BtnSettingBlock";
            VerifyBegin(TestName);

            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabHeaderButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_5");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1?inplace=true");

            VerifyAreEqual("Btn Block", Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Text,
                "btn txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))[1]
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            var url = Driver.Url;
            Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))[1].Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnSettingBlockChoose()
        {
            TestName = "BtnSettingBlockChoose";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabHeaderButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test");
            BtnActionBlockSelectBlock(4);
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).GetAttribute("value"), "new value");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1?inplace=true");

            VerifyAreEqual("Btn Block", Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Text,
                "btn txt client");
            VerifyAreEqual("#block_4",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))[1]
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            var url = Driver.Url;
            Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnSettingBlockIncorrect()
        {
            TestName = "BtnSettingBlockIncorrect";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabHeaderButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test123!@#$%^&*()_+=-?:;№");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1?inplace=true");

            VerifyAreEqual("Btn Block", Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Text,
                "btn txt client");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))[1]
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            var url = Driver.Url;
            Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Displayed,
                "display btn client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Displayed,
                "display btn mobile");

            VerifyFinally(TestName);
        }

        [Test]
        public void BtnSettingBlockNoexist()
        {
            TestName = "BtnSettingBlockNoexist";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabHeaderButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_99");
            BlockSettingsSave();

            ReInitClient();
            GoToClient("lp/test1?inplace=true");

            VerifyAreEqual("Btn Block", Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Text,
                "btn txt client");
            VerifyAreEqual("#block_99",
                Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))[1]
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            var url = Driver.Url;
            Driver.FindElements(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]"))[1].Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BurgerMenuBtn\"]")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");

            VerifyFinally(TestName);
        }
    }
}