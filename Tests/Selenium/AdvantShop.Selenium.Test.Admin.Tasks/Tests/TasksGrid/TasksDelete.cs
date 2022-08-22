using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class DeleteTask : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Orders | ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\AddTask\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\AddTask\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\AddTask\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\AddTask\\Customers.Managers.csv",
                //"data\\Admin\\Tasks\\AddTask\\Customers.ManagerTask.csv",
                "data\\Admin\\Tasks\\AddTask\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\AddTask\\Customers.Task.csv",
                "data\\Admin\\Tasks\\AddTask\\Customers.TaskManager.csv"
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
        public void DelSelectedTask()
        {
            VerifyAreEqual("4", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDropdownButton\"]")).Click();
            Driver.FindElement(
                By.CssSelector(
                    "[data-e2e=\"gridSelectionDropdownItem\"][data-e2e-grid-selection-dropdown-index=\"0\"]")).Click();
            Driver.SwalConfirm();
            VerifyAreEqual("3", Driver.FindElement(By.CssSelector(".ng-tab.nav-item.active span")).Text);
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 3);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test2"));
        }

        [Test]
        public void DelTaskiInEditCancel()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDelTask\"]")).Click();
            Driver.SwalCancel();
            Driver.FindElement(By.CssSelector(".btn.btn-default.btn-cancel")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test2"));
        }

        [Test]
        public void DelTaskiInEditOk()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("a")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"edittaskDelTask\"]")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 2);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
        }

        [Test]
        public void DelTaskiInGridCancel()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalCancel();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
        }

        [Test]
        public void DelTaskiInGridOk()
        {
            Driver.GetGridCellElement(0, "_serviceColumn", by: By.TagName("ui-grid-custom-delete")).Click();
            Driver.SwalConfirm();
            VerifyIsTrue(
                Driver.FindElements(By.CssSelector("[data-e2e=\"gridCell\"][data-e2e-col-index=\"1\"]")).Count == 1);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test4"));
        }
        /*   [Test]
           public void Attachment()
           {
             
               driver.FindElement(By.CssSelector("[data-e2e-grid-cell=\"grid[0][\'_serviceColumn\']\"] span")).Click();
               }*/
    }
}