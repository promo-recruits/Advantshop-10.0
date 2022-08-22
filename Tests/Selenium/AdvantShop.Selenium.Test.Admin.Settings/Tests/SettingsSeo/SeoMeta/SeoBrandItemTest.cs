using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoBrandItemTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsProductsPerPage | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Brand.csv",
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
        public void DefaultSeoBrandsItemItem()
        {
            GoToAdmin("brands/edit/8");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("BrandsDefaultMetaDescription"));

            Driver.FindElement(By.Id("BrandItemDefaultTitle")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultTitle")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultTitle")).SendKeys("New title BrandsItem");

            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).SendKeys(
                "New meta keywords 1 BrandsItem, New meta keywords 2 BrandsItem, New meta keywords 3 BrandsItem");

            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).SendKeys("New description BrandsItem");

            Driver.FindElement(By.Id("BrandItemDefaultH1")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultH1")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultH1")).SendKeys("New h1 BrandsItem");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title BrandsItem",
                Driver.FindElement(By.Id("BrandItemDefaultTitle")).GetAttribute("value"),
                "default seo BrandsItem title admin");
            VerifyAreEqual(
                "New meta keywords 1 BrandsItem, New meta keywords 2 BrandsItem, New meta keywords 3 BrandsItem",
                Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).GetAttribute("value"),
                "default seo BrandsItem keywords admin");
            VerifyAreEqual("New description BrandsItem",
                Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).GetAttribute("value"),
                "default seo BrandsItem description admin");
            VerifyAreEqual("New h1 BrandsItem", Driver.FindElement(By.Id("BrandItemDefaultH1")).GetAttribute("value"),
                "default seo BrandsItem h1 admin");

            //check client
            GoToClient("manufacturers/8");
            VerifyAreEqual("New title BrandsItem", Driver.Title, "default seo BrandsItem title client");
            VerifyAreEqual(
                "New meta keywords 1 BrandsItem, New meta keywords 2 BrandsItem, New meta keywords 3 BrandsItem",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo BrandsItem keywords client");
            VerifyAreEqual("New description BrandsItem",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo BrandsItem description client");
            VerifyAreEqual("New h1 BrandsItem", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text,
                "default seo BrandsItem h1 client");
        }

        [Test]
        public void DefaultSeoBrandsItemVariables()
        {
            //set default meta
            GoToAdmin("brands/edit/9");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("BrandsDefaultMetaDescription"));

            Driver.FindElement(By.Id("BrandItemDefaultTitle")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultTitle")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultTitle")).SendKeys("#STORE_NAME# - #BRAND_NAME#");

            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).SendKeys("#STORE_NAME# - #BRAND_NAME#");

            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).SendKeys("#STORE_NAME# - #BRAND_NAME#");

            Driver.FindElement(By.Id("BrandItemDefaultH1")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultH1")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultH1")).SendKeys("#STORE_NAME# - #BRAND_NAME#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #BRAND_NAME#",
                Driver.FindElement(By.Id("BrandItemDefaultTitle")).GetAttribute("value"),
                "default seo BrandsItem title admin");
            VerifyAreEqual("#STORE_NAME# - #BRAND_NAME#",
                Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).GetAttribute("value"),
                "default seo BrandsItem keywords admin");
            VerifyAreEqual("#STORE_NAME# - #BRAND_NAME#",
                Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).GetAttribute("value"),
                "default seo BrandsItem description admin");
            VerifyAreEqual("#STORE_NAME# - #BRAND_NAME#",
                Driver.FindElement(By.Id("BrandItemDefaultH1")).GetAttribute("value"),
                "default seo BrandsItem h1 admin");

            //check client
            GoToClient("manufacturers/9");
            VerifyAreEqual("Мой магазин - BrandName9", Driver.Title, "default seo BrandsItem title client");
            VerifyAreEqual("Мой магазин - BrandName9",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo BrandsItem keywords client");
            VerifyAreEqual("Мой магазин - BrandName9",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo BrandsItem description client");
            VerifyAreEqual("Мой магазин - BrandName9", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text,
                "default seo BrandsItem h1 client");
        }

        [Test]
        public void DefaultSeoBrandsItemReset()
        {
            //admin set meta for BrandsItem
            GoToAdmin("brands/edit/10");
            Driver.ScrollTo(By.Id("URL"));
            if (Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
            }

            Driver.WaitForElem(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("BrandsItem_Title");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("BrandsItem_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("BrandsItem_SeoDescription");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("BrandsItem_H1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(2000);

            //pre check client
            GoToClient("manufacturers/10");
            VerifyAreEqual("BrandsItem_Title", Driver.Title, "pre check seo BrandsItem title client");
            VerifyAreEqual("BrandsItem_H1", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text,
                "pre check seo BrandsItem h1 client");
            VerifyAreEqual("BrandsItem_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo BrandsItem keywords client");
            VerifyAreEqual("BrandsItem_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo BrandsItem description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("BrandsDefaultMetaDescription"));

            Driver.FindElement(By.Id("BrandItemDefaultTitle")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultTitle")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("BrandItemDefaultH1")).Click();
            Driver.FindElement(By.Id("BrandItemDefaultH1")).Clear();
            Driver.FindElement(By.Id("BrandItemDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("BrandItemDefaultMetaDescription"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех брендов")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("manufacturers/10");
            VerifyAreEqual("1", Driver.Title, "reset seo BrandsItem title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo BrandsItem keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo BrandsItem description client");
            VerifyAreEqual("4", Driver.FindElement(By.CssSelector(".site-body-main h1")).Text,
                "reset seo BrandsItem h1 client");
        }
    }
}