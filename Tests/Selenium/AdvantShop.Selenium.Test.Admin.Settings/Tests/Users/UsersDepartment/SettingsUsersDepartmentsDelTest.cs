using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users.UsersDepartment
{
    [TestFixture]
    public class SettingsUsersDepartmentsDelTest : BaseSeleniumTest
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
        public void SelectDelete()
        {
            //check delete cancel 
            Driver.GetGridCell(0, "_serviceColumn", "Departments").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalCancel();
            VerifyAreEqual("Department1", Driver.GetGridCell(0, "Name", "Departments").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn", "Departments").FindElement(By.TagName("ui-grid-custom-delete"))
                .Click();
            Driver.SwalConfirm();
            VerifyAreEqual("Department2", Driver.GetGridCell(0, "Name", "Departments").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol", "Departments")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Departments")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Departments")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");

            //check delete selected items
            // FilterTabDelete(1);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridDepartments");
            VerifyAreEqual("Department5", Driver.GetGridCell(0, "Name", "Departments").Text, "selected 1 grid delete");
            VerifyAreEqual("Department6", Driver.GetGridCell(1, "Name", "Departments").Text, "selected 2 grid delete");
            VerifyAreEqual("Department7", Driver.GetGridCell(2, "Name", "Departments").Text, "selected 3 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            // FilterTabDelete(1);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridDepartments");
            VerifyAreEqual("Department15", Driver.GetGridCell(0, "Name", "Departments").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("Department24", Driver.GetGridCell(9, "Name", "Departments").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("106",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Departments")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            //FilterTabDelete(1);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridDepartments");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");

            GoToAdmin("settings/userssettings#?tab=Departments");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all 2");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridDepartments\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting 2");
        }
    }
}