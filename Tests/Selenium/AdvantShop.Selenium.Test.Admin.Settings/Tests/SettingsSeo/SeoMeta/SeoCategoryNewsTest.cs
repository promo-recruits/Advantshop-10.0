using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoCategoryNewsTest : BaseSeleniumTest
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
        public void DefaultSeoCategoryNews()
        {
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("StaticPageDefaultMetaDescription"));

            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).SendKeys("New title CategoryNews");

            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).SendKeys(
                "New meta keywords 1 CategoryNews, New meta keywords 2 CategoryNews, New meta keywords 3 CategoryNews");

            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).SendKeys("New description CategoryNews");

            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).SendKeys("New h1 CategoryNews");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title CategoryNews",
                Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).GetAttribute("value"),
                "default seo category news title admin");
            VerifyAreEqual(
                "New meta keywords 1 CategoryNews, New meta keywords 2 CategoryNews, New meta keywords 3 CategoryNews",
                Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo category news keywords admin");
            VerifyAreEqual("New description CategoryNews",
                Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).GetAttribute("value"),
                "default seo category news description admin");
            VerifyAreEqual("New h1 CategoryNews",
                Driver.FindElement(By.Id("CategoryNewsDefaultH1")).GetAttribute("value"),
                "default seo category news h1 admin");

            //check client
            GoToClient("newscategory/common");
            VerifyAreEqual("New title CategoryNews", Driver.Title, "default seo category news title client");
            VerifyAreEqual(
                "New meta keywords 1 CategoryNews, New meta keywords 2 CategoryNews, New meta keywords 3 CategoryNews",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo category news keywords client");
            VerifyAreEqual("New description CategoryNews",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo category news description client");
            VerifyAreEqual("New h1 CategoryNews", Driver.FindElement(By.TagName("h1")).Text,
                "default seo category news h1 client");
        }

        [Test]
        public void DefaultSeoCategoryNewsVariables()
        {
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("StaticPageDefaultMetaDescription"));

            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).SendKeys("#STORE_NAME# - #NEWSCATEGORY_NAME#");

            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).SendKeys("#STORE_NAME# - #NEWSCATEGORY_NAME#");

            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription"))
                .SendKeys("#STORE_NAME# - #NEWSCATEGORY_NAME#");

            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).SendKeys("#STORE_NAME# - #NEWSCATEGORY_NAME#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #NEWSCATEGORY_NAME#",
                Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).GetAttribute("value"),
                "default seo category news title admin");
            VerifyAreEqual("#STORE_NAME# - #NEWSCATEGORY_NAME#",
                Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo category news keywords admin");
            VerifyAreEqual("#STORE_NAME# - #NEWSCATEGORY_NAME#",
                Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).GetAttribute("value"),
                "default seo category news description admin");
            VerifyAreEqual("#STORE_NAME# - #NEWSCATEGORY_NAME#",
                Driver.FindElement(By.Id("CategoryNewsDefaultH1")).GetAttribute("value"),
                "default seo category news h1 admin");

            //check client
            GoToClient("newscategory/computer");
            VerifyAreEqual("Мой магазин - Компьютеры", Driver.Title, "default seo category news title client");
            VerifyAreEqual("Мой магазин - Компьютеры",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo category news keywords client");
            VerifyAreEqual("Мой магазин - Компьютеры",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo category news description client");
            VerifyAreEqual("Мой магазин - Компьютеры", Driver.FindElement(By.TagName("h1")).Text,
                "default seo category news h1 client");
        }

        [Test]
        public void DefaultSeoCategoryNewsReset()
        {
            //admin set meta for category news
            GoToAdmin("newscategory");
            Driver.GetGridCell(2, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Thread.Sleep(2000);
            Driver.WaitForElem(By.CssSelector(".modal-content"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryH1\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryH1\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryH1\"]")).SendKeys("NewCategorySEO H1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryTitle\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryTitle\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryTitle\"]")).SendKeys("NewCategorySEO Title");

            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryMetaKey\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryMetaKey\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryMetaKey\"]"))
                .SendKeys("NewCategorySEO Meta Keywords");

            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryMetaDesc\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryMetaDesc\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"NewsCategoryMetaDesc\"]"))
                .SendKeys("NewCategorySEO Meta Description");

            Driver.FindElement(By.CssSelector("[data-e2e=\"BtnSave\"]")).Click();
            Thread.Sleep(2000);

            //pre check client
            GoToClient("newscategory/fashion");
            VerifyAreEqual("NewCategorySEO Title", Driver.Title, "pre check seo category news title client");
            VerifyAreEqual("NewCategorySEO H1", Driver.FindElement(By.TagName("h1")).Text,
                "pre check seo category news h1 client");
            VerifyAreEqual("NewCategorySEO Meta Keywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo category news keywords client");
            VerifyAreEqual("NewCategorySEO Meta Description",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo category news description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("StaticPageDefaultMetaDescription"));

            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).Click();
            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).Clear();
            Driver.FindElement(By.Id("CategoryNewsDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoryNewsDefaultTitle"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех категорий новостей")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("newscategory/fashion");
            VerifyAreEqual("1", Driver.Title, "reset seo category news title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo category news keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo category news description client");
            VerifyAreEqual("4", Driver.FindElement(By.TagName("h1")).Text, "reset seo category news h1 client");
        }
    }
}