using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Search.Tests.Search
{
    [TestFixture]
    public class SettingsSearchPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.SettingsSearch);
            InitializeService.LoadData("data\\Admin\\SettingsSearch\\Settings.SettingsSearch.csv");

            Init();

            GoToAdmin("settingssearch");
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
        public void SettingsSearchPage()
        {
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "page 1 line 1");
            VerifyAreEqual("test title 107", Driver.GetGridCellText(9, "Title"), "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("test title 108", Driver.GetGridCellText(0, "Title"), "page 2 line 1");
            VerifyAreEqual("test title 116", Driver.GetGridCellText(9, "Title"), "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("test title 117", Driver.GetGridCellText(0, "Title"), "page 3 line 1");
            VerifyAreEqual("test title 125", Driver.GetGridCellText(9, "Title"), "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("test title 126", Driver.GetGridCellText(0, "Title"), "page 4 line 1");
            VerifyAreEqual("test title 134", Driver.GetGridCellText(9, "Title"), "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("test title 135", Driver.GetGridCellText(0, "Title"), "page 5 line 1");
            VerifyAreEqual("test title 143", Driver.GetGridCellText(9, "Title"), "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("test title 144", Driver.GetGridCellText(0, "Title"), "page 6 line 1");
            VerifyAreEqual("test title 17", Driver.GetGridCellText(9, "Title"), "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "page 1 line 1");
            VerifyAreEqual("test title 107", Driver.GetGridCellText(9, "Title"), "page 1 line 10");
        }

        [Test]
        public void SettingsSearchPageToPrevious()
        {
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "page 1 line 1");
            VerifyAreEqual("test title 107", Driver.GetGridCellText(9, "Title"), "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test title 108", Driver.GetGridCellText(0, "Title"), "page 2 line 1");
            VerifyAreEqual("test title 116", Driver.GetGridCellText(9, "Title"), "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("test title 117", Driver.GetGridCellText(0, "Title"), "page 3 line 1");
            VerifyAreEqual("test title 125", Driver.GetGridCellText(9, "Title"), "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("test title 108", Driver.GetGridCellText(0, "Title"), "page 2 line 1");
            VerifyAreEqual("test title 116", Driver.GetGridCellText(9, "Title"), "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("test title 1", Driver.GetGridCellText(0, "Title"), "page 1 line 1");
            VerifyAreEqual("test title 107", Driver.GetGridCellText(9, "Title"), "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("test title 90", Driver.GetGridCellText(0, "Title"), "last page line 1");
            VerifyAreEqual("test title 99", Driver.GetGridCellText(9, "Title"), "last page line 10");
        }
    }
}