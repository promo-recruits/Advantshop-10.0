using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields.CustomerFieldValues
{
    [TestFixture]
    public class SettingsCustomerFieldValuePageTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.Customer.csv",
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.CustomerField.csv",
                "data\\Admin\\Settings\\CustomerFieldValues\\Customers.CustomerFieldValue.csv"
            );

            Init();

            GoToAdmin("settingscustomers#?tab=customerFields");

            Driver.GetGridCell(0, "HasValues", "CustomerFields").Click();
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
        public void CustomerFieldValuePage()
        {
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 1 line 1");
            VerifyAreEqual("Value 10", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 11", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 2 line 1");
            VerifyAreEqual("Value 20", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 21", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 3 line 1");
            VerifyAreEqual("Value 30", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 31", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 4 line 1");
            VerifyAreEqual("Value 40", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 4 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 41", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 5 line 1");
            VerifyAreEqual("Value 50", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 5 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 51", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 6 line 1");
            VerifyAreEqual("Value 60", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 6 line 10");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 1 line 1");
            VerifyAreEqual("Value 10", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 1 line 10");
        }

        [Test]
        public void CustomerFieldValuePageToPrevious()
        {
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 1 line 1");
            VerifyAreEqual("Value 10", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 1 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 11", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 2 line 1");
            VerifyAreEqual("Value 20", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 21", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 3 line 1");
            VerifyAreEqual("Value 30", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 3 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 11", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 2 line 1");
            VerifyAreEqual("Value 20", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 2 line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "page 1 line 1");
            VerifyAreEqual("Value 10", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "page 1 line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();
            Thread.Sleep(2000);
            VerifyAreEqual("Value 131", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "last page line 1");
            VerifyAreEqual("Value 140", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text,
                "last page line 10");
        }
    }
}