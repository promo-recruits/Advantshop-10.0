using AdvantShop.Selenium.Core.Domain;
using AdvantShop.Selenium.Core.Domain.WebDriver.Extensions;
using NUnit.Framework;
using OpenQA.Selenium;

namespace AdvantShop.Selenium.Test.Admin.Customers.Tests.Customers.Customer
{
    [TestFixture]
    public class CustomersFilterCustomerFieldsTest : BaseMultiSeleniumTest
    {
        [SetUp]
        public void SetupTest()
        {
            InitializeService.RollBackDatabase();
            InitializeService.ClearData(ClearType.Customers | ClearType.Orders | ClearType.Catalog);
            InitializeService.LoadData(
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.CustomerGroup.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.Customer.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.Contact.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.Departments.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.Managers.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.CustomerField.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.CustomerFieldValue.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Customers.CustomerFieldValuesMap.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Catalog.Product.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Catalog.Offer.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Catalog.Category.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\Catalog.ProductCategories.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].OrderContact.csv",
                "Data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].OrderSource.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].OrderStatus.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].[Order].csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].OrderCurrency.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].OrderItems.csv",
                "data\\Admin\\Customers\\CustomersFilter\\CustomerFieldFilter\\[Order].OrderCustomer.csv"
            );

            Init();
            GoToAdmin("customers");

            TestName = TestContext.CurrentContext.Test.Name;
            VerifyBegin(TestName);
        }

        [TearDown]
        public void TearDownTest()
        {
            VerifyFinally(TestName);
        }

        [Test]
        public void FilterBySelectType()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_1");

