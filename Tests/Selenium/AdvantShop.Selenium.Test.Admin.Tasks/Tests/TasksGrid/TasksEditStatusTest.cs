using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class EditStatusTaskTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\EditTasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\EditTasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\EditTasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\EditTasks\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\EditTasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\EditTasks\\Customers.Task.csv",
                "data\\Admin\\Tasks\\EditTasks\\Customers.TaskManager.csv"
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
        public void EditTaskStatusInProgress()
        {
            GoToAdmin("tasks");
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusInprogress\"]")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            VerifyAreEqual("В работе (выполняется)",
                (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
        }

        [Test]
        public void EditTaskStatusStoped()
        {
            GoToAdmin("tasks");
            VerifyAreEqual("В работе (выполняется)",
                (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusStop\"]")).Click();
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
        }

        [Test]
        public void EditTaskStatustComplitedOk()
        {
            GoToAdmin("tasks");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();
            Driver.FindElement(By.CssSelector(".form-group input")).SendKeys("Rezult");
            Driver.FindElement(By.CssSelector(".ladda-label")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ng-tab.nav-item.active span")).Count == 0);
            GoToAdmin("tasks?filterby=completed");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyAreEqual("Завершена", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            //edittaskRezult
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            //rezult
            VerifyAreEqual("Rezult", Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskRezult\"]")).Text);
        }

        [Test]
        public void EditTaskStatustComplitedCancel()
        {
            GoToAdmin("tasks");
            string str = Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text;
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusCompleted\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"taskResult\"]")).SendKeys("Rezult");
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            VerifyAreEqual(str, (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
        }

        [Test]
        public void EditTaskStatuzAccepted()
        {
            GoToAdmin("tasks?filterby=completed");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskStatusAccepted\"]")).Click();

            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ng-tab.nav-item.active span")).Count == 0);
            GoToAdmin("tasks?filterby=accepted");
            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyAreEqual("Принято", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
        }
    }
}