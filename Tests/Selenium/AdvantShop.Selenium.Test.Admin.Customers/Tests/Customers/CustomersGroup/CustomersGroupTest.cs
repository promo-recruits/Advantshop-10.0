using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.CustomersGroup
{
    [TestFixture]
    public class CustomersGroupTest : BaseSeleniumTest
    {
        [OneTimeSetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Catalog);
            InitializeService.ClearData(ClearType.Customers);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomersGroup\\Customers.ManagerTask.csv"
            );

            Init();
        }

        [SetUp]
        public void SetUpTest()
        {
            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);

            GoToAdmin("settingscustomers#?tab=customerGroups");
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void CustomersGroupPresent()
        {
            VerifyAreEqual("Группы покупателей",
                Driver.FindElement(By.CssSelector("[data-e2e=\"GroupCustomerTitle\"]")).Text);

            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");

            Driver.GridPaginationSelectItems("10");
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup109", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);

            Driver.GridPaginationSelectItems("100");
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup70", Driver.GetGridCell(99, "GroupName", "CustomerGroups").Text);

            Driver.GridPaginationSelectItems("10");

            //check go to customers
            Driver.ScrollToTop();
            Driver.GetGridCell(0, "_customersColumn", "CustomerGroups").FindElement(By.TagName("a")).Click();
            Functions.OpenNewTab(Driver, BaseUrl);
            VerifyAreEqual("Покупатели", Driver.FindElement(By.TagName("h1")).Text);
            VerifyIsTrue(Driver.FindElement(By.CssSelector("[data-e2e-grid-filter-block-name=\"_GroupId\"]"))
                .Displayed);
            VerifyAreEqual("Обычный покупатель", Driver.FindElement(By.CssSelector(".ui-select-match-text")).Text);
            Functions.CloseTab(Driver, BaseUrl);
        }

        [Test]
        public void CustomersGroupSelectAndDelete()
        {
            VerifyIsTrue(Driver.CheckExpectedValuesInGridPaginationSelect(), "values in select are not expected");
            
            Driver.GridReturnDefaultView10(BaseUrl);
            //check select all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            VerifyAreEqual("200", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check deselect all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionDeselectAllFn\"]")).Click();
            VerifyIsFalse(Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsFalse(Driver.GetGridCell(9, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            Refresh();

            //check delete cancel
            Driver.GetGridCell(1, "_serviceColumn", "CustomerGroups").FindElement(By.TagName("a")).Click();
            Driver.SwalCancel();
            VerifyAreEqual("CustomerGroup101", Driver.GetGridCell(1, "GroupName", "CustomerGroups").Text);

            //check delete
            Driver.GetGridCell(1, "_serviceColumn", "CustomerGroups").FindElement(By.TagName("a")).Click();
            Driver.SwalConfirm();
            VerifyAreNotEqual("CustomerGroup101", Driver.GetGridCell(1, "GroupName", "CustomerGroups").Text);

            //check select
            Driver.GetGridCell(1, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(3, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(4, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Driver.GetGridCell(2, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxWrapSelect\"]")).Click();
            Thread.Sleep(100);
            VerifyAreEqual("4", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);
            VerifyIsTrue(Driver.GetGridCell(1, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(2, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(3, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(4, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);

            //check delete selected items
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreNotEqual("CustomerGroup102", Driver.GetGridCell(1, "GroupName", "CustomerGroups").Text);
            VerifyAreNotEqual("CustomerGroup103", Driver.GetGridCell(2, "GroupName", "CustomerGroups").Text);
            VerifyAreNotEqual("CustomerGroup104", Driver.GetGridCell(3, "GroupName", "CustomerGroups").Text);
            VerifyAreNotEqual("CustomerGroup105", Driver.GetGridCell(4, "GroupName", "CustomerGroups").Text);

            //check select all on page 10
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(9, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("10", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check delete selected items all on page 10
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup193", Driver.GetGridCell(9, "GroupName", "CustomerGroups").Text);

            //check select all on page 100
            Driver.GridPaginationSelectItems("100");
            Driver.ScrollToTop();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            VerifyIsTrue(Driver.GetGridCell(0, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyIsTrue(Driver.GetGridCell(99, "selectionRowHeaderCol", "CustomerGroups")
                .FindElement(By.CssSelector("[data-e2e=\"gridCheckboxSelect\"]")).Selected);
            VerifyAreEqual("100", Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectedCount\"]")).Text);

            //check delete selected items all on page 100
            Functions.GridDropdownDelete(Driver, BaseUrl);
            VerifyAreEqual("Обычный покупатель", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);
            VerifyAreEqual("CustomerGroup189", Driver.GetGridCell(17, "GroupName", "CustomerGroups").Text);

            //check delete all selected items
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            GoToAdmin("settingscustomers#?tab=customerGroups");

            VerifyIsTrue(Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text
                .Contains("Найдено записей: 1"));
        }

        [Test]
        public void CustomersGroupSearchExist()
        {
            //check search exist item
            Driver.GridFilterSendKeys("CustomerGroup126");
            VerifyAreEqual("CustomerGroup126", Driver.GetGridCell(0, "GroupName", "CustomerGroups").Text);

            //check search not exist item
            Driver.GridFilterSendKeys("CustomerGroup556");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search too much symbols
            Driver.GridFilterSendKeys(
                "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww", By.ClassName("ui-grid-custom-filter-total"));
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));

            //check search invalid symbols
            Driver.GridFilterSendKeys("########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"));
        }
    }
}