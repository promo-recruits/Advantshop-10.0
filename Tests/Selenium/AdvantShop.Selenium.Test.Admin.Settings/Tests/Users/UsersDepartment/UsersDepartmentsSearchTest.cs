using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users.UsersDepartment
{
    [TestFixture]
    public class SettingsUsersDepartmentsSearchTest : BaseSeleniumTest
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
        public void SettingsUsersDepartmentSearchExist()
        {
            GoToAdmin("settings/userssettings#?tab=Departments");

            //Driver.GetGridFilterTab(1, "Department111");
            Driver.GetGridIdFilter("gridDepartments", "Department111");
            Driver.XPathContainsText("h2", "Отделы");

            VerifyAreEqual("Department111", Driver.GetGridCell(0, "Name", "Departments").Text, "search exist item");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUsersDepartmentSearchNotExist()
        {
            GoToAdmin("settings/userssettings#?tab=Departments");

            //Driver.GetGridFilterTab(1, "555 Department");
            Driver.GetGridIdFilter("gridDepartments", "555 Department");
            Driver.XPathContainsText("h2", "Отделы");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search not exist");
        }

        [Test]
        public void SettingsUsersDepartmentSearchMuchSymbols()
        {
            GoToAdmin("settings/userssettings#?tab=Departments");

            // Driver.GetGridFilterTab(1, "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.GetGridIdFilter("gridDepartments",
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            Driver.XPathContainsText("h2", "Отделы");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
        }

        [Test]
        public void SettingsUsersDepartmentSearchInvalidSymbols()
        {
            GoToAdmin("settings/userssettings#?tab=Departments");

            //Driver.GetGridFilterTab(1, "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.GetGridIdFilter("gridDepartments", "########@@@@@@@@&&&&&&&******,,,,..");
            Driver.XPathContainsText("h2", "Отделы");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
        }
    }
}