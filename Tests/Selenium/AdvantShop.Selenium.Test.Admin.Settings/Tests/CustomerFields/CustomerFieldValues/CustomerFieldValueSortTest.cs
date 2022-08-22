using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.CustomerFields.CustomerFieldValues
{
    [TestFixture]
    public class SettingsCustomerFieldValueSortTest : BaseSeleniumTest
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
        public void CustomerFieldValueSortName()
        {
            Driver.GetGridCell(-1, "Value", "CustomerFieldValues").Click();
            VerifyAreEqual("Value 1", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "sort Name 1 asc");
            VerifyAreEqual("Value 107", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "sort Name 10 asc");

            Driver.GetGridCell(-1, "Value", "CustomerFieldValues").Click();
            VerifyAreEqual("Value 99", Driver.GetGridCell(0, "Value", "CustomerFieldValues").Text, "sort Name 1 desc");
            VerifyAreEqual("Value 90", Driver.GetGridCell(9, "Value", "CustomerFieldValues").Text, "sort Name 10 desc");
        }

        [Test]
        public void CustomerFieldValueSortSortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "CustomerFieldValues").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "CustomerFieldValues").Text, "sort SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "CustomerFieldValues").Text,
                "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "CustomerFieldValues").Click();
            VerifyAreEqual("140", Driver.GetGridCell(0, "SortOrder", "CustomerFieldValues").Text,
                "sort SortOrder 1 desc");
            VerifyAreEqual("131", Driver.GetGridCell(9, "SortOrder", "CustomerFieldValues").Text,
                "sort SortOrder 10 desc");
        }
    }
}