            //check filter no items
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Customer Field 1 Value 4\"]")).Click();
            VerifyAreEqual("Найдено записей: 0",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter no items count");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter no items");

            //check filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"Customer Field 1 Value 1\"]")).Click();
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            VerifyAreEqual("LastName3 FirstName3",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "filter customers Name 1");
            VerifyAreEqual("LastName2 FirstName2",
                Driver.GetGridCell(1, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "filter customers Name 2");
            VerifyAreEqual("LastName1 FirstName1",
                Driver.GetGridCell(2, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "filter customers Name 3");

            //check all
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemSelect\"]")).Click();
            VerifyIsTrue(Driver.FindElements(By.CssSelector(".ui-select-choices-row-inner")).Count == 5,
                "count managers");
            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.Navigate().GoToUrl(strUrl);
            Refresh();

            Driver.WaitForElem(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total"));
            VerifyAreEqual("Найдено записей: 3",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_1\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_1");
            Refresh();
            VerifyAreEqual("Найдено записей: 117",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 117",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }


        [Test]
        public void FilterByTextType()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_2");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnCustomerField_2", "Text3");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_2", "11111111112222222222222222223333333333333344444444445555555555555555555555555555555");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_2", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist 
            Driver.SetGridFilterValue("_noopColumnCustomerField_2", "Text2");
            VerifyAreEqual("LastName84 FirstName84",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text, "customer Name filter");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.Navigate().GoToUrl(strUrl);
            Refresh();

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_2\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_2");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }

        [Test]
        public void FilterByMultilineTextType()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_4");

            //search by not exist 
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "customer 6 line 2");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "no such element");

            //search too much symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "1111111111222222222222222222333333333333334444444444rrrrrrrrrrrreeeeeeeeewwwwwwwwwwwwwwwwwww");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "too much symbols");

            //search invalid symbols
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "########@@@@@@@@&&&&&&&******,,,,..");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "invalid symbols");

            //search by exist
            Driver.SetGridFilterValue("_noopColumnCustomerField_4", "customer 1 line 2");
            VerifyAreEqual("LastName88 FirstName88",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer Name line 1 filter");
            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            string strUrl = Driver.Url;

            //check go to edit and back 
            Driver.GetGridCell(0, "_serviceColumn", "Customers")
                .FindElement(By.CssSelector(".link-invert.ui-grid-custom-service-icon.fas.fa-pencil-alt")).Click();
            Driver.WaitForElem(By.Id("Customer_LastName"));
            VerifyIsTrue(Driver.Url.Contains("customerIdInfo"), "customer edit");

            Driver.Navigate().GoToUrl(strUrl);
            Refresh();

            VerifyAreEqual("Найдено записей: 1",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter return");
            VerifyIsTrue(Driver
                .FindElement(By.CssSelector(
                    "[data-e2e=\"gridFilterBlock\"][data-e2e-grid-filter-block-name=\"_noopColumnCustomerField_4\"]"))
                .Displayed);

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_4");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 119",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter deleting 2");
        }

        [Test]
        public void FilterByNumberType()
        {
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_3");

            //check min too much symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "1111111111111111111111111111111", "");
            //VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "filter min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter  min many symbols");

            //check max too much symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "", "1111111111111111111111111111111"); 
            //VerifyIsFalse(driver.PageSource.Contains("Ни одной записи не найдено"), "filter max many symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter  max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter max many symbols border color");

            //check min and max too much symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "1111111111111111111111111111111", "1111111111111111111111111111111"); 
            //VerifyIsTrue(driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]"))
                    .GetCssValue("border-color"), "filter  min many symbols");
            VerifyAreEqual("rgb(241, 89, 89)",
                Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).GetCssValue("border-color"),
                "filter  max many symbols");

            //check invalid symbols
            // GoToAdmin("customers");
            //  Functions.GridFilterSet(driver, baseURL, name: "_noopColumnCustomerField_3");

            //check min invalid symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "########@@@@@@@@&&&&&&&******", ""); 
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter min imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter  min many symbols");

            // GoToAdmin("customers");
            //Functions.GridFilterSet(driver, baseURL, name: "_noopColumnCustomerField_3");

            //check max invalid symbols
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter  max many symbols");

            //check min and max invalid symbols
            // GoToAdmin("customers");
            //Functions.GridFilterSet(driver, baseURL, name: "_noopColumnCustomerField_3");
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "########@@@@@@@@&&&&&&&******", "########@@@@@@@@&&&&&&&******");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeFrom\"]")).Text,
                "filter both min imvalid symbols");
            VerifyAreEqual("", Driver.FindElement(By.CssSelector("[data-e2e=\"gridFilterItemRangeTo\"]")).Text,
                "filter both max imvalid symbols");
            VerifyAreEqual("Найдено записей: 120",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter  min/max many symbols");

            //  GoToAdmin("customers");
            // Functions.GridFilterSet(driver, baseURL, name: "_noopColumnCustomerField_3");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "1000", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "", "1000");
            VerifyIsFalse(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter max not exist");

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "1000", "1000");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter
            Driver.SetGridFilterRange("_noopColumnCustomerField_3", "85", "120");
            VerifyAreEqual("Найдено записей: 4",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            VerifyAreEqual("LastName101 FirstName101",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name line 1 filter");
            VerifyAreEqual("LastName87 FirstName87",
                Driver.GetGridCell(1, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name line 2 filter");
            VerifyAreEqual("LastName86 FirstName86",
                Driver.GetGridCell(2, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name line 3 filter");
            VerifyAreEqual("LastName85 FirstName85",
                Driver.GetGridCell(3, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name line 4 filter");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_3");
            VerifyAreEqual("Найдено записей: 116",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 116",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter after deleting 2");
        }

        [Test]
        public void FilterByDateType()
        {
            //check filter 
            Functions.GridFilterSet(Driver, BaseUrl, name: "_noopColumnCustomerField_5");

            //check filter min not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_5", "31.12.2050", "");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min not exist");

            //check max not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_5", "", "31.12.2050");
            Thread.Sleep(500);//костыль
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter max not exist"); 

            //check min and max not exist
            Driver.SetGridFilterRange("_noopColumnCustomerField_5", "31.12.2050", "31.12.2050");
            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "filter min/max not exist");

            //check filter   
            Driver.SetGridFilterRange("_noopColumnCustomerField_5", "05.11.2015", "15.11.2015");
            VerifyAreEqual("Найдено записей: 6",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter count");

            VerifyAreEqual("LastName98 FirstName98",
                Driver.GetGridCell(0, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter line 1");
            VerifyAreEqual("LastName97 FirstName97",
                Driver.GetGridCell(1, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter line 2");
            VerifyAreEqual("LastName96 FirstName96",
                Driver.GetGridCell(2, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter line 3");
            VerifyAreEqual("LastName95 FirstName95",
                Driver.GetGridCell(3, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter line 4");
            VerifyAreEqual("LastName94 FirstName94",
                Driver.GetGridCell(4, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter line 5");
            VerifyAreEqual("LastName93 FirstName93",
                Driver.GetGridCell(5, "Name", "Customers").FindElement(By.TagName("a")).Text,
                "customer name filter line 6");

            //check delete with filter
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridHeaderCheckboxWrapSelectAll\"]")).Click();
            Driver.FindElement(By.CssSelector("[data-e2e=\"gridSelectionSelectAllFn\"]")).Click();
            Functions.GridDropdownDelete(Driver, BaseUrl);

            VerifyIsTrue(Driver.PageSource.Contains("Ни одной записи не найдено"), "delete filtered items");

            //check delete filter
            Driver.ScrollToTop();
            Functions.GridFilterClose(Driver, BaseUrl, name: "_noopColumnCustomerField_5");
            VerifyAreEqual("Найдено записей: 114",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter after deleting 1");

            GoToAdmin("customers");
            VerifyAreEqual("Найдено записей: 114",
                Driver.FindElement(By.CssSelector(".input-group-addon.ui-grid-custom-filter-total")).Text,
                "filter after deleting 2");
        }
    }
}