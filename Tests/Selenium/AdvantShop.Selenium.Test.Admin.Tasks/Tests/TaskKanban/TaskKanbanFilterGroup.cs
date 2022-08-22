using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TaskKanban
{
    [TestFixture]
    public class TaskKanbanFilterGroup : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.Departments.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.Customer.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.Managers.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.TaskGroup.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.Task.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.ViewedTask.csv",
                "data\\Admin\\TasksKanban\\KanbanFilterGroup\\Customers.TaskManager.csv"
            );

            Init();
            Functions.KanbanOn(Driver, BaseUrl, url: "tasks");
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
        public void FilterByAssigned()
        {
            GoToAdmin("projects/1");
            VerifyIsFalse(Driver.PageSource.Contains("Задач нет."), "no task in project");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 13, "count kanban cart begin");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnAssigned", "Admin Ad");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 5, "count kanban cart filter");
            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "1 number");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name");

            VerifyAreEqual("13.", Driver.GetKanbanCard(1, 1).Text, "2 number");
            VerifyAreEqual("test13", Driver.GetKanbanCard(1, 1, "Name").Text, "2 name");

            VerifyAreEqual("9.", Driver.GetKanbanCard(2, 0).Text, "3 number");
            VerifyAreEqual("test9", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name");

            VerifyAreEqual("7.", Driver.GetKanbanCard(2, 1).Text, "4 number");
            VerifyAreEqual("test7", Driver.GetKanbanCard(2, 1, "Name").Text, "4 name");


            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Elena El\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart new");
            VerifyAreEqual("4.", Driver.GetKanbanCard(0, 0).Text, "number 1");
            VerifyAreEqual("test4", Driver.GetKanbanCard(0, 0, "Name").Text, "name 1 ");

            VerifyAreEqual("11.", Driver.GetKanbanCard(1, 0).Text, "number 2");
            VerifyAreEqual("test11", Driver.GetKanbanCard(1, 0, "Name").Text, "name 2 ");

            VerifyAreEqual("6.", Driver.GetKanbanCard(2, 0).Text, "number 3 ");
            VerifyAreEqual("test6", Driver.GetKanbanCard(2, 0, "Name").Text, "name 3 ");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");
            CloseFilter();
        }

        [Test]
        public void FilterByAppointed()
        {
            GoToAdmin("projects/1");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnAppointed", "Admin Ad");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 6, "count kanban cart");
            VerifyAreEqual("3.", Driver.GetKanbanCard(0, 0).Text, "1 number");
            VerifyAreEqual("test3", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name");

            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "2 number");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name");

            VerifyAreEqual("6.", Driver.GetKanbanCard(2, 0).Text, "3 number");
            VerifyAreEqual("test6", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name");


            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Elena El\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 3, "count kanban cart 2");
            VerifyAreEqual("2.", Driver.GetKanbanCard(0, 0).Text, "number 1");
            VerifyAreEqual("test2", Driver.GetKanbanCard(0, 0, "Name").Text, "name 1 ");

            VerifyAreEqual("4.", Driver.GetKanbanCard(0, 1).Text, "number 2");
            VerifyAreEqual("test4", Driver.GetKanbanCard(0, 1, "Name").Text, "name 2 ");

            VerifyAreEqual("9.", Driver.GetKanbanCard(2, 0).Text, "number 3 ");
            VerifyAreEqual("test9", Driver.GetKanbanCard(2, 0, "Name").Text, "name 3 ");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAppointed");
            CloseFilter();
        }

        [Test]
        public void FilterByAssignedandAppointed()
        {
            GoToAdmin("projects/1");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnAppointed", "Admin Ad");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 6, "count kanban cart");
            VerifyAreEqual("3.", Driver.GetKanbanCard(0, 0).Text, "1 number");
            VerifyAreEqual("test3", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name");

            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnAssigned");

            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-filter-block-name=\"_noopColumnAssigned\"] [data-e2e=\"gridFilterItemSelect\"]"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Admin Ad\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 2, "count kanban cart 2");

            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "1 number two");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name  two");
            VerifyAreEqual("7.", Driver.GetKanbanCard(2, 0).Text, "2 number two");
            VerifyAreEqual("test7", Driver.GetKanbanCard(2, 0, "Name").Text, "2 name  two");

            //close appointed
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAppointed");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 5, "count kanban cart");
            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "1 number three");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name three");

            //close assigned
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");
            CloseFilter();
        }

        [Test]
        public void FilterByCreate()
        {
            GoToAdmin("projects/1");
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnDateCreated");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]"), "08.08.2016 00:00",
                clearInput: true, byToDropFocus: By.CssSelector(".main-panel-page"));

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]"), "11.08.2016 21:00",
                clearInput: true, byToDropFocus: By.CssSelector(".main-panel-page"));

            Driver.WaitForElems(By.CssSelector("kanban-card"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart");
            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "1 number");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name");

            VerifyAreEqual("11.", Driver.GetKanbanCard(1, 1).Text, "2 number");
            VerifyAreEqual("test11", Driver.GetKanbanCard(1, 1, "Name").Text, "2 name");

            VerifyAreEqual("10.", Driver.GetKanbanCard(2, 0).Text, "3 number");
            VerifyAreEqual("test10", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name");

            VerifyAreEqual("Новые\r\n0",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n3",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n1",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDateCreated");
            CloseFilter();
        }

        [Test]
        public void FilterByDueDate()
        {
            GoToAdmin("projects/1");
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnDueDateFormatted");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]"), "09.08.2016 00:00",
                clearInput: true, byToDropFocus: By.CssSelector(".main-panel-page"));

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]"), "12.08.2016 21:00",
                clearInput: true, byToDropFocus: By.CssSelector(".main-panel-page"));

            Driver.WaitForElems(By.CssSelector("kanban-card"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart");
            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "1 number");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name");

            VerifyAreEqual("9.", Driver.GetKanbanCard(2, 0).Text, "2 number");
            VerifyAreEqual("test9", Driver.GetKanbanCard(2, 0, "Name").Text, "2 name");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDueDateFormatted");
            CloseFilter();
        }

        [Test]
        public void FilterByDueDateandAssigned()
        {
            GoToAdmin("projects/1");
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnDueDateFormatted");

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]"), "09.08.2016 00:00",
                clearInput: true, byToDropFocus: By.CssSelector(".main-panel-page"));

            Driver.SendKeysInput(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]"), "12.08.2016 21:00",
                clearInput: true, byToDropFocus: By.CssSelector(".main-panel-page"));

            Driver.WaitForElems(By.CssSelector("kanban-card"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart");
            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "1 number");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name");

            VerifyAreEqual("9.", Driver.GetKanbanCard(2, 0).Text, "2 number");
            VerifyAreEqual("test9", Driver.GetKanbanCard(2, 0, "Name").Text, "2 name");

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnAssigned", "test testov");

            VerifyAreEqual("10.", Driver.GetKanbanCard(2, 0).Text, "1 number two");
            VerifyAreEqual("test10", Driver.GetKanbanCard(2, 0, "Name").Text, "1 name two");

            //close duedate
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDueDateFormatted");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart Assigned");
            VerifyAreEqual("3.", Driver.GetKanbanCard(0, 0).Text, "1 number Assigned");
            VerifyAreEqual("test3", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name Assigned");

            VerifyAreEqual("1.", Driver.GetKanbanCard(0, 1).Text, "2 number Assigned");
            VerifyAreEqual("test1", Driver.GetKanbanCard(0, 1, "Name").Text, "2 name Assigned");

            VerifyAreEqual("10.", Driver.GetKanbanCard(2, 0).Text, "3 number Assigned");
            VerifyAreEqual("test10", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name Assigned");

            //close assign
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");
            CloseFilter();
        }

        [Test]
        public void FilterByPriority()
        {
            GoToAdmin("projects/1");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnPriorities", "Низкий");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 5, "count kanban cart low");
            VerifyAreEqual("1.", Driver.GetKanbanCard(0, 0).Text, "1 number low");
            VerifyAreEqual("test1", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name low");

            VerifyAreEqual("13.", Driver.GetKanbanCard(1, 0).Text, "2 number low");
            VerifyAreEqual("test13", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name low");

            VerifyAreEqual("7.", Driver.GetKanbanCard(2, 0).Text, "3 number low");
            VerifyAreEqual("test7", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name low");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Средний\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart medium");
            VerifyAreEqual("2.", Driver.GetKanbanCard(0, 0).Text, "1 number medium");
            VerifyAreEqual("test2", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name medium");

            VerifyAreEqual("11.", Driver.GetKanbanCard(1, 0).Text, "2 number medium");
            VerifyAreEqual("test11", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name medium");

            VerifyAreEqual("5.", Driver.GetKanbanCard(2, 0).Text, "3 number medium");
            VerifyAreEqual("test5", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name medium");
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Высокий\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart high");
            VerifyAreEqual("3.", Driver.GetKanbanCard(0, 0).Text, "1 number high");
            VerifyAreEqual("test3", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name high");
            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "Priority").Displayed, "1 Displayed Priority high");

            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "2 number high");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name high");
            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "Priority").Displayed, "2 Displayed Priority high");

            VerifyAreEqual("6.", Driver.GetKanbanCard(2, 0).Text, "3 number high");
            VerifyAreEqual("test6", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name high");
            VerifyIsTrue(Driver.GetKanbanCard(2, 0, "Priority").Displayed, "3 Displayed Priority high");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnPriorities");
            CloseFilter();
        }

        [Test]
        public void FilterByPriorityAnotherGroup()
        {
            GoToAdmin("projects/2");
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnPriorities", "Низкий");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 5, "count kanban cart low");
            VerifyAreEqual("17.", Driver.GetKanbanCard(0, 0).Text, "1 number low");
            VerifyAreEqual("test17", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name low");

            VerifyAreEqual("26.", Driver.GetKanbanCard(1, 0).Text, "2 number low");
            VerifyAreEqual("test26", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name low");

            VerifyAreEqual("20.", Driver.GetKanbanCard(2, 0).Text, "3 number low");
            VerifyAreEqual("test20", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name low");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Высокий\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 5, "count kanban cart high");
            VerifyAreEqual("19.", Driver.GetKanbanCard(0, 0).Text, "1 number high");
            VerifyAreEqual("test19", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name high");
            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "Priority").Displayed, "1 Displayed Priority high");

            VerifyAreEqual("15.", Driver.GetKanbanCard(1, 0).Text, "2 number high");
            VerifyAreEqual("test15", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name high");
            VerifyIsTrue(Driver.GetKanbanCard(1, 0, "Priority").Displayed, "2 Displayed Priority high");

            VerifyAreEqual("22.", Driver.GetKanbanCard(2, 0).Text, "3 number high");
            VerifyAreEqual("test22", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name high");
            VerifyIsTrue(Driver.GetKanbanCard(2, 0, "Priority").Displayed, "3 Displayed Priority high");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Средний\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 6, "count kanban cart medium");
            VerifyAreEqual("18.", Driver.GetKanbanCard(0, 0).Text, "1 number medium");
            VerifyAreEqual("test18", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name medium");

            VerifyAreEqual("14.", Driver.GetKanbanCard(1, 0).Text, "2 number medium");
            VerifyAreEqual("test14", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name medium");

            VerifyAreEqual("21.", Driver.GetKanbanCard(2, 0).Text, "3 number medium");
            VerifyAreEqual("test21", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name medium");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Критичный\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 1, "count kanban cart critical");
            VerifyAreEqual("16.", Driver.GetKanbanCard(0, 0).Text, "1 number critical");
            VerifyAreEqual("test16", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name critical");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnPriorities");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 17, "count kanban cart close");
            VerifyAreEqual("16.", Driver.GetKanbanCard(0, 0).Text, "1 number close");
            VerifyAreEqual("test16", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name close");

            VerifyAreEqual("15.", Driver.GetKanbanCard(1, 0).Text, "2 number close");
            VerifyAreEqual("test15", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name close");

            VerifyAreEqual("22.", Driver.GetKanbanCard(2, 0).Text, "3 number close");
            VerifyAreEqual("test22", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name close");

            VerifyAreEqual("Новые\r\n4",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new close");
            VerifyAreEqual("В работе\r\n7",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work close");
            VerifyAreEqual("Сделаны\r\n6",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed close");
        }

        [Test]
        public void FiltersFromMeByAssigned()
        {
            GoToAdmin("tasks");
            Driver.FindElement(By.XPath("//label[contains(text(), 'Поставленные мной')]")).Click();

            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnAssigned", "Elena El");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 4, "count kanban cart");
            VerifyAreEqual("11.", Driver.GetKanbanCard(1, 0).Text, "1 number");
            VerifyAreEqual("test11", Driver.GetKanbanCard(1, 0, "Name").Text, "1 name");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 12, "count kanban cart close");
            VerifyAreEqual("3.", Driver.GetKanbanCard(0, 0).Text, "1 number close");
            VerifyAreEqual("test3", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name close");

            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "2 number close");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name close ");

            VerifyAreEqual("6.", Driver.GetKanbanCard(2, 0).Text, "3 number close");
            VerifyAreEqual("test6", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name close");

            VerifyAreEqual("Новые\r\n2",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new close");
            VerifyAreEqual("В работе\r\n5",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work close");
            VerifyAreEqual("Сделаны\r\n5",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed close");
        }

        [Test]
        public void FilterMyTaskByAppointed()
        {
            GoToAdmin("tasks");
            Driver.FindElement(By.XPath("//label[contains(text(), 'Мои задачи')]")).Click();
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, "_noopColumnAppointed", "Elena El");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 2, "count kanban cart my task");
            VerifyAreEqual("9.", Driver.GetKanbanCard(2, 0).Text, "1 number my task");
            VerifyAreEqual("test9", Driver.GetKanbanCard(2, 0, "Name").Text, "1 name my task");

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAppointed");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 14, "count kanban cart close");
            VerifyAreEqual("16.", Driver.GetKanbanCard(0, 0).Text, "1 number close");
            VerifyAreEqual("test16", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name close");

            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "2 number close");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name close ");

            VerifyAreEqual("9.", Driver.GetKanbanCard(2, 0).Text, "3 number close");
            VerifyAreEqual("test9", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name close");

            VerifyAreEqual("Новые\r\n1",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new close");
            VerifyAreEqual("В работе\r\n7",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work close");
            VerifyAreEqual("Сделаны\r\n6",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed close");
        }

        private void CloseFilter()
        {
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 13, "count kanban cart close");
            VerifyAreEqual("3.", Driver.GetKanbanCard(0, 0).Text, "1 number close");
            VerifyAreEqual("test3", Driver.GetKanbanCard(0, 0, "Name").Text, "1 name close");

            VerifyAreEqual("12.", Driver.GetKanbanCard(1, 0).Text, "2 number close");
            VerifyAreEqual("test12", Driver.GetKanbanCard(1, 0, "Name").Text, "2 name close ");

            VerifyAreEqual("6.", Driver.GetKanbanCard(2, 0).Text, "3 number close");
            VerifyAreEqual("test6", Driver.GetKanbanCard(2, 0, "Name").Text, "3 name close");

            VerifyAreEqual("Новые\r\n4",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new close");
            VerifyAreEqual("В работе\r\n3",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work close");
            VerifyAreEqual("Сделаны\r\n6",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed close");
        }
    }
}