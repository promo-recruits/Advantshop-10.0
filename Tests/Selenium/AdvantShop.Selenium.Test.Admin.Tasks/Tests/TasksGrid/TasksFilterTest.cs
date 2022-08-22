using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Tasks.Tests.TasksGrid
{
    [TestFixture]
    public class TasksFilterTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Tasks\\Customers.CustomerGroup.csv",
                "data\\Admin\\Tasks\\Customers.Departments.csv",
                "data\\Admin\\Tasks\\Customers.Customer.csv",
                "data\\Admin\\Tasks\\Customers.Managers.csv",
                "data\\Admin\\Tasks\\Customers.ManagerTask.csv",
                "data\\Admin\\Tasks\\Customers.TaskGroup.csv",
                "data\\Admin\\Tasks\\Customers.Task.csv",
                "data\\Admin\\Tasks\\Customers.ViewedTask.csv",
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
        public void FilterByCreate()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnDateCreated");
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("08.08.2016 00:00");
            Driver.GetGridFilter().Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("15.08.2016 21:00");
            Driver.GetGridFilter().Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test13"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDateCreated");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByPriority()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnPriorities");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Низкий")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test13"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"Средний")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test2"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test11"));
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();


            Driver.FindElement(By.CssSelector("[data-e2e=\"Высокий")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test12"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnPriorities");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByDueDate()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnDueDateFormatted");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("08.08.2016 00:00");
            Driver.GetGridFilter().Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("15.08.2016 21:00");
            Driver.GetGridFilter().Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test13"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDueDateFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByAssigned()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnAssigned");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Admin Ad\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test12"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test13"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByAppointed()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnAppointed");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Admin Ad\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test12"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAppointed");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        //убрали функционал
        /*
        [Test]
        public void FilterByStatus()
        {
            Functions.GridFilterSet(driver, baseURL, "_noopColumnStatuses");
                        
            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")))).SelectByText("В работе");
           
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(3, "Name").Text.Contains("test4"));
            //close
            Functions.GridFilterClose(driver, baseURL, "_noopColumnStatuses");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByPriorityandStatus()
        {
           Functions.GridFilterSet(driver, baseURL, "_noopColumnPriorities");
                      
            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")))).SelectByText("Низкий");
          
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test13"));

            Functions.GridFilterSet(driver, baseURL, "_noopColumnStatuses");
                       
            (new SelectElement(driver.FindElement(By.CssSelector("[data-e2e-grid-filter-block-name=\"_noopColumnStatuses\"] [data-e2e=\"gridFilterItemSelect\"]")))).SelectByText("В работе (выполняется)");
           
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test13"));
            //close priority
            Functions.GridFilterClose(driver, baseURL, "_noopColumnPriorities");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test13"));

            //close status
            Functions.GridFilterClose(driver, baseURL, "_noopColumnStatuses");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }
        */
        [Test]
        public void FilterByAssignedandAppointed()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnAppointed");

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Admin Ad\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test3"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test12"));

            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnAssigned");

            Driver.FindElement(By.CssSelector(
                    "[data-e2e-grid-filter-block-name=\"_noopColumnAssigned\"] [data-e2e=\"gridFilterItemSelect\"]"))
                .Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Admin Ad\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test12"));

            //close appointed
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAppointed");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test12"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test13"));

            //close assigned
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByDueDateandAssigned()
        {
            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnDueDateFormatted");

            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterFrom\"]")).SendKeys("07.11.2016 00:00");
            Driver.GetGridFilter().Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).Clear();
            Driver.FindElement(By.CssSelector("[data-e2e=\"datetimeFilterTo\"]")).SendKeys("11.11.2016 21:00");
            Driver.GetGridFilter().Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test2"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test4"));

            Functions.GridFilterSet(Driver, BaseUrl, "_noopColumnAssigned");


            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"test testov\"]")).Click();

            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test2"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test3"));

            //close duedate
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnDueDateFormatted");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));

            //close assign
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnAssigned");
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
        }

        [Test]
        public void FilterByViewed()
        {
            Functions.GridFilterSelectDropFocus(Driver, BaseUrl, filterName: "_noopColumnViewed", filterItem: "Да");

            VerifyAreEqual("Найдено записей: 5",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test2"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
            VerifyIsTrue(Driver.GetGridCell(3, "Name").Text.Contains("test4"));
            VerifyIsTrue(Driver.GetGridCell(4, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(0, "StatusFormatted").Text.Contains("В работе"));
            VerifyIsTrue(Driver.GetGridCell(1, "StatusFormatted").Text.Contains("В работе"));
            VerifyIsTrue(Driver.GetGridCell(2, "StatusFormatted").Text.Contains("В работе"));
            VerifyIsTrue(Driver.GetGridCell(3, "StatusFormatted").Text.Contains("В работе"));
            VerifyIsTrue(Driver.GetGridCell(4, "StatusFormatted").Text.Contains("В работе (выполняется)"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").GetCssValue("font-weight").Contains("400"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").GetCssValue("font-weight").Contains("400"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").GetCssValue("font-weight").Contains("400"));
            VerifyIsTrue(Driver.GetGridCell(3, "Name").GetCssValue("font-weight").Contains("400"));
            VerifyIsTrue(Driver.GetGridCell(4, "Name").GetCssValue("font-weight").Contains("400"));

            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();

            Driver.FindElement(By.CssSelector("[data-e2e=\"Нет\"]")).Click();

            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test12"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test13"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test14"));
            VerifyIsTrue(Driver.GetGridCell(0, "StatusFormatted").Text.Contains("В работе (выполняется)"));
            VerifyIsTrue(Driver.GetGridCell(1, "StatusFormatted").Text.Contains("В работе (выполняется)"));
            VerifyIsTrue(Driver.GetGridCell(2, "StatusFormatted").Text.Contains("В работе (выполняется)"));
            VerifyIsTrue(Driver.GetGridCell(0, "Name").GetCssValue("font-weight").Contains("600"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").GetCssValue("font-weight").Contains("600"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").GetCssValue("font-weight").Contains("600"));

            //close
            Functions.GridFilterClose(Driver, BaseUrl, "_noopColumnViewed");

            VerifyAreEqual("Найдено записей: 8",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text);
            VerifyIsTrue(Driver.GetGridCell(0, "Name").Text.Contains("test1"));
            VerifyIsTrue(Driver.GetGridCell(1, "Name").Text.Contains("test2"));
            VerifyIsTrue(Driver.GetGridCell(2, "Name").Text.Contains("test3"));
            VerifyIsTrue(Driver.GetGridCell(4, "Name").Text.Contains("test11"));
            VerifyIsTrue(Driver.GetGridCell(5, "Name").Text.Contains("test12"));
            VerifyIsTrue(Driver.GetGridCell(6, "Name").Text.Contains("test13"));
            VerifyIsTrue(Driver.GetGridCell(7, "Name").Text.Contains("test14"));
        }
    }
}