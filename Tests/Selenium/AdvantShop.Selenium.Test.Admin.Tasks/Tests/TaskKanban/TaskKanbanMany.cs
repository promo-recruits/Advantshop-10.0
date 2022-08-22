using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TaskKanban
{
    [TestFixture]
    public class TaskKanbanMany : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.CustomerGroup.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.Departments.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.Customer.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.Managers.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.TaskGroup.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.Task.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.ViewedTask.csv",
                "data\\Admin\\TasksKanban\\KanbanManyTask\\Customers.TaskManager.csv"
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
            VerifyAreEqual("Новые\r\n10",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n180",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n60",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 110, "count kanban cart");

            VerifyIsTrue(Driver.FindElement(By.Id("column0")).FindElements(By.CssSelector("kanban-card")).Count == 10,
                "count new kanban cart");
            VerifyIsTrue(Driver.FindElement(By.Id("column1")).FindElements(By.CssSelector("kanban-card")).Count == 50,
                "count inWork kanban cart");
            VerifyIsTrue(Driver.FindElement(By.Id("column2")).FindElements(By.CssSelector("kanban-card")).Count == 50,
                "count done kanban cart");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Load0\"]")).Count == 0, "btn load new");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Load1\"]")).Displayed, "btn load in work");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Load2\"]")).Displayed, "btn load done");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"Load1\"]")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("column0")).FindElements(By.CssSelector("kanban-card")).Count == 10,
                "count new kanban cart 1");
            VerifyIsTrue(Driver.FindElement(By.Id("column1")).FindElements(By.CssSelector("kanban-card")).Count == 100,
                "count inWork kanban cart 1");
            VerifyIsTrue(Driver.FindElement(By.Id("column2")).FindElements(By.CssSelector("kanban-card")).Count == 50,
                "count done kanban cart 1");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Load0\"]")).Count == 0, "btn load new 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Load1\"]")).Displayed, "btn load in work 1");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Load2\"]")).Displayed, "btn load done 1");

            //Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"Load2\"]")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("column0")).FindElements(By.CssSelector("kanban-card")).Count == 10,
                "count new kanban cart 2");
            VerifyIsTrue(Driver.FindElement(By.Id("column1")).FindElements(By.CssSelector("kanban-card")).Count == 100,
                "count inWork kanban cart 2");
            VerifyIsTrue(Driver.FindElement(By.Id("column2")).FindElements(By.CssSelector("kanban-card")).Count == 60,
                "count done kanban cart 2");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Load0\"]")).Count == 0, "btn load new 2");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Load1\"]")).Displayed, "btn load in work 2");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Load2\"]")).Count == 0, "btn load done 2");

            Driver.ScrollTo(By.TagName("footer"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"Load1\"]")).Click();

            VerifyIsTrue(Driver.FindElement(By.Id("column0")).FindElements(By.CssSelector("kanban-card")).Count == 10,
                "count new kanban cart 3");
            VerifyIsTrue(Driver.FindElement(By.Id("column1")).FindElements(By.CssSelector("kanban-card")).Count == 150,
                "count inWork kanban cart 3");
            VerifyIsTrue(Driver.FindElement(By.Id("column2")).FindElements(By.CssSelector("kanban-card")).Count == 60,
                "count done kanban cart 3");

            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Load0\"]")).Count == 0, "btn load new 3");
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e=\"Load1\"]")).Displayed, "btn load in work 3");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("[data-e2e=\"Load2\"]")).Count == 0, "btn load done 3");
        }
    }
}