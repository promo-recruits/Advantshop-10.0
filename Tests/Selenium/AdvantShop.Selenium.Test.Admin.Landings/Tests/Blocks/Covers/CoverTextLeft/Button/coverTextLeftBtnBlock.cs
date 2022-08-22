using AdvantShop.Selenium.Core.Domain;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Landings.Tests.Blocks.Covers.CoverTextLeft.Button
{
    [TestFixture]
    public class coverTextLeftBtnBlock : LandingsFunctions
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Landing);
            InitializeService.LoadData(
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Catalog.Product.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Catalog.Offer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Catalog.Category.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Catalog.ProductCategories.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Customers.CustomerGroup.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Customers.Customer.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Customers.Contact.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Customers.Departments.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Customers.Managers.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\CMS.Landing.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\CMS.LandingSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\CMS.LandingSite.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\CMS.LandingSiteSettings.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\ManyBlocks\\CMS.LandingBlock.csv",
                "data\\Admin\\LandingTest\\Covers\\coverTextLeft\\ManyBlocks\\CMS.LandingSubBlock.csv"
            );
            Init();
        }

        private readonly string FormBtn = "CoversBtn";

        [Test]
        public void BtnSettingPage()
        {
            TestName = "BtnSettingPage";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).Count == 0,
                "no block field");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_5");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_5",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
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
        public void BtnSettingPageChoose()
        {
            TestName = "BtnSettingPageChoose";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test");
            BtnActionBlockSelectBlock(4);
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"BlockIdInput\"]")).GetAttribute("value"), "new value");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_4",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
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
        public void BtnSettingPageNoexist()
        {
            TestName = "BtnSettingPageNoexist";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("#block_99");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("#block_99",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");

            GoToMobile("lp/test1");
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

        [Test]
        public void BtnSettingPageIncorrect()
        {
            TestName = "BtnSettingPageIncorrect";
            VerifyBegin(TestName);

            ReInit();
            GoToClient("lp/test1?inplace=true");

            BlockSettingsBtn();
            TabSelect("tabCoverButton");
            BtnEnabledButton();
            BtnSetTextButton("Btn Block");
            BtnActionButtonSelect("Переход к блоку");
            BtnActionBlockSetBlock("test123!@#$%^&*()_+=-?:;№");
            BlockSettingsSave();

            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll");
            var url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Displayed, "display btn");

            ReInitClient();
            GoToClient("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt client");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll client");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn client");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Displayed,
                "display btn client");

            GoToMobile("lp/test1");
            VerifyAreEqual("Btn Block", Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Text,
                "btn txt mobile");
            VerifyAreEqual("test123!@#$%^&*()_+=-?:;№",
                Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"] button"))
                    .GetAttribute("data-scroll-to-block"), "btn scroll mobile");
            url = Driver.Url;
            Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Click();
            VerifyAreEqual(url, Driver.Url, "url by btn mobile");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"" + FormBtn + "\"]")).Displayed,
                "display btn mobile");

            VerifyFinally(TestName);
        }
    }
}