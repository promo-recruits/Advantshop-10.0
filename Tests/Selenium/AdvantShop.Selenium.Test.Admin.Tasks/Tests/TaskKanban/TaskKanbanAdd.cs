using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TaskKanban
{
    [TestFixture]
    public class TaskKanbanAdd : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\TasksKanban\\AddTask\\Customers.CustomerGroup.csv",
                "data\\Admin\\TasksKanban\\AddTask\\Customers.Departments.csv",
                "data\\Admin\\TasksKanban\\AddTask\\Customers.Customer.csv",
                "data\\Admin\\TasksKanban\\AddTask\\Customers.Managers.csv",
                "data\\Admin\\TasksKanban\\AddTask\\Customers.CustomerRoleAction.csv",
                "data\\Admin\\TasksKanban\\AddTask\\Customers.TaskGroup.csv"
            );

            Init();
            Functions.KanbanOn(Driver, BaseUrl, url: "tasks");
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("tasks");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void AddCorrectTask()
        {
            Driver.FindElement(By.CssSelector("[data-e2e=\"AddTask\"]")).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).SendKeys("NewTestTask");

            //Assigned  
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskAssigned\"]")).Click();
            Driver.FindElement(By.XPath("//div[contains(text(), 'test testov')]")).Click();

            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).Clear();
            string str = DateTime.Now.AddDays(1).ToString("dd.MM.yyyy HH:mm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskDuedate\"]")).SendKeys(str);

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();
            //Despription
            Driver.SetCkText("Description NewTestTask", "editor1");
            //Group
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskGroup\"]")))).SelectByText("All");

            //Priority
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskPriority\"]"))))
                .SelectByText("Высокий");

            Driver.FindElement(By.CssSelector("[data-e2e=\"newtaskName\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"TaskAdd\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 1, "count kanban cart");
            VerifyAreEqual("2.", Driver.GetKanbanCard(0, 0).Text, "new number");
            VerifyAreEqual("NewTestTask", Driver.GetKanbanCard(0, 0, "Name").Text, "new name");
            VerifyAreEqual("All", Driver.GetKanbanCard(0, 0, "TaskGroupName").Text, "new group");
            VerifyAreEqual("1 мин", Driver.GetKanbanCard(0, 0, "AfterCreate").Text, "After Create");
            VerifyAreEqual("24 ч", Driver.GetKanbanCard(0, 0, "OverDue").Text, "Over Due");
            VerifyAreEqual("test testov", Driver.GetKanbanCard(0, 0, "AssignedCustomer").GetAttribute("alt"),
                "AssignedCustomer");
            VerifyIsTrue(Driver.GetKanbanCard(0, 0, "Priority").Displayed, "Displayed Priority");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Мои задачи (0)"),
                "count my task");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Поставленные мной задачи (1)"),
                "count appointed by me task");
        }


        [Test]
        public void EditTask()
        {
            Driver.FindElement(By.XPath("//label[contains(text(), 'Все')]")).Click();
            Driver.GetKanbanCard(0, 0).Click();
            Driver.WaitForModal();
            //name
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).SendKeys("EditNewTestTask");
            //Appointed  
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"] .close")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAssigned\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'Elena El')]")).Click();
            //Assigned
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAppointed\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskAppointed\"]"))
                .FindElement(
                    By.CssSelector(".ui-select-choices.ui-select-choices-content.ui-select-dropdown.dropdown-menu"))
                .FindElement(By.XPath("//div[contains(text(), 'test testov')]")).Click();

            //Group
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskGroup\"]")))).SelectByText(
                "test group");
            //Priority
            (new SelectElement(Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskPriopity\"]"))))
                .SelectByText("Средний");
            //Despription
            Driver.SetCkText("Edit Description NewTestTask", "editor1");

            //duedate dueDate
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDuedate\"]")).Clear();
            string str = DateTime.Now.AddDays(2).ToString("dd.MM.yyyy HH:mm");
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDuedate\"]")).SendKeys(str);
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskName\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskButtonSave\"]")).Click();
            Driver.WaitForToastSuccess();

            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 1, "count kanban cart");
            VerifyAreEqual("2.", Driver.GetKanbanCard(0, 0).Text, "new number");
            VerifyAreEqual("EditNewTestTask", Driver.GetKanbanCard(0, 0, "Name").Text, "new name");
            VerifyAreEqual("test group", Driver.GetKanbanCard(0, 0, "TaskGroupName").Text, "new group");
            VerifyAreEqual("1 мин", Driver.GetKanbanCard(0, 0, "AfterCreate").Text, "After Create");
            VerifyAreEqual("2 дн.", Driver.GetKanbanCard(0, 0, "OverDue").Text, "Over Due");
            VerifyAreEqual("Elena El", Driver.GetKanbanCard(0, 0, "AssignedCustomer").GetAttribute("alt"),
                "AssignedCustomer");
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".kanban-task-priority")).Displayed, "Displayed Priority");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Мои задачи (0)"),
                "count my task 1");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Поставленные мной задачи (0)"),
                "count appointed by me task 1");

            Driver.GetKanbanCard(0, 0).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"AssignedToMe\"]")).Click();

            Driver.FindElement(By.CssSelector(".btn-cancel")).Click();

            Refresh();
            VerifyAreEqual("Admin Ad", Driver.GetKanbanCard(0, 0, "AssignedCustomer").GetAttribute("alt"),
                "new AssignedCustomer");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Мои задачи (1)"),
                "count my task 2");
            VerifyIsTrue(
                Driver.FindElement(By.CssSelector(".ibox-content")).Text.Contains("Поставленные мной задачи (0)"),
                "count appointed by me task 2");
            GoToAdmin("projects/1");
            VerifyIsTrue(Driver.PageSource.Contains("Задач нет."), "no task in project");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 0, " no count kanban cart");
            GoToAdmin("projects/2");
            VerifyIsFalse(Driver.PageSource.Contains("Задач нет."), "no task in project");
            VerifyIsTrue(Driver.FindElements(By.CssSelector("kanban-card")).Count == 1, "count kanban cart");
        }

        [Test]
        public void EditTaskStatus()
        {
            VerifyAreEqual("Новые\r\n1",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed");
            Driver.GetKanbanCard(0, 0).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusInprogress\"]")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();


            VerifyAreEqual("Новые\r\n0",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new Inprogress");
            VerifyAreEqual("В работе\r\n1",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work Inprogress");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed Inprogress");

            Driver.GetKanbanCard(1, 0).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusStop\"]")).Click();

            Driver.FindElement(By.CssSelector(".close")).Click();


            VerifyAreEqual("Новые\r\n1",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new Stop");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work Stop");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed Stop");

            Driver.GetKanbanCard(0, 0).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector(".form-group input")).SendKeys("Rezult");
            Driver.FindElement(By.CssSelector(".ladda-label")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ng-tab.nav-item.active span")).Count == 0);


            VerifyAreEqual("Новые\r\n0",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new Completed");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work Completed");
            VerifyAreEqual("Сделаны\r\n1",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed Completed");

            Driver.GetKanbanCard(2, 0).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusRestart\"]")).Click();
            Driver.FindElement(By.CssSelector(".close")).Click();


            VerifyAreEqual("Новые\r\n1",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new Restart");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work Restart");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed Restart");


            Driver.GetKanbanCard(0, 0).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector(".form-group input")).SendKeys("Rezult");
            Driver.FindElement(By.CssSelector(".ladda-label")).Click();

            Driver.GetKanbanCard(2, 0).Click();
            Driver.WaitForModal();
            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusAccepted\"]")).Click();


            VerifyAreEqual("Новые\r\n0",
                Driver.FindElement(By.Id("column0")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count new Accepted");
            VerifyAreEqual("В работе\r\n0",
                Driver.FindElement(By.Id("column1")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count in work Accepted");
            VerifyAreEqual("Сделаны\r\n0",
                Driver.FindElement(By.Id("column2")).FindElement(By.CssSelector(".kanban-column-header")).Text,
                "count completed Accepted");
        }
    }
}