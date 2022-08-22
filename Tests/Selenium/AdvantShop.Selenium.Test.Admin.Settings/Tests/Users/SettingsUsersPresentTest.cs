using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Users\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Users\\Customers.Customer.csv",
                "data\\Admin\\Settings\\Users\\Customers.Departments.csv",
                "data\\Admin\\Settings\\Users\\Customers.Managers.csv",
                "data\\Admin\\Settings\\Users\\Customers.ManagerRole.csv",
                "data\\Admin\\Settings\\Users\\Customers.ManagerRolesMap.csv"
            );

            Init();

            GoToAdmin("settings/userssettings");
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
        public void SettingsUsersPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10", "gridUsers");
            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "present line 1");
            VerifyAreEqual("testfirstname10 testlastname10", Driver.GetGridCell(9, "FullName", "Users").Text,
                "present line 10");

            Driver.GridPaginationSelectItems("20", "gridUsers");
            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "present line 1");
            VerifyAreEqual("testfirstname20 testlastname20", Driver.GetGridCell(19, "FullName", "Users").Text,
                "present line 20");
        }
    }
}