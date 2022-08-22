using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Settings.Tests.Users
{
    [TestFixture]
    public class SettingsUsersFilterTest : BaseMultiSeleniumTest
    {
        [SetUp]
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

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SettingsUsersFilterImg()
        {
            //check filter img with
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "PhotoSrc", gridId: "gridUsers");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"С фото\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 151",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img with");

            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName filter img with");
            VerifyIsFalse(
                Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "avatar line 1");
            VerifyIsFalse(
                Driver.GetGridCell(9, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "avatar line 9");

            //check filter img without
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Без фото\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 70",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img without");

            VerifyAreEqual("testfirstname151 testlastname151", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName filter img without");
            VerifyIsTrue(
                Driver.GetGridCell(0, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "no avatar line 1");
            VerifyIsTrue(
                Driver.GetGridCell(9, "PhotoSrc", "Users").FindElement(By.TagName("img")).GetAttribute("src")
                    .Contains("no-avatar"), "no avatar line 9");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "PhotoSrc");
            VerifyAreEqual("Найдено записей: 151",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img after deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 151",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img after deleting 2");
        }

        [Test]
        public void SettingsUsersFilterName()
        {
            //check filter full name
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "FullName", gridId: "gridUsers");

            //search by not exist name
            Driver.SetGridFilterValue("FullName", "123123123 name test 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("FullName", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("FullName", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist name
            Driver.SetGridFilterValue("FullName", "testlastname2");

            VerifyAreEqual("testfirstname2 testlastname2", Driver.GetGridCell(0, "FullName", "Users").Text, "FullName");


            VerifyAreEqual("Найдено записей: 33",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter full name");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyAreEqual("admin testlastname221", Driver.GetGridCell(0, "FullName", "Users").Text,
                "delete filtered items except admin");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter full name");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "FullName");
            VerifyAreEqual("Найдено записей: 189",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter full name deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 189",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter full name deleting 2");
        }


        [Test]
        public void SettingsUsersFilterEmail()
        {
            //check filter email
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Email", gridId: "gridUsers");

            //search by not exist email
            Driver.SetGridFilterValue("Email", "123123123 name test 3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("Email", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("Email", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist email
            Driver.SetGridFilterValue("Email", "testmail@mail.ru2");

            VerifyAreEqual("testfirstname2 testlastname2", Driver.GetGridCell(0, "FullName", "Users").Text, "FullName");
            VerifyAreEqual("testmail@mail.ru2", Driver.GetGridCell(0, "Email", "Users").Text, "Email");
            VerifyAreEqual("Найдено записей: 32",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Email");
            VerifyAreEqual("Найдено записей: 189",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 189",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter email deleting 2");
        }

        [Test]
        public void SettingsUsersFilterDepart()
        {
            //check filter department no
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "_noopColumnDepartments", gridId: "gridUsers");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Не указан\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 214",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter department no");

            //check filter department
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Department1\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter department");

            VerifyAreEqual("testfirstname219 testlastname219", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName line 1");
            VerifyAreEqual("testfirstname220 testlastname220", Driver.GetGridCell(1, "FullName", "Users").Text,
                "FullName line 2");

            //check all departments
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 11,
                "count departments");
            Driver.FindElement(By.ClassName("ui-grid-custom-filter-total")).Click();
            Thread.Sleep(1000);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter department");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            //  Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDepartments");
            VerifyAreEqual("Найдено записей: 219",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter department after deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 219",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter department after deleting 2");
        }

        [Test]
        public void SettingsUsersFilterEnabled()
        {
            //check filter enabled
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Enabled", gridId: "gridUsers");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Активные\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 201",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled");

            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName filter enabled 1 ");
            VerifyAreEqual("testfirstname30 testlastname30", Driver.GetGridCell(9, "FullName", "Users").Text,
                "FullName filter enabled 10");

            //check filter disabled
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Неактивные\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 20",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter disabled");

            VerifyAreEqual("testfirstname2 testlastname2", Driver.GetGridCell(0, "FullName", "Users").Text,
                "FullName filter disabled 1");
            VerifyAreEqual("testfirstname11 testlastname11", Driver.GetGridCell(9, "FullName", "Users").Text,
                "FullName filter disabled 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Enabled");
            VerifyAreEqual("Найдено записей: 201",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled after deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 201",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter enabled after deleting 2");
        }

        [Test]
        public void SettingsUsersFilterRoles()
        {
            //check filter img with
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Roles", gridId: "gridUsers");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Role2\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img with");

            VerifyAreEqual("testfirstname219 testlastname219", Driver.GetGridCell(0, "FullName", "Users").Text,
                "filter role2 FullName");
            VerifyAreEqual("Role2", Driver.GetGridCell(0, "Roles", "Users").Text, "filter role2 role name");

            //check filter img without
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Role1\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 2",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img without");

            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "filter role1 FullName1");
            VerifyAreEqual("Role1", Driver.GetGridCell(0, "Roles", "Users").Text, "filter role1 role name1");
            VerifyAreEqual("testfirstname220 testlastname220", Driver.GetGridCell(1, "FullName", "Users").Text,
                "filter role1 FullName2");
            VerifyAreEqual("Role1", Driver.GetGridCell(1, "Roles", "Users").Text, "filter role1 role name2");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Roles");
            VerifyAreEqual("Найдено записей: 219",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img after deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 219",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img after deleting 2");
        }

        [Test]
        public void SettingsUsersFilterPermissions()
        {
            //check filter img with
            Functions.GridFilterTabSet(Driver, BaseUrl, name: "Permissions", gridId: "gridUsers");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Администратор\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img with");

            VerifyAreEqual("testfirstname9 testlastname9", Driver.GetGridCell(0, "FullName", "Users").Text,
                "filter admin FullName1");
            VerifyAreEqual("Администратор", Driver.GetGridCell(0, "Permissions", "Users").Text,
                "filter admin permission name1");
            VerifyAreEqual("testfirstname18 testlastname18", Driver.GetGridCell(9, "FullName", "Users").Text,
                "filter admin FullName10");
            VerifyAreEqual("Администратор", Driver.GetGridCell(9, "Permissions", "Users").Text,
                "filter admin permission name10");

            //check filter img without
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Модератор\"]")).Click();
            Driver.XPathContainsText("h1", "Сотрудники");
            VerifyAreEqual("Найдено записей: 209",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img without");

            VerifyAreEqual("testfirstname1 testlastname1", Driver.GetGridCell(0, "FullName", "Users").Text,
                "filter moderator FullName1");
            VerifyAreEqual("Модератор", Driver.GetGridCell(0, "Permissions", "Users").Text,
                "filter moderator permission name1");
            VerifyAreEqual("testfirstname21 testlastname21", Driver.GetGridCell(9, "FullName", "Users").Text,
                "filter moderator FullName10");
            VerifyAreEqual("Модератор", Driver.GetGridCell(9, "Permissions", "Users").Text,
                "filter moderator permission name10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Thread.Sleep(500);
            Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                .FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Thread.Sleep(500);
            Functions.GridDropdownDelete(Driver, BaseUrl, gridId: "gridUsers");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, "Permissions");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img after deleting 1");

            GoToAdmin("settings/userssettings");
            VerifyAreEqual("Найдено записей: 12",
                Driver.FindElement(By.CssSelector("[grid-unique-id=\"gridUsers\"]"))
                    .FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter img after deleting 2");
        }
    }
}