using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoDefaultTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsProductsPerPage);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsSeo\\Settings.Settings.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void DefaultSeoMain()
        {
            GoToAdmin("settingsseo");

            Driver.FindElement(By.Id("DefaultTitle")).Click();
            Driver.FindElement(By.Id("DefaultTitle")).Clear();
            Driver.FindElement(By.Id("DefaultTitle")).SendKeys("New title Main Page");

            Driver.FindElement(By.Id("DefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("DefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("DefaultMetaKeywords")).SendKeys(
                "New meta keywords 1 Main Page, New meta keywords 2 Main Page, New meta keywords 3 Main Page");

            Driver.FindElement(By.Id("DefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("DefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("DefaultMetaDescription")).SendKeys("New description Main Page");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title Main Page", Driver.FindElement(By.Id("DefaultTitle")).GetAttribute("value"),
                "default seo main title admin");
            VerifyAreEqual(
                "New meta keywords 1 Main Page, New meta keywords 2 Main Page, New meta keywords 3 Main Page",
                Driver.FindElement(By.Id("DefaultMetaKeywords")).GetAttribute("value"),
                "default seo main keywords admin");
            VerifyAreEqual("New description Main Page",
                Driver.FindElement(By.Id("DefaultMetaDescription")).GetAttribute("value"),
                "default seo main description admin");

            //check client
            GoToClient();
            VerifyAreEqual("New title Main Page", Driver.Title, "default seo main title client");
            VerifyAreEqual(
                "New meta keywords 1 Main Page, New meta keywords 2 Main Page, New meta keywords 3 Main Page",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo main keywords client");
            VerifyAreEqual("New description Main Page",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo main description client");
        }

        [Test]
        public void DefaultSeoMainVariables()
        {
            GoToAdmin("settingsseo");

            Driver.FindElement(By.Id("DefaultTitle")).Click();
            Driver.FindElement(By.Id("DefaultTitle")).Clear();
            Driver.FindElement(By.Id("DefaultTitle")).SendKeys("#STORE_NAME# - #STORE_NAME#");

            Driver.FindElement(By.Id("DefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("DefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("DefaultMetaKeywords")).SendKeys("#STORE_NAME# - #STORE_NAME#");

            Driver.FindElement(By.Id("DefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("DefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("DefaultMetaDescription")).SendKeys("#STORE_NAME# - #STORE_NAME#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #STORE_NAME#",
                Driver.FindElement(By.Id("DefaultTitle")).GetAttribute("value"), "default seo main title admin");
            VerifyAreEqual("#STORE_NAME# - #STORE_NAME#",
                Driver.FindElement(By.Id("DefaultMetaKeywords")).GetAttribute("value"),
                "default seo main keywords admin");
            VerifyAreEqual("#STORE_NAME# - #STORE_NAME#",
                Driver.FindElement(By.Id("DefaultMetaDescription")).GetAttribute("value"),
                "default seo main description admin");

            //check client
            GoToClient();
            VerifyAreEqual("Мой магазин - Мой магазин", Driver.Title, "default seo main title client");
            VerifyAreEqual("Мой магазин - Мой магазин",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo main keywords client");
            VerifyAreEqual("Мой магазин - Мой магазин",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo main description client");
        }

        [Test]
        public void DefaultSeoInstruction()
        {
            GoToAdmin("settingsseo");

            Driver.FindElement(By.CssSelector("[data-e2e=\"metaInstruction\"]")).Click();

            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyIsTrue(Driver.Url.Contains("help") && Driver.Url.Contains("seo-module"), "default seo instruction");
            Functions.CloseTab(Driver, BaseUrl);
        }
    }
}