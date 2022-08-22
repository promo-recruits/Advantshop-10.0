using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TaskKanban
{
    [TestFixture]
    public class TaskKanbanTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\TasksKanban\\Customers.CustomerGroup.csv",
                "data\\Admin\\TasksKanban\\Customers.Departments.csv",
                "data\\Admin\\TasksKanban\\Customers.Customer.csv",
                "data\\Admin\\TasksKanban\\Customers.Managers.csv",
                "data\\Admin\\TasksKanban\\Customers.TaskGroup.csv",
                "data\\Admin\\TasksKanban\\Customers.Task.csv",
                "data\\Admin\\TasksKanban\\Customers.ViewedTask.csv",
                "data\\Admin\\TasksKanban\\Customers.TaskManager.csv"
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
        public void OpenTasks()
        {
            GoToAdmin("tasks");
            VerifyAreEqual("Все задачи", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text, "h1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Мои задачи (3)"),
                "count my task");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Поставленные мной задачи (5)"),
                "count appointed by me task");
            VerifyAreEqual("Новые\r\n5",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n5",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n4",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
        }

        [Test]
        public void SearchTasks()
        {
            GoToAdmin("tasks");
            Driver.GridFilterSendKeys("test10", By.CssSelector(".top-panel"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 1, "count kanban cart");
            VerifyAreEqual("10.", Driver.GetKanbanCard(1, 0).Text, "new number");
            VerifyAreEqual("test10", Driver.GetKanbanCard(1, 0, "Name").Text, "new name");
            VerifyAreEqual("Новые\r\n0",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n1",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");

            Driver.GridFilterSendKeys("test15", By.CssSelector(".top-panel"));

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 0, "count accepted cart");
            VerifyAreEqual("Новые\r\n0",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "no new");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "no in work");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "no completed");

            //search not exist tasks
            Driver.GridFilterSendKeys("test111", By.CssSelector(".top-panel"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 0, "count not exist cart");

            //search too much tasks
            Driver.GridFilterSendKeys("1111111111222222222223333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.CssSelector(".top-panel"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 0, "count much symbols cart");

            //search invalid tasks
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..", By.CssSelector(".top-panel"));
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 0, "count invalid cart");

            GoToAdmin("tasks");
        }

        [Test]
        public void TaskAppointedByMe()
        {
            Driver.FindElement(By.XPath("//label[contains(text(), 'Поставленные мной')]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 5, "count accepted cart");
            VerifyAreEqual("Новые\r\n5",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
        }

        [Test]
        public void TaskAssignedToMe()
        {
            Driver.FindElement(By.XPath("//label[contains(text(), 'Мои задачи')]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 3, "count accepted cart");
            VerifyAreEqual("Новые\r\n3",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
        }

        [Test]
        public void TasksAll()
        {
            Driver.FindElement(By.XPath("//label[contains(text(), 'Все')]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 14, "count accepted cart");
            VerifyAreEqual("Новые\r\n5",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n5",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n4",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
        }

        [Test]
        public void TasksDragDrop()
        {
            GoToAdmin("tasks");
            Driver.FindElement(By.XPath("//label[contains(text(), 'Все')]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 14, "count accepted cart");
            VerifyAreEqual("Новые\r\n5",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n5",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n4",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
            Functions.DragDropElement(Driver, Driver.GetKanbanCard(0, 0), Driver.GetKanbanCard(1, 0));
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "change deal status");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 14, "new count cart");
            VerifyAreEqual("Новые\r\n4",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "new count new");
            VerifyAreEqual("В работе\r\n6",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "new count in work");
            VerifyAreEqual("Сделаны\r\n4",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "new count completed");

            Functions.DragDropElement(Driver, Driver.GetKanbanCard(1, 0), Driver.GetKanbanCard(2, 0));
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".modal-dialog")).Displayed, "show dialog");
            Driver.FindElement(By.CssSelector(".btn.btn-save")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".modal-dialog ")).Count == 0, "hide dialog");
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed, "change deal status 1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 14, "new count cart 2");
            VerifyAreEqual("Новые\r\n4",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "new count new 2");
            VerifyAreEqual("В работе\r\n5",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "new count in work 2");
            VerifyAreEqual("Сделаны\r\n5",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "new count completed 2");
        }
    }
}