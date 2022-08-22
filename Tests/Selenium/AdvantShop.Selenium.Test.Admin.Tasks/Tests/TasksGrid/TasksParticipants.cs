using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksParticipants : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.ManagerRole.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.ManagerRolesMap.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.Task.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.TaskManager.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Tasks\\ParticipantsProject\\Customers.TaskGroupManagerRole.csv"
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
        public void AddNewTaskGroup()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTaskGroup\"]")).Click();


            VerifyAreEqual("Новый проект", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).GetAttribute("value"),
                "name pop up");
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).GetAttribute("value"),
                "taskGroupSortOrder pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).SendKeys("New Test Group");
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).SendKeys("1");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ProjectIsPrivateCommentsSpan\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                .FindElement(By.XPath("//span[contains(text(), 'testfirstname22 testlastname22')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"] input")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                .FindElement(By.XPath("//span[contains(text(), 'testfirstname11 testlastname11')]")).Click();

            Driver.DropFocus("h2");
            Driver.FindElement(By.CssSelector("[data-e2e=\"ParticipantsProject\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ParticipantsProject\"]"))
                .FindElement(By.XPath("//span[contains(text(), 'Role2')]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ParticipantsProject\"] input")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"ParticipantsProject\"]"))
                .FindElement(By.XPath("//span[contains(text(), 'Role3')]")).Click();
            Driver.DropFocus("h2");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]")).Text
                    .Contains("testfirstname11 testlastname11"), "Observer 1 ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]")).Text
                    .Contains("testfirstname22 testlastname22"), "Observer 2 ");

            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ParticipantsProject\"]")).Text.Contains("Role3"),
                "Participants 1 ");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ParticipantsProject\"]")).Text.Contains("Role2"),
                "Participants 2 ");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupButtonSave\"]")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, "display pop-up");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".toast-message")).Displayed, "display toast-container");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                .FindElement(By.CssSelector(".close.ui-select-match-close")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupButtonSave\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 0, "no display pop-up");

            Driver.GridFilterSendKeys("New Test");

            VerifyAreEqual("New Test Group", Driver.GetGridCell(0, "Name").Text, "new name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text, "new sort");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'New Test Group')]")).Displayed,
                "Displayed task group in header");

            GoToAdmin("tasks");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            VerifyIsTrue(selectElem.Text.Contains("New Test Group"), "new group in new tasks ");
        }

        [Test]
        public void AddNewTasks()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            Driver.GridFilterSendKeys("New Test Group");

            Driver.GetGridCell(0, "Name").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).SendKeys("NewTestTask");

            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).SendKeys("12.12.2020 13:40");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();
            //Despription
            Driver.SetCkText("Description NewTestTask", "editor1");
            //Check Group
            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("New Test Group"), "new group in new tasks ");

            //Assigned   ui-select-choices
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname6 testlastname6"),
                "user in ui-select 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname10 testlastname10"), "user in ui-select 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname13 testlastname13"), "user in ui-select 3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname15 testlastname15"), "user in ui-select 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("admin testlastname221"),
                "user in ui-select 5");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname1 testlastname1"),
                "no user in ui-select 1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname5 testlastname5"),
                "no user in ui-select 2");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname16 testlastname16"), "no user in ui-select 3");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname22 testlastname22"), "no user in ui-select 4");
            Driver.FindElement(By.XPath("//div[contains(text(), 'testfirstname6 testlastname6')]")).Click();


            //Priority
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskPriority\"]"))))
                .SelectByText("Высокий");


            Driver.FindElement(By.CssSelector("[data-e2e=\"TaskAdd\"]")).Click();

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("NewTestTask"), "new name");
            VerifyAreEqual("Высокий", (Driver.GetGridCellElement(0, "PriorityFormatted", by: By.TagName("div")).Text),
                "new name");
            VerifyAreEqual("12.12.2020 13:40",
                (Driver.GetGridCellElement(0, "DueDateFormatted", by: By.TagName("div")).Text), "new date");
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text),
                "new status");
            VerifyAreEqual("testfirstname6 testlastname6",
                (Driver.GetGridCellElement(0, "Managers", by: By.TagName("div")).Text), "new assigned");
            VerifyAreEqual("admin testlastname221",
                (Driver.GetGridCellElement(0, "AppointedName", by: By.TagName("div")).Text), "new appointed");

            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".nav-item.active .leads-count-label")).Text,
                "count my task");
        }

        [Test]
        public void AddNewTasksChangeProject()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            Driver.GridFilterSendKeys("New Test Group");

            Driver.GetGridCell(0, "Name").Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).SendKeys("ChangeProject ");

            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Clear();
            string str = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy HH:mm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).SendKeys(str);

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();
            //Despription
            Driver.SetCkText("Description NewTestTask", "editor1");
            //Check Group
            IWebElement selectElem1 = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            SelectElement select3 = new SelectElement(selectElem1);
            VerifyIsTrue(select3.SelectedOption.Text.Contains("New Test Group"), "new group in new tasks ");

            //Assigned   ui-select-choices
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname6 testlastname6"),
                "user in ui-select 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname10 testlastname10"), "user in ui-select 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname13 testlastname13"), "user in ui-select 3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname15 testlastname15"), "user in ui-select 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("admin testlastname221"),
                "user in ui-select 5");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname1 testlastname1"),
                "no user in ui-select 1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname5 testlastname5"),
                "no user in ui-select 2");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname16 testlastname16"), "no user in ui-select 3");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname22 testlastname22"), "no user in ui-select 4");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"))))
                .SelectByText("group1");


            //Assigned   ui-select-choices
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname1 testlastname1"),
                "ChangeProject user in ui-select 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname10 testlastname10"), "ChangeProject user in ui-select 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname5 testlastname5"),
                "ChangeProject user in ui-select 3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("admin testlastname221"),
                "ChangeProject user in ui-select 4");

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname11 testlastname11"), "ChangeProject no user in ui-select 1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname16 testlastname16"), "ChangeProject no user in ui-select 2");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname22 testlastname22"), "ChangeProject no user in ui-select 3");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"))))
                .SelectByText("group2");


            //Assigned   ui-select-choices
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname1 testlastname1"),
                "ChangeProject 1 user in ui-select 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname5 testlastname5"),
                "ChangeProject 1 user in ui-select 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname10 testlastname10"), "ChangeProject 1 user in ui-select 3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname20 testlastname20"), "ChangeProject 1 user in ui-select 4");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname25 testlastname25"), "ChangeProject 1 user in ui-select 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("admin testlastname221"),
                "ChangeProject 1 user in ui-select 6");

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"))))
                .SelectByText("group1");
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text
                    .Contains("testfirstname11 testlastname11"), "ChangeProject 2 no user in ui-select 1");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            Driver.FindElement(By.XPath("//div[contains(text(), 'testfirstname1 testlastname1')]")).Click();

            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]")))).SelectByText(
                "New Test Group");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".toast-message")).Displayed, "display toast-container");
            VerifyIsTrue(Driver.PageSource.Contains("Менеджер testfirstname1 testlastname1 не имеет доступа к проекту"),
                "toster");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("testfirstname2 testlastname2"),
                "ChangeProject 3 no user in ui-select 1");


            Functions.CloseModalTask(Driver);

            Refresh();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("NewTestTask"), "new name");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".nav-item.active .leads-count-label")).Text,
                "count my task");
        }

        [Test]
        public void TaskGroupChangeUserRole()
        {
            GoToAdmin("settings/userssettings");

            Driver.GridFilterSendKeys("testlastname2");

            Driver.GetGridCell(0, "FullName", "Users").Click();

            VerifyAreEqual("Редактирование сотрудника", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"userCity\"]"));
            Driver.FindElement(By.CssSelector(".close.ui-select-match-close")).Click();

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".error-color")).Displayed, "error mes");
            VerifyIsTrue(
                Driver.PageSource.Contains(
                    "Сотрудник является участником группы group1, но не имеет нужной роли (Role1, Role2)"),
                "error text");

            Driver.FindElement(By.CssSelector("[data-e2e=\"userButtonSave\"]")).Click();

            ReInitClient();
            GoToAdmin("projects/1");
            Driver.FindElement(By.Id("txtLogin")).Clear();
            Driver.FindElement(By.Id("txtLogin")).SendKeys("testmail@mail.ru2");
            Driver.FindElement(By.Id("txtPassword")).Clear();
            Driver.FindElement(By.Id("txtPassword")).SendKeys("123123");
            Driver.FindElement(By.CssSelector(".btn-auth")).Click();

            VerifyAreEqual("Все задачи", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text, "h1");

            Refresh();
            MouseTab();
            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(), 'group1')]")).Count == 0,
                "No Displayed task group in header");

            GoToAdmin("tasks");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            VerifyIsFalse(selectElem.Text.Contains("group1"), " no new group in new tasks ");

            GoToAdmin("tasks");
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnTaskGroups");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"] span")).Click();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"] .ui-select-choices-group")).Text
                    .Contains("group1"), " no new group in new filter ");

            ReInit();
        }

        [Test]
        public void TaskGroupUserAdmin()
        {
            ReInit();

            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            VerifyAreEqual("Проекты", Driver.FindElement(By.CssSelector("[data-e2e=\"TaskGroupTitle\"]")).Text, "h1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['_serviceColumn']\"]")).Displayed,
                " Displayed task group serviceColumn");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled")
                    .FindElement(By.CssSelector("ui-grid-custom-switch[readonly=\"false\"]")) != null,
                " inplace task group enabled");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['SortOrder']\"]")).Count > 0,
                " inplace task group sort");
            Driver.GridFilterSendKeys("group1");
            VerifyAreEqual("group1", Driver.GetGridCell(0, "Name").Text, "name task group");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "task group list");

            GoToAdmin("tasks");
            MouseTab();
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'group1')]")).Displayed,
                "Displayed task group in header");

            //GoToAdmin("tasks");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            VerifyIsTrue(selectElem.Text.Contains("group1"), " new group in new tasks ");

            GoToAdmin("tasks");
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnTaskGroups");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ui-select-choices-group")).Text.Contains("group1"),
                " new group in filter ");
            Driver.GridFilterSendKeys("test2");
            VerifyAreEqual("test2", Driver.GetGridCell(0, "Name").Text, "name task search");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), " task search");

            GoToAdmin("tasks#?viewed=true&modal=1");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".toast-message")).Count == 0,
                " no display toast-container");
            VerifyIsFalse(Driver.PageSource.Contains("Нет доступа"), " no access deniy");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, "display pop-up");

            GoToAdmin("tasks#?viewed=true&modal=14");
            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            VerifyIsTrue(selectElem.Text.Contains("group1"), " new group in esit tasks ");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDelTask\"]")).Displayed,
                "btn del tasks");

            GoToAdmin("projects/1");
            VerifyAreEqual("group1", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text,
                "h1 hight group");
            VerifyIsFalse(Driver.PageSource.Contains("Нет доступа"), "no access deniy in hight group");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".page-name-block-item-main .fas.fa-pencil-alt")).Displayed,
                " Displayed task hight group pencil");
            VerifyAreEqual("test1", Driver.GetGridCell(0, "Name").Text, "name hight task");
            VerifyIsTrue(
                Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("ui-grid-custom-delete")).Displayed,
                " Displayed hight task del");

            GoToAdmin("projects/2");
            VerifyAreEqual("group2", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text, "h1 group");
            VerifyIsFalse(Driver.PageSource.Contains("Нет доступа"), "no access deniy in open group");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".page-name-block-item-main .fas.fa-pencil-alt")).Displayed,
                " Displayed task group pencil");
            VerifyAreEqual("test14", Driver.GetGridCell(0, "Name").Text, "name task");
            VerifyIsTrue(
                Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("ui-grid-custom-delete")).Displayed,
                " Displayed task del");
        }

        [Test]
        public void TaskGroupUserManager()
        {
            ReInitClient();
            Functions.LogCustomer(Driver, BaseUrl, "cfc2c33b-1e84-415e-8482-e98156341620",
                "AC40DF3733F39BDB2EE6B014A25470B2A618ECC6CC0D51E9DB35B5C556CC8E13C44021A0B6E3997BC59C009ACC027D7AD14F3A14D8CE8E8DFCCDEF72B7A28C5050BBE54FC405E15FA9AD156EB733E6E5ADB96E95C8696B0CF2CB5F5FA433E84B339744F3C1F5CE8573548A4AE82A72D5519C1C821A0883DD6631A0696B36E176E8AE0F0E9A1D9AC007F89AEC4BD9076793F2511C58966323658EB6D975D31612FA787F15");
            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            VerifyAreEqual("Проекты", Driver.FindElement(By.CssSelector("[data-e2e=\"TaskGroupTitle\"]")).Text, "h1");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['_serviceColumn']\"]")).Count == 0,
                "No Displayed task group serviceColumn");
            VerifyIsTrue(
                Driver.GetGridCell(0, "Enabled")
                    .FindElements(By.CssSelector("ui-grid-custom-switch[readonly=\"false\"]")).Count == 0,
                "no inplace task group enabled");
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e-grid-cell=\"grid[0]['SortOrder']\"] input")).Count == 0,
                "No inplace task group sort");
            Driver.GridFilterSendKeys("group1", byToDropFocus: By.CssSelector(".top-panel"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "No task group list");

            GoToAdmin("tasks");
            MouseTab();
            VerifyIsTrue(Driver.FindElements(By.XPath("//a[contains(text(), 'group1')]")).Count == 0,
                "No Displayed task group in header");

            //GoToAdmin("tasks");
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            VerifyIsFalse(selectElem.Text.Contains("group1"), " no new group in new tasks ");

            GoToAdmin("tasks");
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnTaskGroups");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"] span")).Click();
            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterBlock\"] .ui-select-choices-group")).Text
                    .Contains("group1"), " no new group in new filter ");
            Driver.GridFilterSendKeys("test2", byToDropFocus: By.CssSelector(".top-panel"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "No task search");

            GoToAdmin("tasks#?viewed=true&modal=1", noRefresh: true);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".toast-message")).Displayed, "display toast-container");
            VerifyIsTrue(Driver.PageSource.Contains("Нет доступа"), "access deniy");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog")).Count == 0, "no display pop-up");

            GoToAdmin("tasks#?viewed=true&modal=14");
            selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]"));
            VerifyIsFalse(selectElem.Text.Contains("group1"), " no new group in esit tasks ");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"edittaskDelTask\"]")).Count == 0,
                "No btn del tasks");

            GoToAdmin("projects/2");
            VerifyAreEqual("group2", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text, "h1 group");
            VerifyIsFalse(Driver.PageSource.Contains("Нет доступа"), "no access deniy in open group");
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".sticky-page-name.fas.fa-pencil-alt")).Count == 0,
                "No Displayed task group pencil");
            VerifyAreEqual("test14", Driver.GetGridCell(0, "Name").Text, "name task");
            VerifyIsTrue(
                Driver.GetGridCell(0, "_serviceColumn").FindElements(By.TagName("ui-grid-custom-delete")).Count == 0,
                "No Displayed task del");

            GoToAdmin("projects/1");

            VerifyIsTrue(Driver.PageSource.Contains("У вас нет доступа в данный проект"),
                " access deniy in hidden group");
            VerifyAreEqual("Все задачи", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text,
                "h1 hidden group");

            ReInit();
        }

        public void MouseTab()
        {
            Actions action = new Actions(Driver);
            IWebElement elem = Driver.FindElement(By.CssSelector(".dropdown-toggle.header-bottom-menu-link"))
                .FindElement(By.CssSelector(".fa.fa-angle-down"));
            action.MoveToElement(elem);
            action.Perform();
        }
    }
}