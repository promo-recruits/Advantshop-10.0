using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoStaticPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsProductsPerPage | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Brand.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Tag.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.PropertyGroup.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Property.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.PropertyValue.csv",
                "Data\\Admin\\Settings\\SettingsSeo\\Catalog.Color.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Size.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Product.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.Category.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.ProductPropertyValue.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.ProductCategories.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Catalog.TagMap.csv",
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
        public void DefaultSeoStaticPage()
        {
            GoToAdmin("staticpages/edit/129");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("ProductsDefaultAdditionalDescription"));

            Driver.FindElement(By.Id("StaticPageDefaultTitle")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultTitle")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultTitle")).SendKeys("New title StaticPage");

            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).SendKeys(
                "New meta keywords 1 StaticPage, New meta keywords 2 StaticPage, New meta keywords 3 StaticPage");

            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).SendKeys("New description StaticPage");

            Driver.FindElement(By.Id("StaticPageDefaultH1")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultH1")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultH1")).SendKeys("New h1 StaticPage");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title StaticPage",
                Driver.FindElement(By.Id("StaticPageDefaultTitle")).GetAttribute("value"),
                "default seo static page title admin");
            VerifyAreEqual(
                "New meta keywords 1 StaticPage, New meta keywords 2 StaticPage, New meta keywords 3 StaticPage",
                Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).GetAttribute("value"),
                "default seo static page keywords admin");
            VerifyAreEqual("New description StaticPage",
                Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).GetAttribute("value"),
                "default seo static page description admin");
            VerifyAreEqual("New h1 StaticPage", Driver.FindElement(By.Id("StaticPageDefaultH1")).GetAttribute("value"),
                "default seo static page h1 admin");

            //check client
            GoToClient("pages/about");
            VerifyAreEqual("New title StaticPage", Driver.Title, "default seo static page title client");
            VerifyAreEqual(
                "New meta keywords 1 StaticPage, New meta keywords 2 StaticPage, New meta keywords 3 StaticPage",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo static page keywords client");
            VerifyAreEqual("New description StaticPage",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo static page description client");
            VerifyAreEqual("New h1 StaticPage", Driver.FindElement(By.TagName("h1")).Text,
                "default seo static page h1 client");
        }

        [Test]
        public void DefaultSeoStaticPageVariables()
        {
            //set default meta
            GoToAdmin("staticpages/edit/132");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("ProductsDefaultAdditionalDescription"));

            Driver.FindElement(By.Id("StaticPageDefaultTitle")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultTitle")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultTitle")).SendKeys("#STORE_NAME# - #PAGE_NAME#");

            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).SendKeys("#STORE_NAME# - #PAGE_NAME#");

            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).SendKeys("#STORE_NAME# - #PAGE_NAME#");

            Driver.FindElement(By.Id("StaticPageDefaultH1")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultH1")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultH1")).SendKeys("#STORE_NAME# - #PAGE_NAME#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #PAGE_NAME#",
                Driver.FindElement(By.Id("StaticPageDefaultTitle")).GetAttribute("value"),
                "default seo static page title admin");
            VerifyAreEqual("#STORE_NAME# - #PAGE_NAME#",
                Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).GetAttribute("value"),
                "default seo static page keywords admin");
            VerifyAreEqual("#STORE_NAME# - #PAGE_NAME#",
                Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).GetAttribute("value"),
                "default seo static page description admin");
            VerifyAreEqual("#STORE_NAME# - #PAGE_NAME#",
                Driver.FindElement(By.Id("StaticPageDefaultH1")).GetAttribute("value"),
                "default seo static page h1 admin");

            //check client
            GoToClient("pages/contacts");
            VerifyAreEqual("Мой магазин - Контакты", Driver.Title, "default seo static page title client");
            VerifyAreEqual("Мой магазин - Контакты",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo static page keywords client");
            VerifyAreEqual("Мой магазин - Контакты",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo static page description client");
            VerifyAreEqual("Мой магазин - Контакты", Driver.FindElement(By.TagName("h1")).Text,
                "default seo static page h1 client");
        }

        [Test]
        public void DefaultSeoStaticPageReset()
        {
            //admin set meta for static page
            GoToAdmin("staticpages/edit/131");
            Driver.ScrollTo(By.Id("URL"));
            if (Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
            }

            Driver.WaitForElem(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("StaticPage_Title");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("StaticPage_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("StaticPage_SeoDescription");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("StaticPage_H1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(2000);

            //pre check client
            GoToClient("pages/shipping");
            VerifyAreEqual("StaticPage_Title", Driver.Title, "pre check seo static page title client");
            VerifyAreEqual("StaticPage_H1", Driver.FindElement(By.TagName("h1")).Text,
                "pre check seo static page h1 client");
            VerifyAreEqual("StaticPage_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo static page keywords client");
            VerifyAreEqual("StaticPage_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo static page description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("ProductsDefaultAdditionalDescription"));

            Driver.FindElement(By.Id("StaticPageDefaultTitle")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultTitle")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("StaticPageDefaultH1")).Click();
            Driver.FindElement(By.Id("StaticPageDefaultH1")).Clear();
            Driver.FindElement(By.Id("StaticPageDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("StaticPageDefaultMetaDescription"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех статических страниц")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("pages/shipping");
            VerifyAreEqual("1", Driver.Title, "reset seo static page title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo static page keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo static page description client");
            VerifyAreEqual("4", Driver.FindElement(By.TagName("h1")).Text, "reset seo static page h1 client");
        }
    }
}