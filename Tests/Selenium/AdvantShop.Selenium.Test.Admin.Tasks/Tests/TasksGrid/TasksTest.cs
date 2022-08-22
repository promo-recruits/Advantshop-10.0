using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\Customers.Task.csv",
                "data\\Admin\\Tasks\\Customers.TaskManager.csv"
            );
            Init();
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
        public void OpenTasks()
        {
            VerifyAreEqual("8", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyAreEqual("Все задачи", Driver.FindElement(By.CssSelector(".page-name-block-text")).Text);
        }

        [Test]
        public void CopyLinkTask()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();
            Driver.WaitForModal();
            VerifyIsTrue(Driver.FindElement(By.Name("editTaskForm")).Displayed);
            VerifyAreEqual("Задача №1", Driver.FindElement(By.TagName("h2")).Text);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".copy-link")).Displayed);
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".copy-link")).Text.Contains("/tasks#?modal=1"));
            string path = Driver.FindElement(By.CssSelector(".copy-link")).Text;
            VerifyIsTrue(Driver.FindElement(By.CssSelector(".fa.fa-copy")).Enabled);
            Driver.FindElement(By.CssSelector(".fa.fa-copy")).Click();
            VerifyIsTrue(Driver.FindElement(By.Id("toast-container")).Displayed);

            Driver.FindElement(By.Id("adminCommentsFormText")).SendKeys(Keys.Control + "v");
            Driver.FindElement(By.Id("adminCommentsFormText")).SendKeys(Keys.Control);
            VerifyAreEqual(path, Driver.FindElement(By.Id("adminCommentsFormText")).GetAttribute("value"));
            //editTaskForm
            Functions.CloseModalTask(Driver);

            Driver.Navigate().GoToUrl(path);
            VerifyIsTrue(Driver.FindElement(By.Name("editTaskForm")).Displayed);
            VerifyAreEqual("Задача №1", Driver.FindElement(By.TagName("h2")).Text);
        }

        [Test]
        public void SearchTasks()
        {
            Driver.GridFilterSendKeys("test1");
            Driver.MouseFocus(By.CssSelector(".page-name-block-text"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
        }

        [Test]
        public void AssignedToMeTasks()
        {
            Driver.FindElement(By.PartialLinkText("Мои задачи")).Click();

            VerifyAreEqual("3", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test12"));
        }

        [Test]
        public void AppointedByMeTasks()
        {
            Driver.FindElement(By.PartialLinkText("Поручил")).Click();

            VerifyAreEqual("6", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
        }

        [Test]
        public void CompletedTasks()
        {
            Driver.FindElement(By.PartialLinkText("Завершенные")).Click();

            VerifyAreEqual("6", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test5"));
        }

        [Test]
        public void AcceptedTasks()
        {
            Driver.FindElement(By.PartialLinkText("Принятые")).Click();

            VerifyAreEqual("1", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test15"));
        }
    }
}