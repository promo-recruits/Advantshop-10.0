using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields
{
    [TestFixture]
    public class SettingsCustomerFieldsPresentTest : BaseSeleniumTest
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
        public void CustomerFieldsPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "present line 1");
            VerifyAreEqual("Customer Field 10", Driver.GetGridCell(9, "Name", "CustomerFields").Text,
                "present line 10");

            Driver.GridPaginationSelectItems("100");
            Driver.WaitForElem(By.CssSelector("[data-e2e-grid-cell=\"gridCustomerFields[99][\'Name\']\"]"));
            VerifyAreEqual("Customer Field 1", Driver.GetGridCell(0, "Name", "CustomerFields").Text, "present line 1");
            VerifyAreEqual("Customer Field 100", Driver.GetGridCell(99, "Name", "CustomerFields").Text,
                "present line 100");

            Driver.GridPaginationSelectItems("10");
        }
    }
}