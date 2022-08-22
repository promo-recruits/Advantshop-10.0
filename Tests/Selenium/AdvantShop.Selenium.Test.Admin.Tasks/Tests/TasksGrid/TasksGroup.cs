using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksGroup : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.Task.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.TaskManager.csv"
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
        public void TaskGroupDisplayed()
        {
            GoToAdmin("tasks");

            VerifyIsFalse(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 1')]")).Displayed,
                "No Displayed task group 1");
            VerifyIsFalse(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 2')]")).Displayed,
                "No Displayed task group 2");
            VerifyIsFalse(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 3')]")).Displayed,
                "No Displayed task group 3");
            VerifyIsFalse(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 4')]")).Displayed,
                "No Displayed task group 4");
            VerifyIsFalse(Driver.FindElement(By.XPath("//a[contains(text(), 'Все проекты')]")).Displayed,
                "No Displayed all group");
            VerifyIsFalse(Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить проект')]")).Displayed,
                "No Displayed new project");

            MouseTab();

            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 1')]")).Displayed,
                "Displayed task group 1");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 2')]")).Displayed,
                "Displayed task group 2");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 3')]")).Displayed,
                "Displayed task group 3");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'test group 4')]")).Displayed,
                "Displayed task group 4");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'Все проекты')]")).Displayed,
                "Displayed all group");
            VerifyIsTrue(Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить проект')]")).Displayed,
                "Displayed new project");
        }

        [Test]
        public void TaskGroupCheck()
        {
            GoToAdmin("tasks");

            MouseTab();
            Driver.FindElement(By.XPath("//a[contains(text(), 'test group 1')]")).Click();

            VerifyAreEqual("test group 1", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text, "h1");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);

            var taskNavbarText = Driver.FindElement(By.CssSelector(".tasks-navbar")).Text;
            VerifyIsTrue(taskNavbarText.Contains("Мои задачи\r\n8"),
                "contain my task");
            VerifyIsTrue(taskNavbarText.Contains("Поручил\r\n10"),
                "contain appointed");
            VerifyIsTrue(taskNavbarText.Contains("Завершенные\r\n4"),
                "contain completed");
            VerifyIsTrue(taskNavbarText.Contains("Принятые\r\n1"),
                "contain accepted");
        }

        [Test]
        public void TasksGroupDel()
        {
            GoToAdmin("projects/1");
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text, "count ");
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("9", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);

            var taskNavbarText = Driver.FindElement(By.CssSelector(".tasks-navbar")).Text;
            VerifyIsTrue(taskNavbarText.Contains("Мои задачи\r\n7"),
                "contain my task");
            VerifyIsTrue(taskNavbarText.Contains("Поручил\r\n9"),
                "contain appointed");
            VerifyIsTrue(taskNavbarText.Contains("Завершенные\r\n4"),
                "contain completed");
            VerifyIsTrue(taskNavbarText.Contains("Принятые\r\n1"),
                "contain accepted");
            VerifyIsTrue(Driver.GetGridCellText(0, "Name").Contains("test2"));
        }

        [Test]
        public void TaskGroupDisabled()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            VerifyIsFalse(
                Driver.GetGridCellElement(4, "Enabled", by: By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .Selected, "task group disabled");
            VerifyAreEqual("test group 5", Driver.GetGridCell(4, "Name").Text, "task group disabled name");

            GoToAdmin("tasks");
            MouseTab();
            VerifyIsFalse(Driver.FindElement(By.CssSelector(".submenu-initialized")).Text.Contains("test group 5"),
                "no disabled task group in menu");

            Driver.FindElement(By.CssSelector(".search-input")).Click();


            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnTaskGroups");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            VerifyIsFalse(Driver.FindElement(By.CssSelector(".ui-select-choices")).Text.Contains("test group 5"),
                "no disabled task group in tasks filter");

            GoToAdmin("settingstasks");
            Driver.ScrollTo(By.CssSelector("[data-e2e-settings-task-rule=\"MessageReply\"]"));
            Driver.FindElement(By.CssSelector("[data-e2e-settings-task-rule=\"TaskCreated\"]")).Click();
            Driver.WaitForElem(By.Name("addEditRuleForm"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleFilterAdd\"]")).Click();

            VerifyIsFalse(
                Driver.FindElement(By.CssSelector("[data-e2e=\"BizRuleParam\"]")).Text.Contains("test group 5"),
                "no disabled task group in settings tasks rules");
        }

        public void MouseTab()
        {
            Actions action = new Actions(Driver);
            IWebElement elem = Driver.FindElement(By.CssSelector(".dropdown-toggle.header-bottom-menu-link"))
                .FindElement(By.CssSelector(".fa-angle-down"));
            action.MoveToElement(elem);
            action.Perform();
        }
    }

    [TestFixture]
    public class TasksGroupSort : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Task.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskManager.csv"
            );
            Init();
            GoToAdmin("settingstasks#?tasksTab=taskGroups");
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
        public void ByProject()
        {
            Driver.GetGridCell(-1, "Name").Click();

            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "sort Name asc 1");
            VerifyAreEqual("test group 16", Driver.GetGridCell(9, "Name").Text, "sort Name asc 10");

            Driver.GetGridCell(-1, "Name").Click();

            VerifyAreEqual("test group 99", Driver.GetGridCell(0, "Name").Text, "sort Name desc 1");
            VerifyAreEqual("test group 90", Driver.GetGridCell(9, "Name").Text, "sort Name desc 10");
        }

        [Test]
        public void ByEnabled()
        {
            Driver.GetGridCell(-1, "Enabled").Click();

            VerifyAreEqual("test group 90", Driver.GetGridCell(0, "Name").Text, "sort Enabled asc 1");
            VerifyAreEqual("test group 99", Driver.GetGridCell(9, "Name").Text, "sort Enabled asc 10");

            Driver.GetGridCell(-1, "Enabled").Click();

            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "sort Enabled desc 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "sort Enabled desc 10");
        }

        [Test]
        public void BySortOrder()
        {
            Driver.GetGridCell(-1, "SortOrder").Click();

            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "sort SortOrder asc 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "sort SortOrder asc 10");

            Driver.GetGridCell(-1, "SortOrder").Click();

            VerifyAreEqual("test group 101", Driver.GetGridCell(0, "Name").Text, "sort SortOrder desc 1");
            VerifyAreEqual("test group 92", Driver.GetGridCell(9, "Name").Text, "sort SortOrder desc 10");
        }
    }

    [TestFixture]
    public class TasksGroupSearch : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Task.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskManager.csv"
            );
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void SearchExistByName()
        {
            Driver.GridFilterSendKeys("test group 88");
            Driver.Blur();

            VerifyAreEqual("test group 88", Driver.GetGridCell(0, "Name").Text, "search exist");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchMuchSymbols()
        {
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", 
                By.ClassName("ui-grid-custom-filter-total"));
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search too much symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void SearchInvalidSymbols()
        {
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            Driver.Blur();

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "search invalid symbols");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }
    }

    [TestFixture]
    public class TasksGroupPresent : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Task.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskManager.csv"
            );
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void GroupTasksPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "present line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "present line 10");
        
            Driver.GridPaginationSelectItems("20");
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "present line 1");
            VerifyAreEqual("test group 20", Driver.GetGridCell(19, "Name").Text, "present line 20");
        }
    }

    [TestFixture]
    public class TasksGroupPage : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskGroup.csv"
            );
            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void PageGroupTasks()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();

            VerifyAreEqual("test group 11", Driver.GetGridCell(0, "Name").Text, "line 11");
            VerifyAreEqual("test group 20", Driver.GetGridCell(9, "Name").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();

            VerifyAreEqual("test group 21", Driver.GetGridCell(0, "Name").Text, "line 21");
            VerifyAreEqual("test group 30", Driver.GetGridCell(9, "Name").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();

            VerifyAreEqual("test group 31", Driver.GetGridCell(0, "Name").Text, "line 31");
            VerifyAreEqual("test group 40", Driver.GetGridCell(9, "Name").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();

            VerifyAreEqual("test group 41", Driver.GetGridCell(0, "Name").Text, "line 41");
            VerifyAreEqual("test group 50", Driver.GetGridCell(9, "Name").Text, "line 50");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();

            VerifyAreEqual("test group 51", Driver.GetGridCell(0, "Name").Text, "line 51");
            VerifyAreEqual("test group 60", Driver.GetGridCell(9, "Name").Text, "line 60");
        }

        [Test]
        public void PageGroupTasksToBegin()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 11", Driver.GetGridCell(0, "Name").Text, "line 11");
            VerifyAreEqual("test group 20", Driver.GetGridCell(9, "Name").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 21", Driver.GetGridCell(0, "Name").Text, "line 21");
            VerifyAreEqual("test group 30", Driver.GetGridCell(9, "Name").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 31", Driver.GetGridCell(0, "Name").Text, "line 31");
            VerifyAreEqual("test group 40", Driver.GetGridCell(9, "Name").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 41", Driver.GetGridCell(0, "Name").Text, "line 41");
            VerifyAreEqual("test group 50", Driver.GetGridCell(9, "Name").Text, "line 50");

            //to begin
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();

            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");
        }

        [Test]
        public void PageGroupTasksToEnd()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();

            VerifyAreEqual("test group 101", Driver.GetGridCell(0, "Name").Text, "line 101");
        }

        [Test]
        public void PageGroupTasksToNext()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 11", Driver.GetGridCell(0, "Name").Text, "line 11");
            VerifyAreEqual("test group 20", Driver.GetGridCell(9, "Name").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 21", Driver.GetGridCell(0, "Name").Text, "line 21");
            VerifyAreEqual("test group 30", Driver.GetGridCell(9, "Name").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 31", Driver.GetGridCell(0, "Name").Text, "line 31");
            VerifyAreEqual("test group 40", Driver.GetGridCell(9, "Name").Text, "line 40");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 41", Driver.GetGridCell(0, "Name").Text, "line 41");
            VerifyAreEqual("test group 50", Driver.GetGridCell(9, "Name").Text, "line 50");
        }

        [Test]
        public void PageTaskGroupToPrevious()
        {
            Driver.GridReturnDefaultView10(BaseUrl);
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 11", Driver.GetGridCell(0, "Name").Text, "line 11");
            VerifyAreEqual("test group 20", Driver.GetGridCell(9, "Name").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyAreEqual("test group 21", Driver.GetGridCell(0, "Name").Text, "line 21");
            VerifyAreEqual("test group 30", Driver.GetGridCell(9, "Name").Text, "line 30");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();

            VerifyAreEqual("test group 11", Driver.GetGridCell(0, "Name").Text, "line 11");
            VerifyAreEqual("test group 20", Driver.GetGridCell(9, "Name").Text, "line 20");

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();

            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "line 1");
            VerifyAreEqual("test group 10", Driver.GetGridCell(9, "Name").Text, "line 10");
        }

        [Test]
        public void TaskGroupSelectAndDel()
        {
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "1 grid canсel delete");

            //check delete
            Driver.GetGridCell(0, "_serviceColumn")
                .FindElement(By.CssSelector(".ui-grid-custom-service-icon.fa.fa-times.link-invert")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("test group 1", Driver.GetGridCell(0, "Name").Text, "1 grid delete");

            //check select 
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(1, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 2 grid");
            VerifyIsTrue(
                Driver.GetGridCell(1, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 3 grid");
            VerifyIsTrue(
                Driver.GetGridCell(2, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "selected 4 grid");
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count selected");

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("test group 5", Driver.GetGridCell(0, "Name").Text, "selected 2 grid delete");
            VerifyAreEqual("test group 6", Driver.GetGridCell(1, "Name").Text, "selected 3 grid delete");
            VerifyAreEqual("test group 7", Driver.GetGridCell(2, "Name").Text, "selected 4 grid delete");

            //check select all on page
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            VerifyIsTrue(
                Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 1 grid");
            VerifyIsTrue(
                Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected,
                "selected all on page 10 grid");

            //check delete all on page
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("test group 15", Driver.GetGridCell(0, "Name").Text, "selected all on page 2 grid delete");
            VerifyAreEqual("test group 24", Driver.GetGridCell(9, "Name").Text, "selected all on page 10 grid delete");

            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("87", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text,
                "count all selected after deleting");

            //check deselect all 
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();

            VerifyIsTrue(
                !Driver.GetGridCell(0, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 1 grid");
            VerifyIsTrue(
                !Driver.GetGridCell(9, "selectionRowHeaderCol")
                    .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected, "deselect all 10 grid");

            //check delete all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete all");
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "count all after deleting");
        }
    }

    [TestFixture]
    public class TasksGroupFilter : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Task.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskManager.csv"
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
        public void GroupFilterSortOrder()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //check min too much symbols
            Driver.ClearInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"));
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"), "1111111111",
                clearInput: true, byToDropFocus: By.TagName("h2"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min many symbols");

            //check max too much symbols
            Driver.ClearInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"));
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"), "1111111111",
                clearInput: true, byToDropFocus: By.TagName("h2"));
            //Driver.DropFocus("h2");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max many symbols");

            //check min and max too much symbols
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"), "1111111111",
                clearInput: true);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"), "1111111111",
                clearInput: true, byToDropFocus: By.TagName("h2"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max many symbols");

            //check invalid symbols
            Driver.ClearInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"));

            //check min invalid symbols
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"),
                "########@@@@@@@@&&&&&&&******",
                clearInput: true, byToDropFocus: By.TagName("h2"));
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min many symbols");

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //check max invalid symbols
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"),
                "########@@@@@@@@&&&&&&&******",
                clearInput: true, byToDropFocus: By.TagName("h2"));
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards max many symbols");

            //check min and max invalid symbols

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"),
                "########@@@@@@@@&&&&&&&******",
                clearInput: true);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"),
                "########@@@@@@@@&&&&&&&******",
                clearInput: true, byToDropFocus: By.TagName("h2"));

            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count cards min/max many symbols");

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            Functions.GridFilterSet(Driver, BaseUrl, name: "SortOrder");

            //check filter min not exist
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"), "1000",
                clearInput: true, byToDropFocus: By.TagName("h2"));

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.ClearInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"));
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"), "1000",
                clearInput: true, byToDropFocus: By.TagName("h2"));

            VerifyAreEqual("Найдено записей: 101",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max not exist");

            //check min and max not exist
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"), "1000",
                clearInput: true);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"), "1000",
                clearInput: true, byToDropFocus: By.TagName("h2"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter 
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"), "20",
                clearInput: true);
            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]"), "29",
                clearInput: true, byToDropFocus: By.TagName("h2"));
            VerifyAreEqual("Найдено записей: 10",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter sort order");

            VerifyAreEqual("test group 21", Driver.GetGridCell(0, "Name").Text, "bonus amount card num line 1");
            VerifyAreEqual("test group 30", Driver.GetGridCell(9, "Name").Text, "bonus amount card num line 10");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "SortOrder");
            VerifyAreEqual("Найдено записей: 91",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter after deleting 1");

            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            VerifyAreEqual("Найдено записей: 91",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter after deleting 2");
        }
    }

    [TestFixture]
    public class TasksGroupAddEdit : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.Task.csv",
                "data\\Admin\\Tasks\\GroupTasks\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\Tasks\\GroupTasks\\ManyGroup\\Customers.TaskManager.csv"
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

            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'test testov')]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupButtonSave\"]")).Click();

            Driver.GridFilterSendKeys("New Test");

            VerifyAreEqual("New Test Group", Driver.GetGridCell(0, "Name").Text, "new name");
            VerifyAreEqual("1", Driver.GetGridCell(0, "SortOrder").Text, "new sort");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            GoToAdmin("projects/10");
            MouseTab();
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".submenu-initialized")).Text.Contains("New Test Group"),
                "Displayed task group in header");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            VerifyIsTrue(selectElem.Text.Contains("New Test Group"), "new group in new tasks ");
        }

        [Test]
        public void AddNewTaskGroupFromTab()
        {
            GoToAdmin("tasks");

            MouseTab();
            Driver.FindElement(By.XPath("//a[contains(text(), 'Добавить проект')]")).Click();


            VerifyAreEqual("Новый проект", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).GetAttribute("value"),
                "name pop up");
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).GetAttribute("value"),
                "taskGroupSortOrder pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).SendKeys("Tab New Test Group");
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).SendKeys("777");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'test testov')]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupButtonSave\"]")).Click();

            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            Driver.GridFilterSendKeys("Tab New Test");

            VerifyAreEqual("Tab New Test Group", Driver.GetGridCell(0, "Name").Text, "new name");
            VerifyAreEqual("777", Driver.GetGridCell(0, "SortOrder").Text, "new sort");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");
        }

        [Test]
        public void EditTaskGroup()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();


            VerifyAreEqual("Редактирование проекта", Driver.FindElement(By.TagName("h2")).Text, "pop up h2");

            VerifyAreEqual("test group 1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).GetAttribute("value"),
                "name pop up");
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).GetAttribute("value"),
                "taskGroupSortOrder pop up");

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).SendKeys("New Name Test Group");
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).SendKeys("888");

            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'Elena El')]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"] input")).Click();
            Driver.FindElement(By.XPath("//span[contains(text(), 'test testov')]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupButtonSave\"]")).Click();

            Driver.GridFilterSendKeys("test group 1");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "new name");

            Driver.GridFilterSendKeys("New Name Test Group");

            VerifyAreEqual("New Name Test Group", Driver.GetGridCell(0, "Name").Text, "new name");
            VerifyAreEqual("888", Driver.GetGridCell(0, "SortOrder").Text, "new sort");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text, "count all");

            Driver.GetGridCellElement(0, "_serviceColumn", by: By.CssSelector(".fa-pencil-alt")).Click();


            VerifyAreEqual("New Name Test Group",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).GetAttribute("value"), "name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                    .FindElement(By.CssSelector(".ui-select-match")).Text.Contains("test testov"), "contain manager 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                    .FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Elena El"), "contain manager 2");
            VerifyAreEqual("888",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).GetAttribute("value"),
                "SortOrder");
        }

        [Test]
        public void InplaceEditTaskGroup()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");

            Driver.GridFilterSendKeys("group 3");

            VerifyAreEqual("2", Driver.GetGridCell(0, "SortOrder").Text, "SortOrder");
            VerifyIsTrue(
                Driver.GetGridCellElement(0, "Enabled", by: By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .Selected, "Enabled");

            Driver.GetGridCellElement(0, "SortOrder", by: By.Name("inputForm")).Click();
            Driver.GetGridCellElement(0, "SortOrder", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys(Keys.Backspace);
            Driver.GetGridCellElement(0, "SortOrder", by: By.CssSelector(".ui-grid-custom-edit-field.form-control"))
                .SendKeys("5");
            Driver.GetGridCell(0, "Enabled")
                .FindElement(By.CssSelector("[data-e2e=\"switchOnOffLabel\"] .onoffswitch-inner-on")).Click();
            Driver.DropFocusCss("[data-e2e=\"TaskGroupTitle\"]");
            VerifyAreEqual("5", Driver.GetGridCellText(0, "SortOrder"), "change SortOrder");
            VerifyIsFalse(
                Driver.GetGridCellElement(0, "Enabled", by: By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .Selected, "change Enabled");

            Refresh();

            Driver.GridFilterSendKeys("group 3");

            VerifyAreEqual("5", Driver.GetGridCell(0, "SortOrder").Text, "change SortOrder");
            VerifyIsFalse(
                Driver.GetGridCellElement(0, "Enabled", by: By.CssSelector("[data-e2e=\"switchOnOffInput\"]"))
                    .Selected, "change Enabled");

            GoToAdmin("tasks");
            MouseTab();
            VerifyIsFalse(Driver.FindElements(By.XPath("//a[contains(text(), 'test group 3')]")).Count == 1,
                "in header menu");

            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();

            IWebElement selectElem = Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]"));
            VerifyIsFalse(selectElem.Text.Contains("test group 3"), "group in new tasks");
            VerifyIsTrue(selectElem.Text.Contains("test group 4"), "enabled group in new tasks ");
        }

        public void MouseTab()
        {
            Actions action = new Actions(Driver);
            IWebElement elem = Driver.FindElement(By.CssSelector(".dropdown-toggle.header-bottom-menu-link"))
                .FindElement(By.CssSelector(".fa-angle-down"));
            action.MoveToElement(elem);
            action.Perform();
        }
    }

    [TestFixture]
    public class TasksGroupObserver : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\ObserverTask\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.Task.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.TaskManager.csv",
                "data\\Admin\\Tasks\\ObserverTask\\Customers.TaskGroupManager.csv"
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
        public void OpenGroupCart()
        {
            GoToAdmin("settingstasks#?tasksTab=taskGroups");
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.CssSelector(".fa-pencil-alt")).Click();

            //data-e2e="ObserverProject"
            VerifyAreEqual("group1",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupName\"]")).GetAttribute("value"), "name");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                    .FindElement(By.CssSelector(".ui-select-match")).Text.Contains("test testov"), "contain manager 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                    .FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Admin Ad"), "contain manager 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                    .FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Testname1 LastName1"),
                "contain manager 3");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector("[data-e2e=\"ObserverProject\"]"))
                    .FindElement(By.CssSelector(".ui-select-match")).Text.Contains("Testname3 LastName3"),
                "contain manager 4");
            VerifyAreEqual("0",
                Driver.FindElement(By.CssSelector("[data-e2e=\"taskGroupSortOrder\"]")).GetAttribute("value"),
                "SortOrder");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"ProjectEnabled\"] input")).Selected,
                "Project Enabled ");
        }
    }
}