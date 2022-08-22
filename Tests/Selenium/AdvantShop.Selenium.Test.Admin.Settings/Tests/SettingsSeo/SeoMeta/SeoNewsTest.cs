using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsSeo.SeoMeta
{
    [TestFixture]
    public class SettingsSeoNewsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsProductsPerPage | ClearType.CMS);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsSeo\\Settings.NewsCategory.csv",
                "data\\Admin\\Settings\\SettingsSeo\\Settings.News.csv",
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
        public void DefaultSeoNews()
        {
            GoToAdmin("news/edit/1");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoryNewsDefaultH1"));

            Driver.FindElement(By.Id("NewsDefaultTitle")).Click();
            Driver.FindElement(By.Id("NewsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("NewsDefaultTitle")).SendKeys("New title News");

            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("NewsDefaultMetaKeywords"))
                .SendKeys("New meta keywords 1 News, New meta keywords 2 News, New meta keywords 3 News");

            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).SendKeys("New description News");

            Driver.FindElement(By.Id("NewsDefaultH1")).Click();
            Driver.FindElement(By.Id("NewsDefaultH1")).Clear();
            Driver.FindElement(By.Id("NewsDefaultH1")).SendKeys("New h1 News");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("New title News", Driver.FindElement(By.Id("NewsDefaultTitle")).GetAttribute("value"),
                "default seo news title admin");
            VerifyAreEqual("New meta keywords 1 News, New meta keywords 2 News, New meta keywords 3 News",
                Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo news keywords admin");
            VerifyAreEqual("New description News",
                Driver.FindElement(By.Id("NewsDefaultMetaDescription")).GetAttribute("value"),
                "default seo news description admin");
            VerifyAreEqual("New h1 News", Driver.FindElement(By.Id("NewsDefaultH1")).GetAttribute("value"),
                "default seo news h1 admin");

            //check client
            GoToClient("news/news1");
            VerifyAreEqual("New title News", Driver.Title, "default seo news title client");
            VerifyAreEqual("New meta keywords 1 News, New meta keywords 2 News, New meta keywords 3 News",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo news keywords client");
            VerifyAreEqual("New description News",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo news description client");
            VerifyAreEqual("New h1 News", Driver.FindElement(By.TagName("h1")).Text, "default seo news h1 client");
        }

        [Test]
        public void DefaultSeoNewsVariables()
        {
            //set default meta
            GoToAdmin("news/edit/1");
            Driver.ScrollTo(By.Id("URL"));
            if (!Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
                Driver.ScrollToTop();
                Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
                Thread.Sleep(2000);
            }

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoryNewsDefaultH1"));

            Driver.FindElement(By.Id("NewsDefaultTitle")).Click();
            Driver.FindElement(By.Id("NewsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("NewsDefaultTitle")).SendKeys("#STORE_NAME# - #NEWS_NAME#");

            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).SendKeys("#STORE_NAME# - #NEWS_NAME#");

            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).SendKeys("#STORE_NAME# - #NEWS_NAME#");

            Driver.FindElement(By.Id("NewsDefaultH1")).Click();
            Driver.FindElement(By.Id("NewsDefaultH1")).Clear();
            Driver.FindElement(By.Id("NewsDefaultH1")).SendKeys("#STORE_NAME# - #NEWS_NAME#");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            //check admin
            GoToAdmin("settingsseo");

            VerifyAreEqual("#STORE_NAME# - #NEWS_NAME#",
                Driver.FindElement(By.Id("NewsDefaultTitle")).GetAttribute("value"), "default seo news title admin");
            VerifyAreEqual("#STORE_NAME# - #NEWS_NAME#",
                Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).GetAttribute("value"),
                "default seo news keywords admin");
            VerifyAreEqual("#STORE_NAME# - #NEWS_NAME#",
                Driver.FindElement(By.Id("NewsDefaultMetaDescription")).GetAttribute("value"),
                "default seo news description admin");
            VerifyAreEqual("#STORE_NAME# - #NEWS_NAME#",
                Driver.FindElement(By.Id("NewsDefaultH1")).GetAttribute("value"), "default seo news h1 admin");

            //check client
            GoToClient("news/news1");
            VerifyAreEqual("Мой магазин - Test News 1 title", Driver.Title, "default seo news title client");
            VerifyAreEqual("Мой магазин - Test News 1 title",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "default seo news keywords client");
            VerifyAreEqual("Мой магазин - Test News 1 title",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "default seo news description client");
            VerifyAreEqual("Мой магазин - Test News 1 title", Driver.FindElement(By.TagName("h1")).Text,
                "default seo news h1 client");
        }

        [Test]
        public void DefaultSeoNewsReset()
        {
            //admin set meta for news
            GoToAdmin("news/edit/2");
            Driver.ScrollTo(By.Id("URL"));
            if (Driver.FindElement(By.Id("DefaultMeta")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"DefaultMetaCheckbox\"]")).Click();
            }

            Driver.WaitForElem(By.Id("SeoTitle"));
            Driver.FindElement(By.Id("SeoTitle")).Clear();
            Driver.FindElement(By.Id("SeoTitle")).SendKeys("News_Title");
            Driver.FindElement(By.Id("SeoKeywords")).Clear();
            Driver.FindElement(By.Id("SeoKeywords")).SendKeys("News_SeoKeywords");
            Driver.FindElement(By.Id("SeoDescription")).Clear();
            Driver.FindElement(By.Id("SeoDescription")).SendKeys("News_SeoDescription");
            Driver.FindElement(By.Id("SeoH1")).Clear();
            Driver.FindElement(By.Id("SeoH1")).SendKeys("News_H1");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"btnSave\"]")).Click();
            Thread.Sleep(2000);

            //pre check client
            GoToClient("news/news2");
            VerifyAreEqual("News_Title", Driver.Title, "pre check seo news title client");
            VerifyAreEqual("News_H1", Driver.FindElement(By.TagName("h1")).Text, "pre check seo news h1 client");
            VerifyAreEqual("News_SeoKeywords",
                Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "pre check seo news keywords client");
            VerifyAreEqual("News_SeoDescription",
                Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "pre check seo news description client");

            //reset meta
            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("CategoryNewsDefaultH1"));

            Driver.FindElement(By.Id("NewsDefaultTitle")).Click();
            Driver.FindElement(By.Id("NewsDefaultTitle")).Clear();
            Driver.FindElement(By.Id("NewsDefaultTitle")).SendKeys("1");

            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).Click();
            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).Clear();
            Driver.FindElement(By.Id("NewsDefaultMetaKeywords")).SendKeys("2");

            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).Click();
            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).Clear();
            Driver.FindElement(By.Id("NewsDefaultMetaDescription")).SendKeys("3");

            Driver.FindElement(By.Id("NewsDefaultH1")).Click();
            Driver.FindElement(By.Id("NewsDefaultH1")).Clear();
            Driver.FindElement(By.Id("NewsDefaultH1")).SendKeys("4");

            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"settingsSeoSave\"]")).Click();
            Driver.WaitForToastSuccess();

            GoToAdmin("settingsseo");
            Driver.ScrollTo(By.Id("NewsDefaultMetaDescription"));

            Driver.FindElement(By.LinkText("Сбросить мета информацию для всех новостей")).Click();
            Driver.SwalConfirm();

            //check client
            GoToClient("news/news2");
            VerifyAreEqual("1", Driver.Title, "reset seo news title client");
            VerifyAreEqual("2", Driver.FindElement(By.CssSelector("[name=\"Keywords\"]")).GetAttribute("content"),
                "reset seo news keywords client");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[name=\"Description\"]")).GetAttribute("content"),
                "reset seo news description client");
            VerifyAreEqual("4", Driver.FindElement(By.TagName("h1")).Text, "reset seo news h1 client");
        }
    }
}