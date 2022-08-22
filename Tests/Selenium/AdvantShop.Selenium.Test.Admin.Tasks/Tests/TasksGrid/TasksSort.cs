using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksSort : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\ManyTasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Managers.csv",
                //  "data\\Admin\\Tasks\\ManyTasks\\Customers.ManagerTask.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Task.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.TaskManager.csv"
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
        public void SortByNumber()
        {
            Driver.GetGridCell(-1, "Id").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));
            Driver.GetGridCell(-1, "Id").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test114"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test105"));
            Driver.GetGridCell(-1, "Id").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));
        }

        [Test]
        public void SortByName()
        {
            Driver.GetGridCell(-1, "Name").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test107"));
            Driver.GetGridCell(-1, "Name").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test99"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test90"));
        }

        [Test]
        public void SortByPriority()
        {
            Driver.GetGridCell(-1, "PriorityFormatted").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test28"));
            VerifyAreEqual("Низкий", (Driver.GetGridCellElement(0, "PriorityFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("Низкий", (Driver.GetGridCellElement(9, "PriorityFormatted", by: By.TagName("div")).Text));

            Driver.GetGridCell(-1, "PriorityFormatted").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test35"));
            VerifyAreEqual("Высокий", (Driver.GetGridCellElement(0, "PriorityFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("Высокий", (Driver.GetGridCellElement(9, "PriorityFormatted", by: By.TagName("div")).Text));
        }

        [Test]
        public void SortByDuedate()
        {
            Driver.GetGridCell(-1, "DueDateFormatted").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test114"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test105"));
            Driver.GetGridCell(-1, "DueDateFormatted").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));
        }

        [Test]
        public void SortByStatus()
        {
            Driver.GetGridCell(-1, "StatusFormatted").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("В работе", (Driver.GetGridCellElement(9, "StatusFormatted", by: By.TagName("div")).Text));

            Driver.GetGridCell(-1, "StatusFormatted").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test50"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test59"));
            VerifyAreEqual("В работе (выполняется)",
                (Driver.GetGridCellElement(0, "StatusFormatted", by: By.TagName("div")).Text));
            VerifyAreEqual("В работе (выполняется)",
                (Driver.GetGridCellElement(9, "StatusFormatted", by: By.TagName("div")).Text));
        }

        [Test]
        public void SortByAssigned()
        {
            Driver.GetGridCell(-1, "Managers").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test7"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test27"));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(0, "Managers").Text));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCell(9, "Managers").Text));

            Driver.GetGridCell(-1, "Managers").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test31"));
            VerifyAreEqual("test testov", (Driver.GetGridCell(0, "Managers").Text));
            VerifyAreEqual("test testov", (Driver.GetGridCell(9, "Managers").Text));
        }

        [Test]
        public void SortByAppointed()
        {
            Driver.GetGridCell(-1, "AppointedName").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCellElement(0, "AppointedName", by: By.TagName("div")).Text));
            VerifyAreEqual("Admin Ad", (Driver.GetGridCellElement(9, "AppointedName", by: By.TagName("div")).Text));

            Driver.GetGridCell(-1, "AppointedName").Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyAreEqual("test testov", (Driver.GetGridCellElement(0, "AppointedName", by: By.TagName("div")).Text));
            VerifyAreEqual("test testov", (Driver.GetGridCellElement(9, "AppointedName", by: By.TagName("div")).Text));
        }
    }
}