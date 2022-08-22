using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields.CustomerFieldValues
{
    [TestFixture]
    public class SettingsCustomerFieldValuePresentTest : BaseSeleniumTest
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
        public void CustomerFieldValuePresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "present line 1");
            VerifyAreEqual("Value 10", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "present line 10");

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "present line 1");
            VerifyAreEqual("Value 100", Driver.GetGridCell(99, "Value", "CustomerFieldValues").Text,
                "present line 100");

            Driver.GridPaginationSelectItems("10");
        }
    }
}