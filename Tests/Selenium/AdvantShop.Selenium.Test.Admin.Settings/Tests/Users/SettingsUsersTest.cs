using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersTest : BaseSeleniumTest
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
        public void SettingsUsersGrid()
        {
            VerifyAreEqual("Сотрудники",
                Driver.FindElement(By.CssSelector(".tab-pane.active")).FindElement(By.TagName("h2")).Text,
                "h1 settings users page");

            VerifyIsTrue(
                !Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "avatar");

            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text, "FullName");
            VerifyAreEqual("testmail@mail.ru1", Driver.GetGridCell(0, "Email", "Users").Text, "Email");
            VerifyAreEqual("", Driver.GetGridCell(0, "DepartmentName", "Users").Text, "DepartmentName");
            VerifyAreEqual("Role1", Driver.GetGridCell(0, "Roles", "Users").Text, "Roles");
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text, "Permissions");
            VerifyIsTrue(Driver.GetGridCell(0, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled");

            VerifyAreEqual("testfirstname3 testlastname3", Driver.GetGridCell(2, "FullName", "Users").Text,
                "FullName 2");
            VerifyAreEqual("testmail@mail.ru3", Driver.GetGridCell(2, "Email", "Users").Text, "Email 2");
            VerifyAreEqual("Department2", Driver.GetGridCell(2, "DepartmentName", "Users").Text, "DepartmentName 2");
            VerifyAreEqual("", Driver.GetGridCell(2, "Roles", "Users").Text, "Roles 2");
            VerifyAreEqual("Модератор", Driver.GetGridCell(2, "Permissions", "Users").Text, "Permissions 2");
            VerifyIsFalse(Driver.GetGridCell(2, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled 2");

            VerifyAreEqual("Найдено записей: 221",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SettingsUsersInplaceEnabled()
        {
            VerifyIsTrue(!Driver.GetGridCell(1, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled 1");
            Driver.GetGridCell(1, "Enabled", "Users").Click();
            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled 1");

            GoToAdmin("settings/userssettings");

            VerifyIsTrue(Driver.GetGridCell(1, "Enabled", "Users").FindElement(By.TagName("input")).Selected,
                "Enabled 2");

            //   Driver.GetGridCell(1, "Enabled", "Users").Click();
        }

        [Test]
        public void SettingsUsersLinkEmail()
        {
            Driver.GetGridCell(0, "Email", "Users").FindElement(By.TagName("span")).Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Редактирование сотрудника", Driver.FindElement(By.TagName("h2")).Text, "open pop edit up");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();
        }


        [Test]
        public void SettingsUsersLinkName()
        {
            Driver.GetGridCell(0, "FullName", "Users").Click();
            Driver.WaitForElem(By.CssSelector(".modal-header"));

            VerifyAreEqual("Редактирование сотрудника", Driver.FindElement(By.TagName("h2")).Text, "open pop edit up");

            Driver.FindElement(By.XPath("//button[contains(text(), 'Отмена')]")).Click();
        }

        [Test]
        public void SettingsUsersInplaceSortOrder()
        {
            VerifyAreEqual("2", Driver.GetGridCell(1, "SortOrder", "Users").Text, "pre check sort order");
            VerifyAreEqual("testfirstname2 testlastname2", Driver.GetGridCell(1, "FullName", "Users").Text,
                "pre check name");

            Driver.SendKeysGridCell("678", 1, "SortOrder", "Users");

            VerifyAreEqual("678", Driver.GetGridCell(1, "SortOrder", "Users").Text, "inplace edit sort order");

            GoToAdmin("settings/userssettings");

            Driver.GetGridIdFilter("gridUsers", "testfirstname2 testlastname2");
            Driver.XPathContainsText("h1", "Сотрудники");

            VerifyAreEqual("678", Driver.GetGridCell(0, "SortOrder", "Users").Text,
                "inplace edit sort order after refreshing");
            VerifyAreEqual("testfirstname2 testlastname2", Driver.GetGridCell(0, "FullName", "Users").Text,
                "check name");

            //back default
            Driver.SendKeysGridCell("2", 0, "SortOrder", "Users");

        }

        [Test]
        public void SettingsUsersOpenManagerPage()
        {
            GoToAdmin("settings/userssettings");
            if
            (!Driver.FindElement(By.CssSelector("[data-e2e=\"showManagers\"]")).FindElement(By.TagName("input"))
                .Selected)

            {
                Driver.FindElement(By.CssSelector("[data-e2e=\"showManagers\"]")).FindElement(By.TagName("span"))
                    .Click();
                Thread.Sleep(100);
            }

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"showManagers\"]")).FindElement(By.TagName("input"))
                    .Selected, "checkbox show managers selected");

            Driver.FindElement(By.LinkText("Перейти на страницу менеджеров")).Click();
            Driver.WaitForElem(By.ClassName("managers-page"));
            //Driver.WaitForElem(By.XPath("//h1[contains(text(), 'Менеджеры')]"));

            //check client
            VerifyIsTrue(Driver.Url.Contains("managers"), "check managers url");
            VerifyAreEqual("Менеджеры", Driver.FindElement(By.TagName("h1")).Text, "h1 managers page");

            GoToAdmin("settings/userssettings");

            //check admin
            Driver.FindElement(By.CssSelector("[data-e2e=\"showManagers\"]")).FindElement(By.TagName("span")).Click();
            Driver.WaitForElem(By.ClassName("toast-success"));

            VerifyIsFalse(Driver.FindElements(By.LinkText("Перейти на страницу менеджеров")).Count > 0,
                "no link - checkbox show managers not selected");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"showManagers\"]")).FindElement(By.TagName("input"))
                    .Selected, "checkbox show managers not selected");

            //VerifyIsTrue(Is404Page("managers"), "404 page if not show managers");
        }

        [Test]
        public void SettingsUserzSelectDelete()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            //check delete cancel 
            Driver.GetGridCell(1, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("testfirstname2 testlastname2", Driver.GetGridCell(1, "FullName", "Users").Text,
                "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(1, "_serviceColumn", "Users")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("testfirstname3 testlastname3", Driver.GetGridCell(1, "FullName", "Users").Text,
                "1 grid delete");

            //check select 
            Driver.GetGridCell(1, "selectionRowHeaderCol", "Users")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "Users")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol", "Users")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected 2 grid"); //1 admin
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(3, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text, "count selected");
            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");
            VerifyAreEqual("testfirstname6 testlastname6", Driver.GetGridCell(1, "FullName", "Users").Text,
                "selected 2 grid delete");
            VerifyAreEqual("testfirstname7 testlastname7", Driver.GetGridCell(2, "FullName", "Users").Text,
                "selected 3 grid delete");
            VerifyAreEqual("testfirstname8 testlastname8", Driver.GetGridCell(3, "FullName", "Users").Text,
                "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");
            VerifyAreEqual("testfirstname15 testlastname15", Driver.GetGridCell(0, "FullName", "Users").Text,
                "selected all on page 1 grid delete");
            VerifyAreEqual("testfirstname24 testlastname24", Driver.GetGridCell(9, "FullName", "Users").Text,
                "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("207",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            Thread.Sleep(100);
            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol", "Users")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(100);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("admin testlastname221", Driver.GetGridCell(0, "FullName", "Users").Text,
                "delete all except admin");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }
}