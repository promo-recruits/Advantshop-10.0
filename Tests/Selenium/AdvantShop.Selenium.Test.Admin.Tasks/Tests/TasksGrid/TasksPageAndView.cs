using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksPageAndView : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\ManyTasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\ManyTasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\ManyTasksPage\\Customers.Task.csv"
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
        public void TaskPresent()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));

            Driver.GridPaginationSelectItems("100");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(99, "Name").Text.Contains("test100"));

            Driver.GridPaginationSelectItems("10");
        }

        [Test]
        public void SelectTasks()
        {
            Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        [Test]
        public void SelectAllOnPageTasks()
        {
            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridHeaderCell\"][data-e2e-col-index=\"0\"] [data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Click();

            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        [Test]
        public void SelectAllTasks()
        {
            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridHeaderCell\"][data-e2e-col-index=\"0\"] [data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            VerifyAreEqual("114", Driver.FindElement(By.ClassName("ui-grid-custom-selection-count")).Text);
        }

        [Test]
        public void SelectAllTasksCancel()
        {
            Driver.FindElement(By.CssSelector(
                    "[data-e2e=\"gridHeaderCell\"][data-e2e-col-index=\"0\"] [data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click(); //снять выделение 

            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
        }

        //Паджинация
        [Test]
        public void PageTasks()
        {
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(4) a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test20"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(5) a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test21"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test30"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(6) a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test31"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test40"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-page:nth-child(7) a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test41"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test50"));


            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            //to begin
            Driver.FindElement(By.CssSelector(".pagination-first a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));
        }

        [Test]
        public void PageTaskToPrev()
        {
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test20"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-next a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test21"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test30"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test20"));

            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-prev a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(9, "Name").Text.Contains("test10"));

            //to end
            Driver.ScrollTo(By.CssSelector("[data-e2e=\"gridPagination\"]"));
            Driver.FindElement(By.CssSelector(".pagination-last a")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test111"));
            VerifyIsTrue(Driver.GetGridCell(3, "Name").Text.Contains("test114"));
        }
    }
}