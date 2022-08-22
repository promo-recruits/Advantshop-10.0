using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.BonusSystem.SmsHistory
{
    [TestFixture]
    public class BonusSmsLogPageView : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog | ClearType.Customers | ClearType.Bonuses);
            InitializeService.LoadData(
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.Product.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.Offer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.Category.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Catalog.ProductCategories.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Customers.CustomerGroup.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Customers.Customer.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.Grade.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.Card.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.SmsTemplate.csv",
                "Data\\Admin\\Settings\\BonusSystem\\SmsHistory\\Bonus.SmsLog.csv"
            );
            InitializeService.BonusSystemActive();
            Init();

            GoToAdmin("smstemplates/smslog");
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
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("999999", Driver.GetGridCell(0, "Phone", "log").Text, "page 1 line 1");
            VerifyAreEqual("888888", Driver.GetGridCell(9, "Phone", "log").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            VerifyAreEqual("888888", Driver.GetGridCell(0, "Phone", "log").Text, "page 2 line 1");
            VerifyAreEqual("777777", Driver.GetGridCell(9, "Phone", "log").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            VerifyAreEqual("777777", Driver.GetGridCell(0, "Phone", "log").Text, "page 3 line 1");
            VerifyAreEqual("666666", Driver.GetGridCell(9, "Phone", "log").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            VerifyAreEqual("666666", Driver.GetGridCell(0, "Phone", "log").Text, "page 4 line 1");
            VerifyAreEqual("444444", Driver.GetGridCell(9, "Phone", "log").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("333333", Driver.GetGridCell(0, "Phone", "log").Text, "page 5 line 1");
            VerifyAreEqual("456789", Driver.GetGridCell(9, "Phone", "log").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            VerifyAreEqual("456790", Driver.GetGridCell(0, "Phone", "log").Text, "page 6 line 1");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            VerifyAreEqual("999999", Driver.GetGridCell(0, "Phone", "log").Text, "page 1 line 1");
            VerifyAreEqual("888888", Driver.GetGridCell(9, "Phone", "log").Text, "page 1 line 10");
        }

        [Test]
        public void PageToPrevious()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("999999", Driver.GetGridCell(0, "Phone", "log").Text, "page 1 line 1");
            VerifyAreEqual("888888", Driver.GetGridCell(9, "Phone", "log").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("888888", Driver.GetGridCell(0, "Phone", "log").Text, "page 2 line 1");
            VerifyAreEqual("777777", Driver.GetGridCell(9, "Phone", "log").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            VerifyAreEqual("777777", Driver.GetGridCell(0, "Phone", "log").Text, "page 3 line 1");
            VerifyAreEqual("666666", Driver.GetGridCell(9, "Phone", "log").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("888888", Driver.GetGridCell(0, "Phone", "log").Text, "page 2 line 1");
            VerifyAreEqual("777777", Driver.GetGridCell(9, "Phone", "log").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            VerifyAreEqual("999999", Driver.GetGridCell(0, "Phone", "log").Text, "page 1 line 1");
            VerifyAreEqual("888888", Driver.GetGridCell(9, "Phone", "log").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            VerifyAreEqual("456790", Driver.GetGridCell(0, "Phone", "log").Text, "page last line 1");
        }

        [Test]
        public void LogPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Test Message1", Driver.GetGridCell(0, "Body", "log").Text, "present Body line 1");
            VerifyAreEqual("Test Message10", Driver.GetGridCell(9, "Body", "log").Text, "present Body line 1");
       
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("Test Message1", Driver.GetGridCell(0, "Body", "log").Text, "present Body line 1");
            VerifyAreEqual("Test Message20", Driver.GetGridCell(19, "Body", "log").Text, "present Body line 1");
        }
    }
}