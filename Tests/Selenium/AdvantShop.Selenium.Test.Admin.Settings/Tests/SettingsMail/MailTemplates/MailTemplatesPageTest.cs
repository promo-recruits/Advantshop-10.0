using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.SettingsMail.MailTemplates
{
    [TestFixture]
    public class SettingsMailTemplatesPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.MailTemplate);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\SettingsMail\\MailTemplates\\Settings.MailTemplate.csv"
            );

            Init();
            GoToAdmin("settingsmail#?notifyTab=templates");
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
        public void Page()
        {
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "page 1 line 1");
            VerifyAreEqual("Template Name Test 10", Driver.GetGridCell(9, "Name", "Templates").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 11", Driver.GetGridCell(0, "Name", "Templates").Text, "page 2 line 1");
            VerifyAreEqual("Template Name Test 20", Driver.GetGridCell(9, "Name", "Templates").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 21", Driver.GetGridCell(0, "Name", "Templates").Text, "page 3 line 1");
            VerifyAreEqual("Template Name Test 30", Driver.GetGridCell(9, "Name", "Templates").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 31", Driver.GetGridCell(0, "Name", "Templates").Text, "page 4 line 1");
            VerifyAreEqual("Template Name Test 40", Driver.GetGridCell(9, "Name", "Templates").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 41", Driver.GetGridCell(0, "Name", "Templates").Text, "page 5 line 1");
            VerifyAreEqual("Template Name Test 50", Driver.GetGridCell(9, "Name", "Templates").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 51", Driver.GetGridCell(0, "Name", "Templates").Text, "page 6 line 1");
            VerifyAreEqual("Template Name Test 60", Driver.GetGridCell(9, "Name", "Templates").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "page 1 line 1");
            VerifyAreEqual("Template Name Test 10", Driver.GetGridCell(9, "Name", "Templates").Text, "page 1 line 10");
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "page 1 line 1");
            VerifyAreEqual("Template Name Test 10", Driver.GetGridCell(9, "Name", "Templates").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 11", Driver.GetGridCell(0, "Name", "Templates").Text, "page 2 line 1");
            VerifyAreEqual("Template Name Test 20", Driver.GetGridCell(9, "Name", "Templates").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 21", Driver.GetGridCell(0, "Name", "Templates").Text, "page 3 line 1");
            VerifyAreEqual("Template Name Test 30", Driver.GetGridCell(9, "Name", "Templates").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 11", Driver.GetGridCell(0, "Name", "Templates").Text, "page 2 line 1");
            VerifyAreEqual("Template Name Test 20", Driver.GetGridCell(9, "Name", "Templates").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 1", Driver.GetGridCell(0, "Name", "Templates").Text, "page 1 line 1");
            VerifyAreEqual("Template Name Test 10", Driver.GetGridCell(9, "Name", "Templates").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridTemplates\"]"))
                .FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Template Name Test 101", Driver.GetGridCell(0, "Name", "Templates").Text,
                "last page line 1");
            VerifyAreEqual("Template Name Test 107", Driver.GetGridCell(6, "Name", "Templates").Text,
                "last page line 7");
        }
    }
}