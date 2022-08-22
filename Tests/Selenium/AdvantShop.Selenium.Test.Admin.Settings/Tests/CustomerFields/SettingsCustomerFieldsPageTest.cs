using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsPageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.Customer.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\CustomerFields\\Customers.CustomerFieldValue.csv"
            );

            Init();

            GoToAdmin("settingscustomers#?tab=customerFields");
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
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 1 line 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 11", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 2 line 1");
            VerifyAreEqual("Customer Field 20", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 21", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 3 line 1");
            VerifyAreEqual("Customer Field 30", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 31", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 4 line 1");
            VerifyAreEqual("Customer Field 40", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 41", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 5 line 1");
            VerifyAreEqual("Customer Field 50", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 51", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 6 line 1");
            VerifyAreEqual("Customer Field 60", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 1 line 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 1 line 10");
        }

        [Test]
        public void PageToPrevious()
        {
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 1 line 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 11", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 2 line 1");
            VerifyAreEqual("Customer Field 20", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 21", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 3 line 1");
            VerifyAreEqual("Customer Field 30", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 11", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 2 line 1");
            VerifyAreEqual("Customer Field 20", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "page 1 line 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Customer Field 141", Driver.GetGridCell(0, "Name", "CustomerFields").Text,
                "last page line 1");
            VerifyAreEqual("Customer Field 150", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "last page line 10");
        }
    }
}