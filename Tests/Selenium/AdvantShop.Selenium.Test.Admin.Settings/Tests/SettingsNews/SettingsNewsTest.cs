using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsNews
{
    [TestFixture]
    public class SettingsNewsTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.CMS | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\News\\Settings.NewsCategory.csv",
                "data\\Admin\\Settings\\News\\Settings.News.csv",
                "data\\Admin\\Settings\\News\\Catalog.Product.csv",
                "data\\Admin\\Settings\\News\\Catalog.Offer.csv",
                "data\\Admin\\Settings\\News\\Catalog.Category.csv",
                "data\\Admin\\Settings\\News\\Catalog.ProductCategories.csv"
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
        public void NewsOnMainpageSettingsNews()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            Driver.FindElement(By.Id("NewsMainPageCount")).Clear();
            Driver.FindElement(By.Id("NewsMainPageCount")).SendKeys("6");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".news-block-row")).Count == 6, "news per page 1");

            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            Driver.FindElement(By.Id("NewsMainPageCount")).Clear();
            Driver.FindElement(By.Id("NewsMainPageCount")).SendKeys("3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();

            GoToClient();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".news-block-row")).Count == 3, "news per page 2");
        }

        [Test]
        public void NewsPerPageSettingsNews()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            Driver.FindElement(By.Id("NewsPerPage")).Clear();
            Driver.FindElement(By.Id("NewsPerPage")).SendKeys("5");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient("news");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".row.news-list-item")).Count == 5, "news per page 1");

            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            Driver.FindElement(By.Id("NewsPerPage")).Clear();
            Driver.FindElement(By.Id("NewsPerPage")).SendKeys("2");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();

            GoToClient("news");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".row.news-list-item")).Count == 2, "news per page 2");
        }

        [Test]
        public void NewsRssSettingsNews()
        {
            GoToAdmin("news#?newsTab=news");
            if (!Driver.FindElement(By.CssSelector("[data-e2e=\"RssViewNews\"] input")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"RssViewNews\"] span")).Click();
                Thread.Sleep(2000);
            }

            GoToClient("/news/rss");
            VerifyIsFalse(Is404Page("/news/rss"), "NewsRssSettingsNews");

            GoToAdmin("news#?newsTab=news");
            if (Driver.FindElement(By.CssSelector("[data-e2e=\"RssViewNews\"] input")).Selected)
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"RssViewNews\"] span")).Click();
                Thread.Sleep(2000);
            }
            //driver.FindElement(By.CssSelector("[data-e2e=\"SaveNews\"]")).Click();

            GoToClient("/news/rss");
            VerifyIsTrue(Is404Page("/news/rss"), "NewsRssSettingsNews");
        }

        [Test]
        public void OpenPageSettingsNews()
        {
            GoToAdmin("news#?newsTab=news");
            VerifyAreEqual("Новости", Driver.FindElement(By.CssSelector("[data-e2e=\"titleNewsH1\"]")).Text,
                "open page h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"RssViewNews\"]")).Displayed, "rss field");

            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            VerifyIsTrue(Driver.FindElement(By.Id("NewsPerPage")).Displayed, "count field");
            VerifyIsTrue(Driver.FindElement(By.Id("MainPageText")).Displayed, "title field");
            VerifyIsTrue(Driver.FindElement(By.Id("NewsMainPageCount")).Displayed, "count on mainpage field");
        }

        [Test]
        public void TitleSettingsNews()
        {
            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            Driver.FindElement(By.Id("MainPageText")).Clear();
            Driver.FindElement(By.Id("MainPageText")).SendKeys("Test MainPageText");
            try
            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();
                Driver.WaitForToastSuccess();
            }
            catch
            {
            }

            GoToClient();
            VerifyAreEqual("Test MainPageText", Driver.FindElement(By.CssSelector(".subscribe-block-text")).Text,
                "mainpage subscribe");
            GoToClient("news");
            VerifyAreEqual("Test MainPageText", Driver.FindElement(By.CssSelector(".subscribe-block-text")).Text,
                "news page subscribe");

            GoToAdmin("settingstemplate#?settingsTemplateTab=news");
            Driver.FindElement(By.Id("MainPageText")).Clear();
            Driver.FindElement(By.Id("MainPageText")).SendKeys("New MainPageText");

            Driver.FindElement(By.CssSelector("[data-e2e=\"SettingsTemplateSave\"]")).Click();

            GoToClient();
            VerifyAreEqual("New MainPageText", Driver.FindElement(By.CssSelector(".subscribe-block-text")).Text,
                "mainpage subscribe 2");
            GoToClient("news");
            VerifyAreEqual("New MainPageText", Driver.FindElement(By.CssSelector(".subscribe-block-text")).Text,
                "news page subscribe 2");
        }
    }
}