using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users.UsersDepartment
{
    [TestFixture]
    public class SettingsUsersDepartmentsTest : BaseSeleniumTest
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
        public void SettingsUserDepartmentGrid()
        {
            VerifyAreEqual("Отделы",
                Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Text,
                "h1 settings users departments");

            VerifyAreEqual("Department1", Driver.GetGridCell(0, "Name", "Departments").Text, "Name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "Sort", "Departments").Text, "Sort Order");

            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Departments").FindElement(By.TagName("input")).Selected,
                "Enabled");

            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUserDepartmentInplaceSort()
        {
            //Driver.GetGridFilterTab(1, "Department111");
            Driver.GetGridIdFilter("gridDepartments", "Department111");
            Driver.XPathContainsText("h2", "Отделы");

            VerifyAreEqual("111", Driver.GetGridCell(0, "Sort", "Departments").Text, "before edit");

            Driver.SendKeysGridCell("20", 0, "Sort", "Departments");

            Refresh();

            //  Driver.GetGridFilterTab(1, "Department111");
            Driver.GetGridIdFilter("gridDepartments", "Department111");
            Driver.XPathContainsText("h2", "Отделы");

            VerifyAreEqual("20", Driver.GetGridCell(0, "Sort", "Departments").Text, "edited inplace");

            //back
            Driver.SendKeysGridCell("111", 0, "Sort", "Departments");

            // Driver.GetGridFilterTab(1);
            Driver.GetGridIdFilter("gridDepartments");
        }

        [Test]
        public void SettingsUserDepartmentInplaceEnabled()
        {
            Driver.GetGridCell(0, "Enabled", "Departments").Click();

            VerifyIsTrue(!Driver.GetGridCell(0, "Enabled", "Departments").FindElement(By.TagName("input")).Selected,
                "Enabled 1");

            Refresh();

            VerifyIsTrue(!Driver.GetGridCell(0, "Enabled", "Departments").FindElement(By.TagName("input")).Selected,
                "Enabled 2");
        }
    }
}