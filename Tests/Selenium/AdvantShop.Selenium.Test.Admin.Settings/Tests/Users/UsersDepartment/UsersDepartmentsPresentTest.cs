using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users.UsersDepartment
{
    [TestFixture]
    public class SettingsUsersDepartmentsPresentTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Settings\\Users\\UsersDepartment\\Customers.CustomerGroup.csv",
                "data\\Admin\\Settings\\Users\\UsersDepartment\\Customers.Customer.csv",
                "data\\Admin\\Settings\\Users\\UsersDepartment\\Customers.Departments.csv",
                "data\\Admin\\Settings\\Users\\UsersDepartment\\Customers.Managers.csv",
                "data\\Admin\\Settings\\Users\\UsersDepartment\\Customers.ManagerRole.csv",
                "data\\Admin\\Settings\\Users\\UsersDepartment\\Customers.ManagerRolesMap.csv"
            );

            Init();

            GoToAdmin("settings/userssettings#?tab=Departments");
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
        public void SettingsUsersDepartmentPresent10()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            //  Driver.GridPaginationSelectItems("10", 1);
            Driver.GridPaginationSelectItems("10", "gridDepartments");
            VerifyAreEqual("Department1", Driver.GetGridCell(0, "Name", "Departments").Text, "present line 1");
            VerifyAreEqual("Department10", Driver.GetGridCell(9, "Name", "Departments").Text, "present line 10");
        }

        [Test]
        public void SettingsUsersDepartmentPresent20()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            //Driver.GridPaginationSelectItems("20", 1);
            Driver.GridPaginationSelectItems("20", "gridDepartments");
            VerifyAreEqual("Department1", Driver.GetGridCell(0, "Name", "Departments").Text, "present line 1");
            VerifyAreEqual("Department20", Driver.GetGridCell(19, "Name", "Departments").Text, "present line 20");
        }

        [Test]
        public void SettingsUsersDepartmentPresent50()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            //  Driver.GridPaginationSelectItems("50", 1);
            Driver.GridPaginationSelectItems("50", "gridDepartments");
            VerifyAreEqual("Department1", Driver.GetGridCell(0, "Name", "Departments").Text, "present line 1");
            VerifyAreEqual("Department50", Driver.GetGridCell(49, "Name", "Departments").Text, "present line 50");
        }

        [Test]
        public void SettingsUsersDepartmentPresent100()
        {
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPaginationSelect\"]"));
            //  Driver.GridPaginationSelectItems("100", 1);
            Driver.GridPaginationSelectItems("100", "gridDepartments");
            VerifyAreEqual("Department1", Driver.GetGridCell(0, "Name", "Departments").Text, "present line 1");
            VerifyAreEqual("Department100", Driver.GetGridCell(99, "Name", "Departments").Text, "present line 100");
        }
    }
}