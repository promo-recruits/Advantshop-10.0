using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoBrandsTest : BaseSeleniumTest
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
        public void DefaultSeoBrands()
        {
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("NewsDefaultH1"));

            Driver.FindElement(By.Id("BrandsDefaultTitle")).Click();
            Driver.FindElement(By.Id("BrandsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("BrandsDefaultTitle")).SendKeys("New title Brands");

            Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("BrandsDefaultMetaKeywords"))
                .SendKeys("New meta keywords 1 Brands, New meta keywords 2 Brands, New meta keywords 3 Brands");

            Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).SendKeys("New description Brands");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title Brands", Driver.FindElement(By.Id("BrandsDefaultTitle")).GetAttribute("value"),
                "default seo Brands title admin");
            VerifyAreEqual("New meta keywords 1 Brands, New meta keywords 2 Brands, New meta keywords 3 Brands",
                Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo Brands keywords admin");
            VerifyAreEqual("New description Brands",
                Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).GetAttribute("value"),
                "default seo Brands description admin");

            //check client
            GoToClient("manufacturers");
            VerifyAreEqual("New title Brands", Driver.Title, "default seo Brands title client");
            VerifyAreEqual("New meta keywords 1 Brands, New meta keywords 2 Brands, New meta keywords 3 Brands",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo Brands keywords client");
            VerifyAreEqual("New description Brands",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo Brands description client");
        }

        [Test]
        public void DefaultSeoBrandsVariables()
        {
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("NewsDefaultH1"));

            Driver.FindElement(By.Id("BrandsDefaultTitle")).Click();
            Driver.FindElement(By.Id("BrandsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("BrandsDefaultTitle")).SendKeys("#STORE_NAME#");

            Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).SendKeys("#STORE_NAME#");

            Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).SendKeys("#STORE_NAME#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME#", Driver.FindElement(By.Id("BrandsDefaultTitle")).GetAttribute("value"),
                "default seo Brands title admin");
            VerifyAreEqual("#STORE_NAME#", Driver.FindElement(By.Id("BrandsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo Brands keywords admin");
            VerifyAreEqual("#STORE_NAME#",
                Driver.FindElement(By.Id("BrandsDefaultMetaDescription")).GetAttribute("value"),
                "default seo Brands description admin");

            //check client
            GoToClient("manufacturers");
            VerifyAreEqual("Мой магазин", Driver.Title, "default seo Brands title client");
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo Brands keywords client");
            VerifyAreEqual("Мой магазин",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo Brands description client");
        }
    }
}