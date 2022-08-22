using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersSortTest : BaseSeleniumTest
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
        public void SettingsUsersSortFullName()
        {
            Driver.GetGridCell(-1, "FullName", "Users").Click();
            VerifyAreEqual("admin testlastname221", Driver.GetGridCell(0, "FullName", "Users").Text,
                "sort FullName 1 asc");
            VerifyAreEqual("testfirstname106 testlastname106", Driver.GetGridCell(9, "FullName", "Users").Text,
                "sort FullName 10 asc");

            Driver.GetGridCell(-1, "FullName", "Users").Click();
            VerifyAreEqual("testfirstname99 testlastname99", Driver.GetGridCell(0, "FullName", "Users").Text,
                "sort FullName 1 desc");
            VerifyAreEqual("testfirstname90 testlastname90", Driver.GetGridCell(9, "FullName", "Users").Text,
                "sort FullName 10 desc");
        }

        [Test]
        public void SettingsUsersSortEmail()
        {
            Driver.GetGridCell(-1, "Email", "Users").Click();
            VerifyAreEqual("admin", Driver.GetGridCell(0, "Email", "Users").Text, "sort Email 1 asc");
            VerifyAreEqual("testmail@mail.ru106", Driver.GetGridCell(9, "Email", "Users").Text, "sort Email 10 asc");

            Driver.GetGridCell(-1, "Email", "Users").Click();
            VerifyAreEqual("testmail@mail.ru99", Driver.GetGridCell(0, "Email", "Users").Text, "sort Email 1 desc");
            VerifyAreEqual("testmail@mail.ru90", Driver.GetGridCell(9, "Email", "Users").Text, "sort Email 10 desc");
        }

        [Test]
        public void SettingsUsersSortDepartmentName()
        {
            Driver.GetGridCell(-1, "DepartmentName", "Users").Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "DepartmentName", "Users").Text, "sort DepartmentName 1 asc");
            VerifyAreEqual("", Driver.GetGridCell(9, "DepartmentName", "Users").Text, "sort DepartmentName 10 asc");

            Driver.GetGridCell(-1, "DepartmentName", "Users").Click();
            VerifyAreEqual("Department6", Driver.GetGridCell(0, "DepartmentName", "Users").Text,
                "sort DepartmentName 1 desc");
            VerifyAreEqual("Department5", Driver.GetGridCell(1, "DepartmentName", "Users").Text,
                "sort DepartmentName 2 desc");
            VerifyAreEqual("", Driver.GetGridCell(9, "DepartmentName", "Users").Text, "sort DepartmentName 10 asc");
        }


        [Test]
        public void SettingsUsersSortEnabled()
        {
            Driver.GetGridCell(-1, "Enabled", "Users").Click();
            VerifyIsTrue(!Driver.GetGridCell(0, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "sort Enabled 1 asc");
            VerifyIsTrue(!Driver.GetGridCell(9, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "sort Enabled 10 asc");

            Driver.GetGridCell(-1, "Enabled", "Users").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "sort Enabled 1 desc");
            VerifyIsTrue(Driver.GetGridCell(9, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "sort Enabled 10 desc");
        }

        [Test]
        public void SettingsUsersSortSortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder", "Users").Click();
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder", "Users").Text, "sort SortOrder 1 asc");
            VerifyAreEqual("10", Driver.GetGridCell(9, "SortOrder", "Users").Text, "sort SortOrder 10 asc");

            Driver.GetGridCell(-1, "SortOrder", "Users").Click();
            VerifyAreEqual("221", Driver.GetGridCell(0, "SortOrder", "Users").Text, "sort SortOrder 1 desc");
            VerifyAreEqual("212", Driver.GetGridCell(9, "SortOrder", "Users").Text, "sort SortOrder 10 desc");
        }

        [Test]
        public void SettingsUsersSortRoles()
        {
            Driver.GetGridCell(-1, "Roles", "Users").Click();
            VerifyAreEqual("", Driver.GetGridCell(0, "Roles", "Users").Text, "sort Roles 1 asc");
            VerifyAreEqual("", Driver.GetGridCell(1, "Roles", "Users").Text, "sort Roles 2 asc");
            VerifyAreEqual("", Driver.GetGridCell(2, "Roles", "Users").Text, "sort Roles 3 asc");
            VerifyAreEqual("", Driver.GetGridCell(9, "Roles", "Users").Text, "sort Roles 10 asc");

            Driver.GetGridCell(-1, "Roles", "Users").Click();
            VerifyAreEqual("Role5", Driver.GetGridCell(0, "Roles", "Users").Text, "sort Roles 1 desc");
            VerifyAreEqual("Role4", Driver.GetGridCell(1, "Roles", "Users").Text, "sort Roles 2 desc");
            VerifyAreEqual("Role3", Driver.GetGridCell(2, "Roles", "Users").Text, "sort Roles 3 desc");
            VerifyAreEqual("", Driver.GetGridCell(9, "Roles", "Users").Text, "sort Roles 10 desc");
        }

        [Test]
        public void SettingsUsersSortPermissions()
        {
            Driver.GetGridCell(-1, "Permissions", "Users").Click();
            VerifyAreEqual("Администратор", Driver.GetGridCell(0, "Permissions", "Users").Text,
                "sort Permissions 1 asc");
            VerifyAreEqual("Администратор", Driver.GetGridCell(9, "Permissions", "Users").Text,
                "sort Permissions 10 asc");

            Driver.GetGridCell(-1, "Permissions", "Users").Click();
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text, "sort Permissions 1 desc");
            VerifyAreEqual("Модератор", Driver.GetGridCell(9, "Permissions", "Users").Text, "sort Permissions 10 desc");
        }
    }
